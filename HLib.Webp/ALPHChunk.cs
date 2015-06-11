using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using HLib.Extension;

namespace HLib.Webp
{
    public class ALPHChunk : IWebpChunk
    {
        private ALPHChunk()
        {

        }

        public string Header { get; private set; } = "ALPH";

        public int Length { get; private set; }

        public AlphaChunkFlag Flag { get; private set; }

        public static ALPHChunk ReadFromStream(Stream stream)
        {
            ALPHChunk alph = new ALPHChunk();
            stream.Seek(-4, SeekOrigin.Current);

            if (stream.CheckHeader(alph.Header))
            {
                alph.Length = stream.ReadInt32();
                alph.Flag = (AlphaChunkFlag)stream.ReadByte();

                return alph;
            }
            else
            {
                throw new Exception("ALPH头匹配失败.");
            }
        }
    }

    [Flags]
    public enum AlphaChunkFlag
    {
        NoCompression = 0x0,
        Compressed = 0x1,

        NoFilter = 0x0,
        HorizontalFilter = 0x4,
        VerticalFilter = 0x8,
        GradientFilter = 0xC,

        NoPreprocessing = 0x0,
        LevelReduction = 0x10,
    }
}
