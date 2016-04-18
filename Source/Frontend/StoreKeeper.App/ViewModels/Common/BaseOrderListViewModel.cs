using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using StoreKeeper.App.Printing;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Common
{
    public abstract class BaseOrderListViewModel<TViewModel, TData> : ListViewModelBase<TViewModel, TData>, IPrintingContext
        where TViewModel : BaseOrderViewModel<TData>
        where TData : class, IOrder
    {
        protected BaseOrderListViewModel(IDataAccess dataAccess)
            : base(dataAccess)
        {
        }

        #region Properties

        public ICommand PrintCommand
        {
            get
            {
                return new RelayCommand(ExecutePrintCommand, CanExecutePrintCommand);
            }
        }

        #endregion

        #region IPrintingContext Implementation

        public abstract string Label { get; }

        public abstract IEnumerable<PrintColumnDefinition> Columns { get; }

        public List<object> DataSource
        {
            get { return Data.Select(item => (object)item).ToList(); }
        }

        #endregion

        #region Internals and Helpers

        private bool CanExecutePrintCommand(object param)
        {
            return Count > 0;
        }

        private void ExecutePrintCommand(object param)
        {
            PrintManager.Print(this);
        }

        #endregion
    }
}