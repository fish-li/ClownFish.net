using System.Runtime.InteropServices;
using ClownFish.Base.Internals;

namespace ClownFish.WebClient;

/// <summary>
/// 表示一次HTTP请求的描述信息
/// </summary>
public sealed class HttpOption : ILoggingObject, IToAllText
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public HttpOption()
    {
        _method = "GET";
        Format = SerializeFormat.Form;

        // .NET默认的超时时间太长了，当出现故障时容易产生大量阻塞。 这里强制指定超时时间。
        Timeout = HttpClientDefaults.HttpTimeout;

#if NETCOREAPP
        CancellationToken = CancellationToken.None;
#endif
    }

#if NETCOREAPP

    /// <summary>
    /// CancellationToken
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// HttpCompletionOption
    /// </summary>
    public System.Net.Http.HttpCompletionOption CompletionOption { get; set; } = System.Net.Http.HttpCompletionOption.ResponseContentRead;

    /// <summary>
    /// HttpMessageHandler
    /// </summary>
    public System.Net.Http.HttpMessageHandler MessageHandler { get; set; }

#endif

#if NET6_0_OR_GREATER

    private string _unixSocketEndPoint;

    /// <summary>
    /// 例如："/var/run/docker.sock"
    /// </summary>
    public string UnixSocketEndPoint {
        get { return _unixSocketEndPoint; }
        set {
            if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
                throw new NotSupportedException("当前操作系统不支持此特性！");
            _unixSocketEndPoint = value;
        }
    }

#endif


    /// <summary>
    /// 标记当前对象已经执行了一次发送HTTP请求的任务。
    /// 由于性能日志的设计需要，HttpOption 是不允许重用的，所以增加 Finished 来检测是否重用
    /// </summary>
    internal bool Finished;

    /// <summary>
    /// URL地址（建议查询字符串参数在Data属性中指定，此处只指定文件路径即可）
    /// </summary>
    public string Url { get; set; }


    private string _method;
    /// <summary>
    /// HTTP请求的方法，例如： GET, POST
    /// </summary>
    public string Method {
        get { return _method; }
        set {
            if( string.IsNullOrEmpty(value) )
                throw new ArgumentNullException("value");
            _method = value.NameToUpper();
        }
    }


    private HttpHeaderCollection _headers;

    /// <summary>
    /// 请求头集合。
    /// 允许为当前属性指定一个 Dictionary《string, string》 类型的变量
    /// </summary>
    public HttpHeaderCollection Headers {
        get {
            if( _headers == null )
                _headers = new HttpHeaderCollection();
            return _headers;
        }
        set {
            if( value == null )
                throw new ArgumentNullException(nameof(value));

            _headers = value;
        }
    }


    /// <summary>
    /// 这个属性只能赋值，等同于给 Headers 属性赋值。差别在于这二个属性的类型不一样。
    /// 建议：给当前属性指定一个 匿名对象。属性名做为请求头的 NAME，值做为VALUE。
    /// 说明：如果属性名中包含【下划线】，生成的请求头中将变成【中横线】 例如：prefix_name =》 "prefix-name"
    /// </summary>
    public object Header {
        // 定义一个【只写属性】不是好的设计方式！
        // 这里没有办法，因为 C# 编译器不允许 从 object 到 HttpHeaderCollection 的类型转换，所以不能沿用 Headers ，只能再定义一个变量

        set {
            if( value == null )
                throw new ArgumentNullException(nameof(value));

            _headers = HttpHeaderCollection.Create(value);
        }
    }


    /// <summary>
    /// 需要提交的数据（与 $.ajax()方法的 Data 属性含义类似），
    /// 可指定一个FormDataCollection实例，或者一个 IDictionary实例，或者一个匿名对象实例
    /// 如果是GET请求，数据会自动转变成查询字参数，如果是POST，则随请求体发送
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 数据的序列化方式。相当于指定 Content-Type 请求头。
    /// 注意：不包含请求体的请求，不需要指定这个属性，例如：GET , HEAD
    /// </summary>
    public SerializeFormat Format { get; set; }

    // 不使用 ContentType 的原因有三点：
    // 1，ContentType 是个【长】字符串，容易写错，
    // 2，Json, Json2 这样的序列列没法表达
    // 3，限制范围，只允许枚举定义的几种取值


    /// <summary>
    /// 在发送请求时指定 User-Agent 头。
    /// 如果在请求头中已指定 User-Agent，那么忽略这个设置。
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// 是否允许自动重定向
    /// </summary>
    public bool? AllowAutoRedirect { get; set; }

    /// <summary>
    /// Cookie容器
    /// </summary>
    public CookieContainer Cookie { get; set; }


    /// <summary>
    /// 直接指定要发送什么 COOKIE，通常用于不需要接收Cookier场景
    /// 注意：
    /// 1、如果需要接收Cookie，请设置 Cookie 属性，
    /// 2、cookieHeader的数据需要自行编码
    /// </summary>
    /// <param name="cookieHeader">要发送的COOKIE头内容</param>
    internal HttpOption SetCookieHeader(string cookieHeader)
    {
        this.Headers.Add("Cookie", cookieHeader);
        return this;
    }

    /// <summary>
    /// 获取或设置请求的身份验证信息。
    /// </summary>
    public ICredentials Credentials { get; set; }


    /// <summary>
    /// 获取或设置 HTTP调用的超时值（以毫秒为单位）。
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// KeepAlive
    /// </summary>
    public bool? KeepAlive { get; set; }


    //public IWebProxy Proxy { get; set; }  // TODO: 以后再支持


    /// <summary>
    /// 在读取响应流时自动做解压缩处理
    /// </summary>
    public bool AutoDecompressResponse { get; set; }


