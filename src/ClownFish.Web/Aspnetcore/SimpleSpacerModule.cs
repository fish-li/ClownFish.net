#if NET6_0_OR_GREATER
namespace ClownFish.Web.Aspnetcore;

internal sealed class SimpleSpacerModule
{
    private static readonly bool s_debugHttpLine = LocalSettings.GetBool("ClownFish_Aspnet_DebugHttpLine");
    private static readonly bool s_deleteUselessHeaders = LocalSettings.GetBool("ClownFish_Aspnet_DeleteUselessHeaders", 1);
    private static readonly bool s_logExecutTime = LocalSettings.GetBool("ClownFish_Aspnet_LogExecutTime");

    private readonly RequestDelegate _next;

    public SimpleSpacerModule(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if( s_logExecutTime ) {
            HttpContextUtils.LogExecutTime(httpContext);
        }

        if( s_deleteUselessHeaders ) {
            HttpHeaderUtils.DeleteUselessHeaders(httpContext.Request);
        }

        // 将不规范的URL，例如 //aa/bb/cc  强制修改为  /aa/bb/cc
        string currentUrl = httpContext.Request.Path.Value;
        if( currentUrl.StartsWith0("//") ) {
            httpContext.Request.Path = currentUrl.Substring(1);
        }

        NHttpContext httpContextNetCore = new HttpContextNetCore(httpContext);

        // 设置一些上下文及日志作用域
        using( HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContextNetCore) ) {

            NHttpApplication app = NHttpApplication.Instance;

            try {
                app.InitResponse(httpContextNetCore);
                app.AuthenticateRequest(httpContextNetCore);

                HttpContextUtils.SetRequestBuffering(httpContextNetCore);  // 允许 body 多次读取
                app.BeginRequest(httpContextNetCore);

                bool flag = await app.ExecuteHttpHandlerAsync(httpContextNetCore);
                if( flag == false ) {
                    //httpContextNetCore.Response.StatusCode = 404;
                    await _next(httpContext);
                }
            }
            catch( AbortRequestException ) {
                // 提前结束请求
            }
            catch( Exception ex ) {
                pipelineContext.SetException(ex);
                app.OnError(httpContextNetCore);
            }
            finally {
                app.EndRequest(httpContextNetCore);
            }
        }


        if( s_debugHttpLine )
            Console2.Info("HTTP/" + httpContext.Response.StatusCode.ToString() + " " + httpContextNetCore.Request.HttpMethod + " " + httpContextNetCore.Request.FullUrl);
    }

    

}

#endif

