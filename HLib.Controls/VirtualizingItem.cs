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
    /// 实现可虚拟化的子项的范例类之一,该范例类使用了 <see cref="Primitives.IItemGenerator"/> 接口,适用于增量加载虚拟化
    /// </summary>
    //  实现虚拟化的思路为:
    //  提供继承了 <see cref="Primitives.IItemGenerator"/> 接口的 ViewModel ,使用 ViewModel 实时生成 View.
    //  进行虚拟化时,设置 Content 为 null,释放子项占用的内存,ControlContent 属性会保留 ViewModel.
    //  进行实例化时,使用 ViewModel 的 GenerateItem 方法生成相应的 View,此时将会将生成的结果赋值给 Content.
    public class VirtualizingItem : ListViewItem, Primitives.IVirtualizingItem
    {
        /// <summary>
        /// 控件内容
        /// </summary>
        public Primitives.IItemGenerator ControlContent
        {
            get { return (Primitives.IItemGenerator)GetValue(ControlContentProperty); }
            set { SetValue(ControlContentProperty, value); }
        }

        public static readonly DependencyProperty ControlContentProperty =
            DependencyProperty.Register(nameof(ControlContent), typeof(Primitives.IItemGenerator), typeof(VirtualizingItem), new PropertyMetadata(null, OnControlContentChanged));

        private static void OnControlContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingItem item = d as VirtualizingItem;
            
            if (!item.IsVirtualized)
            {
                item.IsInternalChangeContent = true;
                item.Content = (e.NewValue as Primitives.IItemGenerator).GenerateItem();
            }
        }

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
        
        /// <summary>
        /// 判断是否是内部进行呈现内容变更
        /// </summary>
        private bool IsInternalChangeContent = false;
        
        public VirtualizingItem()
        {
            //this.DefaultStyleKey = typeof(VirtualizingItem);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if(IsVirtualized)
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
            if(!IsVirtualized)
            {
                IsInternalChangeContent = true;
                this.IsVirtualized = true;
                RealSize = this.DesiredSize;
                //从可视化树移除内容
                this.Content = null;
            }
        }

        /// <summary>
        /// 可重新,实现实例化子项的过程
        /// </summary>
        protected virtual void OnRealize()
        {
            if(IsVirtualized)
            {
                IsInternalChangeContent = true;
                this.IsVirtualized = false;
                //从可视化树呈现内容
                this.Content = this.ControlContent.GenerateItem();
            }
        }
    }
}
