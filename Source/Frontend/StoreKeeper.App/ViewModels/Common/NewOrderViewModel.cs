using System;
using System.Windows.Input;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Common
{
    public class NewOrderViewModel : ViewModelBase
    {
        private string _code;
        private readonly ArticleCodeType _codeType;
        private readonly Action<object> _executeAction;
        private readonly Predicate<object> _canExecutePredicate;

        public NewOrderViewModel(ArticleCodeType codeType, Action<object> executeAction, Predicate<object> canExecutePredicate = null)
        {
            _codeType = codeType;
            _executeAction = executeAction;
            _canExecutePredicate = canExecutePredicate;
        }

        #region Properties

        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                NotifyPropertyChanged("Code");
            }
        }

        public ArticleCodeType CodeType
        {
            get { return _codeType; }
        }

        public ICommand AddOrderCommand
        {
            get
            {
                return new RelayCommand(_executeAction, _canExecutePredicate);
            }
        }

        #endregion
    }
}