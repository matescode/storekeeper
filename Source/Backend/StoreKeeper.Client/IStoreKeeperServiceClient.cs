using System;

namespace StoreKeeper.Client
{
    public interface IStoreKeeperServiceClient
    {
        #region Properties

        ConnectionStatus ConnectionStatus { get; }

        DatabaseStatus DatabaseStatus { get; }

        bool DatabaseOperationAllowed { get; }

        IDataAccess DataAccess { get; }

        IServerAdministration ServerAdministration { get; }

        ILongOperationHandler LongOperationHandler { set; }

        IClientMessenger Messenger { set; }

        #endregion

        #region Methods

        bool Connect();

        bool Disconnect();

        void Destroy();

        void GetCurrentAccountingDataAsync(bool reloadAll);

        void CalculationAndSaveAsync();

        void ReloadAllData(bool async, Action reloadAction = null);

        #endregion
    }
}