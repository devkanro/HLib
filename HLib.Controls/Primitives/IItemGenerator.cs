using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HLib.Controls.Primitives
{
    /// <summary>
    /// 能自生成的虚拟化子项
    /// </summary>
    public interface IItemGenerator
    {
        Object GenerateItem();
    }
}
