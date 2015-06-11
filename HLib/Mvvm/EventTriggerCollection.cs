using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLib.Mvvm
{
    /// <summary>
    /// 表示一个事件触发器的集合
    /// </summary>
    public class EventTriggerCollection : ObservableCollection<EventTrigger>
    {
        public EventTriggerCollection()
        {
            this.CollectionChanged += TriggerCollectionChanged;
        }

        private void TriggerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (EventTrigger item in e.NewItems)
                    {
                        item.Collection = this;
                        item.UpdataSubscribe();
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (EventTrigger item in e.OldItems)
                    {
                        item.Collection = null;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (EventTrigger item in e.OldItems)
                    {
                        item.Collection = null;
                    }
                    foreach (EventTrigger item in e.NewItems)
                    {
                        item.Collection = this;
                        item.UpdataSubscribe();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 该集合所属的对象
        /// </summary>
        public Object TargetObject { get; set; }
    }
}
