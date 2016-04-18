using System;
using System.Collections.Generic;
using System.Globalization;

namespace CommonBase.Resources
{
    public interface ILocalizationProvider
    {
        event EventHandler LanguageChanged;

        CultureInfo CurrentLanguage { get; set; }

        IEnumerable<CultureInfo> SupportedLanguages { get; }

        object Translate(string key);

        void LoadLocalizationContent();

        void AddLocalizationFile(Uri uriResource);
    }
}