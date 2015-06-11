using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HLib.Extension
{
    public static class StreamExtension
    {
        /// <summary>
        /// 返回一个值,表示该流是否到了末尾
        /// </summary>
        public static bool IsEnd(this Stream stream)
        {
            return stream.Position == (stream.Length - 1);
        }

        /// <summary>
        /// 从流中读取一个 UTF-8 编码字符串,遇到 0x00 时停止,并将流内位置向前推进相应的数量,如果已达到流未则返回 null
        /// </summary>
        public static String ReadString(this Stream stream)
        {
            return stream.ReadString(new byte[] { 0 }, Encoding.UTF8);
        }

        /// <summary>
        /// 从流中读取一个 UTF-8 编码字符串,并将流内位置向前推进相应的数量,遇到相应的字节序列时停止,最后转换结果不包含该序列,如果已达到流未则返回 null
        /// </summary>
        /// <param name="split">检测的字节序列</param>
        public static String ReadString(this Stream stream, byte[] split)
        {
            return stream.ReadString(split, Encoding.UTF8);
        }

        /// <summary>
        /// 从流中读取一个指定编码的字符串,并将流内位置向前推进相应的数量,遇到相应的字节序列时停止,最后转换结果不包含该序列,如果已达到流未则返回 null
        /// </summary>
        /// <param name="split">检测的字节序列</param>
        /// <param name="encoding">指定的字符编码</param>
        public static String ReadString(this Stream stream, byte[] split, Encoding encoding)
        {
            if(stream.IsEnd())
            {
                return null;
            }

            long pos = stream.Position;
            int count = 0;
            int parseIndex = 0;
            byte b = (Byte)stream.ReadByte();
            bool flag = true;
            
            while(flag)
            {
                if(b == split[parseIndex])
                {
                    parseIndex++;
                    if(parseIndex == split.Length)
                    {
                        break;
                    }
                }
                else
                {
                    count += parseIndex + 1;
                    parseIndex = 0;
                }

                if (stream.IsEnd())
                {
                    count += parseIndex;
                    break;
                }

                b = (Byte)stream.ReadByte();
            }

            stream.Position = pos;
            return stream.ReadString(count, encoding);
        }

        /// <summary>
        /// 从流中读取一个 UTF-8 编码字符串,并将流内位置向前推进相应的数量
        /// </summary>
        /// <param name="count">需要读取的字节数</param>
        public static String ReadString(this Stream stream, int count)
        {
            return stream.ReadString(count, Encoding.UTF8);
        }

        /// <summary>
        /// 从流中读取一个指定编码的字符串,并将流内位置向前推进相应的数量
        /// </summary>
        /// <param name="count">需要读取的字节数</param>
        /// <param name="encoding">指定的字符编码</param>
        public static String ReadString(this Stream stream,int count,Encoding encoding)
        {
            byte[] buff = new byte[count];
            stream.Read(buff, 0, count);
            return encoding.GetString(buff, 0, count);
        }

        /// <summary>
        /// 从流中读取一个64位的整数,并将流内位置向前推进8字节
        /// </summary>
        public static long ReadInt64(this Stream stream)
        {
            byte[] buff = new byte[8];
            stream.Read(buff, 0, 8);
            return ((long)buff[0]) | (((long)buff[1]) << 8) | (((long)buff[2]) << 16) | (((long)buff[3]) << 24) | (((long)buff[4]) << 32) | (((long)buff[5]) << 40) | (((long)buff[6]) << 48) | (((long)buff[7]) << 56);
        }

        /// <summary>
        /// 从流中读取一个64位的无符号整数,并将流内位置向前推进8字节
        /// </summary>
        public static ulong ReadUInt64(this Stream stream)
        {
            byte[] buff = new byte[8];
            stream.Read(buff, 0, 8);
            return ((ulong)buff[0]) | (((ulong)buff[1]) << 8) | (((ulong)buff[2]) << 16) | (((ulong)buff[3]) << 24) | (((ulong)buff[4]) << 32) | (((ulong)buff[5]) << 40) | (((ulong)buff[6]) << 48) | (((ulong)buff[7]) << 56);
        }

        /// <summary>
        /// 从流中读取一个32位的整数,并将流内位置向前推进4字节
        /// </summary>
        public static int ReadInt32(this Stream stream)
        {
            byte[] buff = new byte[4];
            stream.Read(buff, 0, 4);
            return ((int)buff[0]) | (((int)buff[1]) << 8) | (((int)buff[2]) << 16) | (((int)buff[3]) << 24);
        }

        /// <summary>
        /// 从流中读取一个32位的无符号整数,并将流内位置向前推进4字节
        /// </summary>
        public static uint ReadUInt32(this Stream stream)
        {
            byte[] buff = new byte[4];
            stream.Read(buff, 0, 4);
            return ((uint)buff[0]) | (((uint)buff[1]) << 8) | (((uint)buff[2]) << 16) | (((uint)buff[3]) << 24);
        }

        /// <summary>
        /// 从流中读取一个24位的整数,并将流内位置向前推进3字节
        /// </summary>
        public static int ReadInt24(this Stream stream)
        {
            byte[] buff = new byte[3];
            stream.Read(buff, 0, 3);
            return ((int)buff[0]) | (((int)buff[1]) << 8) | (((int)buff[2]) << 16);
        }

        /// <summary>
        /// 从流中读取一个16位的整数,并将流内位置向前推进2字节
        /// </summary>
        public static short ReadInt16(this Stream stream)
        {
            byte[] buff = new byte[2];
            stream.Read(buff, 0, 2);
            return (short)(buff[0] | ((buff[1]) << 8));
        }

        /// <summary>
        /// 从流中读取一个16位的无符号整数,并将流内位置向前推进2字节
        /// </summary>
        public static ushort ReadUInt16(this Stream stream)
        {
            
            byte[] buff = new byte[2];
            stream.Read(buff, 0, 2);
            return (ushort)(buff[0] | ((buff[1]) << 8));
        }

        /// <summary>
        /// 从流中检查一个 UTF-8 编码字符串是否一致,并将流内位置向前推进相关字节
        /// </summary>
        /// <param name="header">需要检查的字符串</param>
        public static bool CheckHeader(this Stream stream, String header)
        {
            return stream.CheckHeader(header, Encoding.UTF8);
        }

        /// <summary>
        /// 从流中检查一个指定编码字符串是否一致,并将流内位置向前推进相关字节
        /// </summary>
        /// <param name="header">需要检查的字符串</param>
        /// <param name="encoding">字符串的编码</param>
        public static bool CheckHeader(this Stream stream, String header, Encoding encoding)
        {
            long pos = stream.Position;
            var buff = encoding.GetBytes(header);
            int b = stream.ReadByte();

            for (int i = 0; i < buff.Length; i++)
            {
                if(b != buff[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
