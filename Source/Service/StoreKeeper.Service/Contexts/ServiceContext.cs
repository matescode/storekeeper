using StoreKeeper.Server;

namespace StoreKeeper.Service.Contexts
{
    public class ServiceContext : CommonBase.Service.ServiceContext
    {
        private ISessionManager _sessionManager;

        private IDataManager _dataManager;

        #region Public Methods

        public static void Initialize()
        {
            Instance._sessionManager = StoreKeeperServer.Service<ISessionManager>();
            Instance._dataManager = StoreKeeperServer.Service<IDataManager>();
        }

        public static ISessionManager SessionManager
        {
            get { return Instance._sessionManager; }
        }

        public static IDataManager DataManager
        {
            get { return Instance._dataManager; }
        }

        #endregion

        #region Overrides

        protected static new ServiceContext Instance
        {
            get
            {
                return CommonBase.Service.ServiceContext.Instance as ServiceContext;
            }
        }

        protected override void OnClosing()
        {
            Instance._sessionManager = null;
            Instance._dataManager = null;
            base.OnClosing();
        }

        #endregion
    }
}