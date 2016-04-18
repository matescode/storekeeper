using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CommonBase.UI;
using CommonBase.UI.Localization;
using CommonBase.UI.MessageDialogs;
using StoreKeeper.App.Printing;
using StoreKeeper.App.Searching;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.App.ViewModels.Material;
using StoreKeeper.App.ViewModels.ProductOrderDetail;
using StoreKeeper.App.Windows;
using StoreKeeper.Client;
using StoreKeeper.Client.Exceptions;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.ProductOrder
{
    public class ProductOrderListViewModel : BaseOrderListViewModel<ProductOrderViewModel, IProductOrder>, ISearchProvider
    {
        private readonly Action _reloadAction;
        private IEnumerable<PrintColumnDefinition> _printColumnDefinitions;
        private NewOrderViewModel _newOrderViewModel;

        public ProductOrderListViewModel(IDataAccess dataAccess, Action reloadAction)
            : base(dataAccess)
        {
            _reloadAction = reloadAction;
        }

        #region Properties

        public ICommand ItemDoubleClickCommand
        {
            get { return new RelayCommand(ExecuteDoubleClickCommand); }
        }

        public ICommand ResolveOrderCommand
        {
            get
            {
                return new RelayCommand(ExecuteResolveOrderCommand, CanExecuteResolveOrderCommand);
            }
        }

        public ICommand DeleteOrderCommand
        {
            get
            {
                return new RelayCommand(ExecuteDeleteOrderCommand, CanExecuteDeleteOrderCommand);
            }
        }

        public NewOrderViewModel NewOrderViewModel
        {
            get
            {
                return _newOrderViewModel ?? (_newOrderViewModel = new NewOrderViewModel(CodeType, ExecuteAddOrderCommand, CanExecuteAddOrderCommand));
            }
        }

        #endregion

        #region ISearchProvider Implementation

        public object FindItem(string codePrefix)
        {
            return Data.FirstOrDefault(v => v.Code.ToUpper().StartsWith(codePrefix.ToUpper()));
        }

        #endregion

        #region Overrides

        public override string Label
        {
            get { return "ProductionOrders".Localize(); }
        }

        public override IEnumerable<PrintColumnDefinition> Columns
        {
            get
            {
                return _printColumnDefinitions
                    ??
                    (_printColumnDefinitions = new List<PrintColumnDefinition>
                    {
                        new PrintColumnDefinition("Priority".Localize(), "Priority", 50, true),
                        new PrintColumnDefinition("Code".Localize(), "Code", 70),
                        new PrintColumnDefinition("NameDescription".Localize(), "NameDescription", 380),
                        new PrintColumnDefinition("StockAvailable".Localize(), "StockAvailable", 60, true),
                        new PrintColumnDefinition("OrderedCount".Localize(), "OrderedCount", 70, true),
                        new PrintColumnDefinition("PossibleMakeCount".Localize(), "PossibleCount", 70, true),
                        new PrintColumnDefinition("OrderTerm".Localize(), "OrderTermStr", 110),
                        new PrintColumnDefinition("PlannedTerm".Localize(), "PlannedTermStr", 105),
                        new PrintColumnDefinition("EndTerm".Localize(), "EndTermStr")
                    });
            }
        }

        protected ArticleCodeType CodeType
        {
            get
            {
                return ArticleCodeType.Product;
            }
        }

        protected IProductOrder CreateData(object param)
        {
            return DataAccess.CreateProductOrder(param.ToString());
        }

        protected override ProductOrderViewModel CreateViewModel(IProductOrder item)
        {
            return new ProductOrderViewModel(item);
        }

        protected override IEnumerable<IProductOrder> LoadData()
        {
            return DataAccess.ProductOrders;
        }

        protected override void OnObjectsDeleted(IEnumerable<ProductOrderViewModel> removedItems)
        {
            foreach (ProductOrderViewModel viewModel in removedItems)
            {
                DataAccess.ResolveProductOrder(viewModel.Item, viewModel.Resolved);
            }
        }

        #endregion

        #region Internals and Helpers

        private void ExecuteDoubleClickCommand(object param)
        {
            ProductOrderViewModel viewModel = param as ProductOrderViewModel;
            if (viewModel == null)
            {
                return;
            }

            IProductOrderDetail productOrderDetail = DataAccess.GetProductOrderDetail(viewModel.Item.OrderId);
            ProductOrderDetailViewModel orderDetailViewModel = new ProductOrderDetailViewModel(DataAccess, productOrderDetail, _reloadAction);
            ProductOrderDetailWindow orderDetailWindow = new ProductOrderDetailWindow { DataContext = orderDetailViewModel };
            orderDetailWindow.ShowDialog();
        }

        private bool CanExecuteResolveOrderCommand(object param)
        {
            ProductOrderViewModel viewModel = param as ProductOrderViewModel;
            return viewModel != null && viewModel.OrderTerm.HasValue && viewModel.PlannedTerm.HasValue && viewModel.EndTerm.HasValue;
        }

        private void ExecuteResolveOrderCommand(object param)
        {
            ProductOrderViewModel viewModel = (ProductOrderViewModel)param;
            viewModel.Resolved = true;
            Remove(viewModel);
            NotifyPropertyChanged("Data");
        }

        private bool CanExecuteDeleteOrderCommand(object param)
        {
            return param is ProductOrderViewModel;
        }

        private void ExecuteDeleteOrderCommand(object param)
        {
            ProductOrderViewModel viewModel = (ProductOrderViewModel)param;

            StringBuilder question = new StringBuilder();
            question.AppendLine("QuestionRemoveOrder".Localize());
            question.AppendFormat("QuestionRemoveOrderDetail".Localize(), viewModel.NameDescription, viewModel.Code);

            if (UIApplication.MessageDialogs.Question(question.ToString()) == QuestionResult.Negative)
            {
                return;
            }

            Remove(viewModel);
            MaterialListNotificator.UnregisterListener(viewModel);
            MaterialListNotificator.Notify(viewModel.MaterialId, "OrderCount");
        }

        private bool CanExecuteAddOrderCommand(object param)
        {
            if (param == null)
            {
                return false;
            }
            string code = param.ToString();
            if (string.IsNullOrWhiteSpace(code) || code.Length < AppContext.Config.SeekCodeCharLimit)
            {
                return false;
            }
            return DataAccess.IsCodeValid(code, CodeType);
        }

        private void ExecuteAddOrderCommand(object param)
        {
            try
            {
                ProductOrderViewModel newItem = Add(CreateData(param));
                MaterialListNotificator.RegisterListener(newItem);
                newItem.RefreshStatisticItems();
                _newOrderViewModel.Code = String.Empty;
            }
            catch (MaterialOrderAlreadyExistException)
            {
                UIApplication.MessageDialogs.Error("MaterialOrderAlreadyExists".Localize());
            }
            catch (DatabaseLockedException)
            {
                UIApplication.MessageDialogs.Error("DatabaseLockedException".Localize());
            }
        }

        #endregion
    }
}