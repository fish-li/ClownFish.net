using ClownFish.WebApi.Controllers;
using ClownFish.WebApi.Routing;

namespace ClownFish.WebApi;

internal class ActionLocator
{
    public static readonly ActionLocator Instance = new ActionLocator();

    public virtual void FindAction(HttpPipelineContext pipelineContext)
    {
        // 允许在框架外部直接指定结果
        if( pipelineContext.Action != null )
            return;


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


        pipelineContext.SetAction(action);
    }

    private static IHttpHandler GetDefaultHandler(HttpPipelineContext pipelineContext)
    {
        return StaticFileHandlerFactory.Instance.GetHandler(pipelineContext.HttpContext)
            ?? DirectoryBrowseHandlerFactory.Instance.GetHandler(pipelineContext.HttpContext)
            ?? Http404Handler.Instance;
    }


    private static ActionDescription TryCreateOptionAction(HttpPipelineContext pipelineContext)
    {
        if( pipelineContext.HttpContext.Request.HttpMethod != HttpOptionsAttribute.MethodName )
            return null;

        return ControllerFactory.CreateHandler(OptionsHandler.Instance);
    }


}
