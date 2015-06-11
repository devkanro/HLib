using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HLib.Data
{
    /// <summary>
    /// 提供简单的数据验证服务
    /// </summary>
    public static class DataVerificationService
    {
        /// <summary>
        /// 获取数据验证器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DataValidator GetValidator(FrameworkElement obj)
        {
            return (DataValidator)obj.GetValue(ValidatorProperty);
        }

        /// <summary>
        /// 设置数据验证器
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetValidator(FrameworkElement obj, DataValidator value)
        {
            GetValidator(obj)?.Inactive();
            value.TargetObject = obj as FrameworkElement;
            obj.SetValue(ValidatorProperty, value);
            value?.Active();
        }
        
        public static readonly DependencyProperty ValidatorProperty =
            DependencyProperty.RegisterAttached("Validator", typeof(DataValidator), typeof(FrameworkElement), null);


    }
}
