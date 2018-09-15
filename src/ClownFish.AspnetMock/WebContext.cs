using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.Globalization;
using System.Web.Hosting;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Principal;
using ClownFish.Web;
using System.Web.Security;
using ClownFish.Base;
using ClownFish.Base.WebClient;

namespace ClownFish.AspnetMock
{
	/// <summary>
	/// 模拟 ASP.NET 请求上下文
	/// </summary>
	public sealed class WebContext : IDisposable
	{
		private HttpContext _context = null;
		
		/// <summary>
		/// 模拟的Request对象
		/// </summary>
		public MockHttpRequest Request { get; private set; }

		///// <summary>
		///// 模拟的Response对象
		///// </summary>
		public MockHttpResponse Response { get; private set; }

		/// <summary>
		/// 当前WebContext作用域内有效的 WebContext 引用
		/// </summary>
		public HttpContext HttpContext
		{
			get { return _context; }
		}


		public MockHttpApplication Application { get; private set; }


	
		/// <summary>
		/// 构造函数，用于构造WEB的运行环境。
		/// </summary>
		/// <param name="url">一个绝对路径的URL字符串：/aa/bb/abc.aspx?id=2&name=xxxx</param>
		public WebContext(string url)
		{
			if( HttpContext.Current != null )
				throw new InvalidProgramException("WebContext 不支持嵌套使用。");

			MockHttpRuntime.EnsureInit();

			url = CheckUrl(url);

			HttpRequest request = CreateRequest(url);
			Request = new MockHttpRequest(request);
			Response = new MockHttpResponse();


			_context = new HttpContext(request, Response.Response);
			HttpContext.Current = _context;

			Application = new MockHttpApplication(_context);



			// 准备Session集合
			MockSessionState myState = new MockSessionState(Guid.NewGuid().ToString("N"),
															new SessionStateItemCollection(), new HttpStaticObjectsCollection(),
															5, true, HttpCookieMode.UseUri, SessionStateMode.InProc, false);

			HttpSessionState state = Activator.CreateInstance(typeof(HttpSessionState),
				 BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic, null,
				 new object[] { myState }, CultureInfo.CurrentCulture) as HttpSessionState;

			_context.Items["AspSession"] = state;



			// 准备Application集合
			SetApplicationState();
		}


		/// <summary>
		/// 根据一段请求文本创建WebContext，并填充相关属性，
		/// 文本格式可参考Fiddler的Inspectors标签页内容
		/// </summary>
		/// <param name="requestText"></param>
		/// <returns></returns>
		public static WebContext FromRawText(string requestText)
		{
			HttpOption option = HttpOption.FromRawText(requestText);

			WebContext context = new WebContext(option.Url);
			context.Request.HttpMethod = option.Method;
			//context.Request.Browser=;

			if( option.Headers != null ) {
				// 填充请求头
				foreach( NameValue nv in option.Headers )
					context.Request.AddHeader(nv.Name, nv.Value);

				// 获取 UrlReferrer
				string urlReferrer = option.Headers["Referer"];
				if( string.IsNullOrEmpty(urlReferrer) == false )
					context.Request.UrlReferrer = urlReferrer;

				// 填充 Cookie
				string cookieLine = option.Headers["Cookie"];
				if( string.IsNullOrEmpty(cookieLine) == false ) {
					List<NameValue> list = cookieLine.SplitString(';', '=');
					foreach( NameValue nv in list )
						context.Request.AddCookie(new HttpCookie(nv.Name, nv.Value));
				}


				// 获取浏览器信息
				string userAgent = option.Headers["User-Agent"];
				if( string.IsNullOrEmpty(userAgent) == false ) {
					SetBrowserInfo(userAgent, context);
				}

                string contentType = option.Headers["Content-Type"];
                if(string.IsNullOrEmpty(contentType) == false)
                    context.Request.ContentType = contentType;
            }

			// 设置请求体
			if( option.Data != null ) {
				context.Request.SetForm((string)option.Data);
				context.Request.SetInputStream((string)option.Data);				
			}

			return context;
		}


		private static void SetBrowserInfo(string userAgent, WebContext context)
		{
			// 最靠谱的做法是根据 C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\Browsers 中的配置信息来解析，但是自行实现还是有些复杂。
			// 这里实现一个简单的版本，满足日常开发需要


			// Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko
			if( userAgent.IndexOf(" Trident/7.0; rv:11.0) like Gecko") > 0 ) {
				context.Request.Browser.Browser = "InternetExplorer";
				context.Request.Browser.MajorVersion = 11;	// 不解析了。
				return;
			}

			// Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)
			if( userAgent.IndexOf("MSIE 10.0") > 0 ) {
				context.Request.Browser.Browser = "IE";
				context.Request.Browser.MajorVersion = 10;
				return;
			}

			// Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
			if( userAgent.IndexOf("MSIE 9.0") > 0 ) {
				context.Request.Browser.Browser = "IE";
				context.Request.Browser.MajorVersion = 9;
				return;
			}

			// Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)
			if( userAgent.IndexOf("MSIE 8.0") > 0 ) {
				context.Request.Browser.Browser = "IE";
				context.Request.Browser.MajorVersion = 8;
				return;
			}

			// Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)
			if( userAgent.IndexOf("MSIE 7.0") > 0 ) {
				context.Request.Browser.Browser = "IE";
				context.Request.Browser.MajorVersion = 7;
				return;
			}

			// Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)
			if( userAgent.IndexOf("MSIE 6.0") > 0 ) {
				context.Request.Browser.Browser = "IE";
				context.Request.Browser.MajorVersion = 6;
				return;
			}

			// Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.93 Safari/537.36 OPR/32.0.1948.69
			if( userAgent.IndexOf(" OPR/") > 0 ) {
				context.Request.Browser.Browser = "Opera";
				context.Request.Browser.MajorVersion = 32;	// 不解析了。
				return;
			}

			// Mozilla/5.0 (Windows NT 6.3; WOW64; rv:35.0) Gecko/20100101 Firefox/35.0
			if( userAgent.IndexOf(" Firefox/") > 0 ) {
				context.Request.Browser.Browser = "Firefox";
				context.Request.Browser.MajorVersion = 35;	// 不解析了。
				return;
			}

			// Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36
			if( userAgent.IndexOf(" Chrome/") > 0 ) {
				context.Request.Browser.Browser = "Chrome";
				context.Request.Browser.MajorVersion = 30;	// 不解析了。
				return;
			}

			// Mozilla/5.0 (Windows; U; Windows NT 6.2; zh-CN) AppleWebKit/533.19.4 (KHTML, like Gecko) Version/5.0.3 Safari/533.19.4
			if( userAgent.IndexOf(" Safari/") > 0 ) {
				context.Request.Browser.Browser = "Safari";
				context.Request.Browser.MajorVersion = 5;	// 不解析了。
				return;
			}

			

		}

		internal static HttpRequest CreateRequest(string url)
		{
			if( string.IsNullOrEmpty(url) )
				throw new ArgumentNullException("url");

			string path = null;
			string queryString = null;
			int p = url.IndexOf('?');
			if( p > 0 ) {
				path = url.Substring(0, p);
				queryString = url.Substring(p + 1);
			}
			else {
				path = url;
			}

			return new HttpRequest(@"c:\web\test\abc.aspx", path, queryString);
		}


		internal static string CheckUrl(string url)
		{
			if( string.IsNullOrEmpty(url) )
				throw new ArgumentNullException("url");


			if( url.StartsWith("/") == false
				&&	url.StartsWith("http://") == false
				&&	url.StartsWith("https://") == false )
				throw new ArgumentException("参数 url 的格式无效。");


			if( url.StartsWith("http://") || url.StartsWith("https://") )
				return url;
			else
				return "http://www.test.com" + url;
		}


		private void SetApplicationState()
		{
			// HttpContext.Application 属性的实现如下：
			// return HttpApplicationFactory.ApplicationState;

			// HttpApplicationFactory.ApplicationState的实现（大致）如下：
			// return _theApplicationFactory._state;


			// 每次都创建一个新的容器对象，避免测试用例存在数据残留。
			HttpApplicationState appState = Activator.CreateInstance(typeof(HttpApplicationState), true) as HttpApplicationState;

			//得到HttpApplicationFactory并且给_state 赋值
			Type t = typeof(System.Web.HttpContext).Assembly.GetType("System.Web.HttpApplicationFactory");
			object httpApplicationFactoryInstance = Activator.CreateInstance(t, true);


			var theApplicationFactoryField = t.GetStaticField("_theApplicationFactory");
			theApplicationFactoryField.SetValue(null, httpApplicationFactoryInstance);


			var stateField = t.GetInstanceField("_state");
			stateField.SetValue(httpApplicationFactoryInstance, appState);
		}



		public void BindPage(System.Web.UI.Page page)
		{
			if( page == null )
				throw new ArgumentNullException("page");

			Type t = typeof(System.Web.UI.Page);

			t.GetInstanceField("_request").SetValue(page, _context.Request);
			t.GetInstanceField("_response").SetValue(page, _context.Response);
			t.GetInstanceField("_application").SetValue(page, _context.Application);
		}



		/// <summary>
		/// 设置当前用户名
		/// </summary>
		/// <param name="username"></param>
		public void SetUserName(string username)
		{
			SetUserName(username, null);
		}

		/// <summary>
		/// 设置当前用户名
		/// </summary>
		/// <param name="username"></param>
		/// <param name="roles"></param>
		public void SetUserName(string username, string[] roles)
		{
			IIdentity _identity = new GenericIdentity(username);
			IPrincipal user = new GenericPrincipal(_identity, roles);
			_context.User = user;
		}


		/// <summary>
		/// 设置当前用户身份为FormsIdentity实例
		/// </summary>
		/// <param name="username"></param>
		/// <param name="userData"></param>
		public void SetFormsUser(string username, string userData)
		{
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, 
					username, DateTime.Now, DateTime.MaxValue, true, userData);

			IIdentity _identity = new FormsIdentity(ticket);
			IPrincipal user = new GenericPrincipal(_identity, null);
			_context.User = user;
		}



		public void AddSession(string name, object value)
		{
			HttpSessionState state = this.HttpContext.Items["AspSession"] as HttpSessionState;
			state.Add(name, value);
		}

		/// <summary>
		/// 清理对象
		/// </summary>
		public void Dispose()
		{
			this._context = null;
			HttpContext.Current = null;
		}


		


	}





}

