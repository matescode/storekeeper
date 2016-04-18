using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommonBase.Application;
using CommonBase.Log;

using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.StoreKeeper;
using StoreKeeper.Common.DataContracts.Sync;
using StoreKeeper.Common.Exceptions;

namespace StoreKeeper.Server
{
    internal class DataManager : IDataManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(DataManager));

        private static readonly DateTime StartingDateTime = new DateTime(1970, 1, 1, 0, 0, 0);

        private readonly object _lockObj = new object();
        private DateTime _lastUpdate;
        private Config _config;

        public DataManager()
        {
            _lastUpdate = LastAccountingDataUpdate;
            _config = ApplicationContext.Config as Config;
        }

        #region IDataManager Implementation

        public bool GetDatabaseLock(SessionId sessionId)
        {
            try
            {
                ISessionManager sessionManager = StoreKeeperServer.Service<ISessionManager>();
                using (StoreKeeperDataContext context = new StoreKeeperDataContext())
                {
                    SqlParameter userIdParam = new SqlParameter("@UserId", sessionManager.GetRelatedUserId(sessionId));
                    SqlParameter unlockParam = new SqlParameter("@Unlock", SqlDbType.Bit) { Value = 0 };
                    context.Database.ExecuteSqlCommand("exec LockDatabase @UserId, @Unlock", userIdParam, unlockParam);
                }
                sessionManager.NotifyDatabaseLockChanged(sessionId);
                return true;
            }
            catch (Exception ex)
            {
                ApplicationContext.Log.Error(GetType(), ex);
                return false;
            }
        }

        public Task<bool> GetCurrentAccountingData(SessionId sessionId, bool reloadAll)
        {
            return Task<bool>.Factory.StartNew(() => GetAccountingData(sessionId, reloadAll));
        }

        public Task<bool> CalculateAndSave(SessionId sessionId)
        {
            return Task<bool>.Factory.StartNew(() =>
                {
                    try
                    {
                        using (StoreKeeperDataContext context = new StoreKeeperDataContext())
                        {
                            if (context.Database.Connection.State == ConnectionState.Closed)
                            {
                                context.Database.Connection.Open();
                            }

                            SqlParameter publishParam = new SqlParameter("@Publish", SqlDbType.Bit) { Value = 1 };

                            DbCommand cmd = context.Database.Connection.CreateCommand();
                            cmd.CommandText = "exec CalculateData @Publish";
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add(publishParam);
                            cmd.ExecuteNonQuery();
                        }
                        ISessionManager sessionManager = StoreKeeperServer.Service<ISessionManager>();
                        sessionManager.NotifyDataUpdated(sessionId);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        ApplicationContext.Log.Error(GetType(), ex);
                        return false;
                    }
                });
        }

        public Task<bool> PublishData(SessionId sessionId)
        {
            return Task<bool>.Factory.StartNew(() =>
                {
                    try
                    {
                        lock (_lockObj)
                        {
                            using (StoreKeeperDataContext context = new StoreKeeperDataContext())
                            {
                                context.Database.ExecuteSqlCommand("exec PublishData");
                            }
                            ISessionManager sessionManager = StoreKeeperServer.Service<ISessionManager>();
                            sessionManager.NotifyDataUpdated(sessionId);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationContext.Log.Error(GetType(), ex);
                        return false;
                    }
                });
        }

        public void IndexDatabase(bool filePrepared)
        {
            AccountingDataSync globalDataSync = ProcessAccountingData(filePrepared, false);

            using (StoreKeeperDataContext context = new StoreKeeperDataContext())
            {
                SqlParameter userIdParam = new SqlParameter("@UserId", new Guid("F2024637-2251-479A-8ED9-940E4354F37B"));
                SqlParameter unlockParam = new SqlParameter("@Unlock", SqlDbType.Bit) { Value = 0 };
                context.Database.ExecuteSqlCommand("exec LockDatabase @UserId, @Unlock", userIdParam, unlockParam);

                context.SaveLoadedData(globalDataSync);
                context.LastUpdate = DateTime.Now;
                context.ResponsibleUser = "SYSTEM";
                context.SaveChanges();
            }
        }

        public void Close()
        {
            _config = null;
        }

        #endregion

        #region Internals and Helpers

        private Config Configuration
        {
            get { return _config; }
        }

        private DateTime LastAccountingDataUpdate
        {
            get
            {
                using (StoreKeeperDataContext context = new StoreKeeperDataContext())
                {
                    return context.LastUpdate;
                }
            }
        }

        private bool GetAccountingData(SessionId callerSessionId, bool reloadAll)
        {
            try
            {
                ISessionManager sessionManager = StoreKeeperServer.Service<ISessionManager>();

                // parse data
                AccountingDataSync globalDataSync = ProcessAccountingData(false, reloadAll);

                using (StoreKeeperDataContext context = new StoreKeeperDataContext())
                {
                    // save to database
                    context.SaveLoadedData(globalDataSync);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new DataSynchronizationError(GetType(), DataSyncErrorType.Runtime, ex.Message);
            }
        }

        private AccountingDataSync ProcessAccountingData(bool resultPrepared, bool reloadAll)
        {
            lock (_lockObj)
            {
                DateTime usedDate = _lastUpdate;
                if (reloadAll)
                {
                    usedDate = StartingDateTime;
                }

                DateTime now = DateTime.Now;
                TimeSpan difference = now.Subtract(_lastUpdate);
                if (difference.TotalMinutes < Configuration.SafetyThresholdInMinutes)
                {
                    Logger.Info(LogId.AccountingDataThresholdReached, "Safety threshold on accounting data reached");
                    return AccountingDataSync.EmptySync;
                }

                if (!resultPrepared)
                {
                    BuildTemplateFile(usedDate);

                    string commandPath = Path.Combine(Configuration.AccountingDataRootFolder,
                                                      ServerSettings.AccountingDataCommandPath);

                    Logger.Debug("Accounting data export started.");
                    Process getDataProcess = new Process();
                    getDataProcess.StartInfo.UseShellExecute = true;
                    getDataProcess.StartInfo.FileName = commandPath;
                    getDataProcess.StartInfo.CreateNoWindow = false;
                    getDataProcess.Start();
                    getDataProcess.WaitForExit();
                }

                string resultFile = Path.Combine(Configuration.AccountingDataRootFolder,
                                                 ServerSettings.AccountingDataResultFile);

                CheckDataFileReady(resultFile);

                Logger.Debug("Accounting data export finished.");

                DataProcessor dataProcessor = new DataProcessor();

                dataProcessor.ProcessFile(resultFile);

                _lastUpdate = now;
                dataProcessor.DataSync.LastUpdate = _lastUpdate;

                Logger.Info(LogId.AccountingDataExportFinished, "Accounting data export finished.");

                return dataProcessor.DataSync;
            }
        }

        private void BuildTemplateFile(DateTime lastUpdate)
        {
            string content;
            Encoding encoding;

            using (FileStream inputStream = File.OpenRead(Path.Combine(Configuration.AccountingDataRootFolder, ServerSettings.AccountingDataRequestTemplate)))
            {
                using (StreamReader reader = new StreamReader(inputStream))
                {
                    content = reader.ReadToEnd();
                    encoding = reader.CurrentEncoding;
                }
            }

            if (content.Contains(ServerSettings.AccountingDataTemplateMatch))
            {
                string time = String.Format("{0}-{1}-{2}T{3}:{4}:{5}",
                    lastUpdate.Year.ToString("D4"),
                    lastUpdate.Month.ToString("D2"),
                    lastUpdate.Day.ToString("D2"),
                    lastUpdate.Hour.ToString("D2"),
                    lastUpdate.Minute.ToString("D2"),
                    lastUpdate.Second.ToString("D2"));

                content = content.Replace(ServerSettings.AccountingDataTemplateMatch, time);
            }

            Encoding enc1250 = Encoding.GetEncoding(1250);

            byte[] buffer = encoding.GetBytes(content);
            byte[] conversion = Encoding.Convert(encoding, enc1250, buffer);

            string outputFile = Path.Combine(Configuration.AccountingDataRootFolder, ServerSettings.AccountingDataTemplateFile);

            Logger.Info("Output file: {0}", outputFile);

            File.WriteAllBytes(outputFile, conversion);
        }

        private void CheckDataFileReady(string fileName)
        {
            while (!File.Exists(fileName))
            {
                Thread.Sleep(ServerSettings.DataParserCheckFileSleep);
            }
            Thread.Sleep(ServerSettings.DataParserSafetySleep);
        }

        #endregion
    }
}