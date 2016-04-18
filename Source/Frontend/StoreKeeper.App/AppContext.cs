using System;
using CommonBase.Application;
using StoreKeeper.App.Log;
using StoreKeeper.Client;

namespace StoreKeeper.App
{
    public class AppContext : ApplicationContext
    {
        private IStoreKeeperServiceClient _storeKeeperClient;

        public AppContext()
            : base(ApplicationType.Windows, new ApplicationLog())
        {
            InitClient();
        }

        #region Overrides

        public override T GetService<T>()
        {
            if (typeof(T) == typeof(IStoreKeeperServiceClient))
            {
                return _storeKeeperClient as T;
            }
            if (typeof (T) == typeof (IDataAccess))
            {
                return _storeKeeperClient.DataAccess as T;
            }
            if (typeof(T) == typeof(ILogBrowser))
            {
                return Log as T;
            }
            return base.GetService<T>();
        }

        protected override void OnClosing()
        {
            base.OnClosing();
            _storeKeeperClient.Destroy();
            _storeKeeperClient = null;
        }

        protected override ApplicationConfiguration CreateConfiguration()
        {
            return new AppConfig();
        }

        protected override IApplicationInfo CreateApplicationInfo()
        {
            return new ClientApplicationInfo();
        }

        public static new AppConfig Config
        {
            get { return ApplicationContext.Config as AppConfig; }
        }

        #endregion

        #region Internals and Helpers

        private void InitClient()
        {
            _storeKeeperClient = ClientFactory.CreateClient(Config);
        }

        #endregion
    }
}