using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace HLib.Mvvm
{
    /// <summary>
    /// 实现属性通知的基类
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 引发属性变更通知,在属性的 Set 访问器中调用时,可忽略 propertyName 参数
        /// </summary>
        /// <param name="propertyName">属性名,如果在 Set 访问器中调用该方法,该参数会自动填充</param>
        private void NotifyPropertyChange([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 通用设定属性值,并提供通知,在属性值发生更改时提供通知,在属性的 Set 访问器中调用时,可忽略 propertyName 参数,但是封箱与拆箱操作可能导致效率损失
        /// </summary>
        /// <param name="field">需要赋值的属性</param>
        /// <param name="newValue">新值</param>
        /// <param name="propertyName">属性名,如果在 Set 访问器中调用该方法,该参数会自动填充</param>
        protected void SetProperty(ref object field, object newValue, [CallerMemberName]string propertyName = "")
        {
            if (field != newValue)
            {
                field = newValue;
                NotifyPropertyChange(propertyName);
            }
        }

        /// <summary>
        /// 使用泛型的设定属性值,并提供通知,在属性值发生更改时提供通知,在属性的 Set 访问器中调用时,可忽略 propertyName 参数
        /// </summary>
        /// <typeparam name="T">属性的类型</typeparam>
        /// <param name="field">需要赋值的属性</param>
        /// <param name="newValue">新值</param>
        /// <param name="propertyName">属性名,如果在 Set 访问器中调用该方法,该参数会自动填充</param>
        protected void SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = "")
        {
            if (!object.Equals(field, newValue))
            {
                field = newValue;
                NotifyPropertyChange(propertyName);
            }
        }
    }
}
