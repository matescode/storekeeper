using System;
using System.Collections.Generic;
using System.Globalization;

namespace CommonBase.Resources
{
    public interface ITranslationProvider
    {
        IEnumerable<CultureInfo> SupportedLanguages { get; }

        void LoadLocalization();

        void AddLocalizationFile(Uri uriResource);

        object Translate(string key);

        object Translate(string key, CultureInfo culture);
    }
}