#if NETFRAMEWORK
    /// <summary>
    /// Request对象创建完成后的回调委托
    /// </summary>
    public Action<System.Net.HttpWebRequest> OnSetRequest { get; set; }
#else
    /// <summary>
    /// Request对象创建完成后的回调委托
    /// </summary>
    public Action<System.Net.Http.HttpRequestMessage> OnSetRequest { get; set; }
#endif

    /// <summary>
    /// 检查传入的属性是否存在冲突的设置
    /// </summary>
    internal void CheckInput()
    {
        if( string.IsNullOrEmpty(this.Url) )
            throw new ArgumentNullException("Url");

        //if( (Method == "GET" || Method == "HEAD") && Format != SerializeFormat.Form )
        //	throw new InvalidOperationException("GET, HEAD 请求只能采用 FORM 序列化方式。");
    }


    /// <summary>
    /// 获取实际的请求址。
    /// 如果是GET请求，将会包含提交数据。
    /// </summary>
    /// <returns></returns>
    public string GetRequestUrl()
    {
        string requestUrl = this.Url;

        // 如果有提交数据，并且是 GET 请求，就需要将参数合并到URL，形成查询字符串参数
        if( this.Data != null && HttpUtils.RequestHasBody(this.Method) == false ) {
            if( this.Url.IndexOf('?') < 0 )
                requestUrl = this.Url + "?" + GetQueryString(this.Data);
            else
                requestUrl = this.Url + "&" + GetQueryString(this.Data);
        }

        return requestUrl;
    }

    private Uri _requestUri;
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Uri GetReuestUri()
    {
        if( _requestUri == null ) {
            string url = this.GetRequestUrl();
            _requestUri = new Uri(url);
        }
        return _requestUri;
    }


    /// <summary>
    /// 将一个对象的Name/Value生成查询字符串参数
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string GetQueryString(object data)
    {
        if( data == null )
            return null;

        if( data.GetType() == typeof(string) )
            return (string)data;

        return FormDataCollection.GetQueryString(data);
    }


    /// <summary>
    /// 获取需要提交的数据。
    /// 如果已指定要提交的数据，但是是GET请求，那么也认为是没有提交数据。
    /// </summary>
    /// <returns></returns>
    public object GetPostData()
    {
        if( this.Data != null && HttpUtils.RequestHasBody(this.Method) )
            return this.Data;
        else
            return null;
    }


    /// <summary>
    /// 设置 Basic-Authorization 请求头
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void SetBasicAuthorization(string username, string password)
    {
        this.Headers.Add("Authorization", "Basic " + (username + ":" + password).ToBase64());
    }


    /// <summary>
    /// 获取当前对象的日志展示文本
    /// </summary>
    /// <returns></returns>
    public string ToLoggingText()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            FillLineAndHeaders(sb);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// 将一个对象的所有信息全部转成文本形式输出
    /// </summary>
    /// <returns></returns>
    public string ToAllText()
    {
        return ToRawText(1);
    }

    /// <summary>
    /// 转换成全文本形式。
    /// 建议：仅在记录日志时才调用当前方法。
    /// </summary>
    /// <param name="mode">0：不包含请求体数据，1：仅仅包含文本内容的请求体，2：包含请求体，不管数据是什么格式。</param>
    /// <returns></returns>
    public string ToRawText(int mode = 1)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            FillLineAndHeaders(sb);
            sb.AppendLineRN();

            string body = this.GetPostBodyAsString(mode);
            if( body != null )
                sb.Append(body);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    private string GetPostBodyAsString(int mode)
    {
        if( mode == 0 )
            return null;

        object data = this.GetPostData();
        if( data == null )
            return null;

        // 有可能 contetType 在请求头中直接指定的，而不是设置的 Format 属性
        string contetType = RequestContentType.GetByFormat(this.Format);

        if( contetType.IsNullOrEmpty() )
            contetType = this.Headers[HttpHeaders.Request.ContentType];



        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {

            RequestWriter writer = new RequestWriter();
            writer.Write(ms, data, this.Format);

            if( HttpUtils.RequestBodyIsText(contetType) ) {
                return Encoding.UTF8.GetString(ms.ToArray());
            }

            if( mode == 2 )
                return "已将二进制数据转成Base64字符串\r\n" + ms.ToArray().ToBase64();
            else  // mode == 1
                return "## 一大堆二进制数据 ##";
        }
    }


    private void FillLineAndHeaders(StringBuilder sb)
    {
        // 填充【请求行】
        sb.Append(this.Method).Append(' ').Append(this.GetRequestUrl()).AppendLineRN(" HTTP/1.1");


        if( _headers != null ) {
            foreach( var x in _headers ) {
                sb.AppendLineRN($"{x.Name}: {x.Value}");
            }
        }

        if( this.Cookie != null ) {
            string value = this.Cookie.GetCookieHeader(new Uri(this.Url));
            if( value.HasValue() ) {
                sb.AppendLineRN($"Cookie: {value}");
            }
        }

        if( HttpUtils.RequestHasBody(this.Method) ) {
            string contentType = RequestContentType.GetByFormat(this.Format);
            if( contentType.IsNullOrEmpty() == false )
                sb.AppendLineRN($"{HttpHeaders.Request.ContentType}: {contentType}");
        }

        if( this.UserAgent.HasValue() )
            sb.AppendLineRN($"{HttpHeaders.Request.UserAgent}: {this.UserAgent}");

    }



    /// <summary>
    /// 根据原始请求信息文本构建 HttpOption 对象（格式可参考Fiddler的Inspectors标签页内容）
    /// 注意：此方法会忽略部分请求头及内容，涉及范围：Content-Length, Connection, Content-Type
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static HttpOption FromRawText(string text)
    {
        // text参数的 示例数据：
        //POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1
        //Host: www.fish-test.com
        //User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
        //Accept: */*
        //Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
        //Accept-Encoding: gzip, deflate
        //Content-Type: application/x-www-form-urlencoded; charset=UTF-8
        //X-Requested-With: XMLHttpRequest
        //Referer: http://www.fish-test.com/Pages/Demo/TestAutoFindAction.htm
        //Content-Length: 72
        //Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;
        //Connection: keep-alive
        //Pragma: no-cache
        //Cache-Control: no-cache

        //input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81

        if( string.IsNullOrEmpty(text) )
            throw new ArgumentNullException("text");

        HttpOption httpOption = new HttpOption();

        // 放弃构造方法中的默认值格式，因为请求头中可能会指定
        httpOption.Format = SerializeFormat.None;

        using( StringReader reader = new StringReader(text.Trim()) ) {

            // 设置请求方法和URL
            httpOption.SetRequestLine(reader.ReadLine());

            // 读取请求头
            httpOption.SetHeaders(reader);

            // 读取请求体数据
            string postText = reader.ReadToEnd();
            if( string.IsNullOrEmpty(postText) == false )
                httpOption.Data = postText;
        }

        // 纠正一些请求头数据
        httpOption.FixHeaders();

        return httpOption;
    }


}
