using System;
using System.Windows.Input;

namespace StoreKeeper.App.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecutePredicate;

        public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate = null)
        {
            _execute = executeAction;
            _canExecutePredicate = canExecutePredicate;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return (_canExecutePredicate == null) || _canExecutePredicate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}