using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.Server;
using StoreKeeper.Service.Contexts;

namespace StoreKeeper.Service
{
    public partial class StoreKeeperContract : IServerAccess
    {
        #region IServerAccess Members

        public SessionId Connect(ConnectionTicket ticket)
        {
            LogRequest("Connect", ticket.ClientComputer);
            return ServiceContext.SessionManager.CreateSession(ticket);
        }

        public bool Disconnect(SessionId sessionId)
        {
            LogRequest("Disconnect", sessionId);
            return ServiceContext.SessionManager.CloseSession(sessionId);
        }

        public bool GetLock(SessionId sessionId)
        {
            LogRequest("BeginGetLock", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return ServiceContext.DataManager.GetDatabaseLock(sessionId);
        }

        public bool EndGetLock(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public IAsyncResult BeginGetCurrentAccountingData(SessionId sessionId, bool reloadAll, AsyncCallback callback, object state)
        {
            LogRequest("BeginGetCurrentAccountingData", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return GetBeginTask(ServiceContext.DataManager.GetCurrentAccountingData(sessionId, reloadAll), callback, state);
        }

        public bool EndGetCurrentAccountingData(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public IAsyncResult BeginCalculationAndSave(SessionId sessionId, AsyncCallback callback, object state)
        {
            LogRequest("BeginCalculateData", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return GetBeginTask(ServiceContext.DataManager.CalculateAndSave(sessionId), callback, state);
        }

        public bool EndCalculationAndSave(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public UserData CreateUser(SessionId sessionId, string username)
        {
            LogRequest("CreateUser", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return ServiceContext.SessionManager.CreateUser(username);
        }

        public List<UserData> GetUsers(SessionId sessionId)
        {
            LogRequest("GetUsers", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return ServiceContext.SessionManager.Users;
        }

        public bool ChangeUserName(SessionId sessionId, Guid userId, string newName)
        {
            LogRequest("ChangeUserName", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return ServiceContext.SessionManager.ChangeUserName(userId, newName);
        }

        public bool RemoveUser(SessionId sessionId, Guid userId)
        {
            LogRequest("RemoveUser", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return ServiceContext.SessionManager.RemoveUser(userId);
        }

        public UserData GetCurrentUserInfo(SessionId sessionId)
        {
            LogRequest("GetCurrentUserInfo", sessionId);
            ServiceContext.SessionManager.EnsureAuthorizedSession(sessionId);
            return ServiceContext.SessionManager.GetRelatedUserInfo(sessionId);
        }

        #endregion

        #region Internals and Helpers

        private Task<TResult> GetBeginTask<TResult>(Task<TResult> task, AsyncCallback callback, object state)
        {
            TaskCompletionSource<TResult> source = new TaskCompletionSource<TResult>(state);

            task.ContinueWith(delegate
                {
                    if (task.IsFaulted)
                    {
                        if (task.Exception != null)
                        {
                            source.TrySetException(task.Exception.InnerExceptions);
                        }
                    }
                    else if (task.IsCanceled)
                    {
                        source.TrySetCanceled();
                    }
                    else
                    {
                        source.TrySetResult(task.Result);
                    }

                    if (callback != null)
                    {
                        callback(source.Task);
                    }
                }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            return source.Task;
        }

        #endregion
    }
}