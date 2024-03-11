#if NETFRAMEWORK

namespace ClownFish.WebClient.V1;

/// <summary>
/// 一个用于发送HTTP请求的客户端（适用于 .net framework）
/// </summary>
/// <example>
/// HttpOption option = new HttpOption{
///     Method = "POST",
///     Url = "http://xxxxxxxxxxxxxxxxxxxxxx",
///     Data = new { aa = 2, bb = 3 },
///     Format = SerializeFormat.Json
/// };
/// var result = option.GetResult();
/// </example>
internal sealed class HttpClient : BaseHttpClient
{ 
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="option"></param>
    public HttpClient(HttpOption option) : base(option)
    {
    }



    /// <summary>
    /// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求
    /// </summary>
    /// <typeparam name="T">返回值的类型参数</typeparam>
    /// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
    public override T Send<T>()
    {
        this.Request = CreateWebRequest();
        SetRequest();

        // 触发【发送前】事件
        this.BeforeSend();

        // 发出HTTP请求，获取响应
        HttpWebResponse response = GetResponse();

        return ReturnResult<T>(response);
    }
           

    /// <summary>
    /// 根据指定的HttpOption参数，用【异步】方式发起一次HTTP请求
    /// </summary>
    /// <typeparam name="T">返回值的类型参数</typeparam>
    /// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
    public override async Task<T> SendAsync<T>()
    {
        this.Request = CreateWebRequest();
        SetRequest();

        // 触发【发送前】事件
        this.BeforeSend();            

        // 发出HTTP请求，获取响应
        HttpWebResponse response = await GetResponseAsync();

        return ReturnResult<T>(response);
    }

    private void SetRequest()
    {
        // 添加链路日志相关请求头
        HttpTraceUtils.SetTraceHeader(this);

        this.HttpOption.OnSetRequest?.Invoke(this.Request);
    }

    private T ReturnResult<T>(HttpWebResponse response)
    {
        Type resultType = typeof(T);

        if( resultType == typeof(HttpWebResponse) )
            return (T)(object)response;


        bool skipDispose = resultType == typeof(Stream) || resultType == typeof(HttpResult<Stream>);

        try {
            return GetResult<T>(response);
        }
        finally {
            if( skipDispose == false )
                response.Dispose();
        }
    }

    private HttpWebResponse GetResponse()
    {
        this.SetStartTime();
        SetRequestData();

        try {
            HttpWebResponse response = (HttpWebResponse)this.Request.GetResponse();

            // 引发结束事件
            this.RequestFinished(response, null);

            return response;
        }
        catch( Exception ex ) {

            // 有可能会是 WebException 或者其它类型的异常
            // 如果直接将异常抛出，那么在日志中根本不知道当前是调用的哪个URL地址，所以这里包装成一个新异常类型再抛出
            RemoteWebException ex2 = new RemoteWebException(ex, this.HttpOption.Url);
            this.RequestFinished(null, ex);
            throw ex2;
        }
    }


    private async Task<HttpWebResponse> GetResponseAsync()
    {
        this.SetStartTime();
        await SetRequestDataAsync();

        try {
            HttpWebResponse response = (HttpWebResponse)await this.Request.GetResponseAsync();

            // 引发结束事件
            this.RequestFinished(response, null);

            return response;
        }
        catch( Exception ex ) {

            // 有可能会是 WebException 或者其它类型的异常
            // 如果直接将异常抛出，那么在日志中根本不知道当前是调用的哪个URL地址，所以这里包装成一个新异常类型再抛出
            RemoteWebException ex2 = new RemoteWebException(ex, this.HttpOption.Url);
            this.RequestFinished(null, ex);
            throw ex2;
        }
    }



#region 发送过程


    private HttpWebRequest CreateWebRequest()
    {
        // 触发事件，允许在发送请求前修改某些参数
        this.BeforeCreateRequest();

        HttpOption option = this.HttpOption;
        HttpWebRequest request = null;
        string url = this.HttpOption.GetRequestUrl();

        try {                
            request = WebRequest.CreateHttp(url);
        }
        catch( Exception ex ) {
            throw new InvalidOperationException("不能根据指定的URL创建HttpWebRequest实例，URL：" + url, ex);
        }

        request.Method = option.Method;
        request.ServicePoint.Expect100Continue = false;            
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        if( option.AllowAutoRedirect.HasValue )
            request.AllowAutoRedirect = option.AllowAutoRedirect.Value;

        if( option.KeepAlive.HasValue )
            request.KeepAlive = option.KeepAlive.Value;

        foreach( NameValue item in option.Headers )
            request.Headers.InternalAdd(item.Name, item.Value);

        if( option.Cookie != null )
            request.CookieContainer = option.Cookie;

        if( option.Credentials != null )
            request.Credentials = option.Credentials;

        if( option.Timeout.HasValue )
            request.Timeout = option.Timeout.Value > 0
                              ? option.Timeout.Value
                              : System.Threading.Timeout.Infinite;


        if( request.Headers[HttpHeaders.Request.UserAgent] == null ) {
            if( string.IsNullOrEmpty(request.UserAgent) == false )
                request.UserAgent = option.UserAgent;
            else
                request.UserAgent = ConstValues.HttpClientUserAgent;
        }

        return request;
    }
    
    private void SetRequestData()
    {
        object data = this.HttpOption.GetPostData();
        if( data == null )
            return;

        using( Stream stream = this.Request.GetRequestStream() ) {
            WriteRequestStream(stream, data);
        }
    }

    private async Task SetRequestDataAsync()
    {
        object data = this.HttpOption.GetPostData();
        if( data == null )
            return;

        using( Stream stream = await this.Request.GetRequestStreamAsync() ) {
            WriteRequestStream(stream, data);
        }
    }

    private void WriteRequestStream(Stream destStream, object data)
    {
        //if( this.Request.Method.EqualsIgnoreCase("GET") )
        //    this.Request.Method = "POST";

        // 下面2种进制数据(Stream, byte[])就直接处理，因为它们不需要序列化或者编码，所以就不使用RequestWriter
        // 可能会有一个小缺陷：data是二进制数据，但是 Format 却是 Multipart，这种情况需要调用方自己覆盖ContentType

        if( data is Stream srcStream ) {
            SetRequestStream1(destStream, srcStream);
            return;
        }

        if( data is byte[] bytes ) {
            SetRequestStream2(destStream, bytes);
            return;
        }


        RequestWriter writer = new RequestWriter();
        writer.Write(destStream, data, this.HttpOption.Format);

        if( writer.ContentType.IsNullOrEmpty() == false )
            this.Request.ContentType = writer.ContentType;
    }


    private void SetRequestStream1(Stream destStream, Stream srcStream)
    {
        if( srcStream.CanRead == false )
            throw new ArgumentException("指定的数据流不能读取。");

        if( srcStream.CanSeek )
            srcStream.Position = 0;

        srcStream.CopyTo(destStream);

        string contentType = ContenTypeUtils.GetByFormat(this.HttpOption.Format);
        if( contentType.IsNullOrEmpty() == false )
            this.Request.ContentType = contentType;
    }

    private void SetRequestStream2(Stream destStream, byte[] bytes)
    {
        destStream.Write(bytes, 0, bytes.Length);

        string contentType = ContenTypeUtils.GetByFormat(this.HttpOption.Format);
        if( contentType.IsNullOrEmpty() == false )
            this.Request.ContentType = contentType;
    }


#endregion

}


#endif
