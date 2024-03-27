using Microsoft.AspNetCore.Http;

namespace ClownFish.Web.Utils;

/// <summary>
/// 工具类
/// </summary>
public static class RequestBufferingUtils
{
    /// <summary>
    /// 设置请求体为缓冲模式，可用于多次读取请求体
    /// </summary>
    /// <param name="httpContextNetCore">NHttpContext实例</param>
    /// <param name="checkBodyFunc">检查请求体是否可以被缓冲的委托。强烈建议：【不要指定这个参数】，或者检查【请求体是小于bufferSize的文本数据】</param>
    public static int SetRequestBuffering(this NHttpContext httpContextNetCore, Func<NHttpContext, int, bool> checkBodyFunc = null)
    {
        if( LoggingOptions.RequestBodyBufferSize <= 0 )
            return 0;

        if( httpContextNetCore.Request.HasBody == false )
            return -1;

        long bodySize = httpContextNetCore.Request.ContentLength;
        if( bodySize <= 0 )
            return -2;

        HttpContext httpContext = httpContextNetCore.OriginalHttpContext as HttpContext;
        if( httpContext.Request.Body.CanSeek )
            return -3;

        // 判断是否需要启用【请求体多次读取】功能，即：允许多次读取 Request.Body
        // https://stackoverflow.com/questions/57407472/what-is-the-alternate-of-httprequest-enablerewind-in-asp-net-core-3-0
        // 如果需要多次读取 “application/x-www-form-urlencoded” 这类请求，则必须在很早的阶段就设置

        if( checkBodyFunc == null )
            checkBodyFunc = BodyIsSmallText;

        if( checkBodyFunc.Invoke(httpContextNetCore, LoggingOptions.RequestBodyBufferSize) == false )
            return -4;


        // 下面这种方式得到的流对象，在遇到请求转发时，会产生莫名奇妙的BUG（读不到请求体内容）
        //httpContext.Request.EnableBuffering(_requestBufferSize);

        MemoryStream ms = MemoryStreamPool.GetStream("RequestBuffering", LoggingOptions.RequestBodyBufferSize);

        httpContext.Request.Body.CopyTo(ms);
        ms.Position = 0;
        httpContext.Request.Body = ms;
        httpContextNetCore.RegisterForDispose(ms);

        return LoggingOptions.RequestBodyBufferSize;  // 返回缓冲区长度，表示设置成功
    }


    /// <summary>
    /// 判断请求体是否为文本数据，且长度小于bufferSize
    /// </summary>
    /// <param name="httpContextNetCore"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static bool BodyIsSmallText(NHttpContext httpContextNetCore, int bufferSize)
    {
        long len = httpContextNetCore.Request.GetBodyTextLength();
        return len > 0 && len < bufferSize;
    }
}

