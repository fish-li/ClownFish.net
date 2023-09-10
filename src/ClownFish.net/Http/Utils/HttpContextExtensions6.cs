#if NETCOREAPP
using System.Net.Http;
using ClownFish.Http.Proxy;

namespace ClownFish.Http.Utils;

/// <summary>
/// HttpContext 相关的扩展方法工具类
/// </summary>
public static partial class HttpContextExtensions6
{
    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="webResponse">做为输出的数据对象</param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, HttpWebResponse webResponse)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));
        if( webResponse == null )
            throw new ArgumentNullException(nameof(webResponse));

        HttpResponseMessage responseMessage = webResponse.ToResponseMessage();

        HttpProxyHandler2 handler = new HttpProxyHandler2(null);
        await handler.CopyResponseAsync(responseMessage, httpContext.Response);
    }

    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="responseMessage">做为输出的数据对象</param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, HttpResponseMessage responseMessage)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));

        HttpProxyHandler2 handler = new HttpProxyHandler2(null);
        await handler.CopyResponseAsync(responseMessage, httpContext.Response);
    }
}
#endif
