using System;
using System.Windows.Input;
using CommonBase;
using CommonBase.UI.Localization;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Storage
{
    public class EditStorageViewModel : ViewModelBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly Action _closeAction;
        private readonly bool _isNew;

        private string _name;
        private string _prefix;
        private string _company;
        private string _street;
        private string _number;
        private string _zip;
        private string _city;
        private string _companyId;
        private string _taxId;

        public EditStorageViewModel(IDataAccess dataAccess, Action closeAction, IExternStorage externStorage = null)
        {
            _dataAccess = dataAccess;
            _closeAction = closeAction;
            _isNew = externStorage == null;
            ExternStorage = externStorage;
            Init();
        }

        #region Properties

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Prefix
        {
            get { return _prefix; }
            set
            {
                _prefix = value;
                NotifyPropertyChanged("Prefix");
            }
        }

        public string CompanyName
        {
            get { return _company; }
            set
            {
                _company = value;
                NotifyPropertyChanged("CompanyName");
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

        public string CompanyId
        {
            get { return _companyId; }
            set
            {
                _companyId = value;
                NotifyPropertyChanged("CompanyId");
            }
        }

        public string TaxId
        {
            get { return _taxId; }
            set
            {
                _taxId = value;
                NotifyPropertyChanged("TaxId");
            }
        }

        public string WindowTitle
        {
            get { return _isNew ? "AddStorage".Localize() : String.Format("{0} - {1}", "EditStorage".Localize(), Name); }
        }

        public ICommand SaveStorageCommand
        {
            get
            {
                return new RelayCommand(ExecuteSaveStorageCommand, CanExecuteSaveStorageCommand);
            }
        }

        public IExternStorage ExternStorage { get; private set; }

        public bool StorageAdded { get; private set; }

        public bool IsValid { get; private set; }

        #endregion

        #region Internals and Helpers

        private void Init()
        {
            Name = ExternStorage != null ? ExternStorage.Name : String.Empty;
            Prefix = ExternStorage != null ? ExternStorage.Prefix : String.Empty;
            CompanyName = ExternStorage != null ? ExternStorage.CompanyName : String.Empty;
            Street = ExternStorage != null ? ExternStorage.Street : String.Empty;
            Number = ExternStorage != null ? ExternStorage.Number : String.Empty;
            ZipCode = ExternStorage != null ? ExternStorage.ZipCode : String.Empty;
            City = ExternStorage != null ? ExternStorage.City : String.Empty;
            CompanyId = ExternStorage != null ? ExternStorage.CompanyId : String.Empty;
            TaxId = ExternStorage != null ? ExternStorage.TaxId : String.Empty;
        }

        private bool CanExecuteSaveStorageCommand(object param)
        {
            bool canSave = true;

            if (String.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

            canSave &= !_dataAccess.ExistsExternStorage(Name, _isNew ? ObjectId.Empty : ExternStorage.StorageId);
            canSave &= !String.IsNullOrWhiteSpace(CompanyName);
            canSave &= !String.IsNullOrWhiteSpace(Number);
            canSave &= !String.IsNullOrWhiteSpace(ZipCode);
            canSave &= !String.IsNullOrWhiteSpace(City);

            return canSave;
        }

        private void ExecuteSaveStorageCommand(object param)
        {
            StorageAdded = false;

            if (_isNew)
            {
                ExternStorage = _dataAccess.CreateExternStorage(Name, CompanyName, Street, Number, ZipCode, City, CompanyId, TaxId);
                StorageAdded = true;
            }
            else
            {
                ExternStorage.Name = Name;
                ExternStorage.Prefix = Prefix;
                ExternStorage.CompanyName = CompanyName;
                ExternStorage.Street = Street ?? String.Empty;
                ExternStorage.Number = Number;
                ExternStorage.ZipCode = ZipCode;
                ExternStorage.City = City;
                ExternStorage.CompanyId = CompanyId ?? String.Empty;
                ExternStorage.TaxId = TaxId ?? String.Empty;
                ExternStorage.Save();
            }
            
            IsValid = true;
            _closeAction();
        }

        #endregion
    }
}