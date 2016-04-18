using System;
using StoreKeeper.Common;
using StoreKeeper.Service.Contexts;

namespace StoreKeeper.Service
{
    public class ServiceConfiguration : IServiceDescriptor
    {
        #region IServiceDescriptor Implementation

        public bool Secured
        {
            get { return false; }
        }

        public string Server
        {
            get { return Environment.MachineName; }
        }

        public int Port
        {
            get { return ApplicationContext.Config.ServerPort; }
        }

        public string ServiceName
        {
            get { return Constants.ServiceHostServiceName; }
        }

        #endregion
    }
}