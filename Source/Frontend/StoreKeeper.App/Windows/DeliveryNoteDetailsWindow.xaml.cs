using System.Windows;

namespace StoreKeeper.App.Windows
{
    /// <summary>
    /// Interaction logic for DeliveryNoteDetailsWindow.xaml
    /// </summary>
    public partial class DeliveryNoteDetailsWindow
    {
        public DeliveryNoteDetailsWindow()
        {
            InitializeComponent();
        }

        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
