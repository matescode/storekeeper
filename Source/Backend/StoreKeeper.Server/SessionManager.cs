using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using CommonBase;
using CommonBase.Application;
using CommonBase.Log;
using CommonBase.Utils;

using StoreKeeper.Common;
using StoreKeeper.Common.DataContracts.Server;
using StoreKeeper.Common.Exceptions;

namespace StoreKeeper.Server
{
    internal class SessionManager : ISessionManager
    {
#if DEBUG
        private const int CheckConnectedUsersTimeInterval = 120 * 1000;
#else
        private const int CheckConnectedUsersTimeInterval = 15 * 60 * 1000;
#endif

        private static readonly ILogger Logger = LogManager.GetLogger(typeof(SessionManager));
        private readonly ConcurrentDictionary<SessionId, SessionData> _sessions;

        private Timer _checkConnectedUsersTimer;

        private readonly object _lockObj = new object();

        internal SessionManager()
        {
            _sessions = new ConcurrentDictionary<SessionId, SessionData>();
        }

        #region ISessionManager Members

        public List<UserData> Users
        {
            get
            {
                lock (_lockObj)
                {
                    try
                    {
                        using (ServerDataContext context = new ServerDataContext())
                        {
                            return context.Users.Select(context.CreateUserData).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        ApplicationContext.Log.Error(GetType(), ex);
                        throw;
                    }
                }
            }
        }

        public UserData CreateUser(string username)
        {
            lock (_lockObj)
            {
                using (ServerDataContext context = new ServerDataContext())
                {
                    User user = context.Users.Create();
                    user.Id = ObjectId.NewId();
                    user.Name = username;
                    user.SecurityToken = ObjectId.NewId().ToString();
                    context.Users.Add(user);
                    context.SaveChanges();
                    return context.CreateUserData(user);
                }
            }
        }

        public bool ChangeUserName(Guid userId, string newName)
        {
            lock (_lockObj)
            {
                using (ServerDataContext context = new ServerDataContext())
                {
                    User user = context.Users.Find(userId);
                    if (user == null)
                    {
                        return false;
                    }
                    user.Name = newName;
                    context.SaveChanges();
                    return true;
                }
            }
        }

        public bool RemoveUser(Guid userId)
        {
            lock (_lockObj)
            {
                using (ServerDataContext context = new ServerDataContext())
                {
                    User user = context.Users.Find(userId);
                    if (user == null)
                    {
                        return false;
                    }

                    context.Users.Remove(user);
                    context.SaveChanges();
                    return true;
                }
            }
        }

        public SessionId CreateSession(ConnectionTicket ticket)
        {
            ArgumentValidator.IsNotNull("ticket", ticket, "Ticket cannot be null!");

            using (ClientProxy clientProxy = CreateClientProxy(ticket))
            {
                try
                {
                    SessionId sessionId = CreateSessionId(ticket);

                    if (_sessions.ContainsKey(sessionId))
                    {
                        return sessionId;
                    }

                    string validationHash = clientProxy.ValidateConnection(Constants.ClientConnectionHash);
                    if (string.Compare(validationHash, Constants.ClientValidResultHash) != 0)
                    {
                        throw new ClientNotValidException(GetType());
                    }

                    User user;
                    if (!CheckUser(ticket.Username, ticket.SecurityToken, out user))
                    {
                        throw new UserIsNotValidException(GetType(), ticket.Username);
                    }

                    if (!_sessions.TryAdd(sessionId, new SessionData(sessionId, ticket)))
                    {
                        throw new CannotCreateSessionException(GetType());
                    }
                    SaveSessionToDatabase(sessionId, user, ticket);
                    InitCheckTimer();
                    Logger.Info("User {0} has been connected (session: {1}).", ticket.Username, sessionId);
                    return sessionId;
                }
                catch (CommunicationException e)
                {
                    ApplicationContext.Log.Error(GetType(), e);
                }
                catch (ClientNotValidException e)
                {
                    ApplicationContext.Log.Error(GetType(), e);
                    throw;
                }
                catch (Exception e)
                {
                    ApplicationContext.Log.Error(GetType(), e);
                }
            }
            return null;
        }

        public bool CloseSession(SessionId sessionId)
        {
            ArgumentValidator.IsNotNull("sessionId", sessionId, "Session cannot be null!");

            SessionData data;
            if (_sessions.TryRemove(sessionId, out data))
            {
                RemoveSessionFromDatabase(sessionId);
                Logger.Info("User {0} has been disconnected (session: {1}).", data.Ticket.Username, sessionId);
                return true;
            }
            if (!_sessions.Any())
            {
                ReleaseCheckTimer();
            }
            return false;
        }

        public void Close()
        {
            foreach (SessionData s in _sessions.Values)
            {
                Logger.Info("Closing session to {0}.", s.Ticket.ClientComputer);

                try
                {
                    using (var clientProxy = CreateClientProxy(s.Ticket))
                    {
                        if (!clientProxy.ClosingConnection())
                        {
                            Logger.Warning("Cannot close session {0}.", s.SessionId);
                        }
                    }
                }
                catch (Exception e)
                {
                    ApplicationContext.Log.Error(GetType(), e);
                }
            }

            ReleaseCheckTimer();
        }

        public IClientInfrastructure CreateClientProxy(SessionId sessionId)
        {
            SessionData sessionData;
            if (!_sessions.TryGetValue(sessionId, out sessionData))
            {
                throw new NotRegisteredSessionException(GetType(), sessionId);
            }
            return CreateClientProxy(sessionData.Ticket);
        }

        public void ReconnectSessions()
        {
            using (ServerDataContext context = new ServerDataContext())
            {
                List<SessionId> toRemove = new List<SessionId>();
                foreach (ActiveSession session in context.ActiveSessions)
                {
                    SessionId sessionId = new SessionId(session.SessionId);
                    SessionData data = new SessionData(sessionId, context.CreateFakeTicket(session));
                    _sessions.TryAdd(sessionId, data);
                    try
                    {
                        using (ClientProxy proxy = CreateClientProxy(data.Ticket))
                        {
                            proxy.ConnectionRestarted();
                            Logger.Info("Restarted session to computer {0}.", data.Ticket.ClientComputer);
                        }
                    }
                    catch (Exception e)
                    {
                        toRemove.Add(sessionId);
                        ApplicationContext.Log.Error(GetType(), e);
                        ApplicationContext.Log.Info(GetType(), "Removed session to: {0}", data.Ticket.ClientComputer);
                    }
                }

                if (toRemove.Count > 0)
                {
                    foreach (SessionId sessionId in toRemove)
                    {
                        SessionData tmpData;
                        context.RemoveSession(sessionId);
                        _sessions.TryRemove(sessionId, out tmpData);
                    }
                    context.SaveChanges();
                }
            }
        }

        public void EnsureAuthorizedSession(SessionId sessionId)
        {
            using (ServerDataContext context = new ServerDataContext())
            {
                if (!context.CheckSession(sessionId))
                {
                    throw new NotAuthorizedRequestException(GetType(), sessionId);
                }
            }
        }

        public void NotifyDataUpdated(SessionId callerSessionId)
        {
            Task.Factory.StartNew(() =>
                {
                    foreach (SessionData sesionData in _sessions.Values.Where(data => data.SessionId != callerSessionId))
                    {
                        IClientInfrastructure clientProxy = CreateClientProxy(sesionData.SessionId);
                        clientProxy.DataUpdated();
                        Logger.Info(LogId.DataUpdatedNotification,
                                    "Send DataUpdated notificaiton to client '{0}'.",
                                    sesionData.Ticket.ClientComputer);
                    }
                });
        }

        public void NotifyDatabaseLockChanged(SessionId callerSessionId)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (SessionData sesionData in _sessions.Values.Where(data => data.SessionId != callerSessionId))
                {
                    IClientInfrastructure clientProxy = CreateClientProxy(sesionData.SessionId);
                    clientProxy.DatabaseLockChanged();
                    Logger.Info(LogId.DatabaseLockChanged,
                                "Send DatabaseLockChanged notificaiton to client '{0}'.",
                                sesionData.Ticket.ClientComputer);
                }
            });
        }

        public string GetRelatedUserName(SessionId sessionId)
        {
            using (ServerDataContext context = new ServerDataContext())
            {
                ActiveSession session = context.GetSession(sessionId);
                return session.User.Name;
            }
        }

        public string GetRelatedUserId(SessionId sessionId)
        {
            using (ServerDataContext context = new ServerDataContext())
            {
                ActiveSession session = context.GetSession(sessionId);
                return session.User.Id.ToString().ToUpper();
            }
        }

        public UserData GetRelatedUserInfo(SessionId sessionId)
        {
            using (ServerDataContext context = new ServerDataContext())
            {
                ActiveSession session = context.GetSession(sessionId);
                return context.CreateUserData(session.User);
            }
        }

        #endregion

        #region Internals and Helpers

        private SessionId CreateSessionId(ConnectionTicket ticket)
        {
            return new SessionId(string.Format("#ses#{0}#{1}#", ticket.ClientComputer, ticket.Username));
        }

        private bool CheckUser(string username, string token, out User user)
        {
            lock (_lockObj)
            {
                using (ServerDataContext context = new ServerDataContext())
                {
                    User result = context.GetUser(username);
                    if (result != null)
                    {
                        if (result.SecurityToken == token)
                        {
                            user = result;
                            return true;
                        }
                    }
                }
            }
            user = null;
            return false;
        }

        private void SaveSessionToDatabase(SessionId sessionId, User user, ConnectionTicket ticket)
        {
            lock (_lockObj)
            {
                try
                {
                    using (ServerDataContext context = new ServerDataContext())
                    {
                        ActiveSession session = new ActiveSession
                            {
                                Id = ObjectId.NewId(),
                                SessionId = sessionId.ToString(),
                                UserId = user.Id,
                                ClientComputer = ticket.ClientComputer,
                                Port = ticket.Port
                            };
                        context.ActiveSessions.Add(session);
                        context.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ApplicationContext.Log.Error(GetType(), e);
                    throw;
                }
            }
        }

        private void RemoveSessionFromDatabase(SessionId sessionId)
        {
            lock (_lockObj)
            {
                try
                {
                    using (ServerDataContext context = new ServerDataContext())
                    {
                        context.RemoveSession(sessionId);
                        context.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ApplicationContext.Log.Error(GetType(), e);
                    throw;
                }
            }
        }

        private ClientProxy CreateClientProxy(ConnectionTicket ticket)
        {
            return new ClientProxy(new ClientServiceDescriptor(ticket));
        }

        private void CheckConnectedUsersCallback(object state)
        {
            Logger.Info("Checking of connected users.");
            foreach (SessionData sessionData in _sessions.Values)
            {
                bool testFailed = false;
                Exception exc = null;
                try
                {
                    using (ClientProxy proxy = CreateClientProxy(sessionData.Ticket))
                    {
                        if (!proxy.IsActive())
                        {
                            testFailed = true;
                        }
                    }
                }
                catch (CommunicationException e)
                {
                    testFailed = true;
                    exc = e;
                }
                catch (Exception e)
                {
                    testFailed = true;
                    exc = e;
                }

                if (testFailed)
                {
                    if (exc != null)
                    {
                        Logger.Error(exc);
                    }
                    Logger.Info("Closing session to not available client computer '{0}'.", sessionData.Ticket.ClientComputer);
                    CloseSession(sessionData.SessionId);
                }
            }
        }

        private void InitCheckTimer()
        {
            if (_checkConnectedUsersTimer != null)
            {
                return;
            }

            _checkConnectedUsersTimer = new Timer(
                CheckConnectedUsersCallback,
                null,
                CheckConnectedUsersTimeInterval,
                CheckConnectedUsersTimeInterval
            );

            Logger.Info("Session check timer started.");
        }

        private void ReleaseCheckTimer()
        {
            if (_checkConnectedUsersTimer == null)
            {
                return;
            }

            Logger.Info("Releasing session check timer.");
            _checkConnectedUsersTimer.Dispose();
            _checkConnectedUsersTimer = null;
        }

        #endregion

        #region Inner classes

        private class SessionData
        {
            public SessionData(SessionId sessionId, ConnectionTicket ticket)
            {
                SessionId = sessionId;
                Ticket = ticket;
                Created = DateTime.Now;
            }

            public SessionId SessionId { get; set; }

            public ConnectionTicket Ticket { get; set; }

            public DateTime Created { get; private set; }
        }

        private class ClientServiceDescriptor : IServiceDescriptor
        {
            private readonly ConnectionTicket _ticket;

            public ClientServiceDescriptor(ConnectionTicket ticket)
            {
                _ticket = ticket;
            }

            #region IServiceDescriptor Implementation

            public bool Secured
            {
                get { return false; }
            }

            public string Server
            {
                get { return _ticket.ClientComputer; }
            }

            public int Port
            {
                get { return _ticket.Port; }
            }

            public string ServiceName
            {
                get { return Constants.ClientHostServiceName; }
            }

            #endregion
        }

        #endregion
    }
}