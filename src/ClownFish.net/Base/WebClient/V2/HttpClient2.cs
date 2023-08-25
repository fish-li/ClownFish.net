#if NETCOREAPP
using System.Net.Http;
using MyHttpOption = ClownFish.Base.WebClient.HttpOption;


// MS 在 .net core 中挖了个大坑，……
// 目前 System.Net.Http.HttpClient 存在一个“不规范设计”，很容易造成“不恰当”地使用，可参考以下链接的描述：
// https://www.oschina.net/news/77036/httpclient
// https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/

// 不仅如此，MS为了保留 HttpWebRequest 这套API，就直接利用HttpClient做了个很马虎的包装，
// 编译是通过了，但是大量使用HttpWebRequest之后，你就能遇到上面所说的这个缺陷！

// 按MS的说法，根本原因还是大家对于HttpClient的使用方式不对！？？
// 因为现在为了支持HTTP2，使用了长连接，而且在内部有连接池，所以每次dispose是不合适（因此dispose也不起作用），
// MS的建议：应该将HttpClient实例设计为可重复使用（静态引用），但是这样又可能造成DNS不能更新的新问题。
// 以至于 asp.net core 团队还设计了一套新的解决方案 HttpClientFactory 来挽救。

// 对于 HttpClientFactory，这个设计给我的感觉是为小项目设计的！  这里不想过多评价…………
// 在新版本的 HttpWebRequest 已经有了一些改进，但是限制很多（5个参数不能指定，13个参数必完全一样），
// 可参考：https://github.com/dotnet/runtime/commit/e63b895c1fa93848b2f696558d3a75f02fd7a67d
// 所以，无奈之下，只能自己动手解决了，
// 过程中参考了 HttpClientFactory 以及新版本的 HttpWebRequest

// WebClient2 这个命名空间就是完善 System.Net.Http.HttpClient 的一个解决方案
// 由于命名冲突以及强类型的关系，所以不能再使用 ClownFish.Base.WebClient 这个命名空间了
// 而且为了便于区分，类型名称中也使用了 XXX2 的命名方式。

// WebClient2与HttpClientFactory的差异：
// 1、不需要提前配置：程序运行中根据（7个）请求参数生成缓存键，不需要大量的“固化”初始化代码，增加新的调用站点完全无感。
// 2、缓存的对象是HttpClient实例，而不是HttpClientHandler，减少GC压力
// 3、HttpClientFactory不支持CookieContainer


namespace ClownFish.Base.WebClient.V2;

/// <summary>
/// 一个用于发送HTTP请求的客户端（适用于 .net core）。
/// </summary>
/// <example>
/// HttpOption option = new HttpOption{
///     Method = "POST",
///     Url = "http://xxxxxxxxxxxxxxxxxxxxxx",
///     Data = new { aa = 2, bb = 3 },
///     Format = ClownFish.Base.Http.SerializeFormat.Json
/// };
/// var result = option.GetResult();
/// </example>
internal sealed partial class HttpClient2 : ClownFish.Base.WebClient.BaseHttpClient
{
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="option"></param>
    public HttpClient2(MyHttpOption option) : base(option)
    {
    }



    /// <summary>
    /// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求
    /// </summary>
    /// <typeparam name="T">返回值的类型参数</typeparam>
    /// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
    public override T Send<T>()
    {
        // 创建请求消息对象，包含设置提交数据
        this.BeforeCreateRequest();
        this.Request = HttpObjectUtils.CreateRequestMessage(this.HttpOption);

        bool clientFromCache = this.HttpOption.IsClientEnableCached();
        HttpClient client = MsHttpClientCache.GetCachedOrCreate(this.HttpOption, clientFromCache);
        try {
            SetRequest(clientFromCache);

            // 触发【发送前】事件
            this.BeforeSend();

            // 发出HTTP请求，获取响应                
            bool checkStatus = typeof(T) != typeof(HttpWebResponse);
            HttpWebResponse response = GetResponse(client, checkStatus);

            return ReturnResult<T>(response);
        }
        finally {
            if( clientFromCache == false )
                client?.Dispose();
        }
    }

    /// <summary>
    /// 根据指定的HttpOption参数，用【同步】方式发起一次HTTP请求
    /// </summary>
    /// <typeparam name="T">返回值的类型参数</typeparam>
    /// <returns>返回服务端的调用结果，并转换成指定的类型</returns>
    public override async Task<T> SendAsync<T>()
    {
        // 创建请求消息对象，包含设置提交数据
        this.BeforeCreateRequest();
        this.Request = HttpObjectUtils.CreateRequestMessage(this.HttpOption);

        bool clientFromCache = this.HttpOption.IsClientEnableCached();
        HttpClient client = MsHttpClientCache.GetCachedOrCreate(this.HttpOption, clientFromCache);
        try {
            SetRequest(clientFromCache);

            // 触发【发送前】事件
            this.BeforeSend();

            // 发出HTTP请求，获取响应                
            bool checkStatus = typeof(T) != typeof(HttpWebResponse);
            HttpWebResponse response = await GetResponseAsync(client, checkStatus);

            return ReturnResult<T>(response);
        }
        finally {
            if( clientFromCache == false )
                client?.Dispose();
        }
    }


