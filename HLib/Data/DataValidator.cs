using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using HLib.Reflection;

namespace HLib.Data
{
    /// <summary>
    /// 简单的基于事件与正则表达式的数据验证器
    /// </summary>
    public class DataValidator
    {
        /// <summary>
        /// 获取或设置需要验证器验证的属性
        /// </summary>
        public String TargetProperty { get; set; }

        /// <summary>
        /// 获取或设置验证时采用正则表达式
        /// </summary>
        public String Regex { get; set; }

        /// <summary>
        /// 获取或设置验证时,使用的值转换器,该转换器将验证的属性的值转换为字符串
        /// </summary>
        public IValueConverter ValueConverter { get; set; }

        /// <summary>
        /// 获取或设置验证时值转换器使用的参数
        /// </summary>
        public String ConvertParameter { get; set; }

        /// <summary>
        /// 获取或设置当正则表达式不匹配时的控件样式
        /// </summary>
        public Style UnmatchedStyle { get; set; }

        /// <summary>
        /// 获取或设置当正则表达式匹配时的控件样式
        /// </summary>
        public Style MatchedStyle { get; set; }

        /// <summary>
        /// 获取或设置侦听的事件,当触发当前事件时进行验证
        /// </summary>
        public String ListeningEvent { get; set; }
        
        /// <summary>
        /// 获取或设置目标对象
        /// </summary>
        public FrameworkElement TargetObject { get; set; }

        /// <summary>
        /// 获取验证时使用的委托
        /// </summary>
        public Delegate VerifyHandler { get; private set; }

        /// <summary>
        /// 获取目标对象的类信息
        /// </summary>
        public Class TargetClass { get; private set; }

        /// <summary>
        /// 取消数据验证
        /// </summary>
        public void Inactive()
        {
            if(TargetClass == null)
            {
                TargetClass = new Class(TargetObject);
            }
            if (ListeningEvent != null)
            {
                TargetClass.RemoveEventHandler(ListeningEvent, VerifyHandler);
            }
        }

        /// <summary>
        /// 开始数据验证
        /// </summary>
        public void Active()
        {
            if (TargetClass == null)
            {
                TargetClass = new Class(TargetObject);
            }
            if (ListeningEvent != null)
            {
                TargetClass.AddEventHandler(ListeningEvent, Verify);
            }
        }
        
        internal void Verify(object sender,object e)
        {
            if (TargetClass == null)
            {
                TargetClass = new Class(TargetObject);
            }
            if(System.Text.RegularExpressions.Regex.IsMatch(ValueConverter != null ? (ValueConverter.Convert(TargetClass.GetProperty(TargetProperty), typeof(String), ConvertParameter, "zh-CN")).ToString() : TargetClass.GetProperty(TargetProperty).ToString(), Regex))
            {
                if(MatchedStyle != null)
                {
                    if(MatchedStyle != (TargetObject as FrameworkElement).Style)
                    {
                        MatchedStyle.BasedOn = (TargetObject as FrameworkElement).Style;
                    }
                    (TargetObject as FrameworkElement).Style = MatchedStyle;
                }
            }
            else
            {
                if (UnmatchedStyle != null)
                {
                    if (UnmatchedStyle != (TargetObject as FrameworkElement).Style)
                    {
                        UnmatchedStyle.BasedOn = (TargetObject as FrameworkElement).Style;
                    }
                    (TargetObject as FrameworkElement).Style = UnmatchedStyle;
                }
            }

        }
    }
}
