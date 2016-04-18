using System;

namespace StoreKeeper.Client
{
    internal interface IClientRepository
    {
        DatabaseStatus DatabaseStatus { get; }

        IDataAccess DataAccess { get; }

        ILongOperationHandler LongOperationHandler { set; }

        IClientMessenger Messenger { set; }

        void Open();

        void CheckDatabaseStatus();

        void ResetDatabaseStatus();
    }
}