using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonBase;
using CommonBase.Application;
using CommonBase.UI.Localization;
using StoreKeeper.Client;
using StoreKeeper.Common;

namespace StoreKeeper.App.ViewModels.Storage
{
    public class StorageListProvider
    {
        public static readonly StorageComboBoxItem AllStoragesItem = new StorageComboBoxItem(ObjectId.Empty, "AllStorages".Localize());

        private readonly List<StorageComboBoxItem> _items;
        private readonly ObservableCollection<StorageComboBoxItem> _itemsForExternStorages;
        private readonly IDataAccess _dataAccess;

        public StorageListProvider()
        {
            _dataAccess = ApplicationContext.Service<IDataAccess>();
            _items = new List<StorageComboBoxItem>();
            _itemsForExternStorages = new ObservableCollection<StorageComboBoxItem>();

            Reload();
        }

        public IEnumerable<StorageComboBoxItem> GetData()
        {
            return _items;
        }

        public ObservableCollection<StorageComboBoxItem> GetDataForExternStorages()
        {
            return _itemsForExternStorages;
        }

        public void Reload()
        {
            _items.Clear();
            _itemsForExternStorages.Clear();
            _itemsForExternStorages.Add(AllStoragesItem);

            foreach (StorageComboBoxItem storageItem in
                    _dataAccess.Storages.Select(storage => new StorageComboBoxItem(storage.StorageId, storage.Name)))
            {
                _items.Add(storageItem);
                if (storageItem.Id != (ObjectId)Constants.CentralStorageId)
                {
                    _itemsForExternStorages.Add(storageItem);
                }
            }
        }

        public StorageComboBoxItem Find(ObjectId storageId)
        {
            StorageComboBoxItem item = _itemsForExternStorages.FirstOrDefault(i => i.Id == storageId);
            return item ?? AllStoragesItem;
        }
    }
}