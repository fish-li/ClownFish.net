using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
    /// <summary>
    /// 提供一些与URL有关的扩展方法
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// 等同于：System.Web.HttpUtility.UrlEncode(text)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UrlEncode(this string text)
        {
            if( string.IsNullOrEmpty(text) )
                return text;

            return System.Web.HttpUtility.UrlEncode(text);
        }


        /// <summary>
        /// 拼接URL字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConcatQueryStringArgs(this string url, string name, string value)
        {
            if( string.IsNullOrEmpty(url) || string.IsNullOrEmpty(name) )
                return url;

            string args = System.Web.HttpUtility.UrlEncode(name) + "=" + System.Web.HttpUtility.UrlEncode(value ?? string.Empty);

            if( url.IndexOf('?') < 0 )
                return url + "?" + args;
            else
                return url + "&" + args;
        }


        /// <summary>
        /// 拼接URL字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ConcatQueryStringArgs(this string url, IEnumerable<NameValue> list)
        {
            if( list == null )
                return url;

            StringBuilder sb = new StringBuilder();
            foreach( var item in list ) {
                if( sb.Length > 0 )
                    sb.Append("&");

                sb.Append(System.Web.HttpUtility.UrlEncode(item.Name))
                    .Append("=")
                    .Append(System.Web.HttpUtility.UrlEncode(item.Value));
            }

            if( sb.Length == 0 )
                return url;

            if( url.IndexOf('?') < 0 )
                return url + "?" + sb.ToString();
            else
                return url + "&" + sb.ToString();
        }

    }
}
