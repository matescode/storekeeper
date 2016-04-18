using System.Windows.Controls;
using StoreKeeper.App.Searching;

namespace StoreKeeper.App.Controls
{
    /// <summary>
    /// Interaction logic for MaterialListControl.xaml
    /// </summary>
    public partial class MaterialListControl : ISearchControl
    {
        public MaterialListControl()
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
