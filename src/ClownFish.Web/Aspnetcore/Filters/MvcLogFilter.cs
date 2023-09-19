using Microsoft.AspNetCore.Mvc.Filters;

namespace ClownFish.Web.Aspnetcore.Filters;

public sealed class MvcLogFilter : IAsyncActionFilter
{
    private static readonly int s_frameworkBeforePerformanceThresholdMs = LocalSettings.GetInt("MVC_FrameworkBefore_PerformanceThresholdMs", 10);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        HttpPipelineContext pipelineContext = HttpPipelineContext.Get2();
        NHttpContext httpContext = pipelineContext.HttpContext;

        Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor actionDescriptor
                = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;

        // 补充 Controller/Action 信息
        ActionDescription action = new ActionDescription(context.Controller,
                                                        actionDescriptor.MethodInfo,
                                                        actionDescriptor.ControllerTypeInfo);

        // 检查当前Action是否与登录有关，如果是，则做个标记，避免日志时记录敏感信息
        bool isLogin = LoginActionAttribute.CurrentIsLogin(action);
        pipelineContext.SetAction(action, isLogin);

        // 在Oprlog中记录当前用户信息
        IUserInfo user = httpContext.GetUserInfo();
        httpContext.SetUserInfoToOprLog(user);


        bool allowed = OnlyTestEnvAttribute.CurrentIsAllow(action);
        if( allowed == false ) {
            context.Result = new NotFoundResult();
            return;
        }


        // 登录请求一定不允许记录请求体，不管有没有 [LogRequestBody] 标记！
        if( isLogin ) {
            httpContext.Request.LogRequestBody = false;
        }
        else {
            // 非登录请求，并且【明确】要求记录请体
            if( action.GetActionAttribute<LogRequestBodyAttribute>() != null ) {
                httpContext.Request.LogRequestBody = true;
            }
        }


        NHttpApplication app = NHttpApplication.Instance;
        app.PostFindAction(httpContext);
        app.AuthorizeRequest(httpContext);

        ControllerInit(httpContext);
        app.PreRequestExecute(httpContext);

        if( pipelineContext.OprLogScope.IsNull == false ) {
            WriteLog(pipelineContext);
        }

        httpContext.BeginExecuteTime = DateTime.Now;
        httpContext.LogFxEvent(new NameTime("UserCode begin", httpContext.BeginExecuteTime));
        await next();
        httpContext.EndExecuteTime = DateTime.Now;
        httpContext.LogFxEvent(new NameTime("UserCode end", httpContext.EndExecuteTime));

        pipelineContext.ActionResult = context.Result;

        app.PostRequestExecute(httpContext);
    }

    private void ControllerInit(NHttpContext httpContext)
    {
        ActionDescription action = httpContext.PipelineContext.Action;

        IControllerInit controller = action.Controller as IControllerInit;
        if( controller != null ) {
            controller.Init(httpContext);
        }

        IDisposable disposable = action.Controller as IDisposable;
        if( disposable != null ) {
            httpContext.RegisterForDispose(disposable);
        }
    }


    private void WriteLog(HttpPipelineContext pipelineContext)
    {
        DateTime endTime = DateTime.Now;
        TimeSpan time = endTime - pipelineContext.StartTime;

        // 如果框架部分的执行时间较短就不记录到日志中
        if( time.TotalMilliseconds < s_frameworkBeforePerformanceThresholdMs )
            return;


        StepItem step = StepItem.CreateNew(pipelineContext.StartTime);

        step.StepKind = "framework";
        step.StepName = "Framework_Before";
        step.End(endTime);

        pipelineContext.OprLogScope.AddStep(step);
    }
}
