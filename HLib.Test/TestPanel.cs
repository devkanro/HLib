using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HLib.Test
{
    public class TestPanel : StackPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            if(!isGeneratorHooked)
            {
                GetOwnerAndGenerator();
            }
            return base.MeasureOverride(availableSize);
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

        private void ScrollOwnerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {

        }

        private void ItemContainerGeneratorItemsChanged(object sender, Windows.UI.Xaml.Controls.Primitives.ItemsChangedEventArgs e)
        {
            int count = ItemsOwner.Items.Count;
            switch (e.Action)
            {
                case 1: //Add
                    break;
                case 2: //Remove
                    break;
                default:
                    break;
            }
        }
    }
}
