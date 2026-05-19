namespace ClownFish.Web.Aspnetcore;

/// <summary>
/// 用于NativeAOT模式下，提供一个精简的WebApi开发模式，并支持经典ASP.NET的管道事件。(目前MVC模式不支持AOT)
/// </summary>
internal sealed class MiniWebApiModule : FirstModule
{
    public MiniWebApiModule(RequestDelegate next) : base(next)
    {
    }

    public override async Task Execute(HttpPipelineContext pipelineContext, HttpContext httpContext)
    {
        bool isHandled = false;  // 标记当前请求是否已经被httphandler处理
        NHttpApplication app = NHttpApplication.Instance;
        NHttpContext httpContextNetCore = pipelineContext.HttpContext;

        ClownFishCounters.Concurrents.HttpConcurrent.Increment();
        ClownFishCounters.ExecuteTimes.HttpCount.Increment();

        try {
            ValidateMaxRequestBodySize(httpContextNetCore);

            string origin = httpContextNetCore.Request.Header("Origin");
            if( origin.HasValue() && IsAllowCors(httpContextNetCore, origin) )
                app.EnableCors(httpContextNetCore, origin);

            app.InitResponse(httpContextNetCore);
            app.BeginRequest(httpContextNetCore);

            // 允许 body 多次读取
            TrySetRequestBodyBuffering(httpContextNetCore);

            isHandled = await app.TryExecuteHttpHandlerAsync(httpContextNetCore);
            if( isHandled == false ) {

                app.AuthenticateRequest(httpContextNetCore);
                app.PostAuthenticateRequest(httpContextNetCore);

                app.ResolveRequestCache(httpContextNetCore);

                app.MapRequestHandler(httpContextNetCore);
                isHandled = await app.TryExecuteHttpHandlerAsync(httpContextNetCore);

                if( isHandled == false ) {
                    // 按 WebApi 的方式继续处理请求
                    app.PreFindAction(httpContextNetCore);
                    WebApiActionInfo action = MiniWebApiUtil.FindAction(httpContextNetCore);

                    if( action != null ) {
                        MiniWebApiUtil.SetHttpContextAction(httpContextNetCore, action);

                        app.PostFindAction(httpContextNetCore);
                        app.AuthorizeRequest(httpContextNetCore);

                        app.PreRequestExecute(httpContextNetCore);
                        await MiniWebApiUtil.ExecuteActionAsync(httpContextNetCore);
                        app.PostRequestExecute(httpContextNetCore);
                    }
                    else {
                        // 进入其它的 RequestDelegate
                        await _next(httpContext);
                    }
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

}
