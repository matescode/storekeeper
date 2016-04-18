using System;
using System.ComponentModel;
using System.Windows;
using CommonBase.Resources;

namespace CommonBase.UI.Localization
{
    public class TranslationData : IWeakEventListener, INotifyPropertyChanged, IDisposable
    {
        private string _key;
        private ILocalizationProvider _localizationProvider;

        public TranslationData(string key)
        {
            _localizationProvider = UIApplication.Service<ILocalizationProvider>();
            _key = key;
            LanguageChangedEventManager.AddListener(_localizationProvider, this);
        }

        ~TranslationData()
        {
            Dispose(false);
        }

        public object Value
        {
            get
            {
                return _localizationProvider.Translate(_key);
            }
        }

        #region IWeakEventListener Implementation

        public bool ReceiveWeakEvent(Type managerType,
                                object sender, EventArgs e)
        {
            if (managerType == typeof(LanguageChangedEventManager))
            {
                OnLanguageChanged(sender, e);
                return true;
            }
            return false;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Internals and Helpers

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LanguageChangedEventManager.RemoveListener(_localizationProvider, this);
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        #endregion
    }
}