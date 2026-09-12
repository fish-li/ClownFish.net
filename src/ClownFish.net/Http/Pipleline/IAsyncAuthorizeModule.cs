namespace ClownFish.Http.Pipleline;

// 下面3个阶段【可能会有】异步的需求，所以提供了异步接口，供模块实现。
// 完全没有异步需求的模块仍然使用同步方法，毕竟异步方法的开销还是比同步方法大一些


/// <summary>
/// 支持异步方式执行 BeginRequest 的接口
/// </summary>
public interface IAsyncBeginModule
{
    /// <summary>
    /// 采用异步方式执行 BeginRequest
    /// </summary>
    /// <param name="httpContext"></param>
    Task BeginRequestAsync(NHttpContext httpContext);
}


/// <summary>
/// 支持异步方式执行 AuthenticateRequest 的接口
/// </summary>
public interface IAsyncAuthenticateModule
{
    /// <summary>
    /// 采用异步方式执行 AuthenticateRequest
    /// </summary>
    /// <param name="httpContext"></param>
    Task AuthenticateRequestAsync(NHttpContext httpContext);
}


/// <summary>
/// 支持异步方式执行 AuthorizeRequest 的接口
/// </summary>
public interface IAsyncAuthorizeModule
{
    /// <summary>
    /// 采用异步方式执行 AuthorizeRequest
    /// </summary>
    /// <param name="httpContext"></param>
    Task AuthorizeRequestAsync(NHttpContext httpContext);
}




