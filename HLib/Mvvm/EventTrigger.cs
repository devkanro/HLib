using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;


using HLib.Reflection;

namespace HLib.Mvvm
{
    /// <summary>
    /// 表示一个事件触发器
    /// </summary>
    public class EventTrigger : NotifyPropertyChanged
    {
        /// <summary>
        /// 获取该事件触发器所在的集合
        /// </summary>
        public EventTriggerCollection Collection { get; internal set; }

        private String eventName;
        private String oldEventName;
        /// <summary>
        /// 获取或设置触发命令的事件名
        /// </summary>
        public String EventName
        {
            get
            {
                return eventName;
            }
            set
            {
                oldEventName = eventName;
                eventName = value;
            }
        }

        /// <summary>
        /// 获取或设置触发的命令
        /// </summary>
        public CommandBase Command { get; set; }

        private Delegate oldEventHanlder;

        /// <summary>
        /// 刷新事件监听
        /// </summary>
        public void UpdataSubscribe()
        {
            var targetObjectClass = new Class(Collection.TargetObject);
            if (oldEventName != null)
            {
                EventHandler handler = Trigger;
                targetObjectClass.RemoveEventHandler(oldEventName, oldEventHanlder);
            }
            if (EventName != null)
            {
                EventHandler handler = Trigger;
                oldEventHanlder = targetObjectClass.AddEventHandler(EventName, Trigger);
                oldEventName = EventName;
            }
        }

        public void Unsubscribe()
        {
            var targetObjectClass = new Class(Collection.TargetObject);
            if (oldEventName != null)
            {
                EventHandler handler = Trigger;
                targetObjectClass.RemoveEventHandler(oldEventName, oldEventHanlder);
            }
        }

        private void Trigger(Object sender, Object e)
        {
            var args = new CommandExecuteArgs(sender, e);
            if (Command.CanExecute(args))
            {
                Command.Execute(args);
            }
        }
    }
}
