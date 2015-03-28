using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace HLib
{
    /// <summary>
    /// 提供对HTTP请求的简易封装,实现简单的HTTP操作
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 使用默认的方法,通过HTTP请求获取文本.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <returns>获取到的文本</returns>
        public static async Task<String> GetString(String url)
        {
            return await GetStringWithMethod(url, null, null, 0);
        }

        /// <summary>
        /// 使用默认的方法,指定Cookies,通过HTTP请求获取文本.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <param name="cookies">请求时使用的Cookies,为Null则不使用Cookies</param>
        /// <returns>获取到的文本</returns>
        public static async Task<String> GetString(String url, String cookies)
        {
            return await GetStringWithMethod(url, null, cookies, 0);
        }

        /// <summary>
        /// 使用默认的方法,通过HTTP请求获取文本,失败时进行重试.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <param name="retryCount">失败时重试的次数,为0则不重试</param>
        /// <returns>获取到的文本</returns>
        public static async Task<String> GetString(String url, int retryCount)
        {
            return await GetStringWithMethod(url, null, null, retryCount);
        }

        /// <summary>
        /// 使用默认的方法,指定Cookies,通过HTTP请求获取文本,失败时进行重试.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <param name="cookies">请求时使用的Cookies,为Null则不使用Cookies</param>
        /// <param name="retryCount">失败时重试的次数,为0则不重试</param>
        /// <returns>获取到的文本</returns>
        public static async Task<String> GetString(String url, String cookies, int retryCount)
        {
            return await GetStringWithMethod(url, null, cookies, retryCount);
        }

        /// <summary>
        /// 使用指定的方法,指定Cookies,通过HTTP请求获取文本,失败时进行重试.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <param name="method">指定的请求方法</param>
        /// <param name="cookies">请求时使用的Cookies,为Null则不使用Cookies</param>
        /// <param name="retryCount">失败时重试的次数,为0则不重试</param>
        /// <returns>获取到的文本</returns>
        public static async Task<String> GetStringWithMethod(String url, String method, String cookies, int retryCount)
        {
            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp(url);

            if(method != null)
            {
                httpRequest.Method = method;
            }

            if (cookies != null)
            {
                httpRequest.Headers["Cookie"] = cookies;
            }

        retry:

            try
            {
                using (var httpResponse = await httpRequest.GetResponseAsync())
                {
                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        return await streamReader.ReadToEndAsync();
                    }
                }
            }
            catch (WebException)
            {
                if (retryCount > 0)
                {
                    retryCount--;
                    goto retry;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 使用默认的方法,通过HTTP请求获得数据.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <returns>获取到的字节流</returns>
        public static async Task<Byte[]> GetData(String url)
        {
            return await GetData(url, null, 0);
        }

        /// <summary>
        /// 使用默认的方法,通过HTTP请求获得数据,失败时进行重试.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <param name="retryCount">失败时重试的次数,为0则不重试</param>
        /// <returns>获取到的字节流</returns>
        public static async Task<Byte[]> GetData(String url, int retryCount)
        {
            return await GetData(url, null, retryCount);
        }

        /// <summary>
        /// 使用默认的方法,指定Cookies,通过HTTP请求获得数据.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <param name="cookies">请求时使用的Cookies,为Null则不使用Cookies</param>
        /// <returns>获取到的字节流</returns>
        public static async Task<Byte[]> GetData(String url, String cookies)
        {
            return await GetData(url, cookies, 0);
        }

        /// <summary>
        /// 使用默认的方法,指定Cookies,通过HTTP请求获得数据,失败时进行重试.
        /// </summary>
        /// <param name="url">要发起请求的Url字符串</param>
        /// <param name="cookies">请求时使用的Cookies,为Null则不使用Cookies</param>
        /// <param name="retryCount">失败时重试的次数,为0则不重试</param>
        /// <returns>获取到的字节流</returns>
        public static async Task<Byte[]> GetData(String url, String cookies, int retryCount)
        {
            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp(url);

            if (cookies != null)
            {
                httpRequest.Headers["Cookie"] = cookies;
            }

        retry:

            try
            {
                using (var httpResponse = await httpRequest.GetResponseAsync())
                {
                    var stream = httpResponse.GetResponseStream();
                    Byte[] result = new Byte[stream.Length];
                    await stream.ReadAsync(result, 0, result.Length);
                    return result;
                }
            }
            catch (WebException)
            {
                if (retryCount > 0)
                {
                    retryCount--;
                    goto retry;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 从Uri中获取Query字典
        /// </summary>
        /// <param name="uri">提供的Uri</param>
        /// <returns>得到的字典</returns>
        public static Dictionary<String,String> GetUrlQuery(Uri uri)
        {
            Dictionary<String, String> result = new Dictionary<string, string>();

            string queryString = uri.Query.Substring(1);
            foreach (var item in queryString.Split('&'))
            {
                if(item == String.Empty)
                {
                    continue;
                }

                var keyValue = item.Split('=');
                if(keyValue.Length == 2)
                {
                    result.Add(keyValue[0], keyValue[1]);
                }
                else
                {
                    throw new ArgumentException(String.Concat("QueryString不符合规范,应该以?分割QueryString与Url,QueryString之间用&连接,用=连接键与值.\r\n", uri.ToString()));
                }
            }
            return result;
        }

        /// <summary>
        /// 从UrlString中获取Query字典
        /// </summary>
        /// <param name="urlString">提供的Url字符串</param>
        /// <returns>得到的字典</returns>
        public static Dictionary<String,String> GetUrlQuery(String urlString)
        {
            return GetUrlQuery(new Uri(urlString));
        }
    }
}