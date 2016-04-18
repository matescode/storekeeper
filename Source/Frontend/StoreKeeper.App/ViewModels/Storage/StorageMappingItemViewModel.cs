using System;
using CommonBase;
using CommonBase.UI.Localization;

using StoreKeeper.Client.Objects;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.App.ViewModels.Storage
{
    public class StorageMappingItemViewModel : ViewModelBase
    {
        private readonly IProductStorageMappingItem _productStorageMappingItem;
        private readonly Action<bool> _isChangedSetter;
        private StorageComboBoxItem _storageItem;

        public StorageMappingItemViewModel(IProductStorageMappingItem productStorageMappingItem, Action<bool> isChangedSetter)
        {
            _productStorageMappingItem = productStorageMappingItem;
            _isChangedSetter = isChangedSetter;
        }

        #region Properties

        public ObjectId ItemId
        {
            get { return _productStorageMappingItem.ItemId; }
        }

        public string Code
        {
            get { return _productStorageMappingItem.Code; }
        }

        public ArticleType Type
        {
            get { return _productStorageMappingItem.Type; }
        }

        public string TypeStr
        {
            get { return _productStorageMappingItem.Type.ToString().Localize(); }
        }

        public string NameDescription
        {
            get { return _productStorageMappingItem.Name; }
        }

        public string Storage
        {
            get { return _productStorageMappingItem.Storage; }
        }

        public StorageComboBoxItem StorageItem
        {
            get { return _storageItem ?? (_storageItem = CreateComboBoxItem()); }
            set
            {
                _storageItem = value;
                _productStorageMappingItem.StorageId = value.Id;
                _isChangedSetter(true);
                NotifyPropertyChanged("StorageItem");
            }
        }

        public bool SkipCalculation
        {
            get { return _productStorageMappingItem.SkipCalculation; }
            set
            {
                _productStorageMappingItem.SkipCalculation = value;
                _isChangedSetter(true);
                NotifyPropertyChanged("SkipCalculation");
            }
        }

        #endregion

        #region Internals and Helpers

        private StorageComboBoxItem CreateComboBoxItem()
        {
            return new StorageComboBoxItem(_productStorageMappingItem.StorageId, _productStorageMappingItem.Storage);
        }

        #endregion
    }
}