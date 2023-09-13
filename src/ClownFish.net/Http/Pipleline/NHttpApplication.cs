//HTTP管线模型，可用于三种运行环境：
//1、WebHost + WebApi： 用于.NET FX 下的 Console, WinForm, WindowsService 项目
//2、ASP.NET + WebApi ：用于经典的 IIS/ASP.NET 运行环境。
//3、ASP.NET CORE 环境


// 此外，NHttpModule 没有提供异步支持，主要有以下原因：
// 1、基本上用不到，至少我没有遇到吧~~
// 2、await 会导致每个方法在执行时，产生一个状态机实例，并增加了代码的复杂度，对于绝大数情况下来说，完全是在浪费性能！

namespace ClownFish.Http.Pipleline;

/// <summary>
/// 表示用于执行HTTP请求的任务过程
/// </summary>
public sealed class NHttpApplication
{
    private readonly List<NHttpModule> _modules = null;

    // 说明：基本上 HttpModule 可以设计成单例模式，即使在多个阶段间需要维持状态，也可以放在 HttpContext.Items 中
    // 因此，这里将 HttpApplication 设计成单例模式，可以减少一些不必要的对象被创建出来。

    /// <summary>
    /// 单例对象引用
    /// </summary>
    public static NHttpApplication Instance { get; private set; }

    private NHttpApplication()
    {
        _modules = NHttpModuleFactory.CreateModuleList();

        foreach( NHttpModule module in _modules ) {
            module.Init();
        }
    }

    /// <summary>
    /// 获取当前加载的所有module清单，克隆一份副本。
    /// </summary>
    /// <returns></returns>
    public List<NHttpModule> GetModules() => _modules.ToList();

    /// <summary>
    /// 启动HTTP处理管道
    /// </summary>
    /// <param name="onlyOnce"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static NHttpApplication Start(bool onlyOnce = true)
    {
        if( Instance != null )
            throw new InvalidOperationException("此方法不允许多次调用！");

        NHttpApplication app = new NHttpApplication();

        if( onlyOnce ) {
            Instance = app;
        }
        return app;
    }


    internal DebugReportBlock GetDebugReportBlock()
    {
        DebugReportBlock block = new DebugReportBlock { Category = nameof(NHttpApplication), Order = 100 };
        block.AppendLine($"Modules:");

        int i = 1;
        foreach( var x in _modules ) {
            block.AppendLine($"{i++,3}: {x.GetType().FullName}, order: {x.Order}");
        }

        return block;
    }

    /// <summary>
    /// EnableCors
    /// </summary>
    /// <param name="httpContext"></param>
    public void EnableCors(NHttpContext httpContext)
    {
        string origin = httpContext.Request.Header("Origin");
        EnableCors(httpContext, origin);
    }

    /// <summary>
    /// EnableCors
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="origin"></param>
    public void EnableCors(NHttpContext httpContext, string origin)
    {
        //string origin = httpContext.Request.Header("Origin");
        if( origin.IsNullOrEmpty() )
            return;

        httpContext.Response.SetHeader("Access-Control-Allow-Origin", origin);

        // 简单 GET 请求，浏览器不会对其发起“预检请求”。
        // 但是，如果服务器端的响应中未携带 Access-Control-Allow-Credentials: true ，浏览器将不会把响应内容返回给请求的发送者。
        // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/CORS#%E9%99%84%E5%B8%A6%E8%BA%AB%E4%BB%BD%E5%87%AD%E8%AF%81%E7%9A%84%E8%AF%B7%E6%B1%82
        httpContext.Response.SetHeader("Access-Control-Allow-Credentials", "true");

        // 下面2个头只在【预检请求】时有效
        // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/CORS#%E9%A2%84%E6%A3%80%E8%AF%B7%E6%B1%82
        if( httpContext.Request.HttpMethod == "OPTIONS" ) {
            httpContext.Response.SetHeader("Access-Control-Allow-Methods", "*");
            httpContext.Response.SetHeader("Access-Control-Allow-Headers", "*");
        }
        //httpContext.Response.SetHeader("p3p", "CP=\"CAO PSA OUR\"");
    }

    /// <summary>
    /// InitResponse
    /// </summary>
    /// <param name="httpContext"></param>
    public void InitResponse(NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;

        //response.SetHeader(HttpHeaders.XResponse.ClownFishVersion, ConstValues.CurrentVersion);
        response.ContentEncoding = Encoding.UTF8;
    }

    /// <summary>
    /// BeginRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public void BeginRequest(NHttpContext httpContext)
    {
        ClownFishCounters.Concurrents.HttpConcurrent.Increment();
        ClownFishCounters.ExecuteTimes.HttpCount.Increment();

        httpContext.LogFxEvent(new NameTime(nameof(BeginRequest)));

        foreach( NHttpModule module in _modules ) {
            module.BeginRequest(httpContext);
        }
    }

    /// <summary>
    /// ExecuteHttpHandlerAsync
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<bool> ExecuteHttpHandlerAsync(NHttpContext httpContext)
    {
        IAsyncNHttpHandler handler = httpContext.PipelineContext.Action?.Controller as IAsyncNHttpHandler;
        if( handler != null ) {

            httpContext.BeginExecuteTime = DateTime.Now;
            httpContext.LogFxEvent(new NameTime("UserCode begin", httpContext.BeginExecuteTime));

            await handler.ProcessRequestAsync(httpContext);

            httpContext.EndExecuteTime = DateTime.Now;
            httpContext.LogFxEvent(new NameTime("UserCode end", httpContext.EndExecuteTime));
            //httpContext.Response.End();
            return true;
        }

        return false;
    }

    /// <summary>
    /// AuthenticateRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public void AuthenticateRequest(NHttpContext httpContext)
    {
        if( httpContext.SkipAuthorization )
            return;

        httpContext.LogFxEvent(new NameTime(nameof(AuthenticateRequest)));

        foreach( NHttpModule module in _modules ) {
            module.AuthenticateRequest(httpContext);
        }
    }

    /// <summary>
    /// PostAuthenticateRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public void PostAuthenticateRequest(NHttpContext httpContext)
    {
        if( httpContext.SkipAuthorization )
            return;

        httpContext.LogFxEvent(new NameTime(nameof(PostAuthenticateRequest)));

        foreach( NHttpModule module in _modules ) {
            module.PostAuthenticateRequest(httpContext);
        }
    }

    /// <summary>
    /// AuthorizeRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public void AuthorizeRequest(NHttpContext httpContext)
    {
        if( httpContext.SkipAuthorization )
            return;

        httpContext.LogFxEvent(new NameTime(nameof(AuthorizeRequest)));

        foreach( NHttpModule module in _modules ) {
            module.AuthorizeRequest(httpContext);
        }
    }


    /// <summary>
    /// ResolveRequestCache
    /// </summary>
    /// <param name="httpContext"></param>
    public void ResolveRequestCache(NHttpContext httpContext)
    {
        httpContext.LogFxEvent(new NameTime(nameof(ResolveRequestCache)));

        foreach( NHttpModule module in _modules ) {
            module.ResolveRequestCache(httpContext);
        }
    }

    /// <summary>
    /// PreFindAction
    /// </summary>
    /// <param name="httpContext"></param>
    public void PreFindAction(NHttpContext httpContext)
    {
        if( httpContext.PipelineContext.Action != null )
            return;

        httpContext.LogFxEvent(new NameTime(nameof(PreFindAction)));

        foreach( NHttpModule module in _modules ) {
            module.PreFindAction(httpContext);
        }
    }

    /// <summary>
    /// PostFindAction
    /// </summary>
    /// <param name="httpContext"></param>
    public void PostFindAction(NHttpContext httpContext)
    {
        httpContext.LogFxEvent(new NameTime(nameof(PostFindAction)));

        foreach( NHttpModule module in _modules ) {
            module.PostFindAction(httpContext);
        }
    }

    /// <summary>
    /// PreRequestExecute
    /// </summary>
    /// <param name="httpContext"></param>
    public void PreRequestExecute(NHttpContext httpContext)
    {
        httpContext.LogFxEvent(new NameTime(nameof(PreRequestExecute)));

        foreach( NHttpModule module in _modules ) {
            module.PreRequestExecute(httpContext);
        }
    }

    /// <summary>
    /// PostRequestExecute
    /// </summary>
    /// <param name="httpContext"></param>
    public void PostRequestExecute(NHttpContext httpContext)
    {
        httpContext.LogFxEvent(new NameTime(nameof(PostRequestExecute)));

        foreach( NHttpModule module in _modules ) {
            module.PostRequestExecute(httpContext);
        }
    }

    /// <summary>
    /// UpdateRequestCache
    /// </summary>
    /// <param name="httpContext"></param>
    public void UpdateRequestCache(NHttpContext httpContext)
    {
        httpContext.LogFxEvent(new NameTime(nameof(UpdateRequestCache)));

        foreach( NHttpModule module in _modules ) {
            module.UpdateRequestCache(httpContext);
        }
    }

    /// <summary>
    /// EndRequest
    /// </summary>
    /// <param name="httpContext"></param>
    public void EndRequest(NHttpContext httpContext)
    {
        httpContext.LogFxEvent(new NameTime("EndRequest"));

        httpContext.PipelineContext.End();

        foreach( NHttpModule module in _modules ) {
            try {
                module.EndRequest(httpContext);
            }
            catch( Exception ex2 ) {
                Console2.Error("NHttpApplication.EndRequest ERROR.", ex2);
            }
        }

        // 对于日志来说，End 阶段不够用，所以再增加一个【内部阶段】
        // 目前主要是 OprLogModule 在用
        foreach( NHttpModule module in _modules ) {
            if( module is IEnd2Request module2 ) {
                try {
                    module2.End2Request(httpContext);
                }
                catch( Exception ex2 ) {
                    Console2.Error("NHttpApplication.End2Request ERROR.", ex2);
                }
            }
        }

        ClownFishCounters.Concurrents.HttpConcurrent.Decrement();
        if( httpContext.IsTransfer == false && StatusCodeUtils.IsServerError(httpContext.Response.StatusCode) )
            ClownFishCounters.ExecuteTimes.HttpError.Increment();

        httpContext.LogFxEvent(new NameTime("framework end"));
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="httpContext"></param>
    public void OnError(NHttpContext httpContext)
    {
        if( httpContext == null || httpContext.PipelineContext == null )
            return;

        foreach( NHttpModule module in _modules ) {

            if( httpContext.LastException == null )
                return;

            try {
                module.OnError(httpContext);
            }
            catch( Exception ex2 ) {
                Console2.Error("NHttpApplication.OnError new exception:", ex2);
                Console2.Error("NHttpApplication.OnError original exception:", httpContext.LastException);
            }
        }
    }



}
