using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLib.Booru
        /// <summary>
        /// 返回预览图片地址
        /// </summary>
{
    /// <summary>
    /// 表示一张Booru所使用的图片,所有Booru所使用的图片都是由此类派生.
    /// </summary>
    public class BasePost
    {
        public Uri PreviewImageUrl { get; set; }
        /// <summary>
        /// 返回预览图片宽
        /// </summary>
        public int PreviewImageWidth { get; set; }
        /// <summary>
        /// 返回预览图片高
        /// </summary>
        public int PreviewImageHeight { get; set; }
        /// <summary>
        /// 返回样图地址
        /// </summary>
        public Uri SampleImageUrl { get; set; }
        /// <summary>
        /// 返回原图地址
        /// </summary>
        public Uri ImageUrl { get; set; }
        /// <summary>
        /// 返回图片ID
        /// </summary>
        public int Id { get; set; }
    }
}
