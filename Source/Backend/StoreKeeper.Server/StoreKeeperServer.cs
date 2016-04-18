using CommonBase;
using CommonBase.Log;
using CommonBase.Utils;

namespace StoreKeeper.Server
{
    public class StoreKeeperServer : SingletonInstance<StoreKeeperServer>
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(StoreKeeperServer));
        private readonly ISessionManager _sessionManager;
        private readonly IDataManager _dataManager;

        private StoreKeeperServer()
        {
            _sessionManager = new SessionManager();
            _dataManager = DataManagerFactory.CreateDataManager();
            Logger.Info("Server instance created.");
        }

        #region Public Methods

        public static void Create()
        {
            if (!IsValid)
            {
                new StoreKeeperServer();
            }
            else
            {
                Logger.Warning("Server instance already created.");
            }
        }

        public static void Close()
        {
            if (IsValid)
            {
                Instance._dataManager.Close();
                Instance._sessionManager.Close();
                CloseInstance();
                Logger.Info("Server instance closed.");
            }
        }

        public static TService Service<TService>() where TService : class
        {
            if (typeof(TService) == typeof(ISessionManager))
            {
                return Instance._sessionManager as TService;
            }
            if (typeof (TService) == typeof (IDataManager))
            {
                return Instance._dataManager as TService;
            }
            return DefaultServiceProvider.GetDefaultProvider<TService>(typeof(StoreKeeperServer));
        }

        #endregion
    }
}