﻿using System.Windows.Data;
using System.Windows.Markup;

namespace CommonBase.UI.Localization
{
    public class TranslateExtension : MarkupExtension
    {
        private string _key;

        public TranslateExtension(string key)
        {
            _key = key;
        }

        [ConstructorArgument("key")]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
#if DEBUG
            return _key;
#else
            Binding binding = new Binding("Value")
            {
                Source = new TranslationData(_key)
            };
            return binding.ProvideValue(serviceProvider);
#endif

        }
    }
}