using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HLib.Mvvm
{
    /// <summary>
    /// 提供一个实现 <see cref="ICommand"/> 接口的基本命令,当被触发时会自动调用 <see cref="CommandBase.ExecuteDelegate"/> 委托
    /// </summary>
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 表示命令执行时的委托
        /// </summary>
        public EventHandler<object> ExecuteDelegate;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            if(parameter is CommandExecuteArgs)
            {
                ExecuteDelegate?.Invoke((parameter as CommandExecuteArgs).Sender, (parameter as CommandExecuteArgs).Parameter);
            }
            else
            {
                ExecuteDelegate?.Invoke(null, new ExecuteArgs(parameter));
            }
        }
    }
    

    /// <summary>
    /// 表示命令非标准执行时使用的参数
    /// </summary>
    public class ExecuteArgs : EventArgs
    {
        public ExecuteArgs() : this(null)
        {

        }

        public ExecuteArgs(object parameter)
        {
            Parameter = parameter;
        }

        public object Parameter { get; set; }
    }
}