using ClownFish.WebApi.Result;

namespace ClownFish.WebApi;

internal static class ActionExecutor
{
    public static async Task Execute(HttpPipelineContext pipelineContext)
    {
        // 允许在框架外部直接指定结果
        if( pipelineContext.RespResult != null )
            return;

        IWebApiActionInfo action = pipelineContext.Action;
        if( action == null )
            return;


        // 创建 Controller 实例
        ControllerFactory.Instance.CreateController(pipelineContext);

        if( action.Controller is IAsyncNHttpHandler httpHandler2 ) {
            await httpHandler2.ProcessRequestAsync(pipelineContext.HttpContext);
            return;
        }

        await ExecuteActionMethod(pipelineContext);
    }



    private static async Task ExecuteActionMethod(HttpPipelineContext pipelineContext)
    {
        IWebApiActionInfo action = pipelineContext.Action;

        // 构造方法的调用参数
        object[] parameters = ActionParameterResolver.GetParameters(action.MethodInfo, pipelineContext.HttpContext.Request);

        object result = await ReflectionUtils.CallMethod(action.Controller, action.MethodInfo, parameters);

        pipelineContext.RespResult = ActionResultConverter.Convert(result);
    }

    public static async Task SendResultAsync(HttpPipelineContext pipelineContext)
    {
        object result = pipelineContext.RespResult;

        // 没有执行结果，直接返回（不产生输出）
        if( result == null )
            return;


        // 转换结果
        IActionResult actionResult = result as IActionResult;
        if( actionResult == null ) {
            // 这里再次调用ResultConverter是有必要的，因为有可能在HttpModuel中重新指定ActionResult
            actionResult = ActionResultConverter.Convert(result);
        }

        if( actionResult == null )
            return;

        await actionResult.OuputAsync(pipelineContext.HttpContext);
    }

}
