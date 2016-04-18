using System.Collections.Generic;
using System.Linq;
using CommonBase;
using CommonBase.UI.Localization;
using StoreKeeper.App.Printing;
using StoreKeeper.App.Searching;
using StoreKeeper.App.ViewModels.Common;
using StoreKeeper.App.ViewModels.Material;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.MaterialOrder
{
    public class MaterialOrderListViewModel : BaseOrderListViewModel<MaterialOrderViewModel, IMaterialOrder>, ISearchProvider
    {
        private IEnumerable<PrintColumnDefinition> _printColumnDefinitions;

        public MaterialOrderListViewModel(IDataAccess dataAccess)
            : base(dataAccess)
        {
        }

        #region Overrides

        public override string Label
        {
            get { return "MaterialOrders".Localize(); }
        }

        public override IEnumerable<PrintColumnDefinition> Columns
        {
            get
            {
                return _printColumnDefinitions
                    ??
                    (_printColumnDefinitions = new List<PrintColumnDefinition>
                    {
                        new PrintColumnDefinition("Code".Localize(), "Code", 70),
                        new PrintColumnDefinition("NameDescription".Localize(), "NameDescription", 200),
                        new PrintColumnDefinition("StockAvailable".Localize(), "StockAvailable", 60, true),
                        new PrintColumnDefinition("StockAvailableEx".Localize(), "StockAvailableEx", 95, true),
                        new PrintColumnDefinition("OrderedCount".Localize(), "OrderedCount", 70, true),
                        new PrintColumnDefinition("OrderTerm".Localize(), "OrderTermStr", 110),
                        new PrintColumnDefinition("PlannedTerm".Localize(), "PlannedTermStr", 105),
                        new PrintColumnDefinition("EndTerm".Localize(), "EndTermStr"),
                        new PrintColumnDefinition("Ordered".Localize(), "OrderedStr", 75),
                        new PrintColumnDefinition("Note".Localize(), "Note", 120)
                    });
            }
        }

        protected override IEnumerable<IMaterialOrder> LoadData()
        {
            return DataAccess.MaterialOrders;
        }

        protected override MaterialOrderViewModel CreateViewModel(IMaterialOrder item)
        {
            return new MaterialOrderViewModel(item);
        }

        #endregion

        #region ISearchProvider Implementation

        public object FindItem(string codePrefix)
        {
            return Data.FirstOrDefault(v => v.Code.ToUpper().StartsWith(codePrefix.ToUpper()));
        }

        #endregion
    }
}