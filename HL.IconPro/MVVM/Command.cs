using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HL.IconPro.MVVM
{
    public class Command : ICommand
    {
        private Action<object> _Action;
        private Predicate<object> _CanExecuteAction;

        public Action<object> Action { get { return _Action; } }
        public Predicate<object> CanExecuteAction { get { return _CanExecuteAction; } }

        public Command(Action<object> Action, Predicate<object> CanExecuteAction = null)
        {
            _Action = Action;
            _CanExecuteAction = CanExecuteAction;
            OnCanExecuteChanged();
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteAction != null)
                return CanExecuteAction.Invoke(parameter);
            else
                return true;
        }

        public event EventHandler CanExecuteChanged;

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public void Execute(object parameter)
        {
            _Action?.Invoke(parameter);
            OnCanExecuteChanged();
        }

        public void Refresh()
        {
            OnCanExecuteChanged();
        }
    }
}
