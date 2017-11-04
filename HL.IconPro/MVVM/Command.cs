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

        public Action<object> Action { get { return _Action; } }

        public Command(Action<object> Action)
        {
            _Action = Action;
        }

        public bool CanExecute(object parameter)
        {
            return _Action != null;
        }
#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_Action != null)
                _Action(parameter);
        }
    }
}
