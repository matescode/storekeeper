using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using CommonBase;
using CommonBase.UI.Localization;
using StoreKeeper.App.Printing;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.ProductOrderDetail
{
    public class ProductOrderDetailViewModel : ViewModelBase, IPrintingContext
    {
        private readonly IDataAccess _dataAccess;
        private readonly IProductOrderDetail _productOrderDetail;
        private readonly Action _reloadAction;
        private readonly ObservableCollection<ProductOrderDetailItemViewModel> _itemsData;
        private IEnumerable<PrintColumnDefinition> _printColumnDefinitions;

        public ProductOrderDetailViewModel(IDataAccess dataAccess, IProductOrderDetail productOrderDetail, Action reloadAction)
        {
            _dataAccess = dataAccess;
            _productOrderDetail = productOrderDetail;
            _reloadAction = reloadAction;
            _itemsData = new ObservableCollection<ProductOrderDetailItemViewModel>();
            ReloadItemsData(_productOrderDetail.ProductId, _productOrderDetail.OrderId);
            // NavigatorRoot = new List<NavigatorItemBase> { NavigatorItemBase.Create(productOrderDetail, OnItemSelected) };
        }

        #region Properties

        public string WindowTitle
        {
            get
            {
                return String.Format("{0} {1}", "ProductOrderDetail".Localize(), _productOrderDetail.Name);
            }
        }

        public string Title
        {
            get { return _productOrderDetail.Name; }
        }

        public ObservableCollection<ProductOrderDetailItemViewModel> ItemsData
        {
            get { return _itemsData; }
        }

        public ICommand CreateOrderCommand
        {
            get
            {
                return new RelayCommand(ExecuteCreateOrderCommand);
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return new RelayCommand(ExecutePrintCommand);
            }
        }

        #endregion

        #region IPrintingContext Implementation

        public string Label
        {
            get { return _productOrderDetail.Name; }
        }

        public IEnumerable<PrintColumnDefinition> Columns
        {
            get
            {
                return _printColumnDefinitions
                        ??
                        (_printColumnDefinitions = new List<PrintColumnDefinition>
                    {
                        new PrintColumnDefinition("Code".Localize(), "Code", 70),
                        new PrintColumnDefinition("Type".Localize(), "TypeStr", 50),
                        new PrintColumnDefinition("NameDescription".Localize(), "NameDescription", 400),
                        new PrintColumnDefinition("CountInProduct".Localize(), "CountInProduct", 105, true),
                        new PrintColumnDefinition("StockAvailable".Localize(), "StockAvailable", 60, true),
                        new PrintColumnDefinition("Storage".Localize(), "Storage", 60),
                        new PrintColumnDefinition("ProductionReservation".Localize(), "ProductionReservation", 120, true),
                        new PrintColumnDefinition("ToOrder".Localize(), "OrderCount", 60, true),
                        new PrintColumnDefinition("Ordered".Localize(), "OrderedStr", 70),
                    });
            }
        }

        public List<object> DataSource
        {
            get { return ItemsData.Select(item => (object)item).ToList(); }
        }

        #endregion

        #region Internals and Helpers

        private void ReloadItemsData(ObjectId productId, ObjectId orderId)
        {
            _itemsData.Clear();
            foreach (IProductOrderItem productItem in _dataAccess.GetProductOrderDetailItems(productId, orderId))
            {
                _itemsData.Add(new ProductOrderDetailItemViewModel(productItem));
            }
        }

        private void OnItemSelected(ObjectId itemId)
        {
            ReloadItemsData(itemId, _productOrderDetail.OrderId);
            NotifyPropertyChanged("ItemsData");
        }

        private void ExecuteCreateOrderCommand(object param)
        {
            ProductOrderDetailItemViewModel itemViewModel = param as ProductOrderDetailItemViewModel;
            if (itemViewModel != null)
            {
                _dataAccess.CreateProductOrder(itemViewModel.Code, itemViewModel.OrderCount);
                if (_reloadAction != null)
                {
                    _reloadAction();
                }
            }
        }

        private void ExecutePrintCommand(object param)
        {
            PrintManager.Print(this);
        }

        #endregion
    }
}