namespace ClownFish.MQ.Messages;

#if NETCOREAPP

/// <summary>
/// 通过数据还原出来的NHttpRequest实例
/// </summary>
public sealed class HttpRequestAlone : NHttpRequest, IDisposable
{
    private readonly RequestData _data;
    private readonly string _httpModeth;
    private readonly string _fullUrl;
    private readonly Uri _uri;

    private string _contentType;
    private NameValueCollection _query;
    private NameValueCollection _headers;
    private List<NameValue> _cookies;

    private MemoryStream _bodyStream;

    /// <summary>
    /// 构造方法
    /// </summary>
    public HttpRequestAlone(RequestData data) : base(HttpContextAlone.Instance)
    {
        if( _data != null )
            throw new InvalidOperationException("不允许多次调用当前方法。");

        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( data.RequestLine.IsNullOrEmpty() )
            throw new ArgumentException("data is invalid, request line is empty.");

        //if( data.Headers.IsNullOrEmpty() )
        //    throw new ArgumentException("data is invalid, headers is empty.");


        string[] parts = data.RequestLine.Split(' ');
        if( parts.Length != 3 )
            throw new ArgumentException("data is invalid, request line is error.");

        _httpModeth = parts[0];
        _fullUrl = parts[1];
        _uri = new Uri(_fullUrl);
        _data = data;
    }


    /// <summary>
    /// 不支持访问此属性。
    /// </summary>
    public override object OriginalHttpRequest => throw new NotSupportedException();

    /// <summary>
    /// HttpContext
    /// </summary>
    public override NHttpContext HttpContext => throw new NotSupportedException();

    /// <summary>
    /// IsHttps
    /// </summary>
    public override bool IsHttps => _fullUrl.StartsWithIgnoreCase("https://");

    /// <summary>
    /// HttpMethod
    /// </summary>
    public override string HttpMethod => _httpModeth;

    /// <summary>
    /// RootUrl
    /// </summary>
    public override string RootUrl => $"{_uri.Scheme}://{_uri.Authority}";

    /// <summary>
    /// Path
    /// </summary>
    public override string Path => _uri.AbsolutePath;

    /// <summary>
    /// Query
    /// </summary>
    public override string Query => _uri.Query;

    /// <summary>
    /// RawUrl
    /// </summary>
    public override string RawUrl => _uri.PathAndQuery;

    /// <summary>
    /// ContentType
    /// </summary>
    public override string ContentType {
        get {
            if( _contentType == null ) {
                string value = this.Header(HttpHeaders.Request.ContentType);
                if( value.HasValue() ) {
                     int p = value.IndexOf(';');
                    if( p > 0 )
                        _contentType = value.Substring(0, p);
                    else
                        _contentType = value;
                }
            }
            return _contentType;
        }
    }

    /// <summary>
    /// UserAgent
    /// </summary>
    public override string UserAgent => this.Header(HttpHeaders.Request.UserAgent);

    /// <summary>
    /// 不支持访问此属性。
    /// </summary>
    public override Stream InputStream {
        get {
            if( _bodyStream == null ) {
                _bodyStream = new MemoryStream(_data.Body, false);
            }
            return _bodyStream;
        }
    }

    /// <summary>
    /// 读取请求流，以二进制数组形式返回结果（不会判断是否压缩）。
    /// </summary>
    /// <returns></returns>
    public override byte[] ReadBodyAsBytes()
    {
        return _data.Body;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Task<byte[]> ReadBodyAsBytesAsync()
    {
        byte[] data = _data.Body;
        return Task.FromResult(data);
    }

    private void InitQuery()
    {
        if( _query == null ) {
            _query = System.Web.HttpUtility.ParseQueryString(_uri.Query);
        }
    }

    /// <summary>
    /// 获取查询字符串的所有KEY
    /// </summary>
    public override string[] QueryStringKeys {
        get {
            InitQuery();
            return _query.AllKeys;
        }
    }

    /// <summary>
    /// 获取一个查询字符串参数值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override string QueryString(string name)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        InitQuery();
        return _query[name];
    }

    /// <summary>
    /// 不支持访问此属性。
    /// </summary>
    public override string[] FormKeys => throw new NotImplementedException();

    /// <summary>
    /// 不支持访问此方法。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override string Form(string name)
    {
        throw new NotImplementedException();
    }
       


    private void InitHeaders()
    {
        if( _headers != null )
            return;

        _headers = new NameValueCollection(16);

        List<NameValue> list = _data.Headers.ToKVList('\n', ':');

        foreach( var x in list ) {
            _headers.Add(x.Name, x.Value);
        }
    }

    /// <summary>
    /// 获取所有的请求头名称
    /// </summary>
    public override string[] HeaderKeys {
        get {
            InitHeaders();
            return _headers.AllKeys;
        }
    }


    /// <summary>
    /// 获取某个请求头的所有值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override string[] GetHeaders(string name)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        InitHeaders();
        return _headers.GetValues(name);
    }

    /// <summary>
    /// 获取某个请求头对应的值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override string Header(string name)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        InitHeaders();
        return _headers.Get(name);
    }


    private void InitCookies()
    {
        if( _cookies != null )
            return;

        string cookieValue = this.Header("Cookie");
        if( cookieValue.IsNullOrEmpty() ) {
            _cookies = new List<NameValue>(0);
        }
        else {
            _cookies = cookieValue.ToKVList(';', '=');
        }
    }


    /// <summary>
    /// 获取所有的Cookie项名称
    /// </summary>
    public override string[] CookieKeys {
        get {
            InitCookies();
            return _cookies.Select(x => x.Name).ToArray();
        }
    }

    /// <summary>
    /// 获取某个Cookie参数值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override string Cookie(string name)
    {
        InitCookies();
        return _cookies.FirstOrDefault(x => x.Name.Is(name))?.Value;
    }


    void IDisposable.Dispose()
    {
        if( _bodyStream != null ) {
            _bodyStream.Dispose();
            _bodyStream = null;
        }
    }

    

}

/// <summary>
/// 
/// </summary>
public sealed class HttpContextAlone : NHttpContext
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly HttpContextAlone Instance = new HttpContextAlone();
    /// <summary>
    /// 
    /// </summary>
    public override object OriginalHttpContext => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public override NHttpRequest Request => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public override NHttpResponse Response => throw new NotImplementedException();
    /// <summary>
    /// 
    /// </summary>
    public override IPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <summary>
    /// 
    /// </summary>
    public override bool SkipAuthorization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <summary>
    /// 
    /// </summary>
    public override XDictionary Items => throw new NotImplementedException();

}
#endif
