using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.UI;

namespace HLib
{
    /// <summary>
    /// 提供简单的 String 转换方法封装
    /// </summary>
    public static class SringConverter
    {
        /// <summary>
        /// 将 Byte 数组数据转换为 HexString
        /// </summary>
        /// <param name="data">需要转换的 Byte 数组</param>
        /// <returns>转换完成的 HexString </returns>
        public static String EncodeToHexString(Byte[] data)
        {
            var buff = CryptographicBuffer.CreateFromByteArray(data);
            return CryptographicBuffer.EncodeToHexString(buff);
        }

        /// <summary>
        /// 将 HexString 转换为 Byte 数组数据
        /// </summary>
        /// <param name="hexString">需要转换的 HexString </param>
        /// <returns>转换完成的 Byte 数组</returns>
        public static Byte[] DecodeFromHexString(String hexString)
        {
            var buff = CryptographicBuffer.DecodeFromHexString(hexString);
            Byte[] data = new Byte[buff.Length];
            CryptographicBuffer.CopyToByteArray(buff, out data);
            return data;
        }

        /// <summary>
        /// 将 HtmlColor 代码转换为 <see cref="Windows.UI.Color"/>
        /// </summary>
        /// <param name="colorCode">提供的 HtmlColor 代码</param>
        /// <returns>转换的 <see cref="Windows.UI.Color"/> </returns>
        public static Color HtmlColorToColor(String colorCode)
        {
            colorCode = colorCode.ToUpper();
            String colorString = String.Empty;
            var match = Regex.Match("colorCode", @"#([0-9A-F]+)");
            if(match.Success)
            {
                var code = match.Groups[1].Value;
                switch (code.Length)
                {
                    case 3:
                        colorString = String.Format("FF{0}{0}{1}{1}{2}{2}", code[0], code[1], code[2]);
                        break;
                    case 4:
                        colorString = String.Format("{0}{0}{1}{1}{2}{2}{3}{3}", code[0], code[1], code[2], code[3]);
                        break;
                    case 6:
                        colorString = String.Concat("FF", code);
                        break;
                    case 8:
                        colorString = code;
                        break;
                    default:
                        break;
                }
            }

            if(colorString != String.Empty)
            {
                var colorData = DecodeFromHexString(colorString);
                return Color.FromArgb(colorData[0], colorData[1], colorData[2], colorData[3]);
            }
            else
            {
                throw new ArgumentException(String.Concat("ColorCode不符合规范,应该以#开头,加上3,4,6或8位的16进制颜色值(ARGB顺序).\r\n#123(#FF112233),#1234(11223344),#123456(#FF123456),#12345678(#12345678),都是合法的颜色代码格式.\r\n", colorCode));
            }
        }

        /// <summary>
        /// 将 <see cref="Windows.UI.Color"/> 转换为 HtmlColor 代码
        /// </summary>
        /// <param name="color">提供的 <see cref="Windows.UI.Color"/> </param>
        /// <returns>转换的 HtmlColor 代码</returns>
        public static String ColorToHtmlColor(Color color)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }
    }
}