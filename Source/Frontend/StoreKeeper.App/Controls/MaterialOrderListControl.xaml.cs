using System.Windows.Controls;
using StoreKeeper.App.Searching;

namespace StoreKeeper.App.Controls
{
    /// <summary>
    /// Interaction logic for MaterialOrderListControl.xaml
    /// </summary>
    public partial class MaterialOrderListControl : ISearchControl
    {
        public MaterialOrderListControl()
        {
            InitializeComponent();
        }

        #region ISearchControl Implementation

        public DataGrid DataGrid
        {
            get { return MainGrid; }
        }

        #endregion
    }
}
