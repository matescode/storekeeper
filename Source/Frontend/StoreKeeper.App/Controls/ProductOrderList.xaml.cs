using System.Windows.Controls;
using StoreKeeper.App.Searching;

namespace StoreKeeper.App.Controls
{
    /// <summary>
    /// Interaction logic for ProductOrderList.xaml
    /// </summary>
    public partial class ProductOrderList : ISearchControl
    {
        public ProductOrderList()
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
