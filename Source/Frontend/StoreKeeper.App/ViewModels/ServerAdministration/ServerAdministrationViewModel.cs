using System;
using System.Windows;
using System.Windows.Input;
using StoreKeeper.Client;

namespace StoreKeeper.App.ViewModels.ServerAdministration
{
    public class ServerAdministrationViewModel : ViewModelBase
    {
        private readonly IServerAdministration _serverAdministration;
        private ServerUserListViewModel _serverUserList;
        private string _username;

        public ServerAdministrationViewModel(IServerAdministration serverAdministration)
        {
            _serverAdministration = serverAdministration;
        }

        #region Properties

        public string LastAccountingUpdate
        {
            get { return _serverAdministration.LastAccountingUpdate.ToString("dd.MM.yyyy HH:mm:ss"); }
        }

        public string ResponsibleUser
        {
            get { return _serverAdministration.ResponsibleUser; }
        }

        public string LockedBy
        {
            get { return _serverAdministration.LockedBy; }
        }

        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyPropertyChanged("UserName");
            }
        }

        public ServerUserListViewModel ServerUserList
        {
            get
            {
                if (_serverUserList == null)
                {
                    _serverUserList = new ServerUserListViewModel(_serverAdministration);
                    _serverUserList.Load();
                }
                return _serverUserList;
            }
        }

        public ICommand AddUserCommand
        {
            get
            {
                return new RelayCommand(ExecuteAddUserCommand, CanExecuteAddUserCommand);
            }
        }

        public ICommand DeleteUserCommand
        {
            get
            {
                return new RelayCommand(ExecuteDeleteUserCommand, CanExecuteDeleteUserCommand);
            }
        }

        public ICommand CopyTokenCommand
        {
            get
            {
                return new RelayCommand(ExecuteCopyTokenCommand, CanExecuteCopyTokenCommand);
            }
        }

        #endregion

        #region Internals and Helpers

        private bool CanExecuteAddUserCommand(object param)
        {
            return !String.IsNullOrWhiteSpace(UserName);
        }

        private void ExecuteAddUserCommand(object param)
        {
            ServerUserList.CreateNewUser(UserName);
            UserName = String.Empty;
        }

        private bool CanExecuteDeleteUserCommand(object param)
        {
            if (param == null)
            {
                return false;
            }

            ServerUserViewModel viewModel = (ServerUserViewModel)param;
            return String.Compare(viewModel.Item.Name, AppContext.Config.User, StringComparison.InvariantCulture) != 0;
        }

        private void ExecuteDeleteUserCommand(object param)
        {
            ServerUserList.RemoveUser(param as ServerUserViewModel);
        }

        private bool CanExecuteCopyTokenCommand(object param)
        {
            ServerUserViewModel viewModel = param as ServerUserViewModel;
            return viewModel != null;
        }

        private void ExecuteCopyTokenCommand(object param)
        {
            ServerUserViewModel viewModel = param as ServerUserViewModel;
            if (viewModel == null)
            {
                return;
            }

            Clipboard.SetText(viewModel.Item.SecurityToken);
        }

        #endregion
    }
}