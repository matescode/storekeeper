using System;
using System.Collections.Generic;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.Server;

namespace StoreKeeper.Server
{
    public interface ISessionManager
    {
        List<UserData> Users { get; }

        UserData CreateUser(string user);

        bool ChangeUserName(Guid userId, string newName);

        bool RemoveUser(Guid userId);

        SessionId CreateSession(ConnectionTicket ticket);

        bool CloseSession(SessionId session);

        void Close();

        void ReconnectSessions();

        IClientInfrastructure CreateClientProxy(SessionId sessionId);

        void EnsureAuthorizedSession(SessionId sessionId);

        void NotifyDataUpdated(SessionId callerSessionId);

        void NotifyDatabaseLockChanged(SessionId callerSessionId);

        string GetRelatedUserName(SessionId sessionId);

        string GetRelatedUserId(SessionId sessionId);

        UserData GetRelatedUserInfo(SessionId sessionId);
    }
}