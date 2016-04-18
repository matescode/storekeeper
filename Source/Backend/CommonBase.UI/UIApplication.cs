using System;
using System.Windows.Media;
using CommonBase.Application;
using CommonBase.Application.Messages;
using CommonBase.Resources;
using CommonBase.UI.MessageDialogs;
using CommonBase.UI.ViewModels;
using CommonBase.UI.Windows;

namespace CommonBase.UI
{
    public abstract class UIApplication : ContextInstance<UIApplication>
    {
        private readonly IMessageStack _stack;
        private IMessageDialogs _messageDialogs;
        private IResourceProvider _resourceProvider;
        private ILocalizationProvider _localization;

        protected UIApplication()
        {
            _stack = new MessageStack();

            ApplicationContext appContext = CreateApplicationContext();
            if (appContext == null)
            {
                throw new InvalidOperationException("Application context cannot be null!");
            }

            InitResourceLibrary();
        }

        #region Properties

        public static IMessageStack MessageStack
        {
            get
            {
                return Instance._stack;
            }
        }

        public static IMessageDialogs MessageDialogs
        {
            get
            {
                return Instance._messageDialogs ?? (Instance._messageDialogs = new MessageDialogs.MessageDialogs(ApplicationContext.Info.LongName));
            }
        }

        private IResourceProvider Resources
        {
            get
            {
                return _resourceProvider ?? (_resourceProvider = ResourceLibrary.Service<IResourceProvider>());
            }
        }

        private ILocalizationProvider Localization
        {
            get
            {
                return _localization ?? (_localization = ResourceLibrary.Service<ILocalizationProvider>());
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract ApplicationContext CreateApplicationContext();

        protected virtual ResourceLibrary CreateResourceLibrary()
        {
            return new ResourceLibrary();
        }

        #endregion

        #region Public Methods

        public static void ShowAboutWindow(IApplicationInfo applicationInfo, ImageSource applicationIcon, string eula, string copyright)
        {
            AboutWindowViewModel viewModel = new AboutWindowViewModel(applicationInfo, applicationIcon, eula, copyright);
            AboutWindow window = new AboutWindow { DataContext = viewModel };
            window.ShowDialog();
        }

        public static void Close()
        {
            Instance._localization = null;
            Instance._resourceProvider = null;
            ResourceLibrary.Close();
            CloseInstance();
        }

        public static void LoadLocalization()
        {
            Instance.Localization.LoadLocalizationContent();
        }

        #endregion

        #region Overrides

        public override T GetService<T>()
        {
            if (typeof(T) == typeof(IResourceProvider))
            {
                return Instance.Resources as T;
            }

            if (typeof(T) == typeof(ILocalizationProvider))
            {
                return Instance.Localization as T;
            }

            return base.GetService<T>();
        }

        #endregion

        #region Internals and Helpers

        private void InitResourceLibrary()
        {
            ResourceLibrary resourceLibrary = CreateResourceLibrary();
            resourceLibrary.PrepareLocalizationResourceFiles();
            Localization.LoadLocalizationContent();
        }

        #endregion
    }
}