using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace HLib
{
    /// <summary>
    /// 对Cryptography功能进行简单封装,支持多种加密算法.
    /// </summary>
    public static class Cryptography
    {
        /// <summary>
        /// 对一个字符串进行Hash加密,可指定使用的算法.
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="algorithm">使用的算法名,可使用 <see cref="HLib.HashAlgorithm"/> 类或者 <see cref="Windows.Security.Cryptography.Core.HashAlgorithmNames"/> 类来获取</param>
        /// <returns>加密后的字符串</returns>
        public static String HashString(String str,String algorithm)
        {
            HashAlgorithmProvider provider = HashAlgorithmProvider.OpenAlgorithm(algorithm);
            var hash = provider.HashData(CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToHexString(hash);
        }

        /// <summary>
        /// 对一个字符串进行Hash加密,使用MD5算法.
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static String HashStringUseMD5(String str)
        {
            return HashString(str, HashAlgorithm.Md5);
        }

        /// <summary>
        /// 对一个字符串进行Hash加密,使用SHA1算法.
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static String HashStringUseSHA1(String str)
        {
            return HashString(str, HashAlgorithm.Sha1);
        }

        /// <summary>
        /// 对一个字符串进行Hash加密,使用SHA256算法.
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static String HashStringUseSHA256(String str)
        {
            return HashString(str, HashAlgorithm.Sha256);
        }

        /// <summary>
        /// 对一个字符串进行Hash加密,使用SHA384算法.
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static String HashStringUseSHA384(String str)
        {
            return HashString(str, HashAlgorithm.Sha384);
        }

        /// <summary>
        /// 对一个字符串进行Hash加密,使用SHA512算法.
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static String HashStringUseSHA512(String str)
        {
            return HashString(str, HashAlgorithm.Sha512);
        }
    }

    /// <summary>
    /// 返回可使用的Hash算法名称
    /// </summary>
    public static class HashAlgorithm
    {
        public static String Md5 { get { return "MD5"; } }
        public static String Sha1 { get { return "SHA1"; } }
        public static String Sha256 { get { return "SHA256"; } }
        public static String Sha384 { get { return "SHA384"; } }
        public static String Sha512 { get { return "SHA512"; } }
    }
}
