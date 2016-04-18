using System.Collections.Generic;

using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.ServerAdministration
{
    public class ServerUserListViewModel : ListViewModelBase<ServerUserViewModel, IServerUser>
    {
        private readonly IServerAdministration _serverAdministration;

        public ServerUserListViewModel(IServerAdministration serverAdministration)
            : base(null)
        {
            _serverAdministration = serverAdministration;
        }

        #region Overrides

        protected override ServerUserViewModel CreateViewModel(IServerUser item)
        {
            return new ServerUserViewModel(item);
        }

        protected override IEnumerable<IServerUser> LoadData()
        {
            return _serverAdministration.Users;
        }

        #endregion

        #region Public Methods

        public void CreateNewUser(string name)
        {
            IServerUser user = _serverAdministration.CreateUser(name);
            if (user != null)
            {
                Add(user);
            }
        }

        public void RemoveUser(ServerUserViewModel userViewModel)
        {
            if (userViewModel == null)
            {
                return;
            }

            Remove(userViewModel);
            _serverAdministration.RemoveUser(userViewModel.Item);
            NotifyPropertyChanged("Data");
        }

        #endregion
    }
}