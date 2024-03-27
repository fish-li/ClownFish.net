namespace ClownFish.Http.Pipleline;

// 设计说明：
// 其实这个类型可以合并到 NHttpContext 中，在多种因素考虑下，最终还是打算增加HttpPipelineContext这个类型：
// 1、不希望让 HttpContext “变味”，希望尽量让它保持经典的模样！
// 2、不改变“原先”的API，减少学习成本

/// <summary>
/// 在HttpPipleline运行过程中的一些状态数据。
/// </summary>
public sealed class HttpPipelineContext : BasePipelineContext, IDisposable
{
    // 设置一个静态的全局引用
    // 如果需要停止传播可参考：https://stackoverflow.com/questions/35826893/c-sharp-async-await-leave-asynclocalt-context-upon-task-creation
    private static readonly AsyncLocal<HttpPipelineContext> s_local = new AsyncLocal<HttpPipelineContext>();

    /// <summary>
    /// 创建一个HttpPipelineContext实例。
    /// 请用 using 语句块的方式使用此实例。
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static HttpPipelineContext Start(NHttpContext httpContext)
    {
        var context =  new HttpPipelineContext(httpContext);
        s_local.Value = context;

        // 开启日志监控
        context.CreateOprLogScope();

        return context;
    }

    private void CreateOprLogScope()
    {
        if( LoggingOptions.HttpActionEnableLog ) {
            OprLogScope scope = OprLogScope.Start(this);
            this.SetOprLogScope(scope);
        }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    void IDisposable.Dispose()
    {
        s_local.Value = null;

        this.HttpContext.DisposeObjects();


        // 释放引用
        this.HttpContext = null;
        this.Action = null;
        this.RespResult = null;
        this.DisposeOprLogScope();
    }


    /// <summary>
    /// 获取一个与当前线程关联的HttpPipelineContext对象
    /// </summary>
    /// <returns></returns>
    public static HttpPipelineContext Get()
    {
        HttpPipelineContext ctx = s_local.Value;

        if( ctx != null && ctx.HttpContext == null ) {

            // HttpPipelineContext实例可能【逃逸】了，原因是 new Thread/Task.Run 之类的写法导致
            // 所以这里还要检查它的有效性

            // 目前遇到的现象是：在AspnetcoreLogger中，第2次执行EndRequest方法时，
            // HttpPipelineContext.Get()居然还能得到一个实例，但是观察里面的属性值，发现这个对象其实是被 Dispose 过的
            
            s_local.Value = null;
            return null;
        }
        return ctx;
    }


    /// <summary>
    /// 获取一个与当前线程关联的HttpPipelineContext对象，如果没有关联的实例则抛出异常
    /// </summary>
    /// <returns></returns>
    public static HttpPipelineContext Get2()
    {
        HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
        if( pipelineContext == null )
            throw new InvalidOperationException("当前请求没有关联到一个PipelineContext实例。");

        return pipelineContext;
    }


    /// <summary>
    /// 当前请求映射到的Action对象
    /// </summary>
    public ActionDescription Action { get; private set; }

    /// <summary>
    /// 当前请求是否为登录相关的操作。
    /// 这类请求有二方面要注意：1，不做登录凭证续期， 2，日志要做特殊的排除。
    /// </summary>
    public bool IsLoginAction { get; private set; }


    /// <summary>
    /// HTTP请求的输出结果，也可以理解为 MVC-Action 的返回值。
    /// </summary>
    public object RespResult { get; set; }

    /// <summary>
    /// HttpContext instance
    /// </summary>
    public NHttpContext HttpContext { get; private set; }


    private HttpPipelineContext(NHttpContext httpContext)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        this.PerformanceThresholdMs = ClownFish.Log.LogConfig.Instance?.Performance?.HttpExecute ?? 0;
        this.HttpContext = httpContext;
        httpContext.PipelineContext = this;
    }

    
    /// <summary>
    /// 设置MVC调用参数
    /// </summary>
    /// <param name="action"></param>
    /// <param name="isLoginAction"></param>
    public void SetAction(ActionDescription action, bool isLoginAction = false)
    {
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        if( this.Action != null )
            throw new InvalidOperationException("SetAction不允许重复调用。");


        this.Action = action;
        this.IsLoginAction = isLoginAction;
    }

    /// <summary>
    /// 指定一个可用于处理当前请求的IAsyncNHttpHandler实例。
    /// 注意：一定要在MVC Action执行前调用此方法。
    /// </summary>
    /// <param name="handler"></param>
    public void SetHttpHandler(IAsyncNHttpHandler handler)
    {
        if( handler == null )
            throw new ArgumentNullException(nameof(handler));

        Type controllerType = handler.GetType();
        MethodInfo method = controllerType.GetMethod(nameof(IAsyncNHttpHandler.ProcessRequestAsync),
                                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                                                    new Type[] { typeof(NHttpContext) }, null);

        this.Action = new ActionDescription(handler, method, controllerType);
    }


    /// <summary>
    /// CompleteRequest
    /// </summary>
    internal void CompleteRequest()
    {
        throw new AbortRequestException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetStatus()
    {
        if( this.HttpContext.IsTransfer )
            return base.GetStatus();
        else
            return this.HttpContext.Response.StatusCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetTitle()
    {
        return this.HttpContext.Request.FullPath;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override object GetRequest()
    {
        return this.HttpContext.Request;
    }
}