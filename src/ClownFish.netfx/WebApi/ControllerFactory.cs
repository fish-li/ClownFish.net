using System.Runtime.InteropServices;

namespace ClownFish.WebApi;

internal class ControllerFactory
{
    public static readonly ControllerFactory Instance = new ControllerFactory();


    internal static ActionDescription CreateHandler(IAsyncNHttpHandler handler)
    {
        if( handler == null )
            return null;

        MethodInfo method = handler.GetType().GetInstanceMethod(nameof(IAsyncNHttpHandler.ProcessRequestAsync));
        return new ActionDescription(handler, method);
    }


    public virtual void CreateController(HttpPipelineContext pipelineContext)
    {
        ActionDescription action = pipelineContext.Action;


        // 允许在框架外部直接指定结果，所以 action.Controller 有可能不为NULL

        if( action.Controller == null ) 
            action.Controller = CreateInstance(action.ControllerType, pipelineContext);


        // bind HttpContext
        IRequireHttpContext requireHttpContext = action.Controller as IRequireHttpContext;
        if( requireHttpContext != null )
            requireHttpContext.NHttpContext = pipelineContext.HttpContext;

    }


    public virtual object CreateInstance(Type controllerType, HttpPipelineContext pipelineContext)
    {
        if( controllerType == null )
            throw new ArgumentNullException(nameof(controllerType));

        // TODO: 以后要不要支持 IOC ??
        object controller = Activator.CreateInstance(controllerType);

        return controller;
    }
}
