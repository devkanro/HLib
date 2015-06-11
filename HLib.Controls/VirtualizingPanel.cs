using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HLib.Controls
{
    public class VirtualizingPanel : Panel
    {
        ///<summary>
        ///设定<see cref="HLib.Controls.VirtualizingPanel"/>的栈布局个数,最小值为1.
        ///</summary>
        public int StackCount
        {
            get { return (int)GetValue(StackCountProperty); }
            set { SetValue(StackCountProperty, value); }
        }

        public static readonly DependencyProperty StackCountProperty =
                DependencyProperty.Register(nameof(StackCount), typeof(int), typeof(VirtualizingPanel), new PropertyMetadata(1, OnStackCountChanged));

        private static void OnStackCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(StackCount), "VirtualizingPanel 的 StackCount 最小值为1.");
            }
            (d as VirtualizingPanel).RequestArrange();
        }

        ///<summary>
        ///设定<see cref="HLib.Controls.VirtualizingPanel"/>的栈布局的间距.
        ///</summary>
        public Double StackSpacing
        {
            get { return (Double)GetValue(StackSpacingProperty); }
            set { SetValue(StackSpacingProperty, value); }
        }

        public static readonly DependencyProperty StackSpacingProperty =
                DependencyProperty.Register(nameof(StackSpacing), typeof(Double), typeof(VirtualizingPanel), new PropertyMetadata(10.0, OnStackSpacingChanged));

        private static void OnStackSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((Double)e.NewValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(StackSpacing), "VirtualizingPanel 的 StackSpacing 最小值为0.");
            }
            (d as VirtualizingPanel).RequestArrange();
        }

        ///<summary>
        ///设定<see cref="HLib.Controls.VirtualizingPanel"/>的子元素的间距.
        ///</summary>
        public Double ItemsSpacing
        {
            get { return (Double)GetValue(ItemsSpacingProperty); }
            set { SetValue(ItemsSpacingProperty, value); }
        }

        public static readonly DependencyProperty ItemsSpacingProperty =
                DependencyProperty.Register(nameof(ItemsSpacing), typeof(Double), typeof(VirtualizingPanel), new PropertyMetadata(10.0, OnItemsSpacingChanged));

        private static void OnItemsSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((Double)e.NewValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ItemsSpacing), "VirtualizingPanel 的 ItemsSpacing 最小值为0.");
            }
            (d as VirtualizingPanel).RequestArrange();
        }

        ///<summary>
        ///设定<see cref="HLib.Controls.VirtualizingPanel"/>的布局方向.
        ///</summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
                DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingPanel), new PropertyMetadata(Windows.UI.Xaml.Controls.Orientation.Vertical, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VirtualizingPanel).RequestArrange();
        }

        /// <summary>
        /// 请求重新布局
        /// </summary>
        private void RequestArrange()
        {
            InvalidateMeasure(); //无效化测量
            InvalidateArrange(); //无效化布局
        }

        public static Rect GetArrange(DependencyObject obj)
        {
            return (Rect)obj.GetValue(ArrangeProperty);
        }

        public static void SetArrange(DependencyObject obj, Rect value)
        {
            obj.SetValue(ArrangeProperty, value);
        }

        public static readonly DependencyProperty ArrangeProperty =
            DependencyProperty.RegisterAttached("Arrange", typeof(Rect), typeof(ContentControl), new PropertyMetadata(Rect.Empty));

        private List<StackLayoutContext> layoutContext = new List<StackLayoutContext>();

        private int layoutRequest = -1;

        /// <summary>
        /// 布局初始化,初始化所有子项的布局
        /// </summary>
        private void LayoutInitialize(Size availableSize)
        {
            //清除布局上下文数据
            layoutContext.Clear();

            //计算子项固定边大小
            double offset = ((Orientation == Orientation.Vertical ? availableSize.Width : availableSize.Height) - (StackCount * StackSpacing)) / StackCount;
            //为每个栈生成布局上下文数据
            for (int i = 0; i < StackCount; i++)
            {
                layoutContext.Add(new StackLayoutContext()
                {
                    //每个栈的子项的固定偏移,纵向排版就为 X 坐标,横向就为 Y 坐标
                    Offset = (offset + StackSpacing) * i,
                    Width = offset
                });
            }

            //遍历子项为每个子项进行测量
            foreach (var item in this.Children)
            {
                //寻找长度最短的栈
                StackLayoutContext minStack = layoutContext.First(c => c.Lenght == layoutContext.Min(s => s.Lenght));

                //根据不同的布局方向进行布局
                if (Orientation == Orientation.Vertical)
                {
                    //栈的固定长度,纵向排版为宽,横向为高
                    availableSize.Width = offset;
                    //子项的坐标,栈的目前长度与栈的固定偏移
                    Point coordinate = new Point(minStack.Offset, minStack.Lenght);
                    //测量子项所需尺寸
                    item.Measure(availableSize);
                    //将测量结果保存在附加属性 Arrange 中,以便布局时使用
                    SetArrange(item, new Rect(coordinate, item.DesiredSize));
                    //将最短栈加上当前子项的长度与子项间距
                    minStack.Lenght += item.DesiredSize.Height + ItemsSpacing;
                }
                else
                {
                    availableSize.Height = offset;
                    Point coordinate = new Point(minStack.Lenght, minStack.Offset);
                    item.Measure(availableSize);
                    SetArrange(item, new Rect(coordinate, item.DesiredSize));
                    minStack.Lenght += item.DesiredSize.Width + ItemsSpacing;
                }

                //判断子项是否实现 IVirtualizingItem 接口
                if (item is Primitives.IVirtualizingItem)
                {
                    //判断子项是否应该被虚拟化
                    if (ShouldBeVirtualized(item))
                    {
                        (item as Primitives.IVirtualizingItem).Virtualize();
                    }
                    else
                    {
                        if (minStack.FirstRealizedItemIndex == -1)
                        {
                            minStack.FirstRealizedItemIndex = minStack.Elements.Count;
                        }
                        minStack.RealizedItemCount++;
                        (item as Primitives.IVirtualizingItem).Realize();
                    }
                }
                //添加子项到最短栈的结尾
                minStack.Elements.Add(item);
            }

            layoutRequest = 0;
        }

        /// <summary>
        /// 添加或移除一个元素布局
        /// </summary>
        private void LayoutItem(UIElement item)
        {
            //开始布局
            StackLayoutContext minStack = layoutContext.First(c => c.Lenght == layoutContext.Min(s => s.Lenght));

            if (Orientation == Orientation.Vertical)
            {
                Size availableSize = new Size(minStack.Width, double.PositiveInfinity);
                Point coordinate = new Point(minStack.Offset, minStack.Lenght);
                item.Measure(availableSize);
                SetArrange(item, new Rect(coordinate, item.DesiredSize));
                minStack.Lenght += item.DesiredSize.Height + ItemsSpacing;
            }
            else
            {
                Size availableSize = new Size(double.PositiveInfinity, minStack.Width);
                Point coordinate = new Point(minStack.Lenght, minStack.Offset);
                item.Measure(availableSize);
                SetArrange(item, new Rect(coordinate, item.DesiredSize));
                minStack.Lenght += item.DesiredSize.Width + ItemsSpacing;
            }

            if (item is Primitives.IVirtualizingItem)
            {
                if (ShouldBeVirtualized(item))
                {
                    (item as Primitives.IVirtualizingItem).Virtualize();
                }
                else
                {
                    if (minStack.FirstRealizedItemIndex == -1)
                    {
                        minStack.FirstRealizedItemIndex = minStack.Elements.Count;
                    }
                    minStack.RealizedItemCount++;
                    (item as Primitives.IVirtualizingItem).Realize();
                }
            }
            minStack.Elements.Add(item);
            //完成一个子项布局,布局请求递减
            layoutRequest--;
        }

        private bool isVirtualizing = false;
        private ScrollViewerView scrollViewerFinalView;

        /// <summary>
        /// 开始虚拟化子项
        /// </summary>
        private void BeginItemsVirtualizing(ScrollViewerView view, ScrollViewerView final)
        {
            Rect viewport = new Rect(view.HorizontalOffset - ScrollOwner.ViewportWidth * 0.5, view.VerticalOffset - ScrollOwner.ViewportHeight * 0.5, ScrollOwner.ViewportWidth * 2, ScrollOwner.ViewportHeight * 2);

            foreach (var context in layoutContext)
            {
                bool IsBeginRealize = false;

                foreach (var item in context.Elements)
                {
                    if(item is Primitives.IVirtualizingItem)
                    {
                        if (ShouldBeVirtualized(item, viewport))
                        {
                            (item as Primitives.IVirtualizingItem).Virtualize();
                            if(IsBeginRealize)
                            {
                                break;
                            }
                        }
                        else
                        {
                            IsBeginRealize = true;
                            (item as Primitives.IVirtualizingItem).Realize();
                        }
                    }
                }
            }


            ////保存滚动终点
            //scrollViewerFinalView = final;

            ////已经在进行虚拟化时不进行虚拟化
            //if (!isVirtualizing)
            //{
            //    isVirtualizing = true;

            //    //判断布局方向
            //    if (Orientation == Orientation.Vertical)
            //    {
            //        //获取到滚动终点视图方块
            //        Rect viewRect = new Rect(0, view.VerticalOffset, scrollOwner.ViewportWidth, scrollOwner.ViewportHeight * 1.5 + final.VerticalOffset);

            //        //判断滚动方向
            //        if (view.VerticalOffset < final.VerticalOffset)
            //        {
            //            //滚动向下进行时

            //            //对每个栈布局进行调整
            //            foreach (var context in layoutContext)
            //            {
            //                //获取第一个未实例化的子项
            //                int index = context.FirstRealizedItemIndex + context.RealizedItemCount;
            //                for (int i = index; i < context.Elements.Count; i++)
            //                {
            //                    //可进行实例化
            //                    if (context.Elements[i] is Primitives.IVirtualizingItem)
            //                    {
            //                        //判断是否应该进行实例化
            //                        if (!ShouldBeVirtualized(context.Elements[i]))
            //                        {
            //                            //实例化
            //                            if ((context.Elements[i] as Primitives.IVirtualizingItem).CanRealize)
            //                            {
            //                                (context.Elements[i] as Primitives.IVirtualizingItem).Realize();
            //                                //实例化子项个数递增
            //                                context.RealizedItemCount++;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            //结束当前栈的实例化
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            //滚动向上进行时

            //            //对每个栈布局进行调整
            //            foreach (var context in layoutContext)
            //            {
            //                //获取第一个未实例化的子项
            //                int index = context.FirstRealizedItemIndex;

            //                for (int i = index; i > -1; i--)
            //                {
            //                    //可进行实例化
            //                    if (context.Elements[i] is Primitives.IVirtualizingItem)
            //                    {
            //                        //判断是否应该进行实例化
            //                        if (!ShouldBeVirtualized(context.Elements[i]))
            //                        {
            //                            //实例化
            //                            if ((context.Elements[i] as Primitives.IVirtualizingItem).CanRealize)
            //                            {
            //                                (context.Elements[i] as Primitives.IVirtualizingItem).Realize();
            //                                context.FirstRealizedItemIndex--;
            //                                context.RealizedItemCount++;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            //结束当前栈的实例化
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Rect viewRect = new Rect(view.HorizontalOffset, 0, scrollOwner.ViewportWidth * 1.5 + final.HorizontalOffset, scrollOwner.ViewportHeight);

            //        if (view.HorizontalOffset < final.HorizontalOffset)
            //        {
            //            foreach (var context in layoutContext)
            //            {
            //                int index = context.FirstRealizedItemIndex + context.RealizedItemCount;
            //                for (int i = index; i < context.Elements.Count; i++)
            //                {
            //                    if (context.Elements[i] is Primitives.IVirtualizingItem)
            //                    {
            //                        if (!ShouldBeVirtualized(context.Elements[i]))
            //                        {
            //                            (context.Elements[i] as Primitives.IVirtualizingItem).Realize();
            //                            context.RealizedItemCount++;
            //                        }
            //                        else
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            foreach (var context in layoutContext)
            //            {
            //                int index = context.FirstRealizedItemIndex;

            //                for (int i = index; i > -1; i--)
            //                {
            //                    if (context.Elements[i] is Primitives.IVirtualizingItem)
            //                    {
            //                        if (!ShouldBeVirtualized(context.Elements[i]))
            //                        {
            //                            (context.Elements[i] as Primitives.IVirtualizingItem).Realize();
            //                            context.FirstRealizedItemIndex--;
            //                            context.RealizedItemCount++;
            //                        }
            //                        else
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //  }
        }

        /// <summary>
        /// 结束虚拟化子项
        /// </summary>
        private void EndItemsVirtualizing()
        {
            //虚拟化中
            if (isVirtualizing)
            {
                //遍历所有实例化的对象,对不需要的对象进行虚拟化
                foreach (var context in layoutContext)
                {
                    for (int i = context.FirstRealizedItemIndex; i < context.RealizedItemCount; i++)
                    {
                        if (context.Elements[i] is Primitives.IVirtualizingItem)
                        {
                            if (ShouldBeVirtualized(context.Elements[i], scrollViewerFinalView))
                            {
                                (context.Elements[i] as Primitives.IVirtualizingItem).Virtualize();
                            }
                        }
                    }
                }

                isVirtualizing = false;
            }
        }

        /// <summary>
        /// 判断一个元素是否应该被虚拟化
        /// </summary>
        /// <param name="item">需要测量的元素</param>
        private bool ShouldBeVirtualized(UIElement item)
        {
            //获取元素的布局方块
            Rect arrange = GetArrange(item);
            //获取可视区域方块(上下左右均包含半个屏幕的缓冲区)
            Rect viewport = new Rect(ScrollOwner.HorizontalOffset - ScrollOwner.ViewportWidth * 0.5, ScrollOwner.VerticalOffset - ScrollOwner.ViewportHeight * 0.5, ScrollOwner.ViewportWidth * 2, ScrollOwner.ViewportHeight * 2);
            //检查可视区域方块和元素布局方块是否有重叠
            viewport.Intersect(arrange);
            return viewport == Rect.Empty; //无重叠代表不在可视区域,需要进行虚拟化
        }

        /// <summary>
        /// 判断一个元素在指定的 ScrollViewerView 是否应该被虚拟化
        /// </summary>
        /// <param name="item">需要测量的元素</param>
        /// <param name="view">指定的视图</param>
        /// <returns></returns>
        private bool ShouldBeVirtualized(UIElement item, ScrollViewerView view)
        {
            Rect arrange = GetArrange(item);
            Rect viewport = new Rect(view.HorizontalOffset - ScrollOwner.ViewportWidth * 0.5, view.VerticalOffset - ScrollOwner.ViewportHeight * 0.5, ScrollOwner.ViewportWidth * 2, ScrollOwner.ViewportHeight * 2);
            viewport.Intersect(arrange);
            return viewport == Rect.Empty;
        }

        /// <summary>
        /// 判断一个元素在指定的 Rect 是否应该被虚拟化
        /// </summary>
        /// <param name="item">需要测量的元素</param>
        /// <param name="viewRect">指定的视图</param>
        /// <returns></returns>
        private bool ShouldBeVirtualized(UIElement item, Rect viewRect)
        {
            Rect arrange = GetArrange(item);
            viewRect.Intersect(arrange);
            return viewRect == Rect.Empty;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //Hook子项生成器
            if (!isGeneratorHooked)
            {
                GetOwnerAndGenerator();
            }

            //有未完成的布局请求,一般出现在非队列最后插入子项,移除子项等操作,此时进行重新布局
            if (layoutRequest != 0)
            {
                LayoutInitialize(availableSize);
            }

            //返回控件大小
            if (Orientation == Orientation.Vertical)
            {
                return new Size(availableSize.Width, layoutContext.Max(s => s.Lenght));
            }
            else
            {
                return new Size(layoutContext.Max(s => s.Lenght), availableSize.Height);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var item in Children)
            {
                item.Arrange(GetArrange(item));
            }
            return finalSize;
        }

        /*
        protected override Size MeasureOverride(Size availableSize)
        {
            Size measure = base.MeasureOverride(availableSize);
            double itemFixed = 0;
            Size requestSize = Size.Empty;

            //判断面板的布局类型,是横向布局还是纵向布局
            if (Orientation == Orientation.Vertical)
            {
                //纵向布局
                //创建一个列表记录所有 Stack 的长度
                List<Double> offsetY = new Double[StackCount].ToList();

                //计算一个 Item 的固定边长度,纵向布局的话是宽固定
                itemFixed = (availableSize.Width - StackSpacing * (StackCount - 1)) / StackCount;
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
                    //将这个 Stack 的长度加上新的 Item 的长度和 Item的间隙
                    offsetY[minIndex] += itemRequestSize.Height + ItemsSpacing;
                }

                //寻找最长的 Stack,这个 Stack 就是面板需要的高度
                requestSize.Height = offsetY.Max();
            }
            else
            {
                //横向布局,内容大同小异,区别就是把长变成了宽
                List<Double> offsetX = new Double[StackCount].ToList();
                //Item 的固定边为长
                itemFixed = (availableSize.Height - StackSpacing * (StackCount - 1)) / StackCount;

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
                for (int i = 0; i < StackCount; i++)
                {
                    //这里的GetOffsetX是计算每个栈的横坐标,计算过程是这样的:
                    //(int index) => index * (this.DesiredSize.Width + StackSpacing) / StackCount
                    offsetX.Add(i * (this.DesiredSize.Width + StackSpacing) / StackCount);
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
                for (int i = 0; i < StackCount; i++)
                {
                    offsetX.Add(0);
                    //这里的 GetOffsetY 的计算过程如下:
                    //(int index) => index * (this.DesiredSize.Height + StackSpacing) / StackCount
                    offsetY.Add(i * (this.DesiredSize.Height + StackSpacing) / StackCount);
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
        */

        private void ScrollOwnerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            BeginItemsVirtualizing(e.NextView, e.FinalView);
        }

        private void ScrollOwnerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //if(ScrollOwner.HorizontalOffset == scrollViewerFinalView.HorizontalOffset && ScrollOwner.VerticalOffset == scrollViewerFinalView.VerticalOffset)
            //{
            //    EndItemsVirtualizing();
            //}
        }

        private void ItemContainerGeneratorItemsChanged(object sender, Windows.UI.Xaml.Controls.Primitives.ItemsChangedEventArgs e)
        {
            int index = -1;

            //判断 Items 的变更动作
            switch (e.Action)
            {
                //添加了项目
                case 1:
                    //将变更的项目加入布局请求
                    layoutRequest += e.ItemCount;

                    //判断是否是在最后加入了 Item
                    if ((index = ItemContainerGenerator.IndexFromGeneratorPosition(e.Position)) == ItemsOwner.Items.Count - 1)
                    {
                        //为加入的 Item 请求布局
                        LayoutItem(Children[index]);
                    }
                    break;

                //移除了项目
                case 2:
                    layoutRequest += e.ItemCount;
                    break;

                //未知操作
                default:
                    layoutRequest += e.ItemCount;
                    break;
            }
        }

        private ItemsControl itemsOwner; //子项的父控件
        private ScrollViewer scrollOwner; //面板所处的 ScrollView
        private ItemContainerGenerator itemContainerGenerator; //子项容器生成器
        private bool isGeneratorHooked;

        private void GetOwnerAndGenerator()
        {
            itemsOwner = ItemsControl.GetItemsOwner(this);
            if (itemsOwner == null)
            {
                throw new InvalidOperationException("无法从 VirtualizingPanel 找到父 ItemsControl.");
            }

            if (!(itemsOwner is ListViewBase))
            {
                throw new InvalidOperationException("VirtualizingPanel 应该在 ListView.ItemsPanel 中被调用");
            }

            scrollOwner = Helper.VisualThreeHelper.FindVisualElement<ScrollViewer>(itemsOwner);
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
                scrollOwner.ViewChanging += ScrollOwnerViewChanging;
                scrollOwner.ViewChanged += ScrollOwnerViewChanged;
                itemContainerGenerator.ItemsChanged += ItemContainerGeneratorItemsChanged;
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
    }
}