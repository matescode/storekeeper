using System;
using System.Windows.Media;

using CommonBase;

using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.MaterialOrder
{
    public class MaterialOrderViewModel : BaseOrderViewModel<IMaterialOrder>
    {
        private MaterialOrderStatusViewModel _materialOrderStatusViewModel;

        public MaterialOrderViewModel(IMaterialOrder materialOrder)
            : base(materialOrder)
        {
        }

        #region Properties

        public double OrderedCount
        {
            get
            {
                if (Item == null)
                {
                    return 0;
                }
                return Item.OrderedCount;
            }
        }

        public string OrderedStr
        {
            get
            {
                if (Item == null)
                {
                    return String.Empty;
                }
                return MaterialOrderStatusViewModel.OrderedStr;
            }
        }

        public Brush OrderedColorBrush
        {
            get
            {
                if (Item == null)
                {
                    return Brushes.Black;
                }
                return MaterialOrderStatusViewModel.OrderedColorBrush;
            }
        }

        public double StockAvailableEx
        {
            get
            {
                if (Item == null)
                {
                    return 0;
                }
                return Item.CurrentTotalCount;
            }
        }

        #endregion

        #region Overrides

        protected override ObjectId ItemId
        {
            get { return Item.MaterialId; }
        }

        protected override void RefreshStatisticItemsInternal()
        {
            NotifyPropertyChanged("StockAvailableEx");
            NotifyPropertyChanged("Ordered");
            NotifyPropertyChanged("OrderedStr");
            NotifyPropertyChanged("OrderedColorBrush");
        }

        #endregion

        #region Internals and Helpers

        private MaterialOrderStatusViewModel MaterialOrderStatusViewModel
        {
            get
            {
                return _materialOrderStatusViewModel ?? (_materialOrderStatusViewModel = new MaterialOrderStatusViewModel(Item.MaterialOrderStatus));
            }
        }

        #endregion
    }
}