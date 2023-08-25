#if NETCOREAPP

using System.Net.Http;
using MyHttpOption = ClownFish.Base.WebClient.HttpOption;


namespace ClownFish.Base.WebClient.V2;

internal static class MsHttpClientCache
{
    private static readonly CacheDictionary<HttpClient> s_httpClients = new CacheDictionary<HttpClient>(false);

    internal static int GetCount()
    {
        return s_httpClients.GetCount();
    }

    public static bool IsClientEnableCached(this MyHttpOption httpOption)
    {
#if NET6_0_OR_GREATER
        if( httpOption.UnixSocketEndPoint.HasValue() )
            return false;
#endif

        if( httpOption.MessageHandler != null )
            return false;

        // 如果【显式】指定KeepAlive为false，则不使用缓存实例
        if( httpOption.KeepAlive.HasValue && httpOption.KeepAlive.Value == false )
            return false;


        // TODO: 目前不支持 Proxy
        // 这里不支持CredentialCache，因为authType没法确定。 绝大部分场景下使用NetworkCredential也够用了
        return (httpOption.Credentials == null || httpOption.Credentials is NetworkCredential);
    }


    public static HttpClient GetCachedOrCreate(MyHttpOption httpOption, bool fromCache)
    {
        if( fromCache ) {

            // 计算缓存KEY
            string cacheKey = GetCacehKey(httpOption);

            // 从缓存中获取实例
            HttpClient client = s_httpClients.Get(cacheKey);
            if( client == null ) {

                lock( s_httpClients ) {
                    client = s_httpClients.Get(cacheKey);
                    if( client == null ) {

                        // 创建新实例
                        client = HttpObjectUtils.CreateClient(httpOption);

                        // 每个HttpClient实例都使用自己的连接池，将其请求与其他 HttpClient 实例执行的请求隔离开来。
                        // HttpClient 旨在实例化一次，并在应用程序的整个生命周期内重复使用。

                        // 参考：https://learn.microsoft.com/zh-cn/dotnet/api/system.net.http.httpclient?view=net-7.0
                        // 参考：https://learn.microsoft.com/zh-cn/dotnet/fundamentals/networking/http/httpclient-guidelines

                        if( HttpDefaults.HttpClientCacheSeconds > 0 )
                            s_httpClients.Set(cacheKey, client, DateTime.Now.AddSeconds(HttpDefaults.HttpClientCacheSeconds));
                        else
                            s_httpClients.Set(cacheKey, client);
                    }
                }
            }
            return client;
        }
        else {
            // 创建一个“一次性”使用的 HttpClientHandler实例
            return HttpObjectUtils.CreateClient(httpOption);
        }
    }


    private static string GetCacehKey(MyHttpOption httpOption)
    {
        string timeout = httpOption.Timeout.HasValue ? httpOption.Timeout.Value.ToString() : "NULL";
        string redirect = httpOption.AllowAutoRedirect.HasValue ? httpOption.AllowAutoRedirect.ToString() : "NULL";

        string credential = "NULL";
        if( httpOption.Credentials != null && httpOption.Credentials is NetworkCredential ) {
            NetworkCredential network = (NetworkCredential)httpOption.Credentials;

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
        return $"{requestUri.Scheme}://{credential}@{requestUri.Host}:{requestUri.Port}/timeout:{timeout}/redirect:{redirect}";
    }


}

#endif
