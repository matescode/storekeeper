using System;

namespace StoreKeeper.Common.DataContracts.Server
{
    public class ActiveSession
    {
        public ActiveSession()
        {
            Created = DateTime.Now;
        }

        #region Properties

        public Guid Id { get; set; }

        public string SessionId { get; set; }

        public DateTime Created { get; set; }

        public string ClientComputer { get; set; }

        public int Port { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; } 

        #endregion
    }
}