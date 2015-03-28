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

namespace HLib.Controls.Primitives
{
    /// <summary>
    /// 提供能够进行UI虚拟化的 Panel 布局容器.需要在 <see cref="Windows.UI.Xaml.Controls.ItemsPanelTemplate"/> 中指定.
    /// </summary>
    public class VirtualizingPanel : Panel
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        protected VirtualizingPanel() : base()
        {

        }


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


        protected override Size ArrangeOverride(Size finalSize)
        {
            List<Double> offsetX = new List<Double>();
            List<Double> offsetY = new List<Double>();

            int minIndex = 1;

            if (Orientation == Orientation.Vertical)
            {
                for (int i = 0; i < StatckCount; i++)
                {
                    offsetX.Add(GetOffsetX(i));
                    offsetY.Add(0);
                }

                foreach (var item in this.Children)
                {
                    double min = offsetY.Min();
                    minIndex = offsetY.IndexOf(min);

                    item.Arrange(new Rect(offsetX[minIndex], offsetY[minIndex], item.DesiredSize.Width, item.DesiredSize.Height));
                    var itemRequestSize = item.DesiredSize;
                    offsetY[minIndex] += (item.DesiredSize.Height + ItemsSpacing);
                }
            }
            else
            {
                for (int i = 0; i < StatckCount; i++)
                {
                    offsetX.Add(0);
                    offsetY.Add(GetOffsetY(i));
                }

                foreach (var item in this.Children)
                {
                    double min = offsetX.Min();
                    minIndex = offsetX.IndexOf(min);

                    item.Arrange(new Rect(offsetX[minIndex], offsetY[minIndex], item.DesiredSize.Width, item.DesiredSize.Height));
                    var itemRequestSize = item.DesiredSize;
                    offsetX[minIndex] += (item.DesiredSize.Width + ItemsSpacing);
                }
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            GetOwnerAndGenerator();
            var measure = base.MeasureOverride(availableSize);

            double itemFixed = 0;
            Size requestSize = Size.Empty;

            if (Orientation == Orientation.Vertical)
            {
                List<Double> offsetY = new List<Double>(StatckCount);

                itemFixed = (availableSize.Width - StatckSpacing * (StatckCount - 1)) / StatckCount;

                requestSize = new Size()
                {
                    Width = availableSize.Width
                };

                foreach (var item in this.Children)
                {
                    int minIndex = offsetY.IndexOf(offsetY.Min());
                    item.Measure(new Size(itemFixed, double.PositiveInfinity));
                    var itemRequestSize = item.DesiredSize;
                    offsetY[minIndex] += itemRequestSize.Height + ItemsSpacing;
                }
                requestSize.Height = offsetY.Max();
            }
            else
            {
                List<Double> offsetX = new new List<Double>(StatckCount);

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

        private static void RequestArrange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TestPanel).InvalidateArrange();
        }
    }
}
