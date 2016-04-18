using System;
using CommonBase;
using StoreKeeper.App.ViewModels.Material;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Common
{
    public abstract class BaseOrderViewModel<T> : ItemViewModelBase<T>, IMaterialChangeListener where T : class, IOrder
    {
        protected BaseOrderViewModel(T order)
            : base(order)
        {
        }

        #region Properties

        protected abstract ObjectId ItemId { get; }

        public string Code
        {
            get
            {
                if (Item == null)
                {
                    return string.Empty;
                }
                return Item.Code;
            }
        }

        public string NameDescription
        {
            get
            {
                if (Item == null)
                {
                    return string.Empty;
                }
                return Item.Name;
            }
        }

        public double StockAvailable
        {
            get
            {
                if (Item == null)
                {
                    return 0;
                }
                return Item.CurrentCount;
            }
        }

        public bool Resolved { get; set; }

        #endregion

        #region IMaterialChangeListener Implementation

        public ObjectId MaterialId
        {
            get { return ItemId; }
        }

        public void Notify(string property)
        {
            NotifyPropertyChanged(property);
        }

        public void NotifyAll()
        {
            Notify("StockAvailable");
        }

        #endregion

        #region Public Methods

        public void RefreshStatisticItems()
        {
            NotifyPropertyChanged("NameDescription");
            NotifyPropertyChanged("StockAvailable");
            NotifyPropertyChanged("OrderTerm");
            NotifyPropertyChanged("ResolveButtonVisibility");
            RefreshStatisticItemsInternal();
        }

        protected virtual void RefreshStatisticItemsInternal()
        {
            
        }

        #endregion
    }
}