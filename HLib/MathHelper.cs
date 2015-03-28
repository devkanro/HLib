
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace HLib
{
    public static class MathHelper
    {
        /// <summary>
        /// 将 Size 根据新的宽度按照原样比例缩放
        /// </summary>
        /// <param name="width">原宽</param>
        /// <param name="height">原高</param>
        /// <param name="newWidth">新宽</param>
        /// <returns>缩放后的 Size</returns>
        public static Size ResizeWithWidth(double width,double height,double newWidth)
        {
            return ResizeWithWidth(new Size(width, height), newWidth);
        }

        /// <summary>
        /// 将 Size 根据新的宽度按照原样比例缩放
        /// </summary>
        /// <param name="size">原 Size</param>
        /// <param name="newWidth">新宽</param>
        /// <returns>缩放后的 Size</returns>
        public static Size ResizeWithWidth(Size size, double newWidth)
        {
            double scale = size.Width / size.Height;
            return new Size(newWidth, newWidth / scale);
        }

        /// <summary>
        /// 将 Size 根据新的高度按照原样比例缩放
        /// </summary>
        /// <param name="width">原宽</param>
        /// <param name="height">原高</param>
        /// <param name="newWidth">新高</param>
        /// <returns>缩放后的 Size</returns>
        public static Size ResizeWithHeight(double width, double height, double newHeight)
        {
            return ResizeWithHeight(new Size(width, height), newHeight);
        }

        /// <summary>
        /// 将 Size 根据新的高度按照原样比例缩放
        /// </summary>
        /// <param name="size">原 Size</param>
        /// <param name="newWidth">新高</param>
        /// <returns>缩放后的 Size</returns>
        public static Size ResizeWithHeight(Size size, double newHeight)
        {
            double scale = size.Width / size.Height;
            return new Size(newHeight * scale, newHeight);
        }
        
    }
}
