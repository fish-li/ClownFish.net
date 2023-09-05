namespace ClownFish.Web.Utils;

/// <summary>
/// 框架内部使用的扩展方法
/// </summary>
public static class HttpContextUtils
{
    private static readonly int s_requestBufferSize = LocalSettings.GetUInt("ClownFish_Aspnet_RequestBufferSize", 0);


    /// <summary>
    /// 设置请求体为缓冲模式，可用于多次读取请求体
    /// </summary>
    /// <param name="httpContextNetCore">NHttpContext实例</param>
    /// <param name="checkBodyFunc">检查请求体是否可以被缓冲的委托。强烈建议：【不要指定这个参数】，或者检查【请求体是小于bufferSize的文本数据】</param>
    public static int SetRequestBuffering(this NHttpContext httpContextNetCore, Func<NHttpContext, int, bool> checkBodyFunc = null)
    {
        if( s_requestBufferSize <= 0 )
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

        if( checkBodyFunc.Invoke(httpContextNetCore, s_requestBufferSize) == false )
            return -4;


        // 下面这种方式得到的流对象，在遇到请求转发时，会产生莫名奇妙的BUG（读不到请求体内容）
        //httpContext.Request.EnableBuffering(_requestBufferSize);

        MemoryStream ms = MemoryStreamPool.GetStream("RequestBuffering", s_requestBufferSize);

        httpContext.Request.Body.CopyTo(ms);
        ms.Position = 0;
        httpContext.Request.Body = ms;
        httpContextNetCore.RegisterForDispose(ms);

        return s_requestBufferSize;  // 返回缓冲区长度，表示设置成功
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



    public static void LogExecutTime(HttpContext httpContext)
    {
        httpContext.Items["x_ClownFish_Web_Aspnetcore_startTime"] = DateTime.Now;
        httpContext.Response.OnStarting(ResponseOnStarting, httpContext);
    }

    private static Task ResponseOnStarting(object state)
    {
        HttpContext httpContext = (HttpContext)state;

        // 当遇到转发请求时，"x-server-exectime" 这个头就有可能会存在
        if( httpContext.Response.Headers.ContainsKey("x-server-exectime") )
            return Task.CompletedTask;

        object value = httpContext.Items["x_ClownFish_Web_Aspnetcore_startTime"];
        if( value != null ) {

            TimeSpan time = DateTime.Now - (DateTime)value;
            httpContext.Response.Headers.Add("x-server-exectime", time.ToString());
        }
        return Task.CompletedTask;
    }



    public static async Task ShowHomePage(HttpContext httpContext)
    {
        string text = $"This is {EnvUtils.GetApplicationName()}, It's worked! \nServer time: {DateTime.Now.ToTime23String()}";

        httpContext.Response.ContentType = ResponseContentType.TextUtf8;
        await httpContext.Response.WriteAsync(text);
    }


    /// <summary>
    /// 获取当前请求相关的用户信息，结果可能为NULL
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static IUserInfo GetUserInfo(this NHttpContext httpContext)
    {
        return (httpContext?.User?.Identity as INbIdentity)?.UserInfo;
    }

    /// <summary>
    /// 将用户信息设置到与当前请求关联的OprLog对象上
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="user"></param>
    public static void SetUserInfoToOprLog(this NHttpContext httpContext, IUserInfo user)
    {
        if( user == null || httpContext == null || httpContext.PipelineContext.OprLogScope.IsNull )
            return;

        OprLog log = httpContext.PipelineContext.OprLogScope.OprLog;

        if( log.TenantId == null )
            log.TenantId = user.TenantId;

        if( log.UserId == null )
            log.UserId = user.UserId;

        if( log.UserName == null )
            log.UserName = user.UserName;

        if( log.UserRole == null )
            log.UserRole = user.UserRole;
    }


    public static string GetToken(this NHttpContext httpContext, LoginTicketSource source)
    {
        if( source == LoginTicketSource.Header )
            return httpContext.Request.Header(AuthOptions.HeaderName);

        if( source == LoginTicketSource.Cookie )
            return httpContext.Request.Cookie(AuthOptions.CookieName);

        return null;
    }
}
