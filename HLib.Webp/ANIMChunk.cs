using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using HLib.Extension;

namespace HLib.Webp
{
    public class ANIMChunk : IWebpChunk
    {
        private ANIMChunk()
        {

        }

        public string Header { get; private set; } = "ANIM";

        public int Length { get; private set; }

        public uint BackgroundColor { get; private set; }
        
        public ushort LoopCount { get; private set; }

        public static ANIMChunk ReadFromStream(Stream stream)
        {
            ANIMChunk anim = new ANIMChunk();
            stream.Seek(-4, SeekOrigin.Current);

            if (stream.CheckHeader(anim.Header))
            {
                anim.Length = stream.ReadInt32();
                if(anim.Length != 0x6)
                {
                    throw new Exception($"ANIM块长度错误,不应该为{anim.Length:D2}.");
                }
                anim.BackgroundColor = stream.ReadUInt32();
                anim.LoopCount = stream.ReadUInt16();
                return anim;
            }
            else
            {
                throw new Exception("ANIM头匹配失败.");
            }
        }
    }
}
