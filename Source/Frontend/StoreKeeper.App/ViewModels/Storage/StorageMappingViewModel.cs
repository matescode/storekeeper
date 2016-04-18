using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using CommonBase.UI.Localization;

using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Storage
{
    public class StorageMappingViewModel : ViewModelBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly IProductStorageMapping _storageMapping;
        private readonly ObservableCollection<StorageMappingItemViewModel> _itemsData;

        public StorageMappingViewModel(IDataAccess dataAccess, IProductStorageMapping storageMapping)
        {
            _dataAccess = dataAccess;
            _storageMapping = storageMapping;
            _itemsData = new ObservableCollection<StorageMappingItemViewModel>();
            IsChanged = false;
            LoadItemsData(false);
        }

        #region Properties

        public string WindowTitle
        {
            get
            {
                return String.Format("{0} - {1}", "ProductStorageMapping".Localize(), _storageMapping.Name);
            }
        }

        public string Title
        {
            get { return _storageMapping.Name; }
        }

        public ObservableCollection<StorageMappingItemViewModel> ItemsData
        {
            get { return _itemsData; }
        }

        public IEnumerable<StorageComboBoxItem> StorageList { get; private set; }

        public ICommand SetAllCommand
        {
            get
            {
                return new RelayCommand(ExecuteSetAllCommand, CanExecuteSetAllCommand);
            }
        }

        public bool IsChanged { get; private set; }

        #endregion

        #region Internals and Helpers

        private void LoadItemsData(bool reload)
        {
            if (reload)
            {
                _storageMapping.Reload();
            }
            _itemsData.Clear();
            foreach (IProductStorageMappingItem storageMappingItem in _storageMapping.Items)
            {
                StorageMappingItemViewModel itemViewModel = new StorageMappingItemViewModel(storageMappingItem, v => IsChanged = v);
                itemViewModel.NotifyPropertyChanged("CanBeChanged");
                _itemsData.Add(itemViewModel);
            }
        }

        private bool CanExecuteSetAllCommand(object param)
        {
            return param != null;
        }

        private void ExecuteSetAllCommand(object param)
        {
            StorageComboBoxItem item = param as StorageComboBoxItem;
            if (item != null)
            {
                _dataAccess.SetStorageMappingAsync(
                    _storageMapping.ProductId,
                    item.Id,
                    () =>
                    {
                        LoadItemsData(true);
                        NotifyPropertyChanged("ItemsData");
                    });
                IsChanged = true;
            }
        }

        #endregion
    }
}