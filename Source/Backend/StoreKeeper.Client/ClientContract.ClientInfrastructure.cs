using CommonBase.Application;
using StoreKeeper.Common;

namespace StoreKeeper.Client
{
    internal partial class ClientContract : IClientInfrastructure
    {
        #region IClientInfrastructure Implementation

        public string ValidateConnection(string ticket)
        {
            return Constants.ClientValidResultHash;
        }

        public bool ClosingConnection()
        {
            if (_callback != null)
            {
                _callback.OnConnectionClosing();
            }
            return true;
        }

        public void ConnectionRestarted()
        {
            if (_callback != null)
            {
                _callback.OnConnectionRestarted();
            }
        }

        public bool IsActive()
        {
            return _checkAvailability();
        }

        public void DataUpdated()
        {
            if (_callback != null)
            {
                _callback.DataUpdated();
            }
        }

        public void DatabaseLockChanged()
        {
            if (_callback != null)
            {
                _callback.DatabaseLockChanged();
            }
        }

        public string GetContractVersion()
        {
            return ApplicationContext.Info.Version.ToString();
        }

        #endregion
    }
}