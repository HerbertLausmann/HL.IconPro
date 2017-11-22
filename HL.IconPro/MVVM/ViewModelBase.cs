using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HL.IconPro.MVVM
{
    public abstract class ViewModelBase : ModelBase
    {
        public ViewModelBase()
        {
            Initialize();
        }
        protected virtual void Initialize()
        {
            _Commands = new Dictionary<string, ICommand>(5);
        }
        private Dictionary<string, ICommand> _Commands;
        protected ICommand GetCommand(ICommand Command, [System.Runtime.CompilerServices.CallerMemberName]string Name = null)
        {
            begin:
            if (_Commands.TryGetValue(Name, out ICommand cmd))
                return cmd;
            else
            {
                _Commands.Add(Name, Command);
                goto begin;
            }
        }
        protected ICommand GetCommand(string Name)
        {
            if (_Commands.TryGetValue(Name, out ICommand cmd))
                return cmd;
            else
                return null;
        }
        protected void SetCommand(ICommand Command, [System.Runtime.CompilerServices.CallerMemberName]string Name = null)
        {
            if (Command == null || string.IsNullOrWhiteSpace(Name)) return;
            if (_Commands.ContainsKey(Name))
            {
                _Commands[Name] = Command;
            }
            else
            {
                _Commands.Add(Name, Command);
            }
        }
    }
}
