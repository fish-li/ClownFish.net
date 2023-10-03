namespace ClownFish.Web.Aspnetcore;

public class FirstModule
{
    private static readonly bool s_debugHttpLine = LocalSettings.GetBool("ClownFish_Aspnet_DebugHttpLine");
    private static readonly bool s_deleteUselessHeaders = LocalSettings.GetBool("ClownFish_Aspnet_DeleteUselessHeaders", 1);
    private static readonly bool s_logExecutTime = LocalSettings.GetBool("ClownFish_Aspnet_LogExecutTime");
    private static readonly bool s_show404Page = LocalSettings.GetBool("ClownFish_Aspnet_Show404Page");

    private readonly RequestDelegate _next;

    public FirstModule(RequestDelegate next)
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

        if( s_debugHttpLine )
            Console2.Debug(httpContextNetCore.Request.HttpMethod + " " + httpContextNetCore.Request.FullUrl);


        // 设置一些上下文及日志作用域
        using( HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContextNetCore) ) {

            // 进入 HTTP 管道的执行过程
            await Execute(pipelineContext, httpContext);
        }
    }


    public virtual async Task Execute(HttpPipelineContext pipelineContext, HttpContext httpContext)
    {
        bool flag = false;  // 一个标记，用于判断当前请求是否已经被httphandler处理
        NHttpApplication app = NHttpApplication.Instance;
        NHttpContext httpContextNetCore = pipelineContext.HttpContext;

        try {
            string origin = httpContextNetCore.Request.Header("Origin");
            if( origin.HasValue() && IsAllowCors(httpContextNetCore, origin) )
                app.EnableCors(httpContextNetCore, origin);

            app.InitResponse(httpContextNetCore);
            app.BeginRequest(httpContextNetCore);

            // 允许 body 多次读取
            SetRequestBuffering(httpContextNetCore);

            flag = await app.ExecuteHttpHandlerAsync(httpContextNetCore);
            if( flag == false ) {                

                app.AuthenticateRequest(httpContextNetCore);
                app.PostAuthenticateRequest(httpContextNetCore);
                app.ResolveRequestCache(httpContextNetCore);

                flag = await app.ExecuteHttpHandlerAsync(httpContextNetCore);
                if( flag == false ) {

                    app.PreFindAction(httpContextNetCore);

                    // 在这个调用中，Action将会被定位，然后进入 MvcLogFilter
                    await _next(httpContext);
                }
            }

            app.UpdateRequestCache(httpContextNetCore);
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


        if( httpContext.Response.StatusCode == 404 && s_show404Page ) {
            await Http404Handler.Instance.ProcessRequestAsync(httpContextNetCore);
        }
    }


    public virtual bool IsAllowCors(NHttpContext httpContext, string origin)
    {
        return origin.HasValue();
    }

    public virtual void SetRequestBuffering(NHttpContext httpContext)
    {
        httpContext.SetRequestBuffering();
    }



}
