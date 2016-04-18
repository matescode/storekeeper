using System.ComponentModel;

namespace CommonBase.UI.Windows
{
    /// <summary>
    /// Interaction logic for BusyWindow.xaml
    /// </summary>
    public partial class BusyWindow
    {
        private bool _forceClose;

        public BusyWindow()
        {
            InitializeComponent();
        }

        private void BusyWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !_forceClose;
        }

        public void ForceClose()
        {
            _forceClose = true;
            Close();
        }

        public string WindowTitle
        {
            set { Title = value; }
        }

        public string OperationName
        {
            set { OperationLabel.Content = value; }
        }
    }
}