    private void SetRequest(bool clientFromCache)
    {
        // 如果 clientFromCached == false，表示HttpClient需要在用过后释放，那么连接也就没有必要保持，即：keepAlive = false
        bool keepAlive = clientFromCache;
        this.Request.SetKeepAlive(keepAlive);

        // 设置Cookie头
        SetRequestCookie();

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

    private void SetRequestCookie()
    {
        // 虽然 System.Net.Http.HttpClient 那套东西也能自动处理 CookieContainer，
        // 如果使用那个设计，就需要设置 clientHandler.CookieContainer
        // 但是这样设计与 clientHandler 的“复用模式”是有冲突的，
        // 因为Cookie表示一个用户的会话过程，是不能被复用的！

        // 也正是因为这个原因，HttpClientFactory 不能支持 CookieContainer
        // 可参考：https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1
        // 还有人提了个很好的建议：https://github.com/dotnet/runtime/issues/1904

        // 所以，为了解决这个问题，这里将自行处理Cookie，
        // 整个过程其实也不复杂，就二点：
        // 1，发送请求时，设置Cookie头，
        // 2，接收响应时，读取Set-Cookie头，供下次使用


        if( this.HttpOption.Cookie == null )
            return;

        Uri requestUri = this.HttpOption.GetReuestUri();
        string header = this.HttpOption.Cookie.GetCookieHeader(requestUri);
        if( header.IsNullOrEmpty() )
            return;

        this.Request.Headers.TryAddWithoutValidation("Cookie", header);
    }

    private void SaveResponseCookie(HttpResponseMessage responseMessage)
    {
        if( this.HttpOption.Cookie == null )
            return;

        IEnumerable<string> cookies;
        if( responseMessage.Headers.TryGetValues("Set-Cookie", out cookies) == false )
            return;

        Uri requestUri = this.HttpOption.GetReuestUri();
        foreach( var cookie in cookies ) {
            this.HttpOption.Cookie.SetCookies(requestUri, cookie);
        }
    }

    private HttpWebResponse GetResponse(HttpClient client, bool checkStatus)
    {
        this.SetStartTime();

        try {
            // 发送HTTP请求
#if NET6_0_OR_GREATER            
            HttpResponseMessage responseMessage = client.Send(this.Request, this.HttpOption.CompletionOption, this.HttpOption.CancellationToken);
#else
            var sendRequestTask = client.SendAsync(this.Request, this.HttpOption.CompletionOption, this.HttpOption.CancellationToken);
            HttpResponseMessage responseMessage = sendRequestTask.GetAwaiter().GetResult();
#endif

            // 读取Cookie头
            SaveResponseCookie(responseMessage);

            // 转成HttpWebResponse类型
            HttpWebResponse response = CreateHttpWebResponse(responseMessage, checkStatus);

            // 引发结束事件
            this.RequestFinished(response, null);

            return response;
        }
        catch( OperationCanceledException ocex ) {
            this.RequestFinished(null, ocex);
            throw;
        }
        catch( Exception ex ) {

            // ex有可能会是：WebException, TaskCanceledException, HttpRequestException
            // 如果直接将异常抛出，那么在日志中根本不知道当前是调用的哪个URL地址，所以这里包装成一个新异常类型再抛出
            RemoteWebException ex2 = new RemoteWebException(ex, this.HttpOption.Url);

            // 执行结束事件
            this.RequestFinished(null, ex2);
            throw ex2;
        }
    }

    private async Task<HttpWebResponse> GetResponseAsync(HttpClient client, bool checkStatus)
    {
        this.SetStartTime();

        try {
            // 发送HTTP请求
            HttpResponseMessage responseMessage = await client.SendAsync(this.Request, this.HttpOption.CompletionOption, this.HttpOption.CancellationToken);

            // 读取Cookie头
            SaveResponseCookie(responseMessage);

            // 转成HttpWebResponse类型
            HttpWebResponse response = CreateHttpWebResponse(responseMessage, checkStatus);

            // 引发结束事件
            this.RequestFinished(response, null);

            return response;
        }
        catch( OperationCanceledException ocex ) {
            this.RequestFinished(null, ocex);
            throw;
        }
        catch( Exception ex ) {

            // ex有可能会是：WebException, TaskCanceledException, HttpRequestException
            // 如果直接将异常抛出，那么在日志中根本不知道当前是调用的哪个URL地址，所以这里包装成一个新异常类型再抛出
            RemoteWebException ex2 = new RemoteWebException(ex, this.HttpOption.Url);

            // 执行结束事件
            this.RequestFinished(null, ex2);
            throw ex2;
        }
    }


    // 这是一个内部的构造方法
    // internal HttpWebResponse(HttpResponseMessage _message, Uri requestUri, CookieContainer cookieContainer)
    // HttpWebResponse httpWebResponse = new HttpWebResponse(httpResponseMessage, _requestUri, _cookieContainer);

    private static readonly ConstructorInfo s_ctor = typeof(HttpWebResponse).GetConstructor(
                                                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                                            null, new Type[] { typeof(HttpResponseMessage), typeof(Uri), typeof(CookieContainer) }, null);


    private HttpWebResponse CreateHttpWebResponse(HttpResponseMessage responseMessage, bool checkStatus)
    {
        Uri requestUri = this.HttpOption.GetReuestUri();
        CookieContainer cookieContainer = this.HttpOption.Cookie;

        HttpWebResponse response = (HttpWebResponse)s_ctor.Invoke(new object[] { responseMessage, requestUri, cookieContainer });

        if( checkStatus && responseMessage.IsSuccessStatusCode == false ) {

            //string message = string.Format("The remote server returned an error: ({0}) {1}.", (int)responseMessage.StatusCode, responseMessage.ReasonPhrase);
            string message = string.Format("远程服务端返回一个错误: ({0}) {1}.", (int)responseMessage.StatusCode, responseMessage.ReasonPhrase);
            throw new WebException(message, null, WebExceptionStatus.ProtocolError, response);
        }

        return response;
    }


}
#endif
