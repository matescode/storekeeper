using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Common
{
    public abstract class ItemViewModelBase<T> : ViewModelBase where T : IClientObject
    {
        protected ItemViewModelBase(T item)
        {
            Item = item;
        }

        #region Properties

        public T Item { get; protected set; }

        #endregion
    }
}