using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HLib.Mvvm
{
    /// <summary>
    /// 为控件提供基于命令的事件服务
    /// </summary>
    public static class EventService
    {
        public static EventTriggerCollection GetTriggers(DependencyObject obj)
        {
            var value = (EventTriggerCollection)obj.GetValue(TriggersProperty);
            if(value == null)
            {
                SetTriggers(obj, new EventTriggerCollection() { TargetObject = obj });
            }
            return (EventTriggerCollection)obj.GetValue(TriggersProperty);
        }

        public static void SetTriggers(DependencyObject obj, EventTriggerCollection value)
        {
            var oldValue = (EventTriggerCollection)obj.GetValue(TriggersProperty);
            obj.SetValue(TriggersProperty, value);
        }
        
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof(EventTriggerCollection), typeof(DependencyObject), null);
    }
}
