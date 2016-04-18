using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Resources;
using System.Xml;

namespace CommonBase.Resources
{
    internal class XmlTranslationProvider : ITranslationProvider
    {
        private Dictionary<CultureInfo, Dictionary<string, string>> _translations;
        private List<CultureInfo> _supportedCultures;
        private List<Uri> _localizationResourceUris;

        public XmlTranslationProvider()
        {
            _translations = new Dictionary<CultureInfo, Dictionary<string, string>>();
            _supportedCultures = new List<CultureInfo>();
            _localizationResourceUris = new List<Uri>();
        }

        #region ITranslationProvider Implementation

        public IEnumerable<CultureInfo> SupportedLanguages
        {
            get { return _supportedCultures; }
        }

        public void LoadLocalization()
        {
            foreach (Uri uriResource in _localizationResourceUris)
            {
                
                try
                {
                    StreamResourceInfo resourceInfo = Application.GetResourceStream(uriResource);
                    if (resourceInfo != null)
                    {
                        LoadLocalization(resourceInfo.Stream);
                    }
                }
                catch
                {
                    // TODO throw exception
                }
            }
        }

        public void AddLocalizationFile(Uri uriResource)
        {
            _localizationResourceUris.Add(uriResource);
        }

        public object Translate(string key)
        {
            return Translate(key, Thread.CurrentThread.CurrentUICulture);
        }

        public object Translate(string key, CultureInfo culture)
        {
            return TryGetTranslation(key, culture);
        }

        #endregion

        #region Internals and Helpers

        private object TryGetTranslation(string key, CultureInfo culture)
        {
            Dictionary<string, string> cultureData;
            if (_translations.TryGetValue(culture, out cultureData))
            {
                string translation;
                if (cultureData.TryGetValue(key, out translation))
                {
                    return translation;
                }
            }
            return null;
        }

        private void LoadLocalization(Stream stream)
        {
            XmlDocument document = new XmlDocument();
            document.Load(stream);

            XmlElement rootElement = document["Localization"];
            string cultureName = rootElement.Attributes["Culture"].Value;
            
            CultureInfo culture = new CultureInfo(cultureName);

            Dictionary<string,string> values;
            if (!_translations.TryGetValue(culture, out values))
            {
                values = new Dictionary<string, string>();
                _translations.Add(culture, values);
            }

            XmlNodeList nodeList = rootElement.SelectNodes("//String");
            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlNode stringElem = nodeList[i];
                string key = stringElem.Attributes["Key"].Value;
                values[key] = stringElem.InnerText;
            }
        }

        #endregion
    }
}