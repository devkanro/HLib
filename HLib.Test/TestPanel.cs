using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

using HLib;

namespace HLib.Test
{
    public class TestPanel : Panel
    {
        private ItemContainerGenerator itemContainerGenerator;
        private ItemsControl itemsOwner;
        private ScrollViewer scrollOwner;
        private bool isGeneratorHooked;

        private ItemContainerGenerator GetOwnerAndGenerator()
        {
            itemsOwner = ItemsControl.GetItemsOwner(this);
            if (itemsOwner == null)
            {
                throw new InvalidOperationException("无法从 VirtualizingPanel 找到父 ItemsControl.");
            }

            scrollOwner = VisualThreeHelper.FindVisualElement<ScrollViewer>(itemsOwner);
            if (scrollOwner == null)
            {
                throw new InvalidOperationException("无法从父 ItemsControl 找到 ScrollView");
            }

            itemContainerGenerator = itemsOwner.ItemContainerGenerator;
            if (itemContainerGenerator == null)
            {
                throw new InvalidOperationException("无法获取 VirtualizingPanel 所属 ItemsControl 的 ItemContainerGenerator");
            }

            if (!isGeneratorHooked)
            {
                isGeneratorHooked = true;
                itemContainerGenerator.ItemsChanged += OnItemsChangedHandler;
                ScrollOwner.ViewChanging += ScrollOwner_ViewChanging;
                //itemContainerGenerator.RemoveAll();
            }
            return itemContainerGenerator;
        }

        private void ScrollOwner_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {

        }

