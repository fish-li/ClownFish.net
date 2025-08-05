using System.Runtime.InteropServices;
using ClownFish.Base.Internals;

namespace ClownFish.WebClient;

/// <summary>
/// 表示一次HTTP请求的描述信息
/// </summary>
public sealed partial class HttpOption
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public HttpOption()
    {
        _method = "GET";
        Format = SerializeFormat.Form;

        // .NET默认的超时时间太长了，当出现故障时容易产生大量阻塞。 这里强制指定超时时间。
        Timeout = HttpClientDefaults.HttpClientTimeout;

#if NETCOREAPP
        CancellationToken = HttpClientDefaults.UseAppExitToken ? ClownFishInit.AppExitToken : CancellationToken.None;
#endif
    }

    /// <summary>
    /// 一个字符串，用于标识当前客户端请求，会记录到Oprlog日志中。配合MockResult一起使用可用于测试时模拟返回结果。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public object MockResult { get; set; }

    /// <summary>
    /// 最大允许的响应体长度，仅当: 设置值大于零 且以 byte[] 方式读取响应流 时执行检查。
    /// </summary>
    public long MaxResponseBodySize { get; set; }

    // 目前用于TxClientX的ExecuteHttp2/ExecuteHttp3命令中，防止用代理通道去下载一个大文件，出现OOM，相关代码如下：
    // http.CheckSuccessStatusCode = false;
    // http.IsProxyRequest = true;
    // http.MaxResponseBodySize = TxOptions.HttpProxy_MaxResponseBodySize;   // <<<######################
    // HttpResult<byte[]> httpResult = await http.GetResultAsync<HttpResult<byte[]>>();
    // uploadArgs.ResponseResult = (httpResult as IBinarySerializer).ToBytes().ToGzip().ToBase64();

#if NETCOREAPP

    /// <summary>
    /// CancellationToken
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// HttpCompletionOption
    /// </summary>
    public System.Net.Http.HttpCompletionOption CompletionOption { get; set; } = System.Net.Http.HttpCompletionOption.ResponseContentRead;

    /// <summary>
    /// HttpMessageHandler
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public System.Net.Http.HttpMessageHandler MessageHandler { get; set; }

    /// <summary>
    /// 是否需要在【非成功】响应状态码时主动抛出异常。默认值：由框架决定（只要返回值不是HttpWebResponse就检查状态码）
    /// </summary>
    public bool? CheckSuccessStatusCode { get; set; }

    /// <summary>
    /// IsProxyRequest
    /// </summary>
    public bool IsProxyRequest { get; set; }

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
    [JsonIgnore]
    [XmlIgnore]
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
    /// 【注意-注意】设置这个属性可能会导致底层的Socket连接不能重用，频繁使用会导致TCP端口耗尽，除非设置为NetworkCredential的实例。
    /// </summary>
    [JsonIgnore]
    [XmlIgnore]
    public ICredentials Credentials { get; set; }


    /// <summary>
    /// 获取或设置 HTTP调用的超时值（以毫秒为单位）。
    /// </summary>
    public int? Timeout { get; set; }


     /// <summary>
    /// 上传数据时，是否【尽量】采用gzip压缩，仅对文本类数据尝试启用gzip，包含：text, json, xml
    /// </summary>
    public bool AutoGzipUpload { get; set; }

#if NETFRAMEWORK

    /// <summary>
    /// 发送请求需要使用的代理
    /// </summary>
    public IWebProxy Proxy { get; set; }

    /// <summary>
    /// Request对象创建完成后的回调委托
    /// </summary>
    public Action<System.Net.HttpWebRequest> OnSetRequest { get; set; }
#else
    // Linux   .NET CORE/5/9  环境下设置 Proxy 需要通过指定 HttpMessageHandler 来实现，可参考 CreateClientHandler 方法

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
    /// GetReuestUri
    /// </summary>
    /// <returns></returns>
    public Uri GetReuestUri()
    {
        if( _requestUri == null ) {
            string url = this.GetRequestUrl();

            try {
                _requestUri = new Uri(url);
            }
            catch( UriFormatException ex ) {
                throw new UriFormatException("Invalid URL: " + url, ex);
            }
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
    public HttpOption SetBasicAuthorization(string username, string password)
    {
        this.Headers.Add("Authorization", "Basic " + (username + ":" + password).ToBase64());
        return this;
    }

    /// <summary>
    /// SetId
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public HttpOption SetId(string id)
    {
        this.Id = id;
        return this;
    }


}



public sealed partial class HttpOption : ILoggingObject, IToAllText, ITextSerializer  // 日志和文本序列化接口的实现
{
    /// <summary>
    /// 获取当前对象的日志展示文本
    /// </summary>
    /// <returns></returns>
    string ILoggingObject.ToLoggingText()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            LogLineAndHeaders(sb);

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
    string IToAllText.ToAllText()
    {
        return ToRawText(1);
    }

    /// <summary>
    /// 将HttpOption的各属性值转换成全文本形式。
    /// 注意：此方法返回的结果（在某些场景下）并不是实际发送的内容，因此结果仅供记录日志时使用。
    /// </summary>
    /// <param name="mode">0：不包含请求体数据，1：仅包含文本内容的请求体（可能需要序列化处理），2：包含请求体，不管数据是什么格式，3：日志场景使用。</param>
    /// <returns></returns>
    public string ToRawText(int mode = 0)
    {
        BodyStringResult bodyResult = this.GetPostBodyAsString(mode);

        StringBuilder sb = StringBuilderPool.Get();
        try {
            LogLineAndHeaders(sb, bodyResult.ContentType);

            if( bodyResult.IsBinData ) {
                sb.Append(BodyBinDataHeaderName).AppendLineRN(": 1");
            }

            sb.AppendLineRN();

            if( bodyResult.Text != null )
                sb.Append(bodyResult.Text);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    internal void LogLineAndHeaders(StringBuilder sb, string contentType = null)
    {
        // 填充【请求行】
        sb.Append(this.Method).Append(' ').Append(this.GetRequestUrl()).AppendLineRN(" HTTP/1.1");

        if( _headers != null ) {

            // 说明：如果用户在 _headers 中指定了不正确的ContentType请求头，那么最后得到的结果就不对了~~， 这里不管了~~
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

        if( this.UserAgent.HasValue() ) {
            sb.AppendLineRN($"{HttpHeaders.Request.UserAgent}: {this.UserAgent}");
        }

        if( HttpUtils.RequestHasBody(this.Method)
            && (_headers == null || _headers.ContainsKey(HttpHeaders.Request.ContentType) == false) ) {
            if( contentType == null ) {
                contentType = ContenTypeUtils.GetByFormat(this.Format);  // 这里得到的结果可能不正确~~，只适用于简单场景
            }
            if( contentType.HasValue() ) {
                sb.AppendLineRN($"{HttpHeaders.Request.ContentType}: {contentType}");
            }
        }
    }

    internal class BodyStringResult
    {
        public string Text;
        public string ContentType;
        public bool IsBinData;

        public static readonly BodyStringResult Empty = new BodyStringResult();

        public static BodyStringResult Create(string text, bool isBinData, string contentType)
        {
            return new BodyStringResult { Text = text, ContentType = contentType, IsBinData = isBinData };
        }
    }

    // mode: 0：不包含请求体数据，1：仅包含文本内容的请求体（可能需要序列化处理），2：包含请求体，不管数据是什么格式，3：日志场景使用。
    internal BodyStringResult GetPostBodyAsString(int mode)
    {
        if( mode < 0 || mode > 3 )
            throw new ArgumentOutOfRangeException(nameof(mode));

        if( mode == 0 )
            return BodyStringResult.Empty;

        object data = this.GetPostData();
        if( data == null )
            return BodyStringResult.Empty;

        if( data is string text ) {
            return BodyStringResult.Create(text, false, null);
        }

        if( mode == 3 ) {
            return GetPostBodyAsString3(data);
        }

        // 下面只处理 mode =1 or mode =2 的场景

        if( data is byte[] bb ) {
            if( mode == 2 )
                return BodyStringResult.Create(bb.ToBase64(), true, null);
            else  // mode == 1
                return BodyStringResult.Create($"##--NOT TEXT DATA, data-type: byte[], Length:({bb.Length})--##", true, null);
        }

        // data 是一个 Entity/DTO/object，需要根据 this.Format 做序列化
        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {

            RequestWriter writer = new RequestWriter();
            writer.Write(ms, data, this.Format, false);   // 生成日志时不做gzip

            bool isBinData = writer.IsBinaryData;
            string contentType = writer.ContentType;

            string contentType2 = contentType;
            if( contentType2.IsNullOrEmpty() )
                contentType2 = ContenTypeUtils.GetByFormat(this.Format);
            if( contentType2.IsNullOrEmpty() )
                contentType2 = this.Headers[HttpHeaders.Request.ContentType];

            if( HttpUtils.RequestBodyIsText(contentType2) ) {
                return BodyStringResult.Create(Encoding.UTF8.GetString(ms.ToArray()), isBinData, contentType);
            }
            else {   // 二进制数据
                if( mode == 2 )
                    return BodyStringResult.Create(ms.ToArray().ToBase64(), isBinData, contentType);
                else  // mode == 1
                    return BodyStringResult.Create($"##--NOT TEXT DATA, data-type: byte[], Length:({ms.Length})--##", isBinData, contentType);
            }
        }
    }

    private BodyStringResult GetPostBodyAsString3(object data)
    {
        // data 肯定不是 string

        if( data is byte[] bb ) {
            if( bb.Length <= 4096 )
                return BodyStringResult.Create(bb.ToBase64(), true, null);
            else
                return BodyStringResult.Create($"##--NOT TEXT DATA, data-type: byte[], Length:({bb.Length})--##", true, null);
        }

        if( data is FormDataCollection || data is Stream )
            return BodyStringResult.Create($"##--NOT TEXT DATA, data-type:({data.GetType().FullName})--##", true, null);
        else
            return BodyStringResult.Create(data.ToJson(), false, null);
    }

    //internal static readonly string BodyBinDataPrefix = "bin-data/base64:";
    // 加前缀的问题在于，生成时要【拼接字符串】，解析时要【裁剪字符串】，都是低效操作！

    internal static readonly string BodyBinDataHeaderName = "[BODY-IS-BIN]";

    // raw-text 示例数据：
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


    /// <summary>
    /// 根据原始请求信息文本构建 HttpOption 对象（格式可参考Fiddler的Inspectors标签页内容）
    /// 注意：1，此方法会忽略部分请求头及内容，涉及范围：Content-Length, Connection
    /// 2，对于 二进制 提交数据，此方法可能并不能正确识别。
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static HttpOption FromRawText(string text)
    {
        HttpOption httpOption = new HttpOption();
        FillFromRawText(httpOption, text);

        return httpOption;
    }

    private static readonly char[] s_trimChars = new char[] { '\r', '\n' };
    private static void FillFromRawText(HttpOption httpOption, string text)
    {
        if( string.IsNullOrEmpty(text) )
            throw new ArgumentNullException("text");

        // 放弃构造方法中的默认值格式，因为请求头中可能会指定
        httpOption.Format = SerializeFormat.None;

        // 注意：这里不能调用 text.Trim() 它会去掉一些空格，导致在个别特殊场景下出现问题，可参考测试用例 Test_EmptyValueHeader
        // 补充：按理说，根本就不需要调用 Trim 方法，主要是为了兼容一些测试用例，那些测试用例为了排版清爽，头尾都加了空格，可搜索 “根据Raw文本发送请求”
        using( StringReader reader = new StringReader(text.Trim(s_trimChars)) ) {

            // 设置请求方法和URL
            PopulateRequestLine(httpOption, reader.ReadLine());

            // 读取请求头
            PopulateHeaders(httpOption, reader);

            // 读取请求体数据
            string postText = reader.ReadToEnd();
            // 设置提交数据
            PopulatePostData(httpOption, postText);
        }
    }

    internal static void PopulateRequestLine(HttpOption httpOption, string requestLine)
    {
        int p1 = requestLine.IndexOf(' ');
        int p2 = requestLine.LastIndexOf(' ');

        if( p1 < 0 || p1 == p2 )
            throw new ArgumentException($"不能识别的请求文本格式，开始行：[{requestLine}]");

        // 设置请求方法，GET OR POST
        httpOption.Method = requestLine.Substring(0, p1);


        // 不使用HTTP协议版本，只做校验。
        string httpVersion = requestLine.Substring(p2 + 1);
        if( httpVersion.StartsWith("HTTP/", StringComparison.Ordinal) == false )
            throw new ArgumentException($"不能识别的请求文本格式，开始行：[{requestLine}]");

        httpOption.Url = requestLine.Substring(p1 + 1, p2 - p1 - 1);
    }

    internal static void PopulateHeaders(HttpOption httpOption, StringReader reader)
    {
        string line = null;
        while( (line = reader.ReadLine()) != null ) {
            if( line.Length > 0 ) {
                // 处理请求头
                int p3 = line.IndexOf(':');
                if( p3 > 0 ) {
                    string name = line.Substring(0, p3);

                    // 这个头直接丢弃，因为文本在计算二进制时会随着编码不同而变化
                    if( name.EqualsIgnoreCase("Content-Length") )
                        continue;

                    // 这里强制要求的请求头格式： "name: value" ，中间一个冒号加一个空格，如果格式不正确，有可能会出现异常！
                    string value = line.Substring(p3 + 2);
                    // line.Substring(p3 + 1).TrimTrimStart(' ')  这种写法会造成无意义的性能浪费，所以不采用！

                    if( name.Is("Connection") ) {
                        // Connection 头有二个可选值，Keep-Alive  or  close
                        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Connection
                        // 但是，几乎不会使用 Connection: close，所以直接忽略这个头
                        // 因为，程序不可能确定说某个站点或者服务，一个很长的时间范围内只访问一次，因此设置为 close 就没有意义了！
                        continue;
                    }

#if NET6_0_OR_GREATER
                    if( name == "--unix-socket" ) {
                        httpOption.UnixSocketEndPoint = value;
                        continue;
                    }
#endif
                    if( value.HasValue() ) {
                        httpOption.Headers.Add(name, value);
                    }
                }
                else {
                    throw new ArgumentException($"不能识别的请求文本格式，请求头：[{line}]");
                }
            }
            else {  // line.Length == 0
                // 空行，表示请求头已读完
                break;
            }
        }


        // 纠正一些请求头数据
        FixContentTypeCharset(httpOption);
    }


#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void FixContentTypeCharset(HttpOption httpOption)
    {
        // 可能的输入格式  Content-Type: xxxxxxx; charset=gb2312
        // 此时强制修改为  Content-Type: xxxxxxx; charset=utf-8

        string contentType = httpOption.Headers[HttpHeaders.Request.ContentType];
        if( contentType.HasValue() ) {
            if( System.Net.Http.Headers.MediaTypeHeaderValue.TryParse(contentType, out var mediaType) ) {
                if( mediaType.CharSet.HasValue() && mediaType.CharSet.Is("utf-8") == false ) {
                    System.Net.Http.Headers.MediaTypeHeaderValue mediaType2 = new(mediaType.MediaType, "utf-8");
                    httpOption.Headers[HttpHeaders.Request.ContentType] = mediaType2.ToString();
                }
            }
        }
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void FixContentTypeCharset(HttpOption httpOption)
    {
        string contentType = httpOption.Headers[HttpHeaders.Request.ContentType];
        if( contentType.HasValue() ) {
            Match m = s_charsetReg.Match(contentType);
            if( m.Success && m.Groups["name"].Value.Is("utf-8") == false ) {
                string value = s_charsetReg.Replace(contentType, "; charset=utf-8");
                httpOption.Headers[HttpHeaders.Request.ContentType] = value;
            }
        }
    }

    private static readonly Regex s_charsetReg = new Regex(";\\s?charset=(?<name>[\\w\\-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
#endif


    private static void PopulatePostData(HttpOption httpOption, string postText)
    {
        if( string.IsNullOrEmpty(postText) == false ) {

            string isBinData = httpOption.Headers[BodyBinDataHeaderName];

            if( isBinData.HasValue() )
                httpOption.Headers.Remove(BodyBinDataHeaderName);

            if( isBinData == "1" ) {
                try {
                    httpOption.Data = Convert.FromBase64String(postText);
                }
                catch {  // $"##--NOT TEXT DATA, Length:({bb.Length})--##";
                    httpOption.Data = postText;
                }
            }
            else {
                httpOption.Data = postText;
            }
        }
    }

    string ITextSerializer.ToText()
    {
        return ToRawText(2);
    }

    void ITextSerializer.LoadData(string text)
    {
        FillFromRawText(this, text);
    }
}



#if NETCOREAPP

public sealed partial class HttpOption : IBinarySerializer  // 二进制序列化相关实现
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetStartLineAndHeaders(string contentType)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            LogLineAndHeaders(sb, contentType);
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetPostBodyBytes(out bool isBinData, out string contentType)
    {
        isBinData = false;
        contentType = null;

        object data = this.GetPostData();
        if( data == null )
            return Empty.Array<byte>();

        if( data is string text ) {
            return text.ToUtf8Bytes();
        }

        if( data is byte[] bb ) {
            isBinData = true;
            return bb;
        }

        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {

            RequestWriter writer = new RequestWriter();
            writer.Write(ms, data, this.Format, false);   // 不做gzip

            contentType = writer.ContentType;
            isBinData = writer.IsBinaryData;
            return ms.ToArray();
        }
    }

    private static void PopulatePostData(HttpOption httpOption, byte[] postData, int bodyDataType)
    {
        if( postData.HasValue() ) {

            if( bodyDataType == 1 ) {  // 二进制数据
                httpOption.Data = postData;
            }
            else {
                httpOption.Data = postData.ToUtf8String();
            }
        }
    }

    byte[] IBinarySerializer.ToBytes()
    {
        byte[] body = GetPostBodyBytes(out bool bodyIsBinData, out string contentType);

        string startLineAndHeaders = GetStartLineAndHeaders(contentType);


        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {

            // 写入 "开始行和请求头"
            byte[] b1 = Encoding.UTF8.GetBytes(startLineAndHeaders);
            byte[] lenBytes = BitConverter.GetBytes(b1.Length);  // 长度固定为 4
            ms.Write(lenBytes, 0, lenBytes.Length);
            ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
            ms.Write(b1, 0, b1.Length);

            // 写入 "数据类型标志"
            int bodyDataType = bodyIsBinData ? 1 : 0;
            byte[] dataTypeBytes = BitConverter.GetBytes(bodyDataType);  // 长度固定为 4
            ms.Write(dataTypeBytes, 0, dataTypeBytes.Length);
            ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读

            // 写入 "请求体"
            byte[] b2 = body;
            lenBytes = BitConverter.GetBytes(b2.Length);  // 长度固定为 4
            ms.Write(lenBytes, 0, lenBytes.Length);
            ms.WriteByte((byte)'\n');  // 在文本情况下方便阅读
            ms.Write(b2, 0, b2.Length);

            return ms.ToArray();
        }
    }


    void IBinarySerializer.LoadData(ReadOnlyMemory<byte> body)
    {
        if( body.Length == 0 )
            return;

        // 放弃构造方法中的默认值格式，因为请求头中可能会指定
        this.Format = SerializeFormat.None;

        int start = 0;
        ReadOnlySpan<byte> span = body.Span;


        // 读取 “开始行和请求头” 的长度
        int len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 5;  // 5 = 4 + 1

        // 读取 “开始行和请求头”  二进制数据
        ReadOnlySpan<byte> data = span.Slice(start, len);
        start += len;

        string text1 = Encoding.UTF8.GetString(data);
        using( StringReader reader = new StringReader(text1) ) {
            PopulateRequestLine(this, reader.ReadLine());   //==========================1
            PopulateHeaders(this, reader);                  //==========================2
        }

        // -------------------------------------------------------

        int bodyDataType = BitConverter.ToInt32(span.Slice(start, 4));
        start += 5;  // 5 = 4 + 1


        // 读取“请求体”的长度
        len = BitConverter.ToInt32(span.Slice(start, 4));
        start += 5;  // 5 = 4 + 1

        if( len > 0 ) {
            // 读取“请求体” 二进制数据
            data = span.Slice(start, len);

            byte[] postData = data.ToArray();
            PopulatePostData(this, postData, bodyDataType);   //==========================3
        }
    }

}
#endif
