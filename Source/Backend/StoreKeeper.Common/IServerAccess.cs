using System;
using System.Collections.Generic;
using System.ServiceModel;

using StoreKeeper.Common.DataContracts.Server;

namespace StoreKeeper.Common
{
    [ServiceContract]
    public interface IServerAccess : IApplicationContract
    {
        [OperationContract]
        SessionId Connect(ConnectionTicket ticket);

        [OperationContract]
        bool Disconnect(SessionId sessionId);

        [OperationContract]
        bool GetLock(SessionId sessionId);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetCurrentAccountingData(SessionId sessionId, bool reloadAll, AsyncCallback callback, object state);

        bool EndGetCurrentAccountingData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCalculationAndSave(SessionId sessionId, AsyncCallback callback, object state);

        bool EndCalculationAndSave(IAsyncResult result);

        [OperationContract]
        UserData CreateUser(SessionId sessionId, string username);

        [OperationContract]
        List<UserData> GetUsers(SessionId sessionId);

        [OperationContract]
        bool ChangeUserName(SessionId sessionId, Guid userId, string newName);

        [OperationContract]
        bool RemoveUser(SessionId sessionId, Guid userId);

        [OperationContract]
        UserData GetCurrentUserInfo(SessionId sessionId);
    }
}