        public ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (itemContainerGenerator == null)
                {
                    GetOwnerAndGenerator();
                }
                return itemContainerGenerator;
            }
        }
        public ItemsControl ItemsOwner
        {
            get
            {
                if (itemsOwner == null)
                {
                    GetOwnerAndGenerator();
                }
                return itemsOwner;
            }
        }

        public ScrollViewer ScrollOwner
        {
            get
            {
                if (scrollOwner == null)
                {
                    GetOwnerAndGenerator();
                }
                return scrollOwner;
            }
        }

        protected virtual void BringIndexIntoView(int index) { }
        protected virtual void OnClearChildren() { }
        protected virtual void OnItemsChanged(object sender, ItemsChangedEventArgs args) { }
        private void OnItemsChangedHandler(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                default:
                    break;
            }
            this.OnItemsChanged(sender, args);
            base.InvalidateMeasure();
        }

        protected void RemoveChildRange(int index, int range)
        {
            for (int i = index + range; i > index;)
            {
                base.Children.RemoveAt(--i);
            }
        }

        private double GetOffsetX(int index)
        {
            return index * (this.DesiredSize.Width + StatckSpacing) / StatckCount;
        }

        private double GetOffsetY(int index)
        {
            return index * (this.DesiredSize.Height + StatckSpacing) / StatckCount;
        }

        /// <summary>
        /// 对面板内的元素进行布局
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //建立两个列表储存 Item 的X坐标和Y坐标
            List<Double> offsetX = new List<Double>();
            List<Double> offsetY = new List<Double>();

            //最短栈默认为第一个
            int minIndex = 0;

            //判定布局类型
            if (Orientation == Orientation.Vertical)
            {
                //纵向布局

                //初始化坐标,由于是纵向布局,纵坐标是从0开始,横坐标则是固定值
                for (int i = 0; i < StatckCount; i++)
                {
                    //这里的GetOffsetX是计算每个栈的横坐标,计算过程是这样的:
                    //(int index) => index * (this.DesiredSize.Width + StatckSpacing) / StatckCount
                    offsetX.Add(GetOffsetX(i));
                    offsetY.Add(0);
                }

                //遍历 Children 进行布局
                foreach (var item in this.Children)
                {
                    //取最短的 Stack 加入新的 Item
                    double min = offsetY.Min();
                    //获取最短的 Stack 的编号
                    minIndex = offsetY.IndexOf(min);

                    //对 item 进行布局
                    item.Arrange(new Rect(offsetX[minIndex], offsetY[minIndex], item.DesiredSize.Width, item.DesiredSize.Height));
                    //递增纵坐标
                    offsetY[minIndex] += (item.DesiredSize.Height + ItemsSpacing);
                }
            }
            else
            {
                //横向布局,内容也是大同小异

                for (int i = 0; i < StatckCount; i++)
                {
                    offsetX.Add(0);
                    //这里的 GetOffsetY 的计算过程如下:
                    //(int index) => index * (this.DesiredSize.Height + StatckSpacing) / StatckCount
                    offsetY.Add(GetOffsetY(i));
                }

                foreach (var item in this.Children)
                {
                    double min = offsetX.Min();
                    minIndex = offsetX.IndexOf(min);

                    item.Arrange(new Rect(offsetX[minIndex], offsetY[minIndex], item.DesiredSize.Width, item.DesiredSize.Height));
                    offsetX[minIndex] += (item.DesiredSize.Width + ItemsSpacing);
                }
            }

            //直接返回参数
            return finalSize;
        }

        /// <summary>
        /// 测量面板需要的空间
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            var measure = base.MeasureOverride(availableSize);

            double itemFixed = 0;
            Size requestSize = Size.Empty;

            //判断面板的布局类型,是横向布局还是纵向布局
            if (Orientation == Orientation.Vertical)
            {
                //纵向布局

                //创建一个列表记录所有 Stack 的长度
                List<Double> offsetY = new Double[StatckCount].ToList();

                //计算一个 Item 的固定边长度,纵向布局的话是宽固定
                itemFixed = (availableSize.Width - StatckSpacing * (StatckCount - 1)) / StatckCount;
                requestSize = new Size()
                {
                    //设定需要的空间的宽,一般是提供多少要多少
                    Width = availableSize.Width
                };

                //遍历 Children 来测量长度
                foreach (var item in this.Children)
                {
                    //寻找最短的 Stack ,将新的 Item 分配到这个 Stack
                    int minIndex = offsetY.IndexOf(offsetY.Min());
                    //向 Item 发送测量请求,让 Item 测量自己需要的空间
                    item.Measure(new Size(itemFixed, double.PositiveInfinity));
                    //测量结果保存在 DesiredSize 属性里面
                    var itemRequestSize = item.DesiredSize;
                    //将这个 Stack 的长度加上新的 Item 的长度
                    offsetY[minIndex] += itemRequestSize.Height + ItemsSpacing;
                }
                //寻找最长的 Stack,这个 Stack 就是面板需要的高度
                requestSize.Height = offsetY.Max();
            }
            else
            {
                //横向布局,内容大同小异,区别就是把长变成了宽

                List<Double> offsetX = new Double[StatckCount].ToList();

                //Item 的固定边为长
                itemFixed = (availableSize.Height - StatckSpacing * (StatckCount - 1)) / StatckCount;

                requestSize = new Size()
                {
                    Height = availableSize.Height
                };

                foreach (var item in this.Children)
                {
                    int minIndex = offsetX.IndexOf(offsetX.Min());
                    item.Measure(new Size(double.PositiveInfinity, itemFixed));
                    var itemRequestSize = item.DesiredSize;
                    offsetX[minIndex] += itemRequestSize.Width;
                }
                requestSize.Width = offsetX.Max();
            }

            //返回我们面板需要的大小
            return requestSize;
        }



        /// <summary>
        /// 设定 <see cref="HLib.Controls.Primitives.VirtualizingPanel"/> 的布局方向.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualizingPanel), new PropertyMetadata(Windows.UI.Xaml.Controls.Orientation.Vertical, RequestArrange));

        /// <summary>
        /// 设定 <see cref="HLib.Controls.Primitives.VirtualizingPanel"/> 的栈布局个数,最小值为1.
        /// </summary>
        public int StatckCount
        {
            get { return (int)GetValue(StatckCountProperty); }
            set { SetValue(StatckCountProperty, value); }
        }

        public static readonly DependencyProperty StatckCountProperty =
            DependencyProperty.Register("StatckCount", typeof(int), typeof(VirtualizingPanel), new PropertyMetadata(1, RequestArrange));

        /// <summary>
        /// 设定 <see cref="HLib.Controls.Primitives.VirtualizingPanel"/> 的栈布局的间距.
        /// </summary>
        public Double StatckSpacing
        {
            get { return (Double)GetValue(StatckSpacingProperty); }
            set { SetValue(StatckSpacingProperty, value); }
        }

        public static readonly DependencyProperty StatckSpacingProperty =
            DependencyProperty.Register("StatckSpacing", typeof(Double), typeof(VirtualizingPanel), new PropertyMetadata(10, RequestArrange));


        /// <summary>
        /// 设定 <see cref="HLib.Controls.Primitives.VirtualizingPanel"/> 的子元素的间距.
        /// </summary>
        public Double ItemsSpacing
        {
            get { return (Double)GetValue(ItemsSpacingProperty); }
            set { SetValue(ItemsSpacingProperty, value); }
        }


        public static readonly DependencyProperty ItemsSpacingProperty =
            DependencyProperty.Register("ItemsSpacing", typeof(Double), typeof(VirtualizingPanel), new PropertyMetadata(10, RequestArrange));

        /// <summary>
        /// 请求重新测量与布局面板
        /// </summary>
        private static void RequestArrange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TestPanel).InvalidateMeasure();
            (d as TestPanel).InvalidateArrange();
        }
    }
}
