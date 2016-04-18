using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

using CommonBase;
using CommonBase.Application;
using CommonBase.UI.Localization;

using StoreKeeper.App.Printing;
using StoreKeeper.App.Printing.DeliveryNote;
using StoreKeeper.App.Searching;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.App.ViewModels.Storage;
using StoreKeeper.App.Windows;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;
using StoreKeeper.Common;

namespace StoreKeeper.App.ViewModels.ExternStorage
{
    public class ExternStorageMaterialListViewModel : ListViewModelBase<ExternStorageMaterialViewModel, IExternStorageMaterial>, ISearchProvider, IPrintingContext, IDeliveryNoteDataSource
    {
        private CollectionView _dataView;
        private IEnumerable<PrintColumnDefinition> _printColumnDefinitions;
        private readonly StorageListProvider _storageListProvider;
        private readonly ICurrentStorageHolder _storageHolder;
        private StorageComboBoxItem _selectedStorageItem;
        private bool _isSelectedAll;

        public ExternStorageMaterialListViewModel(IDataAccess dataAccess, ICurrentStorageHolder storageHolder)
            : base(dataAccess)
        {
            _storageListProvider = new StorageListProvider();
            _storageHolder = storageHolder;
            SelectedStorageItem = _storageListProvider.Find(_storageHolder.CurrentStorage);
            _isSelectedAll = false;
        }

        #region Properties

        public ObservableCollection<StorageComboBoxItem> StorageListData
        {
            get { return _storageListProvider.GetDataForExternStorages(); }
        }

        public StorageComboBoxItem SelectedStorageItem
        {
            get { return _selectedStorageItem; }
            set
            {
                IsSelectedAll = false;
                _selectedStorageItem = value;
                _storageHolder.CurrentStorage = _selectedStorageItem.Id;
                DataView.Refresh();
                NotifyPropertyChanged("SelectedStorageItem");
                NotifyPropertyChanged("IsSelectAllEnabled");
            }
        }

        public CollectionView DataView
        {
            get
            {
                if (_dataView == null)
                {
                    _dataView = (CollectionView)CollectionViewSource.GetDefaultView(Data);
                    _dataView.Filter = FilterMethod;
                }

                return _dataView;
            }
        }

        public ICommand MissingMaterialPrintCommand
        {
            get
            {
                return new RelayCommand(ExecuteMissingMaterialPrintCommand, CanExecuteMissingMaterialPrintCommand);
            }
        }

        public ICommand PrintDeliveryNoteCommand
        {
            get
            {
                return new RelayCommand(ExecutePrintDeliveryNoteCommand, CanExecutePrintDeliveryNoteCommand);
            }
        }

        public bool IsSelectedAll
        {
            get { return _isSelectedAll; }
            set
            {
                _isSelectedAll = value;
                SelectAll(_isSelectedAll);
                NotifyPropertyChanged("IsSelectedAll");
            }
        }

        public bool IsSelectAllEnabled
        {
            get
            {
                bool enabled = SelectedStorageItem.Id != ObjectId.Empty;
                EnableAll(enabled);
                return enabled;
            }
        }

        #endregion

        #region ISearchProvider Implementation

        public object FindItem(string codePrefix)
        {
            return Data.FirstOrDefault(v => (v.Item.StorageId == SelectedStorageItem.Id || SelectedStorageItem.Id == StorageListProvider.AllStoragesItem.Id) && v.Code.ToUpper().StartsWith(codePrefix.ToUpper()));
        }

        #endregion

        #region Overrides

        protected override IEnumerable<IExternStorageMaterial> LoadData()
        {
            return DataAccess.ExternStorageMaterials;
        }

        protected override ExternStorageMaterialViewModel CreateViewModel(IExternStorageMaterial item)
        {
            return new ExternStorageMaterialViewModel(item);
        }

        #endregion

        #region IPrintingContext Implementation

        public string Label
        {
            get { return String.Format("{0} - {1}", "MissingMaterial".Localize(), SelectedStorageItem.Name); }
        }

        public IEnumerable<PrintColumnDefinition> Columns
        {
            get
            {
                return _printColumnDefinitions
                        ??
                        (_printColumnDefinitions = new List<PrintColumnDefinition>
                    {
                        new PrintColumnDefinition("Code".Localize(), "Code"),
                        new PrintColumnDefinition("NameDescription".Localize(), "NameDescription", 550),
                        new PrintColumnDefinition("ToDeliverCount".Localize(), "DeliverCount", 80, true)
                    });
            }
        }

        public List<object> DataSource
        {
            get { return Data.Where(i => i.Item.StorageId == SelectedStorageItem.Id && i.Item.MissingCount > 0).Select(item => (object)item).ToList(); }
        }

        #endregion

        #region IDeliveryNoteDataSource Implementation

        public List<IDeliveryNoteItem> Items
        {
            get { return Data.Where(i => i.Item.StorageId == SelectedStorageItem.Id && i.IsSelected).Select(i => i as IDeliveryNoteItem).Where(i => i.AmountValue > 0).ToList(); }
        }

        public IDeliveryNoteOrganization Supplier
        {
            get
            {
                return new DeliveryNoteOrganization(DataAccess.Storages.First(s => s.StorageId == (ObjectId)Constants.CentralStorageId));
            }
        }

        public IDeliveryNoteOrganization Subscriber
        {
            get
            {
                return new DeliveryNoteOrganization(DataAccess.Storages.First(s => s.StorageId == SelectedStorageItem.Id));
            }
        }

        public IDeliveryNoteSettings Settings
        {
            get { return DataAccess.DeliveryNoteSettings; }
        }

        #endregion

        #region Public Methods

        public void ReloadStorageList()
        {
            _storageListProvider.Reload();
            SelectedStorageItem = StorageListProvider.AllStoragesItem;
            NotifyPropertyChanged("StorageListData");
        }

        #endregion

        #region Internals and Helpers

        private bool FilterMethod(object item)
        {
            ExternStorageMaterialViewModel itemViewModel = (ExternStorageMaterialViewModel)item;
            return (SelectedStorageItem.Id == ObjectId.Empty || SelectedStorageItem.Id == itemViewModel.Item.StorageId);
        }

        private bool CanExecuteMissingMaterialPrintCommand(object param)
        {
            return SelectedStorageItem.Id != ObjectId.Empty;
        }

        private void ExecuteMissingMaterialPrintCommand(object param)
        {
            PrintManager.Print(this);
        }

        private bool CanExecutePrintDeliveryNoteCommand(object param)
        {
            return SelectedStorageItem.Id != ObjectId.Empty && Data.Any(i => i.Item.StorageId == SelectedStorageItem.Id && i.IsSelected);
        }

        private void ExecutePrintDeliveryNoteCommand(object param)
        {
            DeliveryNotePrintContext context = new DeliveryNotePrintContext(this)
                {
                    NoteDate = DateTime.Now,
                    OrderDate = DateTime.Now.Subtract(TimeSpan.FromDays(1))
                };
            DeliveryNoteDetailsWindow detailsWindow = new DeliveryNoteDetailsWindow
                {
                    DataContext = new DeliveryNoteDetailsViewModel(context)
                };
            if (detailsWindow.ShowDialog() == true)
            {
                PrintManager.Print(context);
            }
        }

        private void SelectAll(bool selected)
        {
            if (SelectedStorageItem != null)
            {
                foreach (ExternStorageMaterialViewModel item in Data.Where(i => i.Item.StorageId == SelectedStorageItem.Id))
                {
                    item.IsSelected = selected;
                }
            }
        }

        private void EnableAll(bool enabled)
        {
            foreach (ExternStorageMaterialViewModel item in Data)
            {
                item.IsSelectEnabled = enabled;
            }
        }

        #endregion

        #region Private Class

        private class DeliveryNoteOrganization : IDeliveryNoteOrganization
        {
            private readonly IExternStorage _externStorage;

            public DeliveryNoteOrganization(IExternStorage externStorage)
            {
                _externStorage = externStorage;
            }

            public string Company
            {
                get { return _externStorage.CompanyName; }
            }

            public string Street
            {
                get { return _externStorage.Street; }
            }

            public string Number
            {
                get { return _externStorage.Number; }
            }

            public string ZipCode
            {
                get { return _externStorage.ZipCode; }
            }

            public string City
            {
                get { return _externStorage.City; }
            }

            public string CompanyId
            {
                get { return _externStorage.CompanyId; }
            }

            public string TaxId
            {
                get { return _externStorage.TaxId; }
            }
        }

        #endregion
    }
}