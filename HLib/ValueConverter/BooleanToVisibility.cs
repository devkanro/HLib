using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HLib.ValueConverter
{
    /// <summary>
    /// 进行 <see cref="Boolean"/> 与 <see cref="Visibility"/> 之间的转换,可以提供任意 parameter 翻转结果
    /// </summary>
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ^ parameter != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if((Visibility)value == Visibility.Visible)
            {
                return true ^ parameter != null;
            }
            return false ^ parameter != null;
        }
    }
}
