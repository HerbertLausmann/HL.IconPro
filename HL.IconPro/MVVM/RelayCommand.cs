using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HL.IconPro.MVVM
{
    public class RelayCommand : ICommand
    {
        private Action<object> _Action;
        private Predicate<object> _CanExecutePredicate;

        public Action<object> Action { get { return _Action; } }
        public Predicate<object> CanExecutePredicate { get { return _CanExecutePredicate; } }

        public RelayCommand(Action<object> Action, Predicate<object> CanExecuteAction = null)
        {
            _Action = Action;
            _CanExecutePredicate = CanExecuteAction;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecutePredicate != null)
                return CanExecutePredicate.Invoke(parameter);
            else
                return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _Action?.Invoke(parameter);
        }
    }
}
