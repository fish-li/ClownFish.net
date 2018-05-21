using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ClownFish.Base.Reflection;


namespace ClownFish.Web
{
	internal static class HttpExtensions
	{

        /// <summary>
        /// 获取实际的虚拟路径，如果网站部署在虚拟目录中，将去除虚拟目录的顶层目录名。
        /// </summary>
        /// <param name="context">HttpContext实例的引用</param>
        /// <returns>去除虚拟目录后的实际虚拟路径。</returns>
        public static string GetRealVirtualPath(this HttpContext context)
		{
			string virtualPath = context.Request.Path;

			// 解决虚拟目录问题
			if( context.Request.ApplicationPath != "/" )
				if( virtualPath.StartsWith(context.Request.ApplicationPath + "/") )
					return virtualPath.Substring(context.Request.ApplicationPath.Length);


			return virtualPath;
		}


		private static readonly Regex s_urlRootRegex = new Regex(@"\w+://[^/]+", RegexOptions.Compiled);


		public static string GetWebSiteRoot(string absoluteUrl)
		{
			Match m = s_urlRootRegex.Match(absoluteUrl);		// 提取：http://xxx.xxxx.com

			if( m.Success ) 
				return m.Groups[0].Value;
			else
				return null;
		}


		/// <summary>
		/// 尝试判断是不是JSONP请求，并获取回调方法名称
		/// </summary>
		/// <param name="context"></param>
		/// <param name="callbackParameterName"></param>
		/// <returns></returns>
		public static string TryGetJsonpCallback(this HttpContext context, string callbackParameterName)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			if( string.IsNullOrEmpty(callbackParameterName) )
				return null;

			// 判断 JSONP 的条件：
			// 1. GET 请求
			// 2. 查询字符串存在 callback 参数
			// 3. 存在Referrer头，因为是在页面中调用中，浏览器会设置
			// 4. Referrer与当前站点不相同。

			if( context.Request.HttpMethod == "GET" ) {
				string callback = context.Request.QueryString[callbackParameterName];
				if( string.IsNullOrEmpty(callback) == false ) {

					Uri referer = context.Request.UrlReferrer;
					if( referer != null ) {
						string host1 = referer.Scheme + "://" + referer.Authority;
						string host2 = context.Request.Url.Scheme + "://" + context.Request.Url.Authority;

						if( string.Compare(host1, host2, StringComparison.OrdinalIgnoreCase) != 0 ) {
							return callback;
						}
					}
				}
			}

			return null;
		}




	}
}
