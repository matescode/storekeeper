using System.Collections.Generic;
using System.Windows.Input;
using CommonBase;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.App.Windows;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;
using StoreKeeper.Common;

namespace StoreKeeper.App.ViewModels.Storage
{
    public class StorageListViewModel : ListViewModelBase<StorageViewModel, IExternStorage>
    {
        private string _newStorageName;

        public StorageListViewModel(IDataAccess dataAccess)
            : base(dataAccess)
        {
            IsChanged = false;
        }

        #region Properties

        public ICommand EditStorageCommand
        {
            get
            {
                return new RelayCommand(ExecuteEditStorageCommand);
            }
        }

        public ICommand DeleteStorageCommand
        {
            get
            {
                return new RelayCommand(ExecuteDeleteStorageCommand, CanExecuteDeleteStorageCommand);
            }
        }

        public string NewStorageName
        {
            get { return _newStorageName; }
            set
            {
                _newStorageName = value;
                NotifyPropertyChanged("NewStorageName");
            }
        }

        public bool IsChanged { get; private set; }

        #endregion

        #region Overrides

        protected override IEnumerable<IExternStorage> LoadData()
        {
            return DataAccess.Storages;
        }

        protected override StorageViewModel CreateViewModel(IExternStorage item)
        {
            return new StorageViewModel(item);
        }

        protected override void OnObjectsDeleted(IEnumerable<StorageViewModel> removedItems)
        {
            foreach (StorageViewModel viewModel in removedItems)
            {
                DataAccess.RemoveExternStorage(viewModel.Item);
            }
        }

        #endregion

        #region Internals and Helpers

        private void ExecuteEditStorageCommand(object param)
        {
            IExternStorage storage = null;
            if (param is StorageViewModel)
            {
                storage = (param as StorageViewModel).Item;
            }

            EditStorageWindow editStorageWindow = new EditStorageWindow();
            EditStorageViewModel editStorageViewModel = new EditStorageViewModel(DataAccess, editStorageWindow.Close, storage);
            editStorageWindow.DataContext = editStorageViewModel;
            editStorageWindow.ShowDialog();
            if (editStorageViewModel.StorageAdded)
            {
                Add(editStorageViewModel.ExternStorage);
            }
            Load();
            NotifyPropertyChanged("Data");
            IsChanged = true;
        }

        private bool CanExecuteDeleteStorageCommand(object param)
        {
            if (param == null)
            {
                return false;
            }

            StorageViewModel viewModel = param as StorageViewModel;
            if (viewModel == null || viewModel.StorageId == (ObjectId)Constants.CentralStorageId)
            {
                return false;
            }

            return DataAccess.CanExternStorageBeSafelyRemoved(viewModel.StorageId);
        }

        private void ExecuteDeleteStorageCommand(object param)
        {
            Remove(param as StorageViewModel);
            IsChanged = true;
        }

        #endregion
    }
}