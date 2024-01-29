namespace ClownFish.Log.Logging;

/// <summary>
/// 用于产生操作日志的HTTP模块
/// </summary>
public sealed class OprLogModule : NHttpModule, IEnd2Request
{
    /// <summary>
    /// Order
    /// </summary>
    public override int Order => -99999;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="httpContext"></param>
    public override void BeginRequest(NHttpContext httpContext)
    {
        OprLogScope scope = httpContext.PipelineContext.OprLogScope;

        HttpTraceUtils.SetRootParent(httpContext, scope);
    }



    /// <summary>
    /// OnError
    /// </summary>
    /// <param name="httpContext"></param>
    public override void OnError(NHttpContext httpContext)
    {
        httpContext.PipelineContext.OprLogScope.SetException(httpContext.LastException);
    }

    /// <summary>
    /// End2Request
    /// </summary>
    /// <param name="httpContext"></param>
    public void End2Request(NHttpContext httpContext)
    {
        // EnableLog 属性允许在请求处理过程中修改，所以在这里判断要不要写日志
        if( httpContext.EnableLog == false )
            return;

        // 将 WebSocket 请求视为长任务，因为这类请求最后计算得到的【执行时间】特别长
        // httpContext.WebSockets.IsWebSocketRequest 这个属性在这个项目中用不了，原因是当前项目不是 asp.net core 的项目
        // 其次，这个属性依赖于 UseWebSockets() 扩展方法的调用时机，很坑，所以干脆就不使用了，
        // https://github.com/dotnet/AspNetCore.Docs/issues/21701

        if( httpContext.Response.StatusCode == 101 ) {
            httpContext.PipelineContext.SetAsLongTask();
        }

        httpContext.LogFxEvent(new NameTime("OprLogModule.EndRequest begin"));


        OprLogScope scope = httpContext.PipelineContext.OprLogScope;
        if( scope.IsNull == false ) {

            // 填充 HTTP 相关信息
            OprLog log = scope.OprLog;
            log.SetHttpRequest(httpContext);
            log.SetHttpData(httpContext);
            log.SetResponseData(httpContext);
            log.TryGetBizInfo(httpContext);
            

            if( httpContext.IsTransfer ) {
                log.OprKind = OprKinds.Proxy;
            }
            else {
                // 有一种情况：异常被Action的代理拦截了，外层框架没有捕获到异常
                scope.CheckError500();
            }

            // #####  生成 InvokeLog, OprLog ##### 
            scope.SaveOprLog(httpContext.PipelineContext);
        }

        httpContext.LogFxEvent(new NameTime("OprLogModule.EndRequest end"));
    }


    




}
