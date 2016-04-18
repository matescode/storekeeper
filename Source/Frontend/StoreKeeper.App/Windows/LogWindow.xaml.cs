using System.Windows;
using CommonBase.Application;
using CommonBase.UI;
using CommonBase.UI.Localization;
using StoreKeeper.App.Log;

namespace StoreKeeper.App.Windows
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow
    {
        private ILogBrowser _logBrowser;

        public LogWindow()
        {
            InitializeComponent();
            InitLogs();
        }

        private void InitLogs()
        {
            _logBrowser = ApplicationContext.Service<ILogBrowser>();
            ReloadEntries();
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearButtonClicked(object sender, RoutedEventArgs e)
        {
            _logBrowser.Clear();
            ReloadEntries();
        }

        private void ReloadEntries()
        {
            LogControl.RemoveContent();
            LogControl.SetLogEntries(_logBrowser.LogEntries);
        }

        private void SaveLogButtonClicked(object sender, RoutedEventArgs e)
        {
            if (LogControl.SaveContent())
            {
                UIApplication.MessageDialogs.Info("LogEntriesSaved".Localize());
            }
        }
    }
}
