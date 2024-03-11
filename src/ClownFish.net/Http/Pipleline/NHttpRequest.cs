namespace ClownFish.Http.Pipleline;

/// <summary>
/// HttpRequest类型
/// </summary>
public abstract partial class NHttpRequest
{
    private readonly NHttpContext _httpContext;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="httpContext"></param>
    protected NHttpRequest(NHttpContext httpContext)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        _httpContext = httpContext;
    }


    /// <summary>
    /// 原始的HttpRequest例，与当前运行环境有关。
    /// </summary>
    public abstract object OriginalHttpRequest { get; }

    /// <summary>
    /// HttpContext实例引用
    /// </summary>
    public virtual NHttpContext HttpContext { get => _httpContext; }

    /// <summary>
    /// true if this request is using https; otherwise, false.
    /// </summary>
    public abstract bool IsHttps { get; }

    /// <summary>
    /// 获取由客户端指定的 HTTP 方法。 
    /// </summary>
    public abstract string HttpMethod { get; }

    /// <summary>
    /// 当前请求的根网址。例如：http://www.abc.com:12345
    /// </summary>
    public abstract string RootUrl { get; }

    /// <summary>
    /// 当前请求的路径（不包含查询字符串）。例如：/aa/bb/cc.aspx
    /// </summary>
    public abstract string Path { get; }

    /// <summary>
    /// 当前请求的查询字符串。例如：?xx=2
    /// </summary>
    public abstract string Query { get; }


    /// <summary>
    /// 当前请求的路径的查询参数。例如： /aa/bb/cc.aspx?xx=2
    /// </summary>
    public abstract string RawUrl { get; }

    /// <summary>
    /// 当前请求的路径的查询参数（等同于 RawUrl 属性）。例如： /aa/bb/cc.aspx?xx=2
    /// </summary>
    public string PathAndQuery => RawUrl;

    private string _fullPath;
    /// <summary>
    /// 当前请求的完整路径。例如：http://www.abc.com:12345/aa/bb/cc.aspx
    /// </summary>
    public string FullPath {
        get {
            if( _fullPath == null )
                _fullPath = this.RootUrl + this.Path;
            return _fullPath;
        }
    }

    private string _fullUrl;
    /// <summary>
    /// 当前请求的完整URL。例如：http://www.abc.com:12345/aa/bb/cc.aspx?xx=2
    /// </summary>
    public string FullUrl {
        get {
            if( _fullUrl == null )
                _fullUrl = this.RootUrl + this.RawUrl;
            return _fullUrl;
        }
    }

    private Uri _requestUri;

    /// <summary>
    /// 当前请求的完整URL, new Uri(FullUrl)
    /// </summary>
    public Uri RequestUri {
        get { 
            if( _requestUri == null ) {
                _requestUri = new Uri(this.FullUrl);
            }
            return _requestUri;
        }
    }

    /// <summary>
    /// 当前请求体的数据格式类型
    /// </summary>
    public abstract string ContentType { get; }

    /// <summary>
    /// UserAgent
    /// </summary>
    public abstract string UserAgent { get; }

    internal Match RegexMatch { get; set; }

    /// <summary>
    /// 根据指定的名称，从URL路由匹配结果中获取对应的值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual string Route(string name)
    {
        if( this.RegexMatch != null ) {

            Group g = this.RegexMatch.Groups[name];
            if( g != null && g.Success )
                return g.Value;
        }

        return null;
    }



    /// <summary>
    /// 获取请求体的长度。
    /// 如果没有指定 Content-Length 则返回 -1
    /// </summary>
    public virtual long ContentLength {
        get => this.Header(HttpHeaders.Request.ContentLength)?.TryToLong() ?? -1;
    }

    /// <summary>
    /// 获取请求体文本的长度(ContentLength)。
    /// 如果请求没有请求体，或者请求体不是文本格式，都返回 0，
    /// 如果没有指定Content-Length头，则返回 -1
    /// </summary>
    /// <returns></returns>
    public long GetBodyTextLength()
    {
        long len = this.ContentLength;

        if( len <= 0 )
            return len;

        if( len > 0 && HttpUtils.RequestBodyIsText(this.ContentType) ) {
            return len;
        }
        else {
            return 0;
        }
    }



    /// <summary>
    /// 获取一个 Boolean 值，该值指示请求是否有关联的正文数据。
    /// </summary>
    public virtual bool HasBody => HttpUtils.RequestHasBody(this.HttpMethod);

    /// <summary>
    /// 获取包含正文数据的流，这些数据由客户端发送。
    /// </summary>
    public abstract Stream InputStream { get; }
        


    // 为了性能考虑，以下四类数据（QueryString, Form, Cookie, Header），就不再封装新的集合，好处是可以避免创建不必要的对象，给GC减少压力，
    // 所以这里只定义了一些简单的读取方式：读取单个数据项，读取所有键名

    // 此外，这4类数据集合中，是允许 KEY 重复的，
    // 所以严格来说，根据一个KEY得到的结果应该是一个 string[] 才对，
    // 但是这类场景极少发生，而且是可以可以避免的，
    // 所以为了简化代码，也为了提升性能，就不考虑这类小众使用场景，直接用 string 来定义返回值类型。

    /// <summary>
    /// 根据指定的名称，获取对应的查询字符串参数值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public abstract string QueryString(string name);

    /// <summary>
    /// 获取所有的查询字符串参数项的名称
    /// </summary>
    public abstract string[] QueryStringKeys { get; }

    /// <summary>
    /// 根据指定的名称，获取对应的表单（Form）值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public abstract string Form(string name);

    /// <summary>
    /// 获取所有的表单（Form）项名称
    /// </summary>
    public abstract string[] FormKeys { get; }

    /// <summary>
    /// 根据指定的名称，获取对应的Cookie值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public abstract string Cookie(string name);

    /// <summary>
    /// 获取请求头中所有的Cookie项名称
    /// </summary>
    public abstract string[] CookieKeys { get; }

    /// <summary>
    /// 根据指定的名称，获取对应的请求头值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public abstract string Header(string name);


    /// <summary>
    /// 根据指定的名称，获取对应的请求头值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public abstract string[] GetHeaders(string name);

    /// <summary>
    /// 获取所有的请求头名称
    /// </summary>
    public abstract string[] HeaderKeys { get; }


    // TODO: support files


    /// <summary>
    /// 获取当前请求的字符编码方式
    /// </summary>
    /// <returns></returns>
    public virtual Encoding GetEncoding()
    {
        //return request.ContentEncoding;  // asp.net core 没有这个属性

        string contentType = this.Header(HttpHeaders.Request.ContentType);
        if( string.IsNullOrEmpty(contentType) == false ) {

            string flag = "charset=";
            int p = contentType.IndexOf(flag);
            if( p > 0 ) {
                string value = contentType.Substring(p + flag.Length).Trim();

                try {
                    return Encoding.GetEncoding(value);
                }
                catch { /* 忽略不能识别的设置 */}
            }
        }

        // 默认使用UTF-8编码
        return Encoding.UTF8;
    }

    /// <summary>
    /// 遍历所有请求头并执行指定的委托
    /// </summary>
    /// <param name="action"></param>
    public void AccessHeaders(Action<string, string> action)
    {
        if( action == null )
            throw new ArgumentNullException(nameof(action));


        foreach( string key in this.HeaderKeys ) {

            string[] values = this.GetHeaders(key);
            if( values != null && values.Length > 0 ) {

                foreach( var value in values )
                    action(key, value);
            }
        }
    }

    /// <summary>
    /// 从各种数据集合中获取（Route, QueryString, Form, Header）获取指定名称的数据
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual string GetValue(string name)
    {
        return this.Route(name)
                ?? this.QueryString(name)
                ?? this.Form(name)
                ?? this.Header(name);
    }



    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Url: " + this.Path;
    }


}
