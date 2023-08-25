namespace ClownFish.Http.Pipleline;

/// <summary>
/// 类似 ASP.NET 中的 IHttpModule
/// </summary>
public abstract class NHttpModule
{
    /// <summary>
    /// NHttpModule[]中的排序位置，也可以理解为执行次序。
    /// </summary>
    public virtual int Order => 2000;

    /// <summary>
    /// 模块初始化，只调用一次。
    /// </summary>
    public virtual void Init()
    {
    }

    /// <summary>
    /// BeginRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void BeginRequest(NHttpContext httpContext)
    {
    }

    /// <summary>
    /// AuthenticateRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void AuthenticateRequest(NHttpContext httpContext)
    {
    }

    /// <summary>
    /// PostAuthenticateRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void PostAuthenticateRequest(NHttpContext httpContext)
    {
    }

    /// <summary>
    /// AuthorizeRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void AuthorizeRequest(NHttpContext httpContext)
    {
    }


    /// <summary>
    /// ResolveRequestCache
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void ResolveRequestCache(NHttpContext httpContext)
    {
    }


    /// <summary>
    /// PreFindAction
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public virtual void PreFindAction(NHttpContext httpContext)
    {
    }



    /// <summary>
    /// PostindAction
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void PostFindAction(NHttpContext httpContext)
    {
    }



    /// <summary>
    /// PreRequestExecute
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void PreRequestExecute(NHttpContext httpContext)
    {
    }

    /// <summary>
    /// PostRequestExecute
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void PostRequestExecute(NHttpContext httpContext)
    {
    }

    /// <summary>
    /// UpdateRequestCache
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void UpdateRequestCache(NHttpContext httpContext)
    {
    }

    /// <summary>
    /// EndRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void EndRequest(NHttpContext httpContext)
    {
    }

    /// <summary>
    /// 异常处理。
    /// </summary>
    /// <param name="httpContext"></param>
    public virtual void OnError(NHttpContext httpContext)
    {
    }

}
