#if NETCOREAPP

using System.Net.Http;
using ClownFish.WebClient.V2;

namespace ClownFish.Http.Proxy;

/// <summary>
/// 实现HTTP代理的HttpHandler
/// </summary>
public class HttpProxyHandler2 : IAsyncNHttpHandler
{
    /// <summary>
    /// 转发过程中产生的 HttpRequestMessage 实例
    /// </summary>
    public HttpRequestMessage Request { get; private set; }
    /// <summary>
    /// 转发过程中产生的 HttpResponseMessage 实例
    /// </summary>
    public HttpResponseMessage Response { get; private set; }

    /// <summary>
    /// 转发时，是否需要修改Referer头（指向目标转发地址）
    /// </summary>
    public bool AdjustRefererHeader { get; set; }

    private readonly string _destUr;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="destUrl"></param>
    public HttpProxyHandler2(string destUrl)
    {
        _destUr = destUrl;
    }

    /// <summary>
    /// 执行转发操作
    /// </summary>
    /// <returns></returns>
    public virtual async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        try {
            Uri destUri = new Uri(_destUr);

            // 构造请求消息，包含 headers, body
            HttpRequestMessage requestMessage = CreateRequest(httpContext.Request, destUri);
            this.Request = requestMessage;

            // 发送HTTP请求
            using( HttpResponseMessage responseMessage = await SendRequest(requestMessage) ) {
                this.Response = responseMessage;

                // 复制: 响应头，响应头，响应体
                await CopyResponseAsync(responseMessage, httpContext.Response);
            }
        }
        catch( Exception ex ) {

            if( ex is OutOfMemoryException )
                ClownFishCounters.Status.OomError.Increment();

            await WriteExceptionAsync(httpContext, ex);
        }
    }


    /// <summary>
    /// 发送请求并获取响应
    /// </summary>
    /// <param name="requestMessage"></param>
    /// <returns></returns>
    protected virtual async Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage)
    {
        // 获取HttpClient实例，相同站点的请求共用一个实例
        HttpClient client = GetHttpClient(requestMessage);

        return await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
    }

    /// <summary>
    /// 创建 HttpClient 实例
    /// </summary>
    /// <param name="requestMessage"></param>
    /// <returns></returns>
    protected virtual HttpClient GetHttpClient(HttpRequestMessage requestMessage)
    {
        return ProxyHttpClientCache.GetClient(requestMessage.RequestUri);
    }


    /// <summary>
    /// 创建 HttpRequestMessage 实例
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="destUri"></param>
    /// <returns></returns>
    internal protected virtual HttpRequestMessage CreateRequest(NHttpRequest httpRequest, Uri destUri)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpRequest.HttpMethod), destUri);
        requestMessage.Headers.TransferEncodingChunked = false;
        requestMessage.Version = HttpVersion.Version11;

        // 构造请求体内容
        requestMessage.Content = CreateRequestBody(httpRequest);

        CopyRequestHeaders(httpRequest, requestMessage);

        return requestMessage;
    }

    /// <summary>
    /// 复制【所有的】请求头
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="requestMessage"></param>
    protected virtual void CopyRequestHeaders(NHttpRequest httpRequest, HttpRequestMessage requestMessage)
    {
        //if( string.Equals(httpRequest.Header("Connection"), "keep-alive", StringComparison.OrdinalIgnoreCase) )
        //    requestMessage.SetKeepAlive(true);

        CopyRequestHeadersStatic(httpRequest, requestMessage);

        //SetOriginRequestHeader(httpRequest, requestMessage);

        if( this.AdjustRefererHeader )
            SetRefererRequestHeader(httpRequest, requestMessage);

        AddProxyRequestHeaders(httpRequest, requestMessage);
    }


    /// <summary>
    /// 复制【普通的】请求头
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="requestMessage"></param>
    public static void CopyRequestHeadersStatic(NHttpRequest httpRequest, HttpRequestMessage requestMessage)
    {
        // 复制请求头
        foreach( string name in httpRequest.HeaderKeys ) {

            // 过滤不允许直接指定的请求头
            if( HttpProxyModule.IgnoreRequestHeaders.Contains(name) )
                continue;

            if( HttpObjectUtils.IsWellKnownContentHeader(name) ) {

                string[] values = httpRequest.GetHeaders(name);
                foreach( string value in values ) {
                    requestMessage.Content.Headers.TryAddWithoutValidation(name, value);
                }
            }
            else {
                string[] values = httpRequest.GetHeaders(name);
                foreach( string value in values ) {
                    requestMessage.Headers.TryAddWithoutValidation(name, value);
                }
            }
        }
    }

    private string _destRoot;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetDestUrlRoot()
    {
        if( _destRoot == null )
            _destRoot = Urls.GetWebSiteRoot(_destUr);
        return _destRoot;
    }

    ///// <summary>
    ///// 设置 Origin 请求头
    ///// </summary>
    ///// <param name="httpRequest"></param>
    ///// <param name="requestMessage"></param>
    //protected void SetOriginRequestHeader(NHttpRequest httpRequest, HttpRequestMessage requestMessage)
    //{
    //    // Origin 不包含路径部分，但是其值可以为 null 值。
    //    // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Origin
    //    string origin = httpRequest.Header("Origin");

    //    if( origin.IsNullOrEmpty() )
    //        return;

    //    if( origin == "null" ) {  // 可参考：https://stackoverflow.com/a/42242802/
    //        requestMessage.Headers.TryAddWithoutValidation("Origin", origin);
    //        return;
    //    }

    //    if( httpRequest.FullPath.StartsWith1(origin) ) {  // 当前站点，修改为目标站点
    //        requestMessage.Headers.Remove("Origin");
    //        requestMessage.Headers.TryAddWithoutValidation("Origin", GetDestUrlRoot());
    //    }
    //    else {
    //        //requestMessage.Headers.TryAddWithoutValidation("Origin", origin);
    //        return;
    //    }
    //}

    /// <summary>
    /// 设置 Referer 请求头
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="requestMessage"></param>
    protected void SetRefererRequestHeader(NHttpRequest httpRequest, HttpRequestMessage requestMessage)
    {
        // Referer 请求头的调整逻辑：将 http://srchost:xx/aa/bb/cc.page 修改为：http://desthost:xx/aa/bb/cc.page
        string referer = httpRequest.Header("Referer");

        if( referer.IsNullOrEmpty() )
            return;

        string refererRoot = Urls.GetWebSiteRoot(referer);

        // Referer 头【不可能】是一个 相对地址
        // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Referer
        if( refererRoot.IsNullOrEmpty() )
            return;


        // 只有当 Referer 的地址是当前站点才做修改，否则（跨域）请求就不需要修改这个头
        if( httpRequest.FullPath.StartsWith1(refererRoot) ) {

            string referer2 = GetDestUrlRoot() + referer.Substring(refererRoot.Length);
            requestMessage.Headers.Remove("Referer");
            requestMessage.Headers.TryAddWithoutValidation("Referer", referer2);
        }
        else {   // 跨域引用
            //requestMessage.Headers.TryAddWithoutValidation("Referer", referer);
            return;
        }
    }


    /// <summary>
    /// 补充【代理相关】的请求头
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="requestMessage"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void AddProxyRequestHeaders(NHttpRequest httpRequest, HttpRequestMessage requestMessage)
    {
        // 设置2个代理相关的请求头
        if( httpRequest.HeaderKeys.Contains("X-Forwarded-Proto", StringComparer.OrdinalIgnoreCase) == false ) {
            requestMessage.Headers.TryAddWithoutValidation("X-Forwarded-Proto", httpRequest.RequestUri.Scheme);
        }
        if( httpRequest.HeaderKeys.Contains("X-Forwarded-Host", StringComparer.OrdinalIgnoreCase) == false ) {
            requestMessage.Headers.TryAddWithoutValidation("X-Forwarded-Host", httpRequest.RequestUri.Authority);
        }

        // 设置原始请求地址
        requestMessage.Headers.TryAddWithoutValidation("X-CfProxy-OrgUrl", httpRequest.FullPath);
    }

    /// <summary>
    /// Create Request HttpContent
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <returns></returns>
    protected virtual HttpContent CreateRequestBody(NHttpRequest httpRequest)
    {
        return CreateRequestBodyStatic(httpRequest);
    }

    /// <summary>
    /// 根据NHttpRequest实例创建请求体
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static HttpContent CreateRequestBodyStatic(NHttpRequest httpRequest)
    {
        Stream srcStream = httpRequest.InputStream;

        if( httpRequest.HasBody && srcStream != null && srcStream.CanRead ) {

            if( srcStream.CanSeek )
                srcStream.Position = 0;

            // 在启用Request.EnableBuffering时，这里会导致发出去的请求体为空~~  
            // 补充说明：问题已解决，将EnableBuffering的调用延后。可参考 SpacerModule.SetRequestBuffering 方法的调用时机
            StreamContent result = new StreamContent(srcStream);

            return result;
        }
        else {
            return new ByteArrayContent(Array.Empty<byte>());
        }
    }


    /// <summary>
    /// 异常处理
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    protected virtual async Task WriteExceptionAsync(NHttpContext httpContext, Exception ex)
    {
        try {
            if( httpContext.Response.HasStarted == false ) {
                httpContext.Response.ClearHeaders();
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
            Console2.Warnning($@"HttpProxyHandler2.WriteException ERROR: 
-->ex1.Message : {ex.Message}
-->ex2.Message : {ex2.Message}");
        }
    }




    /// <summary>
    /// 将HttpResponseMessage实例的所有内容做为输出，写入到NHttpResponse实例
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="httpResponse"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual async Task CopyResponseAsync(HttpResponseMessage responseMessage, NHttpResponse httpResponse)
    {
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));
        if( httpResponse == null )
            throw new ArgumentNullException(nameof(httpResponse));


        httpResponse.StatusCode = (int)responseMessage.StatusCode;

        CopyResponseHeaders(responseMessage, httpResponse);

        // response.Content != null 对于 204 这种响应来说没有用，仍然会引发异常，所以需要增加下面的判断
        string requestMethod = httpResponse.HttpContext.Request.HttpMethod;
        if( HttpUtils.CanWriteResponseBody(requestMethod, httpResponse.StatusCode) == false )
            return;

        // 异常样例
        //System.InvalidOperationException: Writing to the response body is invalid for responses with status code 204.
        //   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.ThrowWritingToResponseBodyNotSupported()
        //   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.FirstWriteAsyncInternal(ReadOnlyMemory`1 data, CancellationToken cancellationToken)
        //   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.FirstWriteAsync(ReadOnlyMemory`1 data, CancellationToken cancellationToken)
        //   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.WritePipeAsync(ReadOnlyMemory`1 data, CancellationToken cancellationToken)
        //   at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpResponseStream.WriteAsync(ReadOnlyMemory`1 source, CancellationToken cancellationToken)
        //   at System.Net.Http.HttpContent.InternalCopyToAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
        //   at System.Net.Http.HttpContent.CopyToAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
        //   at ClownFish.Http.Proxy.HttpProxyHandler2.CopyResponse(HttpResponseMessage response, NHttpResponse httpResponse)


        await CopyResponseBodyAsync(responseMessage, httpResponse);
    }

    /// <summary>
    /// 复制响应头
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="httpResponse"></param>
    protected virtual void CopyResponseHeaders(HttpResponseMessage responseMessage, NHttpResponse httpResponse)
    {
        CopyResponseHeadersStatic(responseMessage, httpResponse);

        //SetLocationResponseHeader(responseMessage, httpResponse);
    }

    ///// <summary>
    ///// 设置 Location 响应头
    ///// </summary>
    ///// <param name="responseMessage"></param>
    ///// <param name="httpResponse"></param>
    //protected void SetLocationResponseHeader(HttpResponseMessage responseMessage, NHttpResponse httpResponse)
    //{
    //    // Location 这个头也比较特殊，它支持 相对地址 和 绝对地址 ，
    //    // 如果是【绝对地址】，并且 协议+域名 和当前请求一致，那么它也需要Referer的类似处理
    //    // 参考：https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Location

    //    string location = responseMessage.GetHeader("Location");

    //    if( location.IsNullOrEmpty() )
    //        return;

    //    if( this.ChangeSomeUrlHeader == false ) {  // 无需修改响应头
    //        httpResponse.SetResponseHeader("Location", location);
    //        return;
    //    }

    //    // 相对地址，可以直接复制
    //    if( location[0] == '/' ) {
    //        httpResponse.SetResponseHeader("Location", location);    // ##### 正常站内跳转
    //        return;
    //    }

    //    // 下面是绝对地址

    //    string locationRoot = Urls.GetWebSiteRoot(location);
    //    if( locationRoot.HasValue() ) {
    //        if( _destUr.StartsWith1(locationRoot) ) {  // SB框架或者代码，站内也使用了绝对地址

    //            string location2 = location.Substring(locationRoot.Length);    // 强制修改为 相对地址
    //            httpResponse.SetResponseHeader("Location", location2);

    //        }
    //        else {   // 跨域名跳转，，可以直接复制
    //            httpResponse.SetResponseHeader("Location", location);   // ##### 跨域名跳转
    //        }
    //    }
    //    else {   // 无效的地址？？
    //        httpResponse.SetResponseHeader("Location", location);
    //    }
    //}


    /// <summary>
    /// 复制响应头
    /// </summary>
    /// <param name="responseMessage">源</param>
    /// <param name="httpResponse">目标</param>
    public static void CopyResponseHeadersStatic(HttpResponseMessage responseMessage, NHttpResponse httpResponse)
    {
        string contentType = responseMessage.GetContentType();
        if( contentType.IsNullOrEmpty() == false ) {
            httpResponse.ContentType = contentType;
        }

        foreach( KeyValuePair<string, IEnumerable<string>> kv in responseMessage.Headers ) {
            if( HttpProxyModule.IgnoreResponseHeaders.Contains(kv.Key) )
                continue;

            httpResponse.SetResponseHeaders(kv.Key, kv.Value.ToArray());
        }

        if( responseMessage.Content != null ) {
            foreach( KeyValuePair<string, IEnumerable<string>> kv2 in responseMessage.Content.Headers ) {
                if( HttpProxyModule.IgnoreResponseHeaders.Contains(kv2.Key) )
                    continue;

                //if( HttpHeaders.Response.ContentType.Is(kv2.Key) )  // HttpProxyModule.IgnoreResponseHeaders 已包含
                //    continue;

                httpResponse.SetResponseHeaders(kv2.Key, kv2.Value.ToArray());
            }
        }
    }

    /// <summary>
    /// 复制响应体
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="httpResponse"></param>
    /// <returns></returns>
    protected virtual async Task CopyResponseBodyAsync(HttpResponseMessage responseMessage, NHttpResponse httpResponse)
    {
        if( responseMessage.Content != null ) {
            await responseMessage.Content.CopyToAsync(httpResponse.OutputStream);
        }
    }

}

#endif
