using System.Windows;

namespace StoreKeeper.App.Windows
{
    /// <summary>
    /// Interaction logic for ServerAdministrationWindow.xaml
    /// </summary>
    public partial class ServerAdministrationWindow
    {
        public ServerAdministrationWindow()
        {
            InitializeComponent();
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
