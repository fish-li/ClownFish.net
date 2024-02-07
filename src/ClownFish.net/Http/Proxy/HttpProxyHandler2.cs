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
    /// 
    /// </summary>
    public HttpRequestMessage Request { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public HttpResponseMessage Response { get; private set; }

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
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        try {
            Uri requestUri = new Uri(_destUr);

            // 构造请求消息，包含 headers, body
            HttpRequestMessage requestMessage = CreateRequest(httpContext.Request, requestUri);
            this.Request = requestMessage;

            // 获取HttpClient实例，相同站点的请求共用一个实例
            HttpClient client = MsHttpClientCache2.GetCachedOrCreate(requestUri);


            // 发送HTTP请求
            using( HttpResponseMessage responseMessage = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead) ) {
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
    /// 
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="requestUri"></param>
    /// <returns></returns>
    protected virtual HttpRequestMessage CreateRequest(NHttpRequest httpRequest, Uri requestUri)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpRequest.HttpMethod), requestUri);
        requestMessage.Headers.TransferEncodingChunked = false;
        requestMessage.Version = HttpVersion.Version11;

        // 构造请求体内容
        requestMessage.Content = CreateRequestBody(httpRequest);

        CopyRequestHeaders(httpRequest, requestMessage);

        return requestMessage;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="requestMessage"></param>
    protected virtual void CopyRequestHeaders(NHttpRequest httpRequest, HttpRequestMessage requestMessage)
    {
        // 复制请求头
        foreach( string name in httpRequest.HeaderKeys ) {

            // 过滤不允许直接指定的请求头
            if( HttpProxyModule.IgnoreRequestHeaders.Contains(name))
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

        if( string.Equals(httpRequest.Header("Connection"), "keep-alive", StringComparison.OrdinalIgnoreCase) )
            requestMessage.SetKeepAlive(true);

        string referer = httpRequest.Header("Referer");
        if( string.IsNullOrEmpty(referer) == false ) {
            if( referer.IndexOf("://", StringComparison.Ordinal) > 0 ) {
                string refererRoot = Urls.GetWebSiteRoot(referer);
                string requestRoot = Urls.GetWebSiteRoot(_destUr);

                string referer2 = requestRoot + referer.Substring(refererRoot.Length);
                requestMessage.Headers.TryAddWithoutValidation("Referer", referer2);
            }
        }

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


    private HttpContent CreateRequestBody(NHttpRequest httpRequest)
    {
        Stream srcStream = httpRequest.InputStream;

        if( httpRequest.HasBody && srcStream != null && srcStream.CanRead ) {

            if( srcStream.CanSeek )
                srcStream.Position = 0;

            // 这种做法虽然结果是正确的，但不是最优方法，最好的做法是直接传递流对象而不去读它！
            //return new StreamContent(srcStream.ToMemoryStream()); 

            // 在启用Request.EnableBuffering时，这里会导致发出去的请求体为空~~  
            // 补充说明：问题已解决，将EnableBuffering的调用延后。可参考 SpacerModule.SetRequestBuffering 方法的调用时机
            StreamContent result = new StreamContent(srcStream);

            // 由于 httpRequest.InputStream 的长度未知，所以导致最终发送的请求会采用 Transfer-Encoding: chunked
            // 但是对于输入请求来说，长度是明确的！
            // 所以这里的处理方式是：如果当前请求的【长度明确】，请强行指定 ContentLength
            if( httpRequest.ContentLength > 0 ) {
                result.Headers.ContentLength = httpRequest.ContentLength;
            }
            return result;
        }
        else {
            return new ByteArrayContent(Array.Empty<byte>());
        }
    }


    private async Task WriteExceptionAsync(NHttpContext httpContext, Exception ex)
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
            Console2.Info($@"HttpProxyHandler2.WriteException ERROR: 
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

    /// <summary>
    /// 复制响应头
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="httpResponse"></param>
    protected virtual void CopyResponseHeaders(HttpResponseMessage responseMessage, NHttpResponse httpResponse)
    {
        ResponseUtils.CopyResponseHeaders(responseMessage, httpResponse);
    }


    


}

#endif
