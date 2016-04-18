using System.Windows.Controls;
using StoreKeeper.App.Searching;

namespace StoreKeeper.App.Controls
{
    /// <summary>
    /// Interaction logic for ExternStorageMaterialListControl.xaml
    /// </summary>
    public partial class ExternStorageMaterialListControl : ISearchControl
    {
        public ExternStorageMaterialListControl()
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
