namespace ClownFish.Web.Utils;

/// <summary>
/// 框架内部使用的扩展方法
/// </summary>
public static class HttpContextUtils
{
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
            httpContext.Response.Headers.Append("x-server-exectime", time.ToString());
        }
        return Task.CompletedTask;
    }



    public static async Task ShowHomePage(HttpContext httpContext)
    {
        string text = $"This is {EnvUtils.GetAppName()}, It's worked! \nServer time: {DateTime.Now.ToTime23String()}";

        await httpContext.Response.WriteAllAsync(text);
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

}
