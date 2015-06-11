using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace HLib.Controls
{
    /// <summary>
    /// 实现可虚拟化的子项的范例类之一,该范例类使用了 <see cref="Primitives.IVirtualizingItem"/> 接口,适用于自定义虚拟化
    /// </summary>
    //  实现虚拟化的思路为:
    //  Content 的内容需要继承 IVirtualizingItem 接口,当需要虚拟化时,调用接口的实现,实现自定义虚拟化
    //  对比 VirtualizingItem 的优势为:
    //  01.不需要反复重新生成 View 减少性能开销
    //  02.用户可以根据自己的需求进行轻量的虚拟化,可以选择释放占用内存大的元素
    public class CustomVirtualizingItem : ListViewItem, Primitives.IVirtualizingItem
    {
        /// <summary>
        /// 布局占用大小
        /// </summary>
        public Size RealSize
        {
            get { return (Size)GetValue(RealSizeProperty); }
            set { SetValue(RealSizeProperty, value); }
        }

        public static readonly DependencyProperty RealSizeProperty =
            DependencyProperty.Register(nameof(RealSize), typeof(Size), typeof(VirtualizingItem), new PropertyMetadata(Size.Empty));

        /// <summary>
        /// 是否处于虚拟化状态
        /// </summary>
        public bool IsVirtualized { get; private set; }

        /// <summary>
        /// 是否可进行虚拟化
        /// </summary>
        public bool CanVirtualize { get { return !IsVirtualized; } }

        /// <summary>
        /// 是否可进行实例化
        /// </summary>
        public bool CanRealize { get { return IsVirtualized; } }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsVirtualized)
            {
                //虚拟化时固定大小
                return RealSize;
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }

        /// <summary>
        /// 虚拟化子项
        /// </summary>
        public void Virtualize()
        {
            OnVirtualize();
        }

        /// <summary>
        /// 实例化子项
        /// </summary>
        public void Realize()
        {
            OnRealize();
        }

        /// <summary>
        /// 可重写,实现虚拟化子项的过程
        /// </summary>
        protected virtual void OnVirtualize()
        {
            if (!IsVirtualized)
            {
                this.IsVirtualized = true;
                RealSize = this.DesiredSize;
                //从可视化树移除内容
                if(this.Content is Primitives.IVirtualizingItem)
                {
                    (this.Content as Primitives.IVirtualizingItem).Virtualize();
                }
            }
        }

        /// <summary>
        /// 可重新,实现实例化子项的过程
        /// </summary>
        protected virtual void OnRealize()
        {
            if (IsVirtualized)
            {
                this.IsVirtualized = false;
                //从可视化树呈现内容
                if (this.Content is Primitives.IVirtualizingItem)
                {
                    (this.Content as Primitives.IVirtualizingItem).Realize();
                }
            }
        }
    }
}
