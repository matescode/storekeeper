using System;
using System.Windows;
using System.Windows.Media;
using CommonBase;
using CommonBase.Resources;
using CommonBase.UI.Localization;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.Client.Objects;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.App.ViewModels.ProductOrderDetail
{
    public class ProductOrderDetailItemViewModel : ViewModelBase
    {
        private readonly IProductOrderItem _productOrderItem;
        private MaterialOrderStatusViewModel _materialOrderStatusViewModel;

        public ProductOrderDetailItemViewModel(IProductOrderItem productOrderItem)
        {
            _productOrderItem = productOrderItem;
        }

        #region Properties

        public ObjectId ItemId
        {
            get { return _productOrderItem.ItemId; }
        }

        public string Code
        {
            get { return _productOrderItem.Code; }
        }

        public ArticleType Type
        {
            get { return _productOrderItem.Type; }
        }

        public string TypeStr
        {
            get { return _productOrderItem.Type.ToString().Localize(); }
        }

        public string NameDescription
        {
            get { return _productOrderItem.Name; }
        }

        public double CountInProduct
        {
            get { return _productOrderItem.Count; }
        }

        public double StockAvailable
        {
            get { return _productOrderItem.StockAvailable; }
        }

        public string Storage
        {
            get { return _productOrderItem.Storage; }
        }

        public double ProductionReservation
        {
            get { return _productOrderItem.ProductionReservation; }
        }

        public double OrderCount
        {
            get
            {
                return _productOrderItem.OrderCount;
            }
        }

        public string OrderedStr
        {
            get
            {
                if (Type == ArticleType.Product)
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
                if (Type == ArticleType.Product)
                {
                    return Brushes.Black;
                }
                return MaterialOrderStatusViewModel.OrderedColorBrush;
            }
        }

        public Visibility CreateOrderButtonVisibility
        {
            get { return (OrderCount > 0 && _productOrderItem.Type == ArticleType.Product) ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        private MaterialOrderStatusViewModel MaterialOrderStatusViewModel
        {
            get
            {
                return _materialOrderStatusViewModel ?? (_materialOrderStatusViewModel = new MaterialOrderStatusViewModel(_productOrderItem.MaterialOrderStatus));
            }
        }
    }
}