using System;
using StoreKeeper.Common.DataContracts.Server;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class ServerUser : IServerUser
    {
        private readonly UserData _userData;
        private readonly Func<Guid, string, bool> _nameSetter;

        public ServerUser(UserData userData, Func<Guid, string, bool> nameSetter)
        {
            _userData = userData;
            _nameSetter = nameSetter;
        }

        #region IServerUser Implementation

        public Guid UserId
        {
            get { return _userData.Id; }
        }

        public string Name
        {
            get { return _userData.Name; }
            set
            {
                if (_nameSetter(_userData.Id, value))
                {
                    _userData.Name = value;
                }
            }
        }

        public string SecurityToken
        {
            get { return _userData.SecurityToken; }
        }

        #endregion
    }
}