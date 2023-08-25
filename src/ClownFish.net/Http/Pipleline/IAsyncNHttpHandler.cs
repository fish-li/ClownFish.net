namespace ClownFish.Http.Pipleline;

/// <summary>
/// 用于直接处理请求，而不使用MVC模式。
/// 使用方法：在NHttpModule的BeginRequest方法中给HttpPipelineContext指定一个IAsyncNHttpHandler实例，然后框架会在BeginRequest之后调用它。
/// 例如：HTTP转发，此时不需要执行整个管道流程。
/// </summary>
public interface IAsyncNHttpHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    Task ProcessRequestAsync(NHttpContext httpContext);
}
