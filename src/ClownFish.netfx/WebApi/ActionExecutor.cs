using ClownFish.WebApi.Result;

namespace ClownFish.WebApi;

internal class ActionExecutor
{
    public static readonly ActionExecutor Instance = new ActionExecutor();

    public virtual async Task ExecuteAction(HttpPipelineContext pipelineContext)
    {
        // 允许在框架外部直接指定结果
        if( pipelineContext.RespResult != null )
            return;


        ActionDescription action = pipelineContext.Action;
        if( action == null )
            return;


        // 创建 Controller 实例
        ControllerFactory.Instance.CreateController(pipelineContext);


        // 尝试按 IHttpHandler 的方式执行
        IHttpHandler httpHandler = action.Controller as IHttpHandler;
        if(httpHandler != null ) {
            httpHandler.ProcessRequest(pipelineContext.HttpContext);
            return;
        }


        await ExecuteActionMethod(pipelineContext);
    }



    protected virtual async Task ExecuteActionMethod(HttpPipelineContext pipelineContext)
    {
        ActionDescription action = pipelineContext.Action;

        // 构造方法的调用参数
        object[] parameters = ParameterResolver.GetParameters(action.MethodInfo, pipelineContext.HttpContext.Request);

        object result = null;

        if( action.MethodInfo.IsTaskMethod() ) {
            bool hasReturn = action.MethodInfo.GetTaskMethodResultType() != null;
            if( hasReturn ) {
                Task task = (Task)action.MethodInfo.FastInvoke(action.Controller, parameters);
                await task;

                // 从 Task<T> 中获取返回值
                PropertyInfo property = task.GetType().GetProperty("Result", BindingFlags.Instance | BindingFlags.Public);
                result = property.FastGetValue(task);
            }
            else {
                await (Task)action.MethodInfo.FastInvoke(action.Controller, parameters);
            }
        }
        else {
            if( action.MethodInfo.HasReturn() )
                result = action.MethodInfo.FastInvoke(action.Controller, parameters);
            else
                action.MethodInfo.FastInvoke(action.Controller, parameters);
        }


        pipelineContext.RespResult = ResultConverter.Convert(result);
    }

    public virtual void SendResult(HttpPipelineContext pipelineContext)
    {
        object result = pipelineContext.RespResult;

        // 没有执行结果，直接返回（不产生输出）
        if( result == null )
            return;


        // 转换结果
        IActionResult actionResult = result as IActionResult;
        if( actionResult == null ) {
            // 这里再次调用ResultConverter是有必要的，因为有可能在HttpModuel中重新指定ActionResult
            actionResult = ResultConverter.Convert(result);
        }

        if( actionResult == null )
            return;

        actionResult.Ouput(pipelineContext.HttpContext);
    }

}
