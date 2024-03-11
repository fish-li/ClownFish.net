namespace ClownFish.Http.Proxy;

/// <summary>
/// HTTP请求转发模块基类
/// </summary>
public abstract class HttpProxyModule : NHttpModule
{
    /// <summary>
    /// 不需要处理的请求头
    /// </summary>
    public static readonly HashSet<string> IgnoreRequestHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
        "Connection", "Referer", "Origin",    // 这3个头会有特殊的处理方式，所以在复制时直接忽略。
        "Content-Length",                     // 这个头基本上没用，发送请求时会自动计算请求体长度
        "Host"                                // 这个头也不需要，如果保留反而会出现和完整URL不匹配的情况，所以忽略它会更合适。
    };

    /// <summary>
    /// 不需要复制的响应头
    /// </summary>
    public static readonly HashSet<string> IgnoreResponseHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {        
        "Server", "X-Powered-By", "x-tracesteps",

        // 下面这个响应头只能通过“属性”的方式指定，所以在“复制”时必须排除
        "Content-Type",

        // 下面这个头也比较特殊，通常的用法是：Transfer-Encoding: chunked
        // 在 asp.net 中，如果响应体在发送前没有指定 Content-Length，会自动添加 Transfer-Encoding: chunked
        // 如果强行指定反而还可能会出现错误
        "Transfer-Encoding",
    };


    /// <summary>
    /// BeginRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public override void BeginRequest(NHttpContext httpContext)
    {
        if( httpContext.PipelineContext.Action != null )
            return;

        TrySetProxyHandler(httpContext);
    }


    /// <summary>
    /// 尝试根据当前请求的所有信息，设置ProxyHandler
    /// </summary>
    /// <param name="httpContext"></param>
    private void TrySetProxyHandler(NHttpContext httpContext)
    {
        NHttpRequest request = httpContext.Request;

        // 尝试从配置规则中获取目标地址
        string destUrl = GetDestUrl(request);

        if( string.IsNullOrEmpty(destUrl) == false ) {

            IAsyncNHttpHandler handler = CreateProxyHandler(httpContext, destUrl);
            if( handler != null ) {

                // 将当前主请求标记为【转发请求】
                httpContext.IsTransfer = true;

                // 转发的请求都不监控执行耗时时间
                //httpContext.PipelineContext.SetAsLongTask();

                if( EnvUtils.IsProdEnv == false ) {
                    httpContext.Response.SetHeader("X-Proxy-DestUrl", destUrl); // 用于调试诊断
                }

                // 将当前请求交给某个 转发的HttpHandler来处理
                httpContext.PipelineContext.SetHttpHandler(handler);

                PreExecuteHandler(httpContext, handler);
            }
        }
    }

    /// <summary>
    /// 获取要转发的目标地址
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected abstract string GetDestUrl(NHttpRequest request);


    /// <summary>
    /// CreateProxyHandler
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="destUrl"></param>
    /// <returns></returns>
    protected virtual IAsyncNHttpHandler CreateProxyHandler(NHttpContext httpContext, string destUrl)
    {
#if NETFRAMEWORK
        return new HttpProxyHandler(destUrl);
#else
        return new HttpProxyHandler2(destUrl);
#endif
    }

    /// <summary>
    /// PreExecuteHandler
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="handler"></param>
    protected virtual void PreExecuteHandler(NHttpContext httpContext, IAsyncNHttpHandler handler)
    {

    }


}
