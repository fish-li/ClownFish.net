#if NETCOREAPP

using System.Net.Http;
using ClownFish.WebClient.V2;

namespace ClownFish.Http.Proxy;

internal static class ProxyHttpClientCache
{
    private static readonly CacheDictionary<SocketsHttpHandler> s_httpClients = new CacheDictionary<SocketsHttpHandler>(false);

    public static HttpClient GetClient(Uri requestUri)
    {
        SocketsHttpHandler clientHandler = GetClientHandler(requestUri);

        HttpClient client = new HttpClient(clientHandler, false);
        client.Timeout = TimeSpan.FromMilliseconds(HttpClientDefaults.HttpProxyTimeout);
        return client;
    }

    internal static SocketsHttpHandler GetClientHandler(Uri requestUri)
    {
        // 计算缓存KEY
        string cacheKey = GetCacehKey(requestUri);

        // 从缓存中获取实例
        SocketsHttpHandler clientHandler = s_httpClients.Get(cacheKey);
        if( clientHandler == null ) {

            lock( s_httpClients ) {
                clientHandler = s_httpClients.Get(cacheKey);
                if( clientHandler == null ) {

                    // 创建新实例
                    clientHandler = CreateClientHandler();

                    s_httpClients.Set(cacheKey, clientHandler);
                }
            }
        }

        return clientHandler;
    }

    private static string GetCacehKey(Uri requestUri)
    {
        return $"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}";
    }

    internal static SocketsHttpHandler CreateClientHandler()
    {
        SocketsHttpHandler clientHandler = new SocketsHttpHandler();

        if( ClownFishOptions.HttpClient_EnableSystemWebProxy == false ) {
            clientHandler.UseProxy = false;   // 默认值是 true，程序运行时会获取系统默认的代理
            clientHandler.Proxy = null;
        }
        clientHandler.AutomaticDecompression = DecompressionMethods.None;    // 对于代理来说，肯定不需要自动解压缩
        clientHandler.UseCookies = false;
        clientHandler.AllowAutoRedirect = false;
        clientHandler.MaxResponseHeadersLength = int.MaxValue; // 这个属性的单位是KB，默认值：64，对于代理来说不做这个限制
        clientHandler.SslOptions.RemoteCertificateValidationCallback = MsHttpClientCache.DangerousAcceptAnyServerCertificateValidator;
        //clientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        if( HttpClientDefaults.HttpClientCacheSeconds > 0 ) {
            clientHandler.PooledConnectionLifetime = TimeSpan.FromSeconds(HttpClientDefaults.HttpClientCacheSeconds);
        }

        return clientHandler;
    }

}

#endif
