using System.Windows;

namespace CommonBase.UI.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void OkbuttonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
