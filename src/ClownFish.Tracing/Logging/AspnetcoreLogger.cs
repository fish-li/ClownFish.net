namespace ClownFish.Tracing.Logging;

internal static class AspnetcoreLogger
{
    internal static void Init()
    {
        if( NHttpApplication.Instance == null ) {
            DiagnosticListener.AllListeners.Subscribe(new AspnetEventSubscriber());
        }
    }
}


internal class AspnetEventSubscriber : IObserver<DiagnosticListener>
{
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DiagnosticListener listener)
    {
        switch( listener.Name ) {
            case "Microsoft.AspNetCore":
                listener.Subscribe(new AspnetEventObserver());
                break;
        }
    }
}


internal class AspnetEventObserver : IObserver<KeyValuePair<string, object>>
{
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    /*
    key: Microsoft.AspNetCore.Hosting.HttpRequestIn.Start, value: Microsoft.AspNetCore.Http.DefaultHttpContext
    key: Microsoft.AspNetCore.Hosting.BeginRequest, value: { httpContext = Microsoft.AspNetCore.Http.DefaultHttpContext, timestamp = 4625498291286 }
    key: Microsoft.AspNetCore.Routing.EndpointMatched, value: Microsoft.AspNetCore.Http.DefaultHttpContext
    key: Microsoft.AspNetCore.Mvc.BeforeAction, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeActionEventData
    key: Microsoft.AspNetCore.Mvc.BeforeOnResourceExecuting, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeResourceFilterOnResourceExecutingEventData
    key: Microsoft.AspNetCore.Mvc.AfterOnResourceExecuting, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterResourceFilterOnResourceExecutingEventData
    key: Microsoft.AspNetCore.Mvc.BeforeOnActionExecuting, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeActionFilterOnActionExecutingEventData
    key: Microsoft.AspNetCore.Mvc.AfterOnActionExecuting, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterActionFilterOnActionExecutingEventData
    key: Microsoft.AspNetCore.Mvc.BeforeOnActionExecution, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeActionFilterOnActionExecutionEventData
    key: Microsoft.AspNetCore.Mvc.BeforeActionMethod, value: { actionContext = Microsoft.AspNetCore.Mvc.ControllerContext, actionArguments = System.Collections.Generic.Dictionary`2[System.String,System.Object], controller = Xxxx.Web.InternalController }
    key: Microsoft.AspNetCore.Mvc.BeforeControllerActionMethod, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeControllerActionMethodEventData
    key: Microsoft.AspNetCore.Mvc.AfterControllerActionMethod, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterControllerActionMethodEventData
    key: Microsoft.AspNetCore.Mvc.AfterActionMethod, value: { actionContext = Microsoft.AspNetCore.Mvc.ControllerContext, actionArguments = System.Collections.Generic.Dictionary`2[System.String,System.Object], controller = Microsoft.AspNetCore.Mvc.ControllerContext, result = Microsoft.AspNetCore.Mvc.ObjectResult }
    key: Microsoft.AspNetCore.Mvc.AfterOnActionExecution, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterActionFilterOnActionExecutionEventData
    key: Microsoft.AspNetCore.Mvc.BeforeOnActionExecuted, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeActionFilterOnActionExecutedEventData
    key: Microsoft.AspNetCore.Mvc.AfterOnActionExecuted, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterActionFilterOnActionExecutedEventData
    key: Microsoft.AspNetCore.Mvc.BeforeOnResultExecuting, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeResultFilterOnResultExecutingEventData
    key: Microsoft.AspNetCore.Mvc.AfterOnResultExecuting, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterResultFilterOnResultExecutingEventData
    key: Microsoft.AspNetCore.Mvc.BeforeActionResult, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeActionResultEventData
    key: Microsoft.AspNetCore.Mvc.AfterActionResult, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterActionResultEventData
    key: Microsoft.AspNetCore.Mvc.BeforeOnResultExecuted, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeResultFilterOnResultExecutedEventData
    key: Microsoft.AspNetCore.Mvc.AfterOnResultExecuted, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterResultFilterOnResultExecutedEventData
    key: Microsoft.AspNetCore.Mvc.BeforeOnResourceExecuted, value: Microsoft.AspNetCore.Mvc.Diagnostics.BeforeResourceFilterOnResourceExecutedEventData
    key: Microsoft.AspNetCore.Mvc.AfterOnResourceExecuted, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterResourceFilterOnResourceExecutedEventData
    key: Microsoft.AspNetCore.Mvc.AfterAction, value: Microsoft.AspNetCore.Mvc.Diagnostics.AfterActionEventData
    key: Microsoft.AspNetCore.Hosting.EndRequest, value: { httpContext = Microsoft.AspNetCore.Http.DefaultHttpContext, timestamp = 4625499264539 }
    key: Microsoft.AspNetCore.Hosting.HttpRequestIn.Stop, value: Microsoft.AspNetCore.Http.DefaultHttpContext
     */

    public void OnNext(KeyValuePair<string, object> kvp)
    {
        if( kvp.Key == "Microsoft.AspNetCore.Hosting.BeginRequest" ) {
            BeginRequest(kvp.Value);
            return;
        }

        if( kvp.Key == "Microsoft.AspNetCore.Mvc.BeforeControllerActionMethod" ) {
            BeforeControllerActionMethod(kvp.Value);
            return;
        }

        if( kvp.Key == "Microsoft.AspNetCore.Mvc.AfterControllerActionMethod" ) {
            AfterControllerActionMethod(kvp.Value);
            return;
        }

        if( kvp.Key == "Microsoft.AspNetCore.Hosting.UnhandledException" ) {
            UnhandledException(kvp.Value);
            return;
        }

        if( kvp.Key == "Microsoft.AspNetCore.Mvc.AfterOnResultExecuted" ) {
            AfterOnResultExecuted(kvp.Value);
            return;
        }

        // 说明：在EndRequest阶段，HttpRequestStream 已经 Dispose 了，所以提前运行
        if( kvp.Key == "Microsoft.AspNetCore.Mvc.AfterAction" ) {
            EndRequest(kvp.Value);
            return;
        }

        // 下面这个事件仅当没有经过MVC时才会【有效】运行
        if( kvp.Key == "Microsoft.AspNetCore.Hosting.EndRequest" ) {
            EndRequest(kvp.Value);
            return;
        }
    }

