using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Common
{
    public abstract class ListViewModelBase<TViewModel, TData> : ViewModelBase
        where TViewModel : ItemViewModelBase<TData>
        where TData : IClientObject
    {
        private readonly IDataAccess _dataAccess;
        private readonly ObservableCollection<TViewModel> _data;

        protected ListViewModelBase(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _data = new ObservableCollection<TViewModel>();
            _data.CollectionChanged += DataOnCollectionChanged;
        }

        #region Properties

        protected IDataAccess DataAccess
        {
            get { return _dataAccess; }
        }

        #endregion

        #region Public Properties

        public IEnumerable<TViewModel> Data
        {
            get { return _data; }
        }

        public int Count
        {
            get { return _data.Count; }
        }

        #endregion

        #region Public Methods

        public void Load()
        {
            _data.Clear();
            IEnumerable<TData> data = LoadData();
            foreach (var item in data)
            {
                TViewModel viewModel = CreateViewModel(item);
                _data.Add(viewModel);
            }
        }

        #endregion

        #region Internals and Helpers

        protected abstract IEnumerable<TData> LoadData();

        protected abstract TViewModel CreateViewModel(TData item);

        protected virtual void OnObjectsInserted(IEnumerable<TViewModel> newItems)
        {
        }

        protected virtual void OnObjectsDeleted(IEnumerable<TViewModel> removedItems)
        {
        }

        protected TViewModel Add(TData newItem)
        {
            TViewModel viewModel = CreateViewModel(newItem);
            _data.Add(viewModel);
            NotifyPropertyChanged("Data");
            return viewModel;
        }

        protected bool Remove(TViewModel viewModel)
        {
            bool succeed = _data.Remove(viewModel);
            if (succeed)
            {
                NotifyPropertyChanged("Data");
            }
            return succeed;
        }

        private void DataOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                OnObjectsInserted(notifyCollectionChangedEventArgs.NewItems.Cast<TViewModel>());
            }
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                OnObjectsDeleted(notifyCollectionChangedEventArgs.OldItems.Cast<TViewModel>());
            }
        }

        #endregion
    }
}