using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Web;
using ClownFish.Base.Reflection;
using ClownFish.Web.Reflection;
using ClownFish.Web.Client;


namespace ClownFish.Web.Proxy
{
	/// <summary>
	/// 用于服务端代理转发请求的处理器
	/// </summary>
	public class ProxyTransferHandler : HttpTaskAsyncHandler
	{
		/// <summary>
		/// 用于【外部模块】给 ProxyTransferHandler 传递目标网址
		/// </summary>
		public static readonly string TargetUrlKeyName = "x-target-url";


		static ProxyTransferHandler()
		{
			// 触发 HttpClient 的静态构造函数
			HttpClient.TriggerCctor();
		}


		/// <summary>
		/// 获取需要转发的目标地址
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual string GetTransferAddress(HttpContext context)
		{
			// 扩展点：允许替换默认实现方式，从其它地方获取目标地址

			return (context.Items[TargetUrlKeyName] as string)
						?? context.Request.Headers[TargetUrlKeyName]
						?? context.Request[TargetUrlKeyName]
						;
		}


		/// <summary>
		/// 创建 HttpWebRequest 对象
		/// </summary>
		/// <param name="destAddress">需要转发的目标地址</param>
		/// <param name="context">HttpContext实例</param>
		/// <returns></returns>
		protected virtual HttpWebRequest CreateWebRequest(string destAddress, HttpContext context)
		{
			// 扩展点：允许替换默认实现方式，增加一些额外的HttpWebRequest属性配置

			// 创建请求对象
			HttpWebRequest webRequest = WebRequest.CreateHttp(destAddress);

			webRequest.Method = context.Request.HttpMethod;

			webRequest.AllowAutoRedirect = false;	// 禁止自动重定向，用于返回302信息
			webRequest.ServicePoint.Expect100Continue = false;

			return webRequest;
		}


		/// <summary>
		/// 以异步方式执行HttpHanlder（基类方法重写）
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async override Task ProcessRequestAsync(HttpContext context)
		{
			string destAddress = GetTransferAddress(context);
			if( string.IsNullOrEmpty(destAddress) )
				throw new ArgumentNullException("destAddress");


			// 创建请求对象
			HttpWebRequest webRequest = CreateWebRequest(destAddress, context);

			try {
				// 复制请求头
				CopyRequestHeaders(context, webRequest);


				// 复制请求体
				if( context.Request.InputStream != null && context.Request.InputStream.Length > 0 ) {

					using( Stream requestStream = await webRequest.GetRequestStreamAsync() ) {
						context.Request.InputStream.CopyTo(requestStream);
					}
				}

				// 发送请求，并等待返回
				WebException lastException = null;
				HttpWebResponse webResponse = null;
				try {
					webResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
				}
				catch( WebException webException ) {
					webResponse = (HttpWebResponse)webException.Response;
					lastException = webException;
				}


				if( webResponse == null ) {
					if( lastException != null ) {
						// 重写错误结果
						context.Response.StatusCode = 500;

						IActionResult result = new TextResult(lastException.ToString());
						result.Ouput(context);
					}

					return;     // 有时候没有异常，却会莫名奇妙地进入这里，实在是没法解释，所以只能是不处理了。
				}
				else {
					using( webResponse ) {
						// 获取响应流，这里不考虑GZIP压缩的情况（因为不需要考虑）
						using( Stream responseStream = webResponse.GetResponseStream() ) {

							// 写响应头
							CopyResponseHeaders(context, webResponse);

							// 写响应流
							//responseStream.CopyTo(context.Response.OutputStream);
							// 如果是 Thunk 编码，responseStream.CopyTo不能得到正确的结果（上面的代码）。
							// 所以，重新实现了流的复制版本，就是下面的方法。
							CopyStream(responseStream, context.Response.OutputStream);
						}
					}
				}
			}
			catch( Exception ex ) {

				WriteException(context, ex);
			}
		}


		private void CopyStream(Stream src, Stream dest)
		{
			byte[] buffer = new byte[1024 * 2];

			using( BinaryReader reader = new BinaryReader(src) ) {
				for( ; ; ) {
					int length = reader.Read(buffer, 0, buffer.Length);
					if( length > 0 )
						dest.Write(buffer, 0, length);
					else
						break;
				}
			}
		}


		private void WriteException(HttpContext context, Exception ex)
		{
			context.Response.ClearHeaders();
			context.Response.ClearContent();

			// 重写错误结果
			context.Response.StatusCode = 500;

			IActionResult result = new TextResult(ex.ToString());
			result.Ouput(context);
		}


		private static readonly string[] s_IgnoreRequestHeaders = new string[] {
			"Connection", "Referer"
		};

		private static readonly string[] s_IgnoreResponseHeaders = new string[] {
			"Content-Type", "Content-Length", "Server", "X-Powered-By"	, "Transfer-Encoding"
			//, "Keep-Alive", "Transfer-Encoding", "WWW-Authenticate"
		};


		internal static readonly string ProxyFlagHeader = "x-ClownFish.Web-Proxy";

		/// <summary>
		/// 复制请求头
		/// </summary>
		/// <param name="context"></param>
		/// <param name="webRequest"></param>
		protected virtual void CopyRequestHeaders(HttpContext context, HttpWebRequest webRequest)
		{
			// 扩展点：允许替换默认实现方式

			// 复制请求头
			foreach( string name in context.Request.Headers.AllKeys ) {
				// 过滤不允许直接指定的请求头
				if( s_IgnoreRequestHeaders.FirstOrDefault(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)) != null )
					continue;


				string value = context.Request.Headers[name];
				SetRequestHeader(context, webRequest, name, value);
			}

			webRequest.Headers.Add(ProxyFlagHeader, "1");

			webRequest.Headers.Add("x-UserHostAddress", context.Request.UserHostAddress);
			//webRequest.Headers.Remove("Cache-Control");

			if( string.Equals(context.Request.Headers["Connection"], "keep-alive", StringComparison.OrdinalIgnoreCase) )
				webRequest.KeepAlive = true;


			string referer = context.Request.Headers["Referer"];
			if( string.IsNullOrEmpty(referer) == false ) {
				if( referer.IndexOf("://") > 0 ) {
					string refererRoot = HttpExtensions.GetWebSiteRoot(referer);
					string requestRoot = HttpExtensions.GetWebSiteRoot(webRequest.RequestUri.AbsoluteUri);

					string referer2 = requestRoot + referer.Substring(refererRoot.Length);
					SetRequestHeader(context, webRequest, "Referer", referer2);
				}
			}
		}


		/// <summary>
		/// 设置请求头
		/// </summary>
		/// <param name="context"></param>
		/// <param name="webRequest"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		protected virtual void SetRequestHeader(HttpContext context, HttpWebRequest webRequest, string name, string value)
		{
			// 扩展点：允许替换默认实现方式

			try {
				webRequest.Headers.InternalAdd(name, value);
			}
			catch {
				// 有可能浏览器会发送不规范的请求头，
				// 例如，IE 11 发送了这样一个请求头（CSS文件中引用了一张图片）：
				//      Referer: http://www.fish-reverseproxy.com/桌面部件/普通桌面/4.jpg
				//      请求头的内容没有做编码处理（超出RFC规范定义的字符范围）。
			}
		}



		/// <summary>
		/// 复制响应头
		/// </summary>
		/// <param name="context"></param>
		/// <param name="webResponse"></param>
		protected virtual void CopyResponseHeaders(HttpContext context, HttpWebResponse webResponse)
		{
			// 扩展点：允许替换默认实现方式

			context.Response.StatusCode = (int)webResponse.StatusCode;
			
			// 注意：不能直接使用Content-Type的内容，这里非常坑爹！
			//  例如： Content-Type： text/css
			//  如果直接将结果写入Response.Headers
			//  得到的结果是：Content-Type： text/html，导致响应类型是错误的，
			//  对于包含编码的时候，编码会丢失！

			context.Response.ContentType = webResponse.ContentType;



			// 注意：HttpWebResponse 有个BUG，有些响应头是允许重复指定的，但是通过 HttpWebResponse.Headers 读取时，结果会合并这些响应头
			//      虽然 Headers 提供了 GetValues(name) 方法，但它却会解析里面的值，结果导致原本是单一的标头会折成二行，结果仍然是错误的，
			//      例如，从服务端删除Cookie时，响应头： Set-Cookie: mvc-user=; expires=Mon, 11-Oct-1999 16:00:00 GMT; path=/; HttpOnly
			//      由于中间有个逗号，调用 GetValues("Set-Cookie") 会返回二行的数组：
			//      [0]: "mvc-user=; expires=Mon"
			//      [1]: "11-Oct-1999 16:00:00 GMT; path=/; HttpOnly"

			//  然而，webResponse.Headers 内部的 InnerCollection 属性的保存结果是正确的，
			//       调用它的GetValues(name) 方法，在这种情况下得到的结果是正确的
			//  所以，这里就直接用反射的方式拿到 InnerCollection ，用它获取所需的结果，避开微软错误的实现方式。

			PropertyInfo propInfo = webResponse.Headers.GetType().GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			NameValueCollection headers = (NameValueCollection)propInfo.FastGetValue(webResponse.Headers);



			// 复制响应头
			foreach( string name in webResponse.Headers.AllKeys ) {		// 这行代码可以不修改，内部直接返回InnerCollection.AllKeys
				if( s_IgnoreResponseHeaders.FirstOrDefault(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)) != null )
					continue;


				//string[] values = webResponse.Headers.GetValues(name);	// 这行代码的结果不正确
				string[] values = headers.GetValues(name);
				foreach(string value in values )
					SetResponseHeader(context, webResponse, name, value);
			}


			if( context.Request.Browser.Browser == "Safari" ) {
				// 可参考 StreamResult.cs 中的注释
				// 主要原因是 Safari 这货在响应头中的文件名支持有问题（也有可能是我没找到更有效的方法吧。）

				string filename = headers["X-Content-Disposition-proxy"];
				if( string.IsNullOrEmpty(filename) == false ) {
					filename = HttpUtility.UrlDecode(filename);						// 从备用的响应头拿到正确的文件名

					context.Response.Headers.Remove("X-Content-Disposition-proxy");	// 删除这个头，因为对用户浏览器没有用。
					context.Response.Headers.Remove("Content-Disposition");			// 删除这个头，因为它的结果现在是错误的。

					string headerValue = string.Format("attachment; filename=\"{0}\"", filename);
					context.Response.Headers.Add("Content-Disposition", headerValue);	// 重新写入正确的响应头
				}
			}

			
		}


		private void SetAllowMultiResponseHeader(HttpContext context, HttpWebResponse webResponse, string name)
		{
			string value1 = webResponse.Headers[name];
			if( string.IsNullOrEmpty(value1) == false ) {
				if( value1.IndexOf(',') < 0 ) {
					SetResponseHeader(context, webResponse, name, value1);
				}
				else {
					string[] valueArray = value1.SplitTrim(',');

					foreach( string value2 in valueArray )
						SetResponseHeader(context, webResponse, name, value2);
				}
			}
		}

		/// <summary>
		/// 设置响应头
		/// </summary>
		/// <param name="context"></param>
		/// <param name="webResponse"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		protected virtual void SetResponseHeader(HttpContext context, HttpWebResponse webResponse, string name, string value)
		{
			// 扩展点：允许替换默认实现方式

			try {
				context.Response.Headers.Add(name, value);
			}
			catch {
				// 防止出现不允许设置的请求头，未来可以增加日志记录
			}
		}

	}


}



	//HttpWorkerRequest.DefineHeader(true, true, 0, "Cache-Control", "HTTP_CACHE_CONTROL");
	//HttpWorkerRequest.DefineHeader(true, true, 1, "Connection", "HTTP_CONNECTION");
	//HttpWorkerRequest.DefineHeader(true, true, 2, "Date", "HTTP_DATE");
	//HttpWorkerRequest.DefineHeader(true, true, 3, "Keep-Alive", "HTTP_KEEP_ALIVE");
	//HttpWorkerRequest.DefineHeader(true, true, 4, "Pragma", "HTTP_PRAGMA");
	//HttpWorkerRequest.DefineHeader(true, true, 5, "Trailer", "HTTP_TRAILER");
	//HttpWorkerRequest.DefineHeader(true, true, 6, "Transfer-Encoding", "HTTP_TRANSFER_ENCODING");
	//HttpWorkerRequest.DefineHeader(true, true, 7, "Upgrade", "HTTP_UPGRADE");
	//HttpWorkerRequest.DefineHeader(true, true, 8, "Via", "HTTP_VIA");
	//HttpWorkerRequest.DefineHeader(true, true, 9, "Warning", "HTTP_WARNING");
	//HttpWorkerRequest.DefineHeader(true, true, 10, "Allow", "HTTP_ALLOW");
	//HttpWorkerRequest.DefineHeader(true, true, 11, "Content-Length", "HTTP_CONTENT_LENGTH");
	//HttpWorkerRequest.DefineHeader(true, true, 12, "Content-Type", "HTTP_CONTENT_TYPE");
	//HttpWorkerRequest.DefineHeader(true, true, 13, "Content-Encoding", "HTTP_CONTENT_ENCODING");
	//HttpWorkerRequest.DefineHeader(true, true, 14, "Content-Language", "HTTP_CONTENT_LANGUAGE");
	//HttpWorkerRequest.DefineHeader(true, true, 15, "Content-Location", "HTTP_CONTENT_LOCATION");
	//HttpWorkerRequest.DefineHeader(true, true, 16, "Content-MD5", "HTTP_CONTENT_MD5");
	//HttpWorkerRequest.DefineHeader(true, true, 17, "Content-Range", "HTTP_CONTENT_RANGE");
	//HttpWorkerRequest.DefineHeader(true, true, 18, "Expires", "HTTP_EXPIRES");
	//HttpWorkerRequest.DefineHeader(true, true, 19, "Last-Modified", "HTTP_LAST_MODIFIED");
	//HttpWorkerRequest.DefineHeader(true, false, 20, "Accept", "HTTP_ACCEPT");
	//HttpWorkerRequest.DefineHeader(true, false, 21, "Accept-Charset", "HTTP_ACCEPT_CHARSET");
	//HttpWorkerRequest.DefineHeader(true, false, 22, "Accept-Encoding", "HTTP_ACCEPT_ENCODING");
	//HttpWorkerRequest.DefineHeader(true, false, 23, "Accept-Language", "HTTP_ACCEPT_LANGUAGE");
	//HttpWorkerRequest.DefineHeader(true, false, 24, "Authorization", "HTTP_AUTHORIZATION");
	//HttpWorkerRequest.DefineHeader(true, false, 25, "Cookie", "HTTP_COOKIE");
	//HttpWorkerRequest.DefineHeader(true, false, 26, "Expect", "HTTP_EXPECT");
	//HttpWorkerRequest.DefineHeader(true, false, 27, "From", "HTTP_FROM");
	//HttpWorkerRequest.DefineHeader(true, false, 28, "Host", "HTTP_HOST");
	//HttpWorkerRequest.DefineHeader(true, false, 29, "If-Match", "HTTP_IF_MATCH");
	//HttpWorkerRequest.DefineHeader(true, false, 30, "If-Modified-Since", "HTTP_IF_MODIFIED_SINCE");
	//HttpWorkerRequest.DefineHeader(true, false, 31, "If-None-Match", "HTTP_IF_NONE_MATCH");
	//HttpWorkerRequest.DefineHeader(true, false, 32, "If-Range", "HTTP_IF_RANGE");
	//HttpWorkerRequest.DefineHeader(true, false, 33, "If-Unmodified-Since", "HTTP_IF_UNMODIFIED_SINCE");
	//HttpWorkerRequest.DefineHeader(true, false, 34, "Max-Forwards", "HTTP_MAX_FORWARDS");
	//HttpWorkerRequest.DefineHeader(true, false, 35, "Proxy-Authorization", "HTTP_PROXY_AUTHORIZATION");
	//HttpWorkerRequest.DefineHeader(true, false, 36, "Referer", "HTTP_REFERER");
	//HttpWorkerRequest.DefineHeader(true, false, 37, "Range", "HTTP_RANGE");
	//HttpWorkerRequest.DefineHeader(true, false, 38, "TE", "HTTP_TE");
	//HttpWorkerRequest.DefineHeader(true, false, 39, "User-Agent", "HTTP_USER_AGENT");
	//HttpWorkerRequest.DefineHeader(false, true, 20, "Accept-Ranges", null);
	//HttpWorkerRequest.DefineHeader(false, true, 21, "Age", null);
	//HttpWorkerRequest.DefineHeader(false, true, 22, "ETag", null);
	//HttpWorkerRequest.DefineHeader(false, true, 23, "Location", null);
	//HttpWorkerRequest.DefineHeader(false, true, 24, "Proxy-Authenticate", null);
	//HttpWorkerRequest.DefineHeader(false, true, 25, "Retry-After", null);
	//HttpWorkerRequest.DefineHeader(false, true, 26, "Server", null);
	//HttpWorkerRequest.DefineHeader(false, true, 27, "Set-Cookie", null);
	//HttpWorkerRequest.DefineHeader(false, true, 28, "Vary", null);
	//HttpWorkerRequest.DefineHeader(false, true, 29, "WWW-Authenticate", null);


//internal HeaderInfo(string name, bool requestRestricted, bool responseRestricted, bool multi, HeaderParser p)

//static HeaderInfoTable() {

//	HeaderInfo[] InfoArray = new HeaderInfo[] {
//		new HeaderInfo(HttpKnownHeaderNames.Age,                false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Allow,              false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Accept,             true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Authorization,      false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptRanges,       false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptCharset,      false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptEncoding,     false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.AcceptLanguage,     false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Cookie,             false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Connection,         true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentMD5,         false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentType,        true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.CacheControl,       false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentRange,       false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentLength,      true,   true,   false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentEncoding,    false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentLanguage,    false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ContentLocation,    false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Date,               true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.ETag,               false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Expect,             true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Expires,            false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.From,               false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Host,               true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfMatch,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfRange,            false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfNoneMatch,        false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfModifiedSince,    true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.IfUnmodifiedSince,  false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.KeepAlive,          false,  true,   false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Location,           false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.LastModified,       false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.MaxForwards,        false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Pragma,             false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ProxyAuthenticate,  false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ProxyAuthorization, false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.ProxyConnection,    true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Range,              true,   false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Referer,            true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.RetryAfter,         false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Server,             false,  false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.SetCookie,          false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.SetCookie2,         false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.TE,                 false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Trailer,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.TransferEncoding,   true,   true,   true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Upgrade,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.UserAgent,          true,   false,  false,  SingleParser),
//		new HeaderInfo(HttpKnownHeaderNames.Via,                false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Vary,               false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.Warning,            false,  false,  true,   MultiParser),
//		new HeaderInfo(HttpKnownHeaderNames.WWWAuthenticate,    false,  true,   true,   SingleParser)
//	};