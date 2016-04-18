using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace CommonBase.Resources
{
    internal class LocalizationProvider : ILocalizationProvider
    {
        public LocalizationProvider(ITranslationProvider translationProvider)
        {
            TranslationProvider = translationProvider;
        }

        public ITranslationProvider TranslationProvider { get; private set; }

        #region ILocalizationProvider Implementation

        public event EventHandler LanguageChanged;

        public CultureInfo CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if (!Equals(value, Thread.CurrentThread.CurrentUICulture))
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    OnLanguageChanged();
                }
            }
        }

        public IEnumerable<CultureInfo> SupportedLanguages
        {
            get
            {
                if (TranslationProvider != null)
                {
                    return TranslationProvider.SupportedLanguages;
                }
                return Enumerable.Empty<CultureInfo>();
            }
        }

        public object Translate(string key)
        {
            if (TranslationProvider != null)
            {
                object translatedValue = TranslationProvider.Translate(key);
                if (translatedValue != null)
                {
                    return translatedValue;
                }
            }
            return string.Format("!{0}!", key);
        }

        public void LoadLocalizationContent()
        {
            TranslationProvider.LoadLocalization();
        }

        public void AddLocalizationFile(Uri uriResource)
        {
            TranslationProvider.AddLocalizationFile(uriResource);
        }

        #endregion

        #region Internals and Helpers

        private void OnLanguageChanged()
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}