    private static readonly OprLogModule s_oprLogModule = new OprLogModule();

    private void BeginRequest(object eventData)
    {
        HttpContext httpContext = eventData.Get<HttpContext>("httpContext");

        NHttpContext httpContextNetCore = new HttpContextNetCore(httpContext);
        HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContextNetCore);

        httpContextNetCore.SetRequestBuffering();

        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull == false ) {
            // 运行到这里表示程序代码有代码，导致OprLogScope泄露，
            // 但是有一点可以肯定：当前线程捕获的OprLogScope对象是无效的，所以这里强制清除它
            ((IDisposable)scope).Dispose();
        }

        s_oprLogModule.BeginRequest(httpContextNetCore);
    }

    

    private void BeforeControllerActionMethod(object eventData)
    {
        HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
        if( pipelineContext == null )
            return;

        ActionContext context = eventData.Get<ActionContext>("ActionContext");
        object controller = eventData.Get<object>("Controller");

        ControllerActionDescriptor actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        HttpContext httpContext = pipelineContext.HttpContext.OriginalHttpContext as HttpContext;
        ActionDescription action = new ActionDescription(controller,
                                                       actionDescriptor.MethodInfo,
                                                       actionDescriptor.ControllerTypeInfo);

        pipelineContext.HttpContext.BeginExecuteTime = DateTime.Now;

        pipelineContext.SetAction(action);

        // 检查框架执行时间是否过长
        CheckFrameworkBeforeExecuteTime(pipelineContext);
    }


    private static readonly int s_beforeActionTotalMilliseconds = LocalSettings.GetInt("Aspnet_BeforeAction_TotalMilliseconds", 20);
    private static readonly int s_afterActionTotalMilliseconds = LocalSettings.GetInt("Aspnet_AfterAction_TotalMilliseconds", 20);

    private void CheckFrameworkBeforeExecuteTime(HttpPipelineContext pipelineContext)
    {
        TimeSpan time = pipelineContext.HttpContext.BeginExecuteTime - pipelineContext.StartTime;
        if( time.TotalMilliseconds >= s_beforeActionTotalMilliseconds && pipelineContext.OprLogScope.IsNull == false ) {
            StepItem step = StepItem.CreateNew(pipelineContext.StartTime);
            step.StepKind = "ext";
            step.StepName = "Framework_BeforeAction";
            step.End(pipelineContext.HttpContext.BeginExecuteTime);
            pipelineContext.OprLogScope.AddStep(step);
        }
    }

    private void CheckFrameworkAfterExecuteTime(HttpPipelineContext pipelineContext)
    {        
        TimeSpan time = pipelineContext.EndTime - pipelineContext.HttpContext.EndExecuteTime;
        if( time.TotalMilliseconds >= s_afterActionTotalMilliseconds && pipelineContext.OprLogScope.IsNull == false ) {
            StepItem step = StepItem.CreateNew(pipelineContext.HttpContext.EndExecuteTime);
            step.StepKind = "ext";
            step.StepName = "Framework_AfterAction";
            step.End(pipelineContext.EndTime);
            pipelineContext.OprLogScope.AddStep(step);
        }
    }

    private void AfterControllerActionMethod(object eventData)
    {
        HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
        if( pipelineContext == null )
            return;

        pipelineContext.HttpContext.EndExecuteTime = DateTime.Now;        
    }

    private void AfterOnResultExecuted(object eventData)
    {
        HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
        if( pipelineContext == null )
            return;

        ResultExecutedContext context = eventData.Get<ResultExecutedContext>("ResultExecutedContext");
        pipelineContext.SetResponseResult(context.Result);
    }

    private void UnhandledException(object eventData)
    {
        HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
        if( pipelineContext == null )
            return;

        try {
            Exception exception = eventData.Get<Exception>("exception");
            pipelineContext.SetException(exception);

            s_oprLogModule.OnError(pipelineContext.HttpContext);

            // 这个事件产生后，就不会再触发EndRequest

            // 检查框架执行时间是否过长
            CheckFrameworkAfterExecuteTime(pipelineContext);
            pipelineContext.End();
            // 写日志
            s_oprLogModule.End2Request(pipelineContext.HttpContext);
        }
        catch( Exception ex ) {
            Console2.Error(ex);
        }

        Console2.Debug("UnhandledException: " + pipelineContext.HttpContext.Request.FullPath);
        (pipelineContext as IDisposable).Dispose();
    }

    private void EndRequest(object eventData)
    {
        HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
        if( pipelineContext == null )
            return;

        try {
            // 检查框架执行时间是否过长
            CheckFrameworkAfterExecuteTime(pipelineContext);
            pipelineContext.End();
            // 写日志
            s_oprLogModule.End2Request(pipelineContext.HttpContext);
        }
        catch( Exception ex ) {
            Console2.Error(ex);
        }

        Console2.Debug("EndRequest: " + pipelineContext.HttpContext.Request.HttpMethod + " " + pipelineContext.HttpContext.Request.FullPath);
        (pipelineContext as IDisposable).Dispose();
    }


    

}
