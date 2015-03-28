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
    /// 封装对UI进行虚拟化的虚拟化引擎
    /// </summary>
    public class VirtualizingEngine
    {
        /// <summary>
        /// 初始化一个虚拟化引擎
        /// </summary>
        /// <param name="itemContainerGenerator"></param>
        public VirtualizingEngine(VirtualizingPanel virtualizingPanel)
        {
            panel = virtualizingPanel;
            itemContainerGenerator = virtualizingPanel.ItemContainerGenerator;
        }

        /// <summary>
        /// 虚拟化引擎所在的 <see cref="VirtualizingPanel"/>
        /// </summary>
        public VirtualizingPanel Panel
        {
            get
            {
                return panel;
            }
        }
        private VirtualizingPanel panel;

        /// <summary>
        /// 虚拟化引擎所必须的 ItemContainerGenerator
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                return itemContainerGenerator;
            }
        }
        private ItemContainerGenerator itemContainerGenerator;

        /// <summary>
        /// 获取指定的Item在链表上的上一个Item
        /// </summary>
        /// <param name="obj">指定的Item</param>
        /// <returns>上一个Item</returns>
        public static UIElement GetPreviousItem(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(PreviousItemProperty);
        }

        /// <summary>
        /// 设置指定的Item在链表上的上一个Item
        /// </summary>
        /// <param name="obj">指定的Item</param>
        /// <param name="value">上一个Item</param>
        public static void SetPreviousItem(DependencyObject obj, UIElement value)
        {
            obj.SetValue(PreviousItemProperty, value);
            if(value != null)
            {
                value.SetValue(NextItemProperty, obj);
            }
        }
        
        public static readonly DependencyProperty PreviousItemProperty =
            DependencyProperty.RegisterAttached("PreviousItem", typeof(UIElement), typeof(UIElement), null);
        

        /// <summary>
        /// 获取指定的Item在链表上的下一个Item
        /// </summary>
        /// <param name="obj">指定的Item</param>
        /// <returns>下一个Item</returns>
        public static UIElement GetNextItem(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(NextItemProperty);
        }

        /// <summary>
        /// 设置指定的Item在链表上的下一个Item
        /// </summary>
        /// <param name="obj">指定的Item</param>
        /// <param name="value">下一个Item</param>
        public static void SetNextItem(DependencyObject obj, UIElement value)
        {
            obj.SetValue(NextItemProperty, value);
            if(value != null)
            {
                value.SetValue(PreviousItemProperty, obj);
            }
        }
        
        public static readonly DependencyProperty NextItemProperty =
            DependencyProperty.RegisterAttached("NextItem", typeof(UIElement), typeof(UIElement), null);


        /// <summary>
        /// 获取Item在VirtualizingPanel中的偏移
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(OffsetProperty);
        }

        /// <summary>
        /// 设置Item在VirtualizingPanel中的偏移
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetOffset(DependencyObject obj, double value)
        {
            obj.SetValue(OffsetProperty, value);
        }
        
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.RegisterAttached("Offset", typeof(double), typeof(UIElement), null);
        
        /// <summary>
        /// 获取当前虚拟化引擎的第一个Item
        /// </summary>
        public UIElement FirstItem
        {
            get
            {
                return firstItem;
            }
        }
        private UIElement firstItem;

        /// <summary>
        /// 获取当前虚拟化引擎的最后一个Item
        /// </summary>
        public UIElement LastItem
        {
            get
            {
                return lastItem;
            }
        }
        private UIElement lastItem;

        /// <summary>
        /// 获取当前虚拟化引擎的Item数
        /// </summary>
        public int ItemCount
        {
            get
            {
                UIElement item = FirstItem;
                int count = 0;

                while (item != null)
                {
                    count++;
                    item = GetNextItem(item);
                }

                return count;
            }
        }

        /// <summary>
        /// 获取当前UI呈现的第一个Item
        /// </summary>
        public UIElement FirstRealizedItem
        {
            get
            {
                return firstRealizedItem;
            }
        }
        private UIElement firstRealizedItem;

        /// <summary>
        /// 获取当前UI呈现的最后一个Item
        /// </summary>
        public UIElement LastRealizedItem
        {
            get
            {
                return lastRealizedItem;
            }
        }
        private UIElement lastRealizedItem;

        /// <summary>
        /// 获取当前UI呈现的所有Item
        /// </summary>
        public List<UIElement> RealizedItems
        {
            get
            {
                List<UIElement> list = new List<UIElement>();
                UIElement item = firstRealizedItem;

                while(item != lastRealizedItem)
                {
                    list.Add(item);
                    item = GetNextItem(item);
                }

                return list;
            }
        }

        /// <summary>
        /// 获取当前UI呈现的所有Item的数量
        /// </summary>
        public int RealizedItemsCount
        {
            get
            {
                UIElement item = firstRealizedItem;
                int count = 0;

                while (item != LastRealizedItem)
                {
                    count++;
                    item = GetNextItem(item);
                }

                return count;
            }
        }

        private List<UIElement> itemList = new List<UIElement>();

        /// <summary>
        /// 将一个Item加入虚拟化引擎Item链表
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(UIElement item)
        {
            if(!itemList.Contains(item))
            {
                double offset = NextOffset;
                itemOffsetList.Add(offset);
                itemList.Add(item);
                
                if (firstItem == null)
                {
                    firstItem = item;
                    lastItem = item;
                }
                else
                {
                    SetNextItem(lastItem, item);
                    lastItem = item;
                }
            }
            else
            {
                throw new InvalidOperationException("该 Item 已经存在,请勿重复添加.");
            }
        }

        /// <summary>
        /// 将一个Item从虚拟化引擎Item链表移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveItem(UIElement item)
        {
            if(itemList.Contains(item))
            {
                int index = itemList.IndexOf(item);

                UIElement preItem = GetPreviousItem(item);
                UIElement nextItem = GetNextItem(item);
                
                if(preItem == null && nextItem == null)
                {
                    firstItem = lastItem = null;
                }
                else if(preItem != null && nextItem == null)
                {
                    lastItem = preItem;
                    SetNextItem(preItem, null);
                }
                else if (preItem == null && nextItem != null)
                {
                    firstItem = nextItem;
                    SetPreviousItem(nextItem, null);
                }
                else
                {
                    SetNextItem(preItem, nextItem);
                }

                SetOffset(item, 0);
                itemList.RemoveAt(index);
                itemOffsetList.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将一个Item插入虚拟化引擎Item链表
        /// </summary>
        /// <param name="item">要插入的Item</param>
        /// <param name="index">插入的索引</param>
        public void InsItem(UIElement item,int index)
        {
            if (!itemList.Contains(item))
            {
                if(index == 0)
                {
                    SetPreviousItem(firstItem, item);
                    firstItem = item;
                }
                else
                {
                    UIElement preItem = itemList[index - 1];
                    UIElement nextItem = itemList[index];

                    SetNextItem(preItem, item);
                    SetPreviousItem(nextItem, item);
                }

                itemOffsetList.Insert(index, MeasureItemOffset(index));
                itemList.Insert(index, item);
                ResetOffset(index + 1);
            }
            else
            {
                throw new InvalidOperationException("该 Item 已经存在,请勿重复添加.");
            }
        }

        public int GetItemIndex(UIElement item)
        {
            return itemList.IndexOf(item);
        }

        /// <summary>
        /// 获取Item偏移列表
        /// </summary>
        public IReadOnlyList<double> ItemOffsetList
        {
            get
            {
                return itemOffsetList;
            }
        }
        private List<double> itemOffsetList = new List<double>();

        /// <summary>
        /// 获取下一Item在<see cref="VirtualizingPanel"/>中的偏移
        /// </summary>
        public double NextOffset
        {
            get
            {
                if (itemOffsetList.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return itemOffsetList.Last() + (panel.Orientation == Orientation.Vertical ? itemList.Last().DesiredSize.Height : itemList.Last().DesiredSize.Width) + panel.ItemsSpacing;
                }
            }
        }

        /// <summary>
        /// 计算与测量指定索引处的Item的偏移
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double MeasureItemOffset(int index)
        {
            if(index == 0)
            {
                return 0;
            }
            else
            {
                return itemOffsetList[index - 1] + (panel.Orientation == Orientation.Vertical ? itemList[index - 1].DesiredSize.Height : itemList[index - 1].DesiredSize.Width) + panel.ItemsSpacing;
            }
        }

        /// <summary>
        /// 重新测量从指定的索引处开始Item的偏移,这个过程也会导致对虚拟化引擎内的数据进行校验,校验失败会抛出 <see cref="DataMisalignedException"/> 异常
        /// </summary>
        /// <param name="index">要重新开始测量的Item的索引,为0则重新测量整个链表</param>
        /// <returns>被更改偏移的Item数量</returns>
        public int ResetOffset(int index = 0)
        {
            CheckCount();

            if (itemList.Count != 0)
            {
                UIElement item = itemList[index];
                double offset = MeasureItemOffset(index);
                int count = 0;

                do
                {
                    if(GetOffset(item) != offset)
                    {
                        SetOffset(item, offset);
                        itemOffsetList[index] = offset;
                        count++;
                    }

                    //校验链表数据和ItemList数据是否匹配
                    try
                    {
                        UIElement nextItem = GetNextItem(item);
                        if (nextItem != null)
                        {
                            if(nextItem != itemList[index - 1])
                            {
                                throw new DataMisalignedException("虚拟化引擎数据校验失败,ItemList与链表数据无法匹配");
                            }
                        }
                    }
                    catch (DataMisalignedException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new DataMisalignedException("虚拟化引擎数据校验失败,数据不合法.", ex);
                    }
                    
                    index++;
                    item = GetNextItem(item);
                    offset = MeasureItemOffset(index);

                } while (item != null);

                return count;
            }
            else
            {
                return 0;
            }
        }

        private void CheckCount()
        {
            if (itemList.Count != itemOffsetList.Count)
            {
                throw new DataMisalignedException("虚拟化引擎数据校验失败,ItemList与链表数据数量不一致");
            }
            else if (itemList.Count != ItemCount)
            {
                throw new DataMisalignedException("虚拟化引擎数据校验失败,ItemList与ItemOffsetList数量不一致");
            }
        }

        private void CheckData()
        {
            UIElement item = firstItem;

            for (int i = 0; i < itemList.Count; i++)
            {
                if(itemList[i] != item)
                {
                    throw new DataMisalignedException("虚拟化引擎数据校验失败,ItemList与链表数据数量不一致");
                }

                if (itemOffsetList[i] != GetOffset(item))
                {
                    throw new DataMisalignedException("虚拟化引擎数据校验失败,ItemOffsetList与Item数据不一致");
                }

                item = GetNextItem(item);

                if (i == 0)
                {
                    continue;
                }

                if(itemOffsetList[i] < itemOffsetList[i - 1])
                {
                    throw new DataMisalignedException("虚拟化引擎数据校验失败,非法的Offset");
                }
            }
        }

        
    }
}
