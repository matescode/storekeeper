using System;
using System.Diagnostics;
using CommonBase.Exceptions;
using CommonBase.Utils;

namespace CommonBase.Resources
{
    public class ResourceLibrary : IResourceProvider
    {
        private readonly ResourceList _resourceList;
        private ILocalizationProvider _localizationProvider;

        public ResourceLibrary()
        {
            if (Instance != null)
            {
                throw new SingletonAlreadyInitializedException(typeof(ResourceLibrary));
            }
            _resourceList = new ResourceList();
            _resourceList.InitializeComponent();

            InitLocalization();
            
            Instance = this;
        }

        #region Properties

        private static ResourceLibrary Instance
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        public static T Service<T>() where T : class
        {
            if (typeof(T) == typeof(IResourceProvider))
            {
                return Instance as T;
            }

            if (typeof(T) == typeof(ILocalizationProvider))
            {
                return Instance._localizationProvider as T;
            }
            T service = Instance.GetService<T>();
            return service ?? DefaultServiceProvider.GetDefaultProvider<T>(typeof(ResourceLibrary));
        }

        public static void Close()
        {
            if (Instance == null)
            {
                Debug.Assert(false, "Instance is already closed.");
            }
            Instance = null;
        }

        #endregion

        #region IResourceProvider Members

        public T GetResource<T>(string resourceKey) where T : class
        {
            if (_resourceList.Contains(resourceKey))
            {
                return _resourceList[resourceKey] as T;
            }
            return GetApplicationResource(resourceKey) as T;
        }

        #endregion

        #region Internals and Helpers

        private void InitLocalization()
        {
            _localizationProvider = new LocalizationProvider(new XmlTranslationProvider());
            _localizationProvider.AddLocalizationFile(new Uri(@"pack://application:,,,/CommonBase.Resources;Component/LocalizationData/BaseLocalization-en.xml", UriKind.Absolute));
            _localizationProvider.AddLocalizationFile(new Uri(@"pack://application:,,,/CommonBase.Resources;Component/LocalizationData/BaseLocalization-cs.xml", UriKind.Absolute));
        }

        protected virtual T GetService<T>() where T : class
        {
            return null;
        }

        protected virtual object GetApplicationResource(string resourceKey)
        {
            return null;
        }

        public virtual void PrepareLocalizationResourceFiles()
        {
        }

        #endregion
    }
}