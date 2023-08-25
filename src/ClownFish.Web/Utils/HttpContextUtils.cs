namespace ClownFish.Web.Utils;

internal static class HttpContextUtils
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
            httpContext.Response.Headers.Add("x-server-exectime", time.ToString());
        }
        return Task.CompletedTask;
    }


}
