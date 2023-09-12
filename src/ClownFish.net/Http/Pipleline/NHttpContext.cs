namespace ClownFish.Http.Pipleline;

/// <summary>
/// HttpContext
/// </summary>
public abstract class NHttpContext
{
    /// <summary>
    /// 
    /// </summary>
    public HttpPipelineContext PipelineContext { get; internal set; }


    /// <summary>
    /// 原始的HttpContext实例，与当前运行环境有关。
    /// </summary>
    public abstract object OriginalHttpContext { get; }

    /// <summary>
    /// 用于操作Request的对象
    /// </summary>
    public abstract NHttpRequest Request { get; }

    /// <summary>
    /// 用于操作Response的对象
    /// </summary>
    public abstract NHttpResponse Response { get; }

    /// <summary>
    /// 获取用于为客户端获取标识、身份验证信息和安全角色的对象
    /// </summary>
    public abstract IPrincipal User { get; set; }

    /// <summary>
    /// 获取或设置一个值，该值指示是否应跳过对当前请求的授权检查
    /// </summary>
    public abstract bool SkipAuthorization { get; set; }

    /// <summary>
    /// 存放一些与请求相关的临时数据
    /// </summary>
    public abstract XDictionary Items { get; }


    /// <summary>
    /// 是否启用日志(OprLog + InvokeLog)，如果日志不启用，那么将不会统计调用次数，Venus界面看不到统计结果。
    /// 默认值：true
    /// </summary>
    public bool EnableLog { get; set; } = ClownFish.Log.LoggingOptions.HttpActionEnableLog;
    

    /// <summary>
    /// 当前是否为转发请求
    /// </summary>
    public bool IsTransfer { get; set; }

    /// <summary>
    /// 最近一次产生的异常对象
    /// </summary>
    public Exception LastException => this.PipelineContext.LastException;

    /// <summary>
    /// 当前请求的用户是否为已登录用户
    /// </summary>
    public bool IsAuthenticated {
        get {
            bool? isAuth = this.User?.Identity?.IsAuthenticated;
            return isAuth.GetValueOrDefault();
        }
    }

    /// <summary>
    /// 记录一些时间序列，描述在什么时候开始执行什么操作，用于性能监控。
    /// 此属性需要在要请求入口时赋值，如果属性为NULL表示不启用。
    /// </summary>
    public List<NameTime> TimeEvents { get; set; } // = new List<NameTime>(20);

    /// <summary>
    /// Action代码的开始执行时间
    /// </summary>
    public DateTime BeginExecuteTime { get; set; }

    /// <summary>
    /// Action代码的结束执行时间
    /// </summary>
    public DateTime EndExecuteTime { get; set; }


    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Request Url: " + this.Request.Path;
    }


    private List<IDisposable> _disposableObjects;

    /// <summary>
    /// 注册一个对象，它将在请求结束时调用IDisposable的Dispose方法。
    /// </summary>
    /// <param name="disposable"></param>
    public void RegisterForDispose(IDisposable disposable)
    {
        if( disposable == null )
            return;

        if( _disposableObjects == null )
            _disposableObjects = new List<IDisposable>();

        _disposableObjects.Add(disposable);
    }


    internal void DisposeObjects()
    {
        if( _disposableObjects != null ) {
            foreach( var x in _disposableObjects ) {
                try {
                    x.Dispose();
                }
                catch { /* 忽略所有异常  */ }
            }

            _disposableObjects = null;
        }
    }

}
