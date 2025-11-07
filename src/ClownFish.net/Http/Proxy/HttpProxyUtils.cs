#if NETCOREAPP

using System.Net.Http;

namespace ClownFish.Http.Proxy;

/// <summary>
/// 一组可用于HTTP代理转发的通用方法
/// </summary>
public static class HttpProxyUtils
{
    /// <summary>
    /// 创建 HttpClient 实例
    /// </summary>
    /// <param name="requestUri"></param>
    /// <returns></returns>
    public static HttpClient GetHttpClient(Uri requestUri)
    {
        return ProxyHttpClientCache.GetClient(requestUri);
    }


    /// <summary>
    /// 复制【普通的】请求头
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <param name="requestMessage"></param>
    /// <param name="ignoreRequestHeaders"></param>
    public static void CopyRequestHeaders(NHttpRequest httpRequest, HttpRequestMessage requestMessage, HashSet<string> ignoreRequestHeaders)
    {
        // 复制请求头
        foreach( string name in httpRequest.HeaderKeys ) {

            // 过滤不允许直接指定的请求头
            if( ignoreRequestHeaders != null && ignoreRequestHeaders.Contains(name) )
                continue;

            if( HttpUtils.IsWellKnownContentHeader(name) ) {

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


    /// <summary>
    /// 根据NHttpRequest实例创建请求体
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HttpContent CreateRequestBody(NHttpRequest httpRequest)
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
    /// 复制响应头
    /// </summary>
    /// <param name="responseMessage">源</param>
    /// <param name="httpResponse">目标</param>
    /// <param name="ignoreResponseHeaders"></param>
    public static void CopyResponseHeaders(HttpResponseMessage responseMessage, NHttpResponse httpResponse, HashSet<string> ignoreResponseHeaders)
    {
        string contentType = responseMessage.GetContentType();
        if( contentType.IsNullOrEmpty() == false ) {
            httpResponse.ContentType = contentType;
        }

        foreach( KeyValuePair<string, IEnumerable<string>> kv in responseMessage.Headers ) {
            if( ignoreResponseHeaders != null && ignoreResponseHeaders.Contains(kv.Key) )
                continue;

            httpResponse.SetResponseHeaders(kv.Key, kv.Value.ToArray());
        }

        if( responseMessage.Content != null ) {
            foreach( KeyValuePair<string, IEnumerable<string>> kv2 in responseMessage.Content.Headers ) {
                if( ignoreResponseHeaders != null && ignoreResponseHeaders.Contains(kv2.Key) )
                    continue;

                //if( HttpHeaders.Response.ContentType.Is(kv2.Key) )  // HttpProxyModule.IgnoreResponseHeaders 已包含
                //    continue;

                httpResponse.SetResponseHeaders(kv2.Key, kv2.Value.ToArray());
            }
        }
    }

}
#endif
