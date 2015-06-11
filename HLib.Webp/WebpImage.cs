using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using HLib.Extension;
using Windows.UI.Xaml.Media.Imaging;

namespace HLib.Webp
{
    public class WebpImage
    {
        public WebpImage(Stream stream)
        {
            stream.Position = 0;
            if (!stream.CheckHeader("RIFF"))
            {
                throw new Exception("RIFF头匹配失败.");
            }

            RIFFSize = stream.ReadInt32();

            if (!stream.CheckHeader("WEBP"))
            {
                throw new Exception("WEBP头匹配失败.");
            }

            String vp8Hearder = stream.ReadString(4);
            switch (vp8Hearder)
            {
                case "VP8 ":

                    break;
                case "VP8L":

                    break;
                case "VP8X":
                    VP8X = VP8XChunk.ReadFromStream(stream);
                    break;
            }

            while (!stream.IsEnd())
            {

            }
        }

        public VP8XChunk VP8X { get; set; }

        public int RIFFSize
        {
            get;
            set;
        }
    }
}
