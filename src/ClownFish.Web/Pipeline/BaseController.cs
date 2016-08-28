using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 所有控制器的基类
	/// </summary>
	public abstract class BaseController
	{
		private HttpContextBase _httpContextBase;

		/// <summary>
		/// HTTP上下文相关对象（HttpContextBase的实例）
		/// </summary>
		public HttpContextBase Context
		{
			get
			{
				if( _httpContextBase == null )
					_httpContextBase = new HttpContextWrapper(this.HttpContext);
				return _httpContextBase;
			}
		}


		/// <summary>
		/// HTTP上下文相关对象（HttpContext的实例）
		/// </summary>
		public HttpContext HttpContext { get; internal set; }


		/// <summary>
		/// 获取 WebRuntime 实例的引用
		/// </summary>
		public WebRuntime WebRuntime 
		{
			get { return WebRuntime.Instance; }
		}


		/// <summary>
		/// 从当前请求中读取Cookie
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public HttpCookie GetCookie(string name)
		{
			return this.HttpContext.Request.Cookies[name];
		}

		/// <summary>
		/// 写入一个Cookie到当前响应输出
		/// </summary>
		/// <param name="cookie"></param>
		public void WriteCookie(HttpCookie cookie)
		{
			this.HttpContext.Response.Cookies.Add(cookie);
		}

		/// <summary>
		/// 输出一个响应头
		/// </summary>
		/// <param name="headerName">响应头的名称</param>
		/// <param name="value">响应头的值</param>
		public void WriteHeader(string headerName, string value)
		{
			this.WebRuntime.WriteResponseHeader(this.HttpContext.Response, headerName, value);
		}


		/// <summary>
		/// 获取指定的请求头内容
		/// </summary>
		/// <param name="headerName">请求头的名称</param>
		/// <returns></returns>
		public string GetHeader(string headerName)
		{
			return this.HttpContext.Request.Headers[headerName];
		}

	}
}
