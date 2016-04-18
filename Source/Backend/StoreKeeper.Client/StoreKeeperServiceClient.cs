using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using CommonBase.Application;
using CommonBase.Log;
using CommonBase.Utils;

using StoreKeeper.Client.Exceptions;
using StoreKeeper.Client.Objects;
using StoreKeeper.Client.Objects.Implementation;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.Server;

namespace StoreKeeper.Client
{
    internal partial class StoreKeeperServiceClient : IStoreKeeperServiceClient, IServerAdministration, IDatabaseAccess
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(StoreKeeperServiceClient));

        private SessionId _sessionId;
        private readonly IClientConfiguration _clientConfiguration;
        private ServiceHost _clientHost;
        private ClientContract _contractInstance;
        private readonly IClientInfrastructureCallback _infrastructureCallback;
        private IClientRepository _clientRepository;
        private ConnectionStatus _connectionStatus;
        private ILongOperationHandler _longOperationHandler;
        private IClientMessenger _clientMessenger;
        private StoreKeeperServiceProxy _currentProxy;
        private bool _clientHostOpened;

        public StoreKeeperServiceClient(IClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration;
            ConnectionStringHolder.Initialize(clientConfiguration.ConnectionString);
            _infrastructureCallback = new InfrastructureCallbackHandler(this);
            if (!clientConfiguration.IsOffline)
            {
                InitClient();
            }
            InitClientRepository();
            _connectionStatus = ConnectionStatus.Disconnected;
        }

        #region IStoreKeeperServiceClient Implementation

        public ConnectionStatus ConnectionStatus
        {
            get { return _connectionStatus; }
            internal set { _connectionStatus = value; }
        }

        public DatabaseStatus DatabaseStatus
        {
            get { return ClientRepository.DatabaseStatus; }
        }

        public bool DatabaseOperationAllowed
        {
            get { return DatabaseStatus == DatabaseStatus.Connected || DatabaseStatus == DatabaseStatus.Locked; }
        }

        public IDataAccess DataAccess
        {
            get { return _clientRepository.DataAccess; }
        }

        public IServerAdministration ServerAdministration
        {
            get { return this; }
        }

        public ILongOperationHandler LongOperationHandler
        {
            set
            {
                _clientRepository.LongOperationHandler = value;
                _longOperationHandler = value;
            }
        }

        public IClientMessenger Messenger
        {
            private get { return _clientMessenger; }
            set
            {
                _clientRepository.Messenger = value;
                _clientMessenger = value;
            }
        }

        public bool Connect()
        {
            if (_connectionStatus == ConnectionStatus.Connected || _connectionStatus == ConnectionStatus.Inactive)
            {
                return true;
            }

            try
            {
                using (StoreKeeperServiceProxy proxy = new StoreKeeperServiceProxy(_clientConfiguration))
                {
                    _sessionId = proxy.Connect(CreateTicket());
                    _connectionStatus = _sessionId != null ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
                    if (_connectionStatus == ConnectionStatus.Connected)
                    {
                        string userId = proxy.GetCurrentUserInfo(_sessionId).Id.ToString().ToUpper();
                        UserContext.Initialize(userId);
                        ClientRepository.CheckDatabaseStatus();
                    }
                }
            }
            catch (Exception e)
            {
                ApplicationContext.Log.Error(GetType(), e);
                _connectionStatus = ConnectionStatus.Disconnected;
            }

            return _connectionStatus == ConnectionStatus.Connected;
        }

        public bool Disconnect()
        {
            if (_connectionStatus == ConnectionStatus.Disconnected || _connectionStatus == ConnectionStatus.Inactive)
            {
                return true;
            }

            try
            {
                using (StoreKeeperServiceProxy proxy = new StoreKeeperServiceProxy(_clientConfiguration))
                {
                    if (proxy.Disconnect(_sessionId))
                    {
                        _connectionStatus = ConnectionStatus.Disconnected;
                        UserContext.Close();
                    }
                }
            }
            catch (Exception e)
            {
                ApplicationContext.Log.Error(GetType(), e);
                _connectionStatus = ConnectionStatus.Disconnected;
                return false;
            }

            return _connectionStatus == ConnectionStatus.Disconnected;
        }

        public void Destroy()
        {
            Disconnect();

            if (_clientHost != null)
            {
                if (_clientHostOpened)
                {
                    _clientHost.Close();
                }
                _clientHost = null;
                _contractInstance.Close();
                _contractInstance = null;
            }

            DestroyClientRepository();
            ConnectionStringHolder.Close();
        }

        public void GetCurrentAccountingDataAsync(bool reloadAll)
        {
            Task.Factory.StartNew(() =>
                {
                    GetLock();

                    _longOperationHandler.Start("AccountingDataSync");

                    try
                    {
                        _currentProxy = new StoreKeeperServiceProxy(_clientConfiguration);
                        _currentProxy.BeginGetCurrentAccountingData(_sessionId, reloadAll, ProcessAccountingDataSync, null);
                    }
                    catch (Exception ex)
                    {
                        _longOperationHandler.OperationFailed(ex.Message);
                    }
                });
        }

        public void CalculationAndSaveAsync()
        {
            Task.Factory.StartNew(() =>
                {
                    GetLock();

                    _longOperationHandler.Start("Calculation", "Calculating");

                    try
                    {
                        _currentProxy = new StoreKeeperServiceProxy(_clientConfiguration);
                        _currentProxy.BeginCalculationAndSave(_sessionId, ProcessCalculation, null);
                    }
                    catch (Exception ex)
                    {
                        _longOperationHandler.OperationFailed(ex.Message);
                    }
                });
        }

        public void ReloadAllData(bool async, Action reloadAction = null)
        {
            if (async)
            {
                _clientRepository.DataAccess.ReloadDataAsync(reloadAction);
            }
            else
            {
                _clientRepository.DataAccess.ReloadData();
            }
        }

        #endregion

        #region IServerAdministration Implementation

        public DateTime LastAccountingUpdate
        {
            get
            {
                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    return dataContext.LastUpdate;
                }
            }
        }

        public string ResponsibleUser
        {
            get
            {
                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    return dataContext.ResponsibleUser;
                }
            }
        }

        public string LockedBy
        {
            get
            {
                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    return dataContext.LockedByUser;
                }
            }
        }

        public List<IServerUser> Users
        {
            get
            {
                using (StoreKeeperServiceProxy proxy = new StoreKeeperServiceProxy(_clientConfiguration))
                {
                    List<UserData> users = proxy.GetUsers(_sessionId);
                    return users.Select(userData => new ServerUser(userData, ChangeUserName)).Cast<IServerUser>().ToList();
                }
            }
        }

        public IServerUser CreateUser(string username)
        {
            using (StoreKeeperServiceProxy proxy = new StoreKeeperServiceProxy(_clientConfiguration))
            {
                UserData newUser = proxy.CreateUser(_sessionId, username);
                return new ServerUser(newUser, ChangeUserName);
            }
        }

        public bool RemoveUser(IServerUser user)
        {
            using (StoreKeeperServiceProxy proxy = new StoreKeeperServiceProxy(_clientConfiguration))
            {
                return proxy.RemoveUser(_sessionId, user.UserId);
            }
        }

        #endregion

        #region IDatabaseAccess Implementation

        public void GetLock()
        {
            if (DatabaseStatus == DatabaseStatus.Locked || DatabaseStatus == DatabaseStatus.NotConnected)
            {
                return;
            }

            if (DatabaseStatus == DatabaseStatus.Blocked)
            {
                throw new DatabaseLockedException(GetType(), "DatabaseLockedException");
            }

            try
            {
                using (StoreKeeperServiceProxy proxy = new StoreKeeperServiceProxy(_clientConfiguration))
                {
                    bool result = proxy.GetLock(_sessionId);
                    if (!result)
                    {
                        Logger.Error("Cannot get database lock!");
                        throw new DatabaseLockedException(GetType(), "DatabaseLockedException");
                    }
                }

                Logger.Info("Lock acquired succesfully.");
                ClientRepository.CheckDatabaseStatus();
                Messenger.DatabaseStatusChanged();
            }
            catch (DatabaseLockedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _longOperationHandler.OperationFailed(ex.Message);
            }
        }

        #endregion

        #region Internals and Helpers

        private IClientRepository ClientRepository
        {
            get
            {
                if (_clientRepository == null)
                {
                    throw new RepositoryNotInitializedException(GetType());
                }
                return _clientRepository;
            }
        }

        private void InitClient()
        {
            _clientHostOpened = false;
            try
            {
                CreateClientHost();
                _clientHostOpened = true;
            }
            catch (Exception ex)
            {
                ApplicationContext.Log.Error(GetType(), ex);
            }
        }

        private void InitClientRepository()
        {
            _clientRepository = new ClientRepository(this);
            ClientRepository.Open();
        }

        private void DestroyClientRepository()
        {
            _clientRepository = null;
        }

        private bool CheckClient()
        {
            return _clientHost.State == CommunicationState.Opened;
        }

        private void CreateClientHost()
        {
            IServiceDescriptor clientDescriptor = new ClientConfiguration(_clientConfiguration.ClientPort);
            string uriClient = Infrastructure.GetEndpointUrl(clientDescriptor);

            _contractInstance = new ClientContract(_infrastructureCallback, CheckClient);

            _clientHost = new ServiceHost(_contractInstance, new Uri(uriClient));

            Infrastructure.AddServiceBehaviors(_clientHost.Description);

            _clientHost.AddServiceEndpoint(
                typeof(IClientInfrastructure),
                Infrastructure.CreateApplicationBinding(false),
                Constants.ClientAccessContract
            );

            _clientHost.Open();
        }

        private ConnectionTicket CreateTicket()
        {
            return new ConnectionTicket
                {
                    ClientComputer = NetworkUtils.LocalIpAddress,
                    Username = _clientConfiguration.User,
                    SecurityToken = _clientConfiguration.SecurityToken,
                    Port = _clientConfiguration.ClientPort
                };
        }

        private void ProcessAccountingDataSync(IAsyncResult asyncResult)
        {
            try
            {
                using (_currentProxy)
                {
                    bool syncResult = _currentProxy.EndGetCurrentAccountingData(asyncResult);
                    if (!syncResult)
                    {
                        _longOperationHandler.OperationFailed("Cannot perform synchronization with Pohoda");
                        return;
                    }

                    LongOperationResult result = new LongOperationResult { RefreshAll = true };
                    _longOperationHandler.End(result);
                    Logger.Info("Accounting data synchronization finished succesfully.");
                    ClientRepository.ResetDatabaseStatus();
                }
                _currentProxy = null;
            }
            catch (Exception ex)
            {
                _longOperationHandler.OperationFailed(ex.Message);
            }
        }

        private void ProcessCalculation(IAsyncResult asyncResult)
        {
            try
            {
                using (_currentProxy)
                {
                    _currentProxy.EndCalculationAndSave(asyncResult);
                    ReloadAllData(false);
                    _longOperationHandler.End(new LongOperationResult { RefreshAll = true });
                    ClientRepository.ResetDatabaseStatus();
                    Messenger.CalculationRequested = false;
                }
                _currentProxy = null;
            }
            catch (Exception ex)
            {
                _longOperationHandler.OperationFailed(ex.Message);
                throw;
            }
        }

        private bool ChangeUserName(Guid userId, string newUserName)
        {
            using (StoreKeeperServiceProxy proxy = new StoreKeeperServiceProxy(_clientConfiguration))
            {
                return proxy.ChangeUserName(_sessionId, userId, newUserName);
            }
        }

        #endregion
    }
}