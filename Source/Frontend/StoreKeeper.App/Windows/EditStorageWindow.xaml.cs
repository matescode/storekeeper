using System.Windows;

namespace StoreKeeper.App.Windows
{
    /// <summary>
    /// Interaction logic for EditStorageWindow.xaml
    /// </summary>
    public partial class EditStorageWindow
    {
        public EditStorageWindow()
        {
            InitializeComponent();
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
