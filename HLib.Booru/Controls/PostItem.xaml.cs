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
using HLib;
using HLib.Booru.Controls.ViewModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace HLib.Booru.Controls
{
    public sealed partial class PostItem : UserControl
    {
        public PostItem()
        {
            
            this.DataContext = new PostViewModel();
            this.InitializeComponent();
        }
        

protected override Size MeasureOverride(Size availableSize)
{
    if(Double.IsInfinity(availableSize.Width))
    {
        if(Double.IsInfinity(availableSize.Height))
        {
            return new Size((this.DataContext as PostViewModel).Post.PreviewImageWidth, (this.DataContext as PostViewModel).Post.PreviewImageHeight);
        }
        else
        {
            return MathHelper.ResizeWithHeight((this.DataContext as PostViewModel).Post.PreviewImageWidth, (this.DataContext as PostViewModel).Post.PreviewImageHeight, availableSize.Height);
        }
    }
    else
    {
        if (Double.IsInfinity(availableSize.Height))
        {
            return MathHelper.ResizeWithWidth((this.DataContext as PostViewModel).Post.PreviewImageWidth, (this.DataContext as PostViewModel).Post.PreviewImageHeight, availableSize.Width);
        }
        else
        {
            double scale = availableSize.Width / availableSize.Height;
            if((DataContext as PostViewModel).Post.PreviewImageHeight * scale > (DataContext as PostViewModel).Post.PreviewImageWidth)
            {
                return MathHelper.ResizeWithHeight((this.DataContext as PostViewModel).Post.PreviewImageWidth, (this.DataContext as PostViewModel).Post.PreviewImageHeight, availableSize.Height);
            }
            else
            {
                return MathHelper.ResizeWithWidth((this.DataContext as PostViewModel).Post.PreviewImageWidth, (this.DataContext as PostViewModel).Post.PreviewImageHeight, availableSize.Width);
            }
        }
    }
}



    }
}
