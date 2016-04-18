using System;
using System.Threading.Tasks;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.Sync;

namespace StoreKeeper.Server
{
    public interface IDataManager
    {
        bool GetDatabaseLock(SessionId sessionId);

        Task<bool> GetCurrentAccountingData(SessionId sessionId, bool reloadAll);

        Task<bool> CalculateAndSave(SessionId sessionId);

        void IndexDatabase(bool filePrepared);

        void Close();
    }
}