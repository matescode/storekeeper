using System;
using System.Windows;
using CommonBase.Application;
using CommonBase.UI;

namespace StoreKeeper.App
{
    public partial class StoreKeeperApplication
    {
        public StoreKeeperApplication()
        {
            StartupUri = new Uri("Windows/MainWindow.xaml", UriKind.Relative);
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        #region Overrides

        protected override void OnStartup(StartupEventArgs e)
        {
            new Application();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            UIApplication.Close();
            ApplicationContext.Close();
            base.OnExit(e);
        }

        #endregion
    }
}