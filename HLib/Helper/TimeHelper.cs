using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HLib.Helper
{
    public static class TimeHelper
    {
        /// <summary>
        /// 将 Unix 时间戳转换为 <see cref="DateTime"/>,并采用指定的时区.
        /// </summary>
        /// <param name="seconds">时间戳</param>
        /// <param name="timeZone">时区</param>
        /// <returns></returns>
        public static DateTime UnixTimeToDataTime(int seconds, TimeZoneInfo timeZone)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0) + timeZone.BaseUtcOffset + new TimeSpan(0, 0, seconds);
        }

        /// <summary>
        /// 将 Unix 时间戳转换为 <see cref="DateTime"/>,使用本地时区
        /// </summary>
        /// <param name="seconds">时间戳</param>
        /// <returns></returns>
        public static DateTime UnixTimeToDataTime(int seconds)
        {
            return UnixTimeToDataTime(seconds, TimeZoneInfo.Local);
        }

        /// <summary>
        /// 将 <see cref="DateTime"/> 转换为 Unix 时间戳,并指定 DateTime 的时区
        /// </summary>
        /// <param name="dateTime">时间戳</param>
        /// <param name="timeZone">时区</param>
        /// <returns></returns>
        public static Double DateTimeToUnixTime(DateTime dateTime, TimeZoneInfo timeZone)
        {
            return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0) - timeZone.BaseUtcOffset).TotalSeconds;
        }

        /// <summary>
        /// 将 <see cref="DateTime"/> 转换为 Unix 时间戳,并指定 DateTime 的时区
        /// </summary>
        /// <param name="dateTime">时间戳</param>
        /// <param name="timeZone">时区</param>
        /// <returns></returns>
        public static Double DateTimeToUnixTime(DateTime dateTime)
        {
            return DateTimeToUnixTime(dateTime, TimeZoneInfo.Local);
        }

        /// <summary>
        /// 从 higam.me 获取网络UTC时间,网络无法访问时,会引发 <see cref="System.Net.WebException"/>
        /// </summary>
        /// <returns>得到的网络时间</returns>
        public static async Task<DateTime> GetInternetTimeUseHLib()
        {
            return DateTime.Parse(await HttpHelper.GetString("http://higan.sinaapp.com/time.php")) - new TimeSpan(8, 0, 0);
        }

        /// <summary>
        /// 从 timeapi.org 获取网络UTC时间,网络无法访问时,会引发 <see cref="System.Net.WebException"/>
        /// </summary>
        /// <returns>得到的网络时间</returns>
        public static async Task<DateTime> GetInternetTimeUseTimeAPI()
        {
            return DateTime.Parse(await HttpHelper.GetString("http://www.timeapi.org/utc/now"));
        }

        /// <summary>
        /// 从 Url 获取网络UTC时间,网络无法访问时,会引发 <see cref="System.Net.WebException"/>,如果没有 Date 头会引发 "找不到 Date 头" 异常
        /// </summary>
        /// <returns>得到的网络时间</returns>
        public static async Task<DateTime> GetInternetTimeUseUrl(String url)
        {
            HttpWebRequest httpRequest = HttpWebRequest.CreateHttp(url);

            using (var httpResponse = await httpRequest.GetResponseAsync())
            {
                var date = httpResponse.Headers["Date"];
                if (date != null)
                {
                    return DateTime.Parse(date);
                }
                throw new Exception("找不到 Date 头");
            }
        }
    }
}
