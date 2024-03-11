#if NETFRAMEWORK

namespace ClownFish.Http.Proxy;

/// <summary>
/// HTTP请求转发处理器
/// </summary>
public class HttpProxyHandler : IAsyncNHttpHandler
{
    private readonly string _destUr;

    static HttpProxyHandler()
    {
        SysNetInitializer.Init();
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="destUrl"></param>
    public HttpProxyHandler(string destUrl)
    {
        _destUr = destUrl;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        try {
            // 创建请求对象
            HttpWebRequest webRequest = CreateWebRequest(httpContext.Request, _destUr);

            // 复制请求头
            CopyRequestHeaders(httpContext.Request, webRequest);

            // 复制请求体
            await CopyRequestBodyAsync(httpContext.Request, webRequest);


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
                    await WriteExceptionAsync(httpContext, lastException);
                }

                return;     // 有时候没有异常，却会莫名奇妙地进入这里，实在是没法解释，所以只能是不处理了。
            }
            else {
                using( webResponse ) {
                    CopyResponse(httpContext.Request.HttpMethod, webResponse, httpContext.Response);
                }
            }
        }
        catch( Exception ex ) {

            await WriteExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    /// 创建 HttpWebRequest 对象
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="destUrl">需要转发的目标地址</param>
    /// <returns></returns>
    protected virtual HttpWebRequest CreateWebRequest(NHttpRequest httpRequest, string destUrl)
    {
        // 创建请求对象
        HttpWebRequest webRequest = WebRequest.CreateHttp(destUrl);

        webRequest.Method = httpRequest.HttpMethod;
        //webRequest.KeepAlive = false;
        webRequest.AllowAutoRedirect = false;   // 禁止自动重定向，用于返回302信息
        webRequest.ServicePoint.Expect100Continue = false;

        return webRequest;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="webRequest"></param>
    protected virtual void CopyRequestHeaders(NHttpRequest httpRequest, HttpWebRequest webRequest)
    {
        // 复制请求头
        foreach( string name in httpRequest.HeaderKeys ) {
            // 过滤不允许直接指定的请求头
            if( HttpProxyModule.IgnoreRequestHeaders.Contains(name) )
                continue;


            string[] values = httpRequest.GetHeaders(name);
            foreach( string value in values )
                SetRequestHeader(webRequest, name, value);
        }

        //webRequest.Headers.Remove("Cache-Control");

        if( string.Equals(httpRequest.Header("Connection"), "keep-alive", StringComparison.OrdinalIgnoreCase) )
            webRequest.KeepAlive = true;

        string destRoot = null;

        string referer = httpRequest.Header("Referer");
        if( string.IsNullOrEmpty(referer) == false ) {
            if( referer.IndexOf("://", StringComparison.Ordinal) > 0 ) {
                string refererRoot = Urls.GetWebSiteRoot(referer);
                if( destRoot == null ) {
                    destRoot = Urls.GetWebSiteRoot(_destUr);
                }
                string referer2 = destRoot + referer.Substring(refererRoot.Length);
                SetRequestHeader(webRequest, "Referer", referer2);
            }
        }

        string origin = httpRequest.Header("Origin");
        if( string.IsNullOrEmpty(origin) == false ) {
            if( destRoot == null ) {
                destRoot = Urls.GetWebSiteRoot(_destUr);
            }
            SetRequestHeader(webRequest, "Origin", destRoot);
        }

        // 设置2个代理相关的请求头
        if( httpRequest.HeaderKeys.Contains("X-Forwarded-Proto", StringComparer.OrdinalIgnoreCase) == false ) {
            SetRequestHeader(webRequest, "X-Forwarded-Proto", httpRequest.RequestUri.Scheme);
        }
        if( httpRequest.HeaderKeys.Contains("X-Forwarded-Host", StringComparer.OrdinalIgnoreCase) == false ) {
            SetRequestHeader(webRequest, "X-Forwarded-Host", httpRequest.RequestUri.Authority);
        }

        // 设置原始请求地址
        webRequest.Headers.Add("X-CfProxy-OrgUrl", httpRequest.FullPath);
    }


    private void SetRequestHeader(HttpWebRequest webRequest, string name, string value)
    {
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
    /// 
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="webRequest"></param>
    /// <returns></returns>
    protected virtual async Task CopyRequestBodyAsync(NHttpRequest httpRequest, HttpWebRequest webRequest)
    {
        if( httpRequest.HasBody && httpRequest.InputStream != null ) {

            using( Stream requestStream = await webRequest.GetRequestStreamAsync() ) {
                httpRequest.InputStream.CopyTo(requestStream);
            }
        }
    }


    private async Task WriteExceptionAsync(NHttpContext httpContext, Exception ex)
    {
        try {
            if( httpContext.Response.HasStarted == false ) {
                //context.Response.ClearHeaders();
                //context.Response.ClearContent();

                // 重写错误结果
                httpContext.Response.StatusCode = 500;
                httpContext.Response.SetHeader("X-HttpProxyHandler-error", "1");
                httpContext.Response.ContentType = ResponseContentType.TextUtf8;
                await httpContext.Response.WriteAllAsync(ex.ToString().GetBytes());
            }
        }
        catch( Exception ex2 ) {
            // 实在是不能发送就只能忽略异常
            Console2.Info($@"HttpProxyHandler.WriteException ERROR: 
-->ex1.Message : {ex.Message}
-->ex2.Message : {ex2.Message}");
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestMethod"></param>
    /// <param name="webResponse"></param>
    /// <param name="httpResponse"></param>
    public virtual void CopyResponse(string requestMethod, HttpWebResponse webResponse, NHttpResponse httpResponse)
    {
        if( webResponse == null )
            throw new ArgumentNullException(nameof(webResponse));
        if( httpResponse == null )
            throw new ArgumentNullException(nameof(httpResponse));


        httpResponse.StatusCode = (int)webResponse.StatusCode;

        // 写响应头
        CopyResponseHeaders(httpResponse, webResponse);


        if( HttpUtils.CanWriteResponseBody(requestMethod, httpResponse.StatusCode) == false )
            return;


        // 获取响应流，这里不考虑GZIP压缩的情况（因为不需要考虑）
        using( Stream responseStream = webResponse.GetResponseStream() ) {

            // 写响应流
            //responseStream.CopyTo(context.Response.OutputStream);
            // 如果是 Thunk 编码，responseStream.CopyTo不能得到正确的结果（上面的代码）。
            // 所以，重新实现了流的复制版本，就是下面的方法。
            CopyResponseBody(responseStream, httpResponse.OutputStream);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpResponse"></param>
    /// <param name="webResponse"></param>
    protected virtual void CopyResponseHeaders(NHttpResponse httpResponse, HttpWebResponse webResponse)
    {
        httpResponse.ClearHeaders();

        // 注意：不能直接使用Content-Type的内容，这里非常坑爹！
        //  例如： Content-Type： text/css
        //  如果直接将结果写入Response.Headers
        //  得到的结果是：Content-Type： text/html，导致响应类型是错误的，
        //  对于包含编码的时候，编码会丢失！

        if( webResponse.ContentType.IsNullOrEmpty() == false ) {
            httpResponse.ContentType = webResponse.ContentType;
        }

        NameValueCollection headers = webResponse.Headers;

        // 复制响应头
        foreach( string name in headers.AllKeys ) {
            if( HttpProxyModule.IgnoreResponseHeaders.Contains(name))
                continue;


            string[] values = headers.GetValues(name);
            SetResponseHeader(httpResponse, name, values);
        }
    }


    private void SetResponseHeader(NHttpResponse httpResponse, string name, string[] values)
    {
        if( values == null )
            return;

        try {
            httpResponse.SetHeaders(name, values, true);
        }
        catch {
            // 防止出现不允许设置的请求头，未来可以增加日志记录
        }
    }

    private void CopyResponseBody(Stream src, Stream dest)
    {
        using( ByteBuffer byteBuffer = new ByteBuffer(1024 * 4) ) {
            byte[] buffer = byteBuffer.Buffer;

            using( BinaryReader reader = new BinaryReader(src) ) {
                while( true ) {
                    int length = reader.Read(buffer, 0, buffer.Length);
                    if( length > 0 )
                        dest.Write(buffer, 0, length);
                    else
                        break;
                }
            }
        }
    }


}

#endif