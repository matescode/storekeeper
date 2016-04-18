using System;
using System.Globalization;
using CommonBase.Resources;

namespace StoreKeeper.Resources
{
    public class StoreKeeperResourceLibrary : ResourceLibrary
    {
        private readonly StoreKeeperResourceList _resourceList;

        public StoreKeeperResourceLibrary()
        {
            _resourceList = new StoreKeeperResourceList();
            _resourceList.InitializeComponent();
        }

        #region Overrides

        protected override object GetApplicationResource(string resourceKey)
        {
            if (_resourceList.Contains(resourceKey))
            {
                return _resourceList[resourceKey];
            }
            return base.GetApplicationResource(resourceKey);
        }

        public override void PrepareLocalizationResourceFiles()
        {
            ILocalizationProvider localizationProvider = Service<ILocalizationProvider>();
            localizationProvider.CurrentLanguage = new CultureInfo("cs-CZ");
            localizationProvider.AddLocalizationFile(new Uri(@"pack://application:,,,/StoreKeeper.Resources;Component/Localization/Localization-en.xml", UriKind.Absolute));
            localizationProvider.AddLocalizationFile(new Uri(@"pack://application:,,,/StoreKeeper.Resources;Component/Localization/Localization-cs.xml", UriKind.Absolute));
        }

        #endregion
    }
}