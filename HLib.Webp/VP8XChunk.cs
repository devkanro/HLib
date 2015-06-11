using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using HLib.Extension;

namespace HLib.Webp
{
    public class VP8XChunk : IWebpChunk
    {
        private VP8XChunk()
        {

        }

        public string Header { get; private set; } = "VP8X";

        public int Length { get; private set; }

        public VP8XChunkFlag Flag { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }
        
        public static VP8XChunk ReadFromStream(Stream stream)
        {
            VP8XChunk vp8x = new VP8XChunk();
            stream.Seek(-4, SeekOrigin.Current);

            if(stream.CheckHeader(vp8x.Header))
            {
                vp8x.Length = stream.ReadInt32();
                if(vp8x.Length != 0x0A)
                {
                    throw new Exception($"VP8X块长度错误,不应该为{vp8x.Length:D2}.");
                }
                vp8x.Flag = (VP8XChunkFlag)stream.ReadInt32();
                vp8x.Width = stream.ReadInt24();
                vp8x.Height = stream.ReadInt24();
                return vp8x;
            }
            else
            {
                throw new Exception("VP8X头匹配失败.");
            }
        }
    }

    [Flags]
    public enum VP8XChunkFlag
    {
        ICC = 0x20,
        Alpha = 0x10,
        EXIF = 0x8,
        XMP = 0x4,
        Animation = 0x2,
        Fragmentation = 0x1
    }
}
