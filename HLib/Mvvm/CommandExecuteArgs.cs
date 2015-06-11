using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLib.Mvvm
{
    /// <summary>
    /// 表示调用 <see cref="CommandBase.Execute(object)"/> 时传递的参数
    /// </summary>
    public class CommandExecuteArgs
    {
        public CommandExecuteArgs() : this(null, null)
        {

        }

        public CommandExecuteArgs(object parameter) : this(null, parameter)
        {

        }

        public CommandExecuteArgs(object sender, object parameter)
        {
            Sender = sender;
            Parameter = parameter;
        }

        public object Sender { get; set; }
        public object Parameter { get; set; }
    }
}
