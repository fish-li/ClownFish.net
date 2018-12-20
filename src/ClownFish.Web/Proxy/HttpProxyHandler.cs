using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base;
using ClownFish.Base.Http;
using ClownFish.Base.Reflection;

namespace ClownFish.Web.Proxy
{
    /// <summary>
    /// HTTP请求转发处理器
    /// </summary>
    public class HttpProxyHandler : HttpTaskAsyncHandler
    {
        private string _srcUrl;
        private string _destUr;

        static HttpProxyHandler()
        {
            SysNetInitializer.Init();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="srcUrl"></param>
        /// <param name="destUrl"></param>
        public HttpProxyHandler(string srcUrl, string destUrl)
        {
            _srcUrl = srcUrl;
            _destUr = destUrl;
        }


        /// <summary>
        /// 实现接口方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            // 创建请求对象
            HttpWebRequest webRequest = CreateWebRequest(_destUr, context);

            try {
                // 复制请求头
                CopyRequestHeaders(context, webRequest);


                // 复制请求体
                if( context.Request.InputStream != null  && context.Request.InputStream.Length > 0 ) {      // 用于 System.Web.HttpRequest
                //if( context.Request.InputStream != null && context.Request.HasEntityBody ) {              // 用于 System.Net.HttpListenerRequest

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

            webRequest.AllowAutoRedirect = false;   // 禁止自动重定向，用于返回302信息
            webRequest.ServicePoint.Expect100Continue = false;

            // 设置原始请求地址
            webRequest.Headers.Add("X-HttpProxyHandler-OriginalUrl", _srcUrl);

            return webRequest;
        }

        private void CopyStream(Stream src, Stream dest)
        {
            byte[] buffer = new byte[1024 * 4];

            using( BinaryReader reader = new BinaryReader(src) ) {
                for(; ; ) {
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
            "Content-Type", "Content-Length", "Server", "X-Powered-By"  , "Transfer-Encoding"
			//, "Keep-Alive", "Transfer-Encoding", "WWW-Authenticate"
		};


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

            // 增加一个特殊的标记，在 StreamResult.cs 中会有对应的处理逻辑
            webRequest.Headers.Add("x-HttpProxyHandler", "1");

            webRequest.Headers.Add("x-UserHostAddress", context.Request.UserHostAddress);
            //webRequest.Headers.Remove("Cache-Control");

            if( string.Equals(context.Request.Headers["Connection"], "keep-alive", StringComparison.OrdinalIgnoreCase) )
                webRequest.KeepAlive = true;


            string referer = context.Request.Headers["Referer"];
            if( string.IsNullOrEmpty(referer) == false ) {
                if( referer.IndexOf("://") > 0 ) {
                    string refererRoot = UrlExtensions.GetWebSiteRoot(referer);
                    string requestRoot = UrlExtensions.GetWebSiteRoot(webRequest.RequestUri.AbsoluteUri);

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
            foreach( string name in webResponse.Headers.AllKeys ) {     // 这行代码可以不修改，内部直接返回InnerCollection.AllKeys
                if( s_IgnoreResponseHeaders.FirstOrDefault(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)) != null )
                    continue;


                //string[] values = webResponse.Headers.GetValues(name);	// 这行代码的结果不正确
                string[] values = headers.GetValues(name);
                foreach( string value in values )
                    SetResponseHeader(context, webResponse, name, value);
            }


            if( context.Request.UserAgent.IndexOf("Safari") >= 0 ) {
                // 可参考 StreamResult.cs 中的注释
                // 主要原因是 Safari 这货在响应头中的文件名支持有问题（也有可能是我没找到更有效的方法吧。）

                string filename = headers["X-Content-Disposition-proxy"];
                if( string.IsNullOrEmpty(filename) == false ) {
                    filename = System.Web.HttpUtility.UrlDecode(filename);                     // 从备用的响应头拿到正确的文件名

                    context.Response.Headers.Remove("X-Content-Disposition-proxy"); // 删除这个头，因为对用户浏览器没有用。
                    context.Response.Headers.Remove("Content-Disposition");         // 删除这个头，因为它的结果现在是错误的。

                    string headerValue = string.Format("attachment; filename=\"{0}\"", filename);
                    context.Response.Headers.Add("Content-Disposition", headerValue);   // 重新写入正确的响应头
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






