#if NETCOREAPP

using System.Net.Http;
using System.Net.Security;
using MyHttpOption = ClownFish.WebClient.HttpOption;


namespace ClownFish.WebClient.V2;

internal static class MsHttpClientCache
{
    // 2024-12-13 将 CacheDictionary<HttpClient> 修改为 CacheDictionary<SocketsHttpHandler>
    // 原因，1，Timeout 在 HttpClient 中，访问同一个站点，设置不同的超时时间意味着要保留 多个Socket连接池，这也太SB了！
    //       2，有些 HttpOption 需要用于“反向代理”行为，因此对于 SocketsHttpHandler 的设置是不一样的（需要调用ProxyHttpClientCache.GetClientHandler)

    private static readonly CacheDictionary<SocketsHttpHandler> s_httpClients = new CacheDictionary<SocketsHttpHandler>(false);

    internal static int GetCount()
    {
        return s_httpClients.GetCount();
    }

    public static SocketsHttpHandler GetClientHandler(MyHttpOption httpOption)
    {
        // 计算缓存KEY
        string cacheKey = GetCacehKey(httpOption);

        // 从缓存中获取实例
        SocketsHttpHandler clientHandler = s_httpClients.Get(cacheKey);
        if( clientHandler == null ) {

            lock( s_httpClients ) {
                clientHandler = s_httpClients.Get(cacheKey);
                if( clientHandler == null ) {

                    // 创建新实例
                    clientHandler = CreateClientHandler(httpOption.Credentials, httpOption.AllowAutoRedirect);

                    // 每个HttpClient实例都使用自己的连接池，将其请求与其他 HttpClient 实例执行的请求隔离开来。
                    // HttpClient 旨在实例化一次，并在应用程序的整个生命周期内重复使用。
                    // 参考：https://learn.microsoft.com/zh-cn/dotnet/fundamentals/networking/http/httpclient-guidelines

                    s_httpClients.Set(cacheKey, clientHandler);
                }
            }
        }
        return clientHandler;
    }


    private static string GetCacehKey(MyHttpOption httpOption)
    {
        //string timeout = httpOption.Timeout.HasValue ? httpOption.Timeout.Value.ToString() : "NULL";
        string redirect = httpOption.AllowAutoRedirect.GetValueOrDefault(true).ToString();

        string credential = "NULL";
        if( httpOption.Credentials != null && httpOption.Credentials is NetworkCredential network ) {

            // 在KEY中包含密码不是安全的做法，在抓包时就会泄露密码。
            //credential = $"{network.UserName}:{network.Password}";

            // 所以，在计算KEY时，只取用户名。
            // 但是这样会带来一个新问题：如果第一次传入了错误的密码，此时HttpClient已缓存，后面将会一直使用，一直出错~~
            // 为了解决不泄露密码问题，可以通过对密码做HASH来解决，但是这样会造成性能浪费，毕竟这种场景只是“可能”会发生，
            // 为了这种可能性极低的情况去牺牲性能，也不恰当！ 而且HASH密码方案会造成错误密码的连接一直存在，也是一大浪费。
            // 最终决定，不考虑这种极低概率情况，如果真的出现，那应该也是配置问题，可以在调整配置后重启程序~~~
            credential = network.UserName;
        }

        // 缓存键的构成：
        // 1、访问协议
        // 2、用户名
        // 3、密码
        // 4、域名和端口
        // 5、超时阀值
        // 6、重定向参数
        Uri requestUri = httpOption.GetReuestUri();
        return $"{requestUri.Scheme}://{credential}@{requestUri.Host}:{requestUri.Port}/redirect:{redirect}";
    }


    //public static HttpMessageHandler CreateClientHandler(MyHttpOption httpOption)
    public static SocketsHttpHandler CreateClientHandler(ICredentials credentials, bool? allowAutoRedirect)
    {
        // .net SocketsHttpHandler 的设计真有问题！
        // SocketsHttpHandler 它应该只负责连接和连接池的管理，这样才适合“对象复用”
        // 但是，它把 AutomaticDecompression，Credentials， AllowAutoRedirect， CookieContainer 这些东西搞进来就不方便 “对象复用” 了
        // 试想下：针对同一个站点，我用2个不同的身份去访问，难道需要使用2个Socket连接池？？

        // ########################################################################################
        // 因此，这个方法中2次访问 httpOption 参数的属性，它们会分别创建不同的 clientHandler 实例，
        //       这个和上面的 GetCacehKey 方法保持一致
        // ########################################################################################

        SocketsHttpHandler clientHandler = new SocketsHttpHandler();

        clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Brotli;

        if( credentials != null )
            clientHandler.Credentials = credentials;

        if( allowAutoRedirect.HasValue )
            clientHandler.AllowAutoRedirect = allowAutoRedirect.Value;

        //clientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        clientHandler.SslOptions.RemoteCertificateValidationCallback = MsHttpClientCache.DangerousAcceptAnyServerCertificateValidator;

        //clientHandler.PreAuthenticate = false;
        //clientHandler.MaxAutomaticRedirections = 50;    // The default value is 50
        //clientHandler.MaxConnectionsPerServer = 1024;   // default value: int.MaxValue
        clientHandler.MaxResponseHeadersLength = 256;     // 256 kB, default value: 64

        clientHandler.UseCookies = false;    // 为了让HttpClientHandler能重用，在外面处理 Cookie

        // 明确不使用代理
        if( ClownFishOptions.HttpClient_EnableSystemWebProxy == false ) {
            clientHandler.UseProxy = false;   // 默认值是 true，程序运行时会获取系统默认的代理
            clientHandler.Proxy = null;
        }
        // 默认值：HttpConnectionSettings._useProxy = true,  in  HttpConnectionPoolManager ctor:
        // if (settings._useProxy)
        //     _proxy = settings._proxy ?? HttpClient.DefaultProxy;

        if( HttpClientDefaults.HttpClientCacheSeconds > 0 ) {
            clientHandler.PooledConnectionLifetime = TimeSpan.FromSeconds(HttpClientDefaults.HttpClientCacheSeconds);
        }

        return clientHandler;
    }


    internal static readonly RemoteCertificateValidationCallback DangerousAcceptAnyServerCertificateValidator = DangerousAcceptAnyServerCertificateValidator0;

    private static bool DangerousAcceptAnyServerCertificateValidator0(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

}

#endif
