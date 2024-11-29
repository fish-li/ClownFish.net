using ClownFish.WebApi.Controllers;
using ClownFish.WebApi.Routing;

namespace ClownFish.WebApi;

internal static class ActionLocator
{
    public static ActionDescription FindAction(HttpPipelineContext pipelineContext)
    {
        // 检查是不是 OPTIONS 请求
        ActionDescription action = TryCreateOptionAction(pipelineContext);

        if( action == null ) {

            // 查找路由表
            RoutingObject routing = RoutingManager.FindAction(pipelineContext.HttpContext);

            if( routing != null ) {
                // 这里先不创建 Controller 实例
                action = new ActionDescription(routing.ControllerType, routing.MethodInfo);
            }
        }

        if( action == null )
            action = ControllerFactory.CreateHandler(GetDefaultHandler(pipelineContext));

        if( action != null )
            pipelineContext.SetAction(action);

        return action;
    }

    private static IHttpHandler GetDefaultHandler(HttpPipelineContext pipelineContext)
    {
        return StaticFileHandlerFactory.Instance.GetHandler(pipelineContext.HttpContext)
            ?? DirectoryBrowseHandlerFactory.Instance.GetHandler(pipelineContext.HttpContext)
            ?? null;
    }


    private static ActionDescription TryCreateOptionAction(HttpPipelineContext pipelineContext)
    {
        if( pipelineContext.HttpContext.Request.HttpMethod != HttpOptionsAttribute.MethodName )
            return null;

        return ControllerFactory.CreateHandler(OptionsHandler.Instance);
    }


}
