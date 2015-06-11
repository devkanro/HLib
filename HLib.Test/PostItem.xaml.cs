using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using HLib.Helper;
using Windows.UI.Xaml.Media.Imaging;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace HLib.Test
{
    public sealed partial class PostItem : UserControl
    {
        public PostItem(BasePost post)
        {
            Post = post;
            this.DataContext = this;
            this.InitializeComponent();
        }

        public BasePost Post
        {
            get { return (BasePost)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        public static readonly DependencyProperty PostProperty =
            DependencyProperty.Register("Post", typeof(BasePost), typeof(PostItem), null);
        

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Double.IsInfinity(availableSize.Width))
            {
                if (Double.IsInfinity(availableSize.Height))
                {
                    
                    return new Size(Post.PreviewImageWidth, Post.PreviewImageHeight);
                }
                else
                {
                    return Helper.SizeHelper.ResizeWithHeight(Post.PreviewImageWidth, Post.PreviewImageHeight, availableSize.Height);
                }
            }
            else
            {
                if (Double.IsInfinity(availableSize.Height))
                {
                    return Helper.SizeHelper.ResizeWithWidth(Post.PreviewImageWidth, Post.PreviewImageHeight, availableSize.Width);
                }
                else
                {
                    double scale = availableSize.Width / availableSize.Height;
                    if (Post.PreviewImageHeight * scale > Post.PreviewImageWidth)
                    {
                        return Helper.SizeHelper.ResizeWithHeight(Post.PreviewImageWidth, Post.PreviewImageHeight, availableSize.Height);
                    }
                    else
                    {
                        return Helper.SizeHelper.ResizeWithWidth(Post.PreviewImageWidth, Post.PreviewImageHeight, availableSize.Width);
                    }
                }
            }
        }
    }
}
