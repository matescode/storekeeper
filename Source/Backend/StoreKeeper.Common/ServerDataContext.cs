using System.Data.Entity;
using System.Linq;

using CommonBase;
using CommonBase.Utils;

using StoreKeeper.Common.DataContracts.Server;

namespace StoreKeeper.Common
{
    public class ServerDataContext : DbContext
    {
        public ServerDataContext()
            : base(ConnectionStringHolder.Value)
        {
            Database.SetInitializer<ServerDataContext>(null);
        }

        #region Entities

        public DbSet<User> Users { get; set; }

        public DbSet<ActiveSession> ActiveSessions { get; set; }

        #endregion

        #region Public Methods

        public User GetUser(string name)
        {
            ArgumentValidator.IsTrue("name", !string.IsNullOrEmpty(name));
            return Users.FirstOrDefault(u => u.Name == name);
        }

        public User GetUser(ObjectId id)
        {
            ArgumentValidator.IsTrue("id", !ObjectId.IsNullOrEmpty(id));
            return Users.FirstOrDefault(u => new ObjectId(u.Id) == id);
        }

        public bool RemoveUser(ObjectId id)
        {
            ArgumentValidator.IsTrue("id", !ObjectId.IsNullOrEmpty(id));
            User user = GetUser(id);
            if (user == null)
            {
                return false;
            }
            Users.Remove(user);
            return true;
        }

        public ActiveSession GetSession(SessionId sessionId)
        {
            ArgumentValidator.IsNotNull("sessionId", sessionId);
            string sessionStr = sessionId.ToString();
            return ActiveSessions.FirstOrDefault(s => s.SessionId == sessionStr);
        }

        public bool RemoveSession(SessionId sessionId)
        {
            ArgumentValidator.IsNotNull("sessionId", sessionId);
            ActiveSession session = GetSession(sessionId);
            if (session == null)
            {
                return false;
            }
            ActiveSessions.Remove(session);
            return true;
        }

        public bool CheckSession(SessionId sessionId)
        {
            ArgumentValidator.IsNotNull("sessionId", sessionId);
            ActiveSession session = GetSession(sessionId);
            return session != null;
        }

        public ConnectionTicket CreateFakeTicket(ActiveSession session)
        {
            ConnectionTicket ticket = new ConnectionTicket();
            ticket.ClientComputer = session.ClientComputer;
            ticket.Username = session.User.Name;
            ticket.Port = session.Port;
            return ticket;
        }

        public UserData CreateUserData(User user)
        {
            return new UserData { Id = user.Id, Name = user.Name, SecurityToken = user.SecurityToken };
        }

        #endregion
    }
}