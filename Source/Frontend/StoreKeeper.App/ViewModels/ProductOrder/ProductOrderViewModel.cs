using System;
using System.Windows.Media;

using CommonBase;

using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.App.ViewModels.Material;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.ProductOrder
{
    public class ProductOrderViewModel : BaseOrderViewModel<IProductOrder>
    {
        private double _subOrderCount;

        public ProductOrderViewModel(IProductOrder productOrder)
            : base(productOrder)
        {
        }

        #region Properties

        public int Priority
        {
            get
            {
                if (Item == null)
                {
                    return 0;
                }
                return Item.Priority;
            }
            set
            {
                if (Item == null)
                {
                    return;
                }
                Item.Priority = value;
            }
        }

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
            set
            {
                Item.OrderedCount = value;
                NotifyPropertyChanged("OrderCount");
                MaterialListNotificator.Notify(ItemId, "OrderCount");
            }
        }

        public double PossibleCount
        {
            get
            {
                if (Item == null)
                {
                    return 0;
                }
                return Item.PossibleCount;
            }
        }

        public DateTime? OrderTerm
        {
            get
            {
                if (Item == null)
                {
                    return null;
                }
                return Item.OrderPeriod;
            }
            set
            {
                if (Item == null)
                {
                    return;
                }
                Item.OrderPeriod = value.HasValue ? value.Value : DateTime.Now;
                NotifyPropertyChanged("OrderTerm");
            }
        }

        public string OrderTermStr
        {
            get
            {
                if (Item == null)
                {
                    return null;
                }
                return Item.OrderPeriod.HasValue ? Item.OrderPeriod.Value.ToString("dd.MM.yyyy") : String.Empty;
            }
        }

        public DateTime? PlannedTerm
        {
            get
            {
                return Item != null ? Item.PlannedPeriod : null;
            }
            set
            {
                if (Item == null)
                {
                    return;
                }
                Item.PlannedPeriod = value;
                NotifyPropertyChanged("PlannedTerm");
            }
        }

        public string PlannedTermStr
        {
            get
            {
                if (Item == null)
                {
                    return null;
                }
                return Item.PlannedPeriod.HasValue ? Item.PlannedPeriod.Value.ToString("dd.MM.yyyy") : String.Empty;
            }
        }

        public DateTime? EndTerm
        {
            get
            {
                return Item != null ? Item.EndPeriod : null;
            }
            set
            {
                if (Item == null)
                {
                    return;
                }
                Item.EndPeriod = value;
                NotifyPropertyChanged("EndTerm");
            }
        }

        public string EndTermStr
        {
            get
            {
                if (Item == null)
                {
                    return null;
                }
                return Item.EndPeriod.HasValue ? Item.EndPeriod.Value.ToString("dd.MM.yyyy") : String.Empty;
            }
        }

        public Brush CompleteBrush
        {
            get
            {
                if (Item == null)
                {
                    return Brushes.Red;
                }
                return Item.IsComplete ? Brushes.ForestGreen : Brushes.Red;
            }
        }

        public double SubOrderCount
        {
            get { return _subOrderCount; }
            set
            {
                _subOrderCount = value;
                NotifyPropertyChanged("SubOrderCount");
            }
        }

        #endregion

        #region Overrides

        protected override ObjectId ItemId
        {
            get { return Item.ProductId; }
        }

        protected override void RefreshStatisticItemsInternal()
        {
            NotifyPropertyChanged("PossibleCount");
            NotifyPropertyChanged("CompleteBrush");
        }

        #endregion
    }
}