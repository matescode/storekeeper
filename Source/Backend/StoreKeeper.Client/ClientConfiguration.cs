using System;
using StoreKeeper.Common;

namespace StoreKeeper.Client
{
    internal class ClientConfiguration : IServiceDescriptor
    {
        public ClientConfiguration(int port)
        {
            Port = port;
        }

        #region IServiceDescriptor Implementation

        public bool Secured
        {
            get { return false; }
        }

        public string Server
        {
            get { return Environment.MachineName; }
        }

        public int Port { get; private set; }

        public string ServiceName
        {
            get { return Constants.ClientHostServiceName; }
        }

        #endregion
    }
}