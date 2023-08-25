namespace ClownFish.Http.Pipleline;

/// <summary>
/// HttpResponse
/// </summary>
public abstract class NHttpResponse
{
    private readonly NHttpContext _httpContext;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="httpContext"></param>
    protected NHttpResponse(NHttpContext httpContext)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        _httpContext = httpContext;
    }

    /// <summary>
    /// 原始的HttpRequest例，与当前运行环境有关。
    /// </summary>
    public abstract object OriginalHttpResponse { get; }

    /// <summary>
    /// 获取NHttpContext的引用
    /// </summary>
    public virtual NHttpContext HttpContext { get => _httpContext; }

    /// <summary>
    /// 获取或设置返回给客户端的 HTTP 状态代码。
    /// </summary>
    public abstract int StatusCode { get; set; }

    /// <summary>
    /// 获取或设置返回内容的 MIME 类型。 
    /// </summary>
    public abstract string ContentType { get; set; }

    /// <summary>
    /// ContentEncoding
    /// </summary>
    public abstract Encoding ContentEncoding { get; set; }

    /// <summary>
    /// 获取将响应写入其中的 System.IO.Stream 对象。
    /// </summary>
    public abstract Stream OutputStream { get; }

    /// <summary>
    /// 响应体是否已经发出
    /// </summary>
    public abstract bool HasStarted { get; }

    /// <summary>
    /// ContentLength
    /// </summary>
    public abstract long ContentLength { get; set; }


    /// <summary>
    /// 写Cookie
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="expires"></param>
    public abstract void SetCookie2(string name, string value, DateTime? expires = null);

    /// <summary>
    /// 写Cookie
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="expires"></param>
    public void SetCookie(string name, string value, TimeSpan? expires = null)
    {
        if( expires.HasValue ) {
            DateTime time = DateTime.Now.Add(expires.Value);
            SetCookie2(name, value, time);
        }
        else {
            SetCookie2(name, value, null);
        }
    }

    //public abstract string[] CookieKeys { get; }

    /// <summary>
    /// 添加一个响应头
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="ignoreExist"></param>
    public abstract bool SetHeader(string name, string value, bool ignoreExist = false);

    /// <summary>
    /// 添加一批响应头，这些响应头使用同一个名称。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="values"></param>
    /// <param name="ignoreExist"></param>
    public abstract bool SetHeaders(string name, string[] values, bool ignoreExist = false);

    /// <summary>
    /// 获取所有响应头
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetAllHeaders();

    /// <summary>
    /// 遍历所有响应头并执行指定的委托
    /// </summary>
    /// <param name="action"></param>
    public void AccessHeaders(Action<string, string> action)
    {
        if( action == null )
            throw new ArgumentNullException(nameof(action));

        foreach( var x in GetAllHeaders() ) {
            foreach( var v in x.Value ) {
                action(x.Key, v);
            }
        }
    }


    /// <summary>
    /// 删除一个响应头
    /// </summary>
    /// <param name="name"></param>
    public abstract bool RemoveHeader(string name);


    /// <summary>
    /// 添加或者更新响应头。
    /// 如果指定的响应头不存在，它将会被添加，如果指定的响应头存在，它将会被更新。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetOrUpdateHeader(string name, string value)
    {
        this.RemoveHeader(name);
        this.SetHeader(name, value);
    }


    /// <summary>
    /// 设置 Cache-Control 缓存头
    /// </summary>
    /// <param name="time"></param>
    public void SetCacheControl(TimeSpan time)
    {
        this.RemoveHeader("Cache-Control");
        this.SetHeader("Cache-Control", "public, max-age=" + time.TotalSeconds.ToString());
    }

    


    /// <summary>
    /// 清除已存在的响应头
    /// </summary>
    public abstract void ClearHeaders();


    /// <summary>
    /// 将数据写入响应流
    /// </summary>
    /// <param name="buffer">将要写入响应流的二进制内容</param>
    public abstract void Write(byte[] buffer);

    /// <summary>
    /// 一次性将数据写入响应流，并设置 Content-Length 响应头，因此后面【不能】再写入其它内容。
    /// </summary>
    /// <param name="buffer">将要写入响应流的二进制内容</param>
    public abstract void WriteAll(byte[] buffer);


    /// <summary>
    /// 将数据写入响应流
    /// </summary>
    /// <param name="buffer">将要写入响应流的二进制内容</param>
    public abstract Task WriteAsync(byte[] buffer);


    /// <summary>
    /// 一次性将数据写入响应流，并设置 Content-Length 响应头，因此后面【不能】再写入其它内容。
    /// </summary>
    /// <param name="buffer">将要写入响应流的二进制内容</param>
    public abstract Task WriteAllAsync(byte[] buffer);


    /// <summary>
    /// 结束本次请求
    /// </summary>
    public void End()
    {
        this.HttpContext.PipelineContext.CompleteRequest();
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract void Close();

}
