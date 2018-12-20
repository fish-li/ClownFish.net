using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web.Proxy
{
	/// <summary>
	/// 一个简单的反向代理的HTTP模块，用cookie维持目标网址
	/// </summary>
	public sealed class CookieProxyModule : IHttpModule
	{
		/// <summary>
		/// 常量字符串：代理站点的Cookie名字
		/// </summary>
		private static readonly string s_ProxySiteCookieName = "fmps12";// 随便取个特殊的名字，只要不被其他人使用就好


        /// <summary>
        /// 生成可供ReverseProxyModule读取的代理站点Cookie
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="response"></param>
        public static void CreateProxySiteCookie(string siteAddress, HttpResponse response)
        {
            if( string.IsNullOrEmpty(siteAddress) )
                throw new ArgumentNullException("siteAddress");
            if( response == null )
                throw new ArgumentNullException(nameof(response));

            if( (siteAddress.StartsWith("http://") == false) && (siteAddress.StartsWith("https://") == false) )
                throw new ArgumentException("参数要求以 http:// 或者 https:// 开头的绝对地址URL格式。");

            if( siteAddress.EndsWith("/") ) // 因为要和 Request.RawUrl 拼接，所以需要去掉结尾的【反斜杠】字符
                siteAddress = siteAddress.TrimEnd('/');

            string value = Convert.ToBase64String(Encoding.UTF8.GetBytes(siteAddress)); // 编码，防止出现特殊字符影响
            HttpCookie cookie = new HttpCookie(s_ProxySiteCookieName, value);           // 临时会话Cookie
            cookie.HttpOnly = true;
            response.Cookies.Add(cookie);
        }



        /// <summary>
        /// 删除代理的标记Cookie
        /// </summary>
        /// <param name="response"></param>
        public static void RemoveProxySiteCookie(HttpResponse response)
        {
            if( response == null )
                throw new ArgumentNullException(nameof(response));

            HttpCookie cookie = new HttpCookie(s_ProxySiteCookieName, string.Empty);
            cookie.Expires = DateTime.Now.AddYears(-1);
            response.Cookies.Add(cookie);
        }




        /// <summary>
        /// 实现 IHttpModule.Init 方法
        /// </summary>
        /// <param name="app"></param>
        public void Init(HttpApplication app)
		{
			app.BeginRequest += app_BeginRequest;
		}

		void app_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;

            if( app.Request.Path == "/" && app.Request.QueryString["target"] == "clear" ) {
                RemoveProxySiteCookie(app.Response);
                app.Response.Redirect("/");
                return;
            }


			string destRoot = GetProxySiteFromCookie(app);
			if( string.IsNullOrEmpty(destRoot) )
				return;

            string destUrl = destRoot + app.Request.RawUrl;
            string srcUrl = app.Request.Url.AbsoluteUri;
            
            HttpProxyHandler hander = new HttpProxyHandler(srcUrl, destUrl);
            app.Context.RemapHandler(hander);
            app.Context.Response.Headers.Add("X-CookieProxyModule", destUrl);	// 用于调试诊断
		}


        /// <summary>
		/// 尝试从Cookie中获取代理目标的站点地址
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		private string GetProxySiteFromCookie(HttpApplication app)
        {
            HttpCookie cookie = app.Request.Cookies[s_ProxySiteCookieName];
            if( cookie != null ) {
                string value = cookie.Value;
                if( string.IsNullOrEmpty(value) == false )
                    try {
                        return Encoding.UTF8.GetString(Convert.FromBase64String(value));
                    }
                    catch { /* 如果无法正确读取，就忽略   */ }
            }

            return null;
        }



		/// <summary>
		/// 实现 IHttpModule.Dispose 方法
		/// </summary>
		public void Dispose()
		{
		}
	}
}
