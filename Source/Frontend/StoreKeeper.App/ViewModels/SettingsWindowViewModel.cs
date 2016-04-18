using System;
using System.Windows.Input;
using CommonBase.UI;
using CommonBase.UI.Localization;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        private readonly IDeliveryNoteSettings _deliveryNoteSettings;
        private readonly Action _saveAction;
        private string _serverName;
        private int _serverPort;
        private string _userName;
        private string _securityToken;
        private int _clientPort;
        private int _seekLimit;
        private bool _needRestart;

        private string _createdByName;
        private string _createdByEmail;
        private string _parlor;
        private string _street;
        private string _number;
        private string _zip;
        private string _city;
        private string _phone;
        private string _cellPhone;
        private string _email;
        private string _web;

        public SettingsWindowViewModel(IDeliveryNoteSettings deliveryNoteSettings, Action saveAction)
        {
            _deliveryNoteSettings = deliveryNoteSettings;
            _saveAction = saveAction;
            _serverName = AppContext.Config.ServerName;
            _serverPort = AppContext.Config.ServerPort;
            _userName = AppContext.Config.User;
            _securityToken = AppContext.Config.SecurityToken;
            _clientPort = AppContext.Config.ClientPort;
            _seekLimit = AppContext.Config.SeekCodeCharLimit;

            _createdByName = AppContext.Config.DeliveryNoteCreatedByName;
            _createdByEmail = AppContext.Config.DeliveryNoteCreatedByMail;
            _parlor = _deliveryNoteSettings.Parlor;
            _street = _deliveryNoteSettings.Street;
            _number = _deliveryNoteSettings.Number;
            _zip = _deliveryNoteSettings.ZipCode;
            _city = _deliveryNoteSettings.City;
            _phone = _deliveryNoteSettings.Phone;
            _cellPhone = _deliveryNoteSettings.CellPhone;
            _email = _deliveryNoteSettings.Email;
            _web = _deliveryNoteSettings.Web;
        }

        public string ServerName
        {
            get { return _serverName; }
            set
            {
                _serverName = value;
                _needRestart = true;
                NotifyPropertyChanged("ServerName");
            }
        }

        public string ServerPort
        {
            get { return _serverPort.ToString(); }
            set
            {
                int result;
                if (!int.TryParse(value, out result))
                {
                    result = 0;
                }
                _serverPort = result;
                _needRestart = true;
                NotifyPropertyChanged("ServerPort");
            }
        }

        public string Username
        {
            get { return _userName; }
            set
            {
                _userName = value;
                _needRestart = true;
                NotifyPropertyChanged("Username");
            }
        }

        public string SecurityToken
        {
            get { return _securityToken; }
            set
            {
                _securityToken = value;
                _needRestart = true;
                NotifyPropertyChanged("SecurityToken");
            }
        }

        public string ClientPort
        {
            get { return _clientPort.ToString(); }
            set
            {
                int result;
                if (!int.TryParse(value, out result))
                {
                    result = 0;
                }
                _clientPort = result;
                _needRestart = true;
                NotifyPropertyChanged("ClientPort");
            }
        }

        public string SeekLimit
        {
            get { return _seekLimit.ToString(); }
            set
            {
                int result;
                if (!int.TryParse(value, out result))
                {
                    result = 0;
                }
                _seekLimit = result;
                NotifyPropertyChanged("SeekLimit");
            }
        }

        public string CreatedByName
        {
            get { return _createdByName; }
            set
            {
                _createdByName = value;
                NotifyPropertyChanged("CreatedByName");
            }
        }

        public string CreatedByEmail
        {
            get { return _createdByEmail; }
            set
            {
                _createdByEmail = value;
                NotifyPropertyChanged("CreatedByEmail");
            }
        }

        public string Parlor
        {
            get { return _parlor; }
            set
            {
                _parlor = value;
                NotifyPropertyChanged("Parlor");
            }
        }

        public string Street
        {
            get { return _street; }
            set
            {
                _street = value;
                NotifyPropertyChanged("Street");
            }
        }

        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                NotifyPropertyChanged("Number");
            }
        }

        public string ZipCode
        {
            get { return _zip; }
            set
            {
                _zip = value;
                NotifyPropertyChanged("ZipCode");
            }
        }

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                NotifyPropertyChanged("City");
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        public string CellPhone
        {
            get { return _cellPhone; }
            set
            {
                _cellPhone = value;
                NotifyPropertyChanged("CellPhone");
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyPropertyChanged("Email");
            }
        }

        public string Web
        {
            get { return _web; }
            set
            {
                _web = value;
                NotifyPropertyChanged("Web");
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            }
        }

        #region Internals and Helpers

        private bool CanExecuteSaveCommand(object param)
        {
            bool result = true;
            result &= !String.IsNullOrWhiteSpace(ServerName);
            result &= _serverPort > 0 && _serverPort < 65535;
            result &= !String.IsNullOrWhiteSpace(Username);
            result &= !String.IsNullOrWhiteSpace(SecurityToken);
            result &= _clientPort > 0 && _clientPort < 65535;
            result &= _seekLimit > 0 && _seekLimit < 20;

            // result &= !String.IsNullOrWhiteSpace();

            return result;
        }

        private void ExecuteSaveCommand(object param)
        {
            try
            {
                AppContext.Config.ServerName = _serverName;
                AppContext.Config.ServerPort = _serverPort;
                AppContext.Config.User = _userName;
                AppContext.Config.SecurityToken = _securityToken;
                AppContext.Config.ClientPort = _clientPort;
                AppContext.Config.SeekCodeCharLimit = _seekLimit;
                AppContext.Config.DeliveryNoteCreatedByName = _createdByName;
                AppContext.Config.DeliveryNoteCreatedByMail = _createdByEmail;

                _deliveryNoteSettings.Parlor = _parlor;
                _deliveryNoteSettings.Street = _street;
                _deliveryNoteSettings.Number = _number;
                _deliveryNoteSettings.ZipCode = _zip;
                _deliveryNoteSettings.City = _city;
                _deliveryNoteSettings.Phone = _phone;
                _deliveryNoteSettings.CellPhone = _cellPhone;
                _deliveryNoteSettings.Email = _email;
                _deliveryNoteSettings.Web = _web;

                _deliveryNoteSettings.Save();

                if (_needRestart)
                {
                    UIApplication.MessageDialogs.Warning("SettingsSavedWarning".Localize());
                }
                else
                {
                    UIApplication.MessageDialogs.Info("SettingsSavedConfirmation".Localize());
                }

                _saveAction();
            }
            catch (Exception ex)
            {
                UIApplication.MessageDialogs.Error(ex);
            }
        }

        #endregion
    }
}