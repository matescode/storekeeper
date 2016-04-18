using CommonBase.Log;
using CommonBase.Utils;

namespace StoreKeeper.Client
{
    internal partial class StoreKeeperServiceClient
    {
        private class InfrastructureCallbackHandler : IClientInfrastructureCallback
        {
            private static readonly ILogger Logger = LogManager.GetLogger(typeof (InfrastructureCallbackHandler));

            private readonly StoreKeeperServiceClient _client;
            private ConnectionStatus _oldStatus = ConnectionStatus.Inactive;

            public InfrastructureCallbackHandler(StoreKeeperServiceClient client)
            {
                ArgumentValidator.IsNotNull("client", client);
                _client = client;
            }

            #region IClientInfrastructureCallback Implementation

            public void OnConnectionClosing()
            {
                if (_client.Messenger != null)
                {
                    _client.Messenger.ConnectionClosing();
                }
                _oldStatus = _client.ConnectionStatus;
                _client.ConnectionStatus = ConnectionStatus.Inactive;

                Logger.Info("Connection to server interrupted.");
            }

            public void OnConnectionRestarted()
            {
                if (_client.Messenger != null)
                {
                    _client.Messenger.ConnectionRestarted();
                }
                _client.ConnectionStatus = _oldStatus;
                _oldStatus = ConnectionStatus.Inactive;

                Logger.Info("Connection to server restored.");
            }

            public void DataUpdated()
            {
                if (_client.Messenger != null)
                {
                    _client.Messenger.DataUpdated = false;
                }
            }

            public void DatabaseLockChanged()
            {
                if (_client.Messenger != null)
                {
                    _client.Messenger.DatabaseStatusChanged();
                }
            }

            #endregion
        }

    }
}