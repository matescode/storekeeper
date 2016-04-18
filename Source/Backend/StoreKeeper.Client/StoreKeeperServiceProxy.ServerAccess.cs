using System;
using System.Collections.Generic;
using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.Server;

namespace StoreKeeper.Client
{
    public partial class StoreKeeperServiceProxy
    {
        #region IServerAccess Implementation

        public SessionId Connect(ConnectionTicket ticket)
        {
            return Channel.Connect(ticket);
        }

        public bool Disconnect(SessionId sessionId)
        {
            return Channel.Disconnect(sessionId);
        }

        public bool GetLock(SessionId sessionId)
        {
            return Channel.GetLock(sessionId);
        }

        public IAsyncResult BeginGetCurrentAccountingData(SessionId sessionId, bool reloadAll, AsyncCallback callback, object state)
        {
            return Channel.BeginGetCurrentAccountingData(sessionId, reloadAll, callback, state);
        }

        public bool EndGetCurrentAccountingData(IAsyncResult result)
        {
            return Channel.EndGetCurrentAccountingData(result);
        }

        public IAsyncResult BeginCalculationAndSave(SessionId sessionId, AsyncCallback callback, object state)
        {
            return Channel.BeginCalculationAndSave(sessionId, callback, state);
        }

        public bool EndCalculationAndSave(IAsyncResult result)
        {
            return Channel.EndCalculationAndSave(result);
        }

        public UserData CreateUser(SessionId sessionId, string username)
        {
            return Channel.CreateUser(sessionId, username);
        }

        public List<UserData> GetUsers(SessionId sessionId)
        {
            return Channel.GetUsers(sessionId);
        }

        public bool RemoveUser(SessionId sessionId, Guid userId)
        {
            return Channel.RemoveUser(sessionId, userId);
        }

        public bool ChangeUserName(SessionId sessionId, Guid userId, string newName)
        {
            return Channel.ChangeUserName(sessionId, userId, newName);
        }

        public UserData GetCurrentUserInfo(SessionId sessionId)
        {
            return Channel.GetCurrentUserInfo(sessionId);
        }

        #endregion
    }
}