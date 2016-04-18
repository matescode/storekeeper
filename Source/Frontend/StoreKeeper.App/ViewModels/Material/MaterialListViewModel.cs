using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CommonBase.UI.Localization;
using StoreKeeper.App.Printing;
using StoreKeeper.App.Searching;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.App.ViewModels.Storage;
using StoreKeeper.App.Windows;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;
using StoreKeeper.Common.DataContracts;

namespace StoreKeeper.App.ViewModels.Material
{
    public class MaterialListViewModel : ListViewModelBase<MaterialViewModel, IMaterial>, ISearchProvider, IPrintingContext
    {
        private IEnumerable<PrintColumnDefinition> _printColumnDefinitions;

        public MaterialListViewModel(IDataAccess dataAccess)
            : base(dataAccess)
        {
        }

        #region Overrides

        protected override IEnumerable<IMaterial> LoadData()
        {
            return DataAccess.Materials;
        }

        protected override MaterialViewModel CreateViewModel(IMaterial item)
        {
            MaterialViewModel viewModel = new MaterialViewModel(item);
            MaterialListNotificator.RegisterListener(viewModel);
            return viewModel;
        }

        #endregion

        #region Properties

        public ICommand ItemDoubleClickCommand
        {
            get { return new RelayCommand(ExecuteDoubleClickCommand, CanExecuteDoubleClickCommand); }
        }

        public ICommand PrintCommand
        {
            get
            {
                return new RelayCommand(ExecutePrintCommand);
            }
        }

        #endregion

        #region ISearchProvider Implementation

        public object FindItem(string codePrefix)
        {
            return Data.FirstOrDefault(v => v.Code.ToUpper().StartsWith(codePrefix.ToUpper()));
        }

        #endregion

        #region IPrintingContext Implementation

        public string Label
        {
            get { return "MissingMaterial".Localize(); }
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
                        new PrintColumnDefinition("SupplierCode".Localize(), "SupplierCode", 110),
                        new PrintColumnDefinition("Type".Localize(), "TypeStr", 50),
                        new PrintColumnDefinition("NameDescription".Localize(), "NameDescription", 320),
                        new PrintColumnDefinition("StockAvailable".Localize(), "StockAvailable", 60, true),
                        new PrintColumnDefinition("CountToOrder".Localize(), "CountToOrder", 100, true),
                        new PrintColumnDefinition("OrderedCount".Localize(), "OrderedCount", 70, true),
                        new PrintColumnDefinition("InOrderedProducts".Localize(), "ProductCount", 60, true),
                        new PrintColumnDefinition("ExternStorageCount".Localize(), "ExternStorageCount", 80, true),
                        new PrintColumnDefinition("Price".Localize(), "Price", 60, true),
                    });
            }
        }

        public List<object> DataSource
        {
            get { return Data.Where(i => i.CountToOrder > 0).Select(item => (object)item).ToList(); }
        }

        #endregion

        #region Internals and Helpers

        private bool CanExecuteDoubleClickCommand(object param)
        {
            MaterialViewModel itemViewModel = param as MaterialViewModel;
            if (itemViewModel != null)
            {
                return itemViewModel.Item.Type == ArticleType.Product;
            }
            return false;
        }

        private void ExecuteDoubleClickCommand(object param)
        {
            MaterialViewModel viewModel = param as MaterialViewModel;
            if (viewModel == null)
            {
                return;
            }

            IProductStorageMapping productStorageMapping = DataAccess.GetProductStorageMapping(viewModel.MaterialId);
            StorageMappingViewModel storageMappingViewModel = new StorageMappingViewModel(DataAccess, productStorageMapping);
            StorageMappingWindow storageMappingWindow = new StorageMappingWindow { DataContext = storageMappingViewModel };
            storageMappingWindow.ShowDialog();
            if (storageMappingViewModel.IsChanged)
            {
                DataAccess.RequestForCalculation();
            }
        }

        private void ExecutePrintCommand(object param)
        {
            PrintManager.Print(this);
        }

        #endregion
    }
}