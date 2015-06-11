using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HLib.Mvvm
{
    public class CommandBehaviorBase<T>
    {
        public CommandBehaviorBase(T targetObject)
        {
            TargetObject = targetObject;
        }

        public ICommand Command { get; set; }

        public void ExecuteCommand()
        {
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
        }

        public T TargetObject { get; private set; }

        public Object CommandParameter { get; set; }
    }
}
