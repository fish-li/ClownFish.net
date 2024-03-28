namespace ClownFish.Web.Aspnetcore;

public class FirstModule
{
    private readonly RequestDelegate _next;

    public FirstModule(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if( ClownFishWebOptions.LogExecutTime ) {
            HttpContextUtils.LogExecutTime(httpContext);
        }

        if( ClownFishWebOptions.DeleteUselessHeaders ) {
            HttpHeaderUtils.DeleteUselessHeaders(httpContext.Request);
        }

        // 将不规范的URL，例如 //aa/bb/cc  强制修改为  /aa/bb/cc
        string currentUrl = httpContext.Request.Path.Value;
        if( currentUrl.StartsWith0("//") ) {
            httpContext.Request.Path = currentUrl.Substring(1);
        }

        NHttpContext httpContextNetCore = new HttpContextNetCore(httpContext);

        if( ClownFishWebOptions.DebugHttpLine )
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

        ClownFishCounters.Concurrents.HttpConcurrent.Increment();
        ClownFishCounters.ExecuteTimes.HttpCount.Increment();

        try {
            CheckMaxRequestBodySize(httpContextNetCore);

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

            ClownFishCounters.Concurrents.HttpConcurrent.Decrement();

            if( httpContextNetCore.IsTransfer == false && StatusCodeUtils.IsServerError(httpContext.Response.StatusCode) )
                ClownFishCounters.ExecuteTimes.HttpError.Increment();
        }


        if( httpContext.Response.StatusCode == 404 && ClownFishWebOptions.Show404Page && httpContext.Response.HasStarted == false ) {
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

    private void CheckMaxRequestBodySize(NHttpContext httpContext)
    {
        if( httpContext.Request.ContentLength > ClownFishWebOptions.MaxRequestBodySize ) {

            // 上传文件可以不检查
            SerializeFormat format = ContenTypeUtils.GetFormat(httpContext.Request.ContentType);
            if( format == SerializeFormat.Binary || format == SerializeFormat.Multipart )
                return;

            httpContext.HttpReply(413, $"Request body too large.");

            throw new AbortRequestException();
        }
    }

}
