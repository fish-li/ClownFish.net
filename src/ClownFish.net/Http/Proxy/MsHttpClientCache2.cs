#if NETCOREAPP

using System.Net.Http;

namespace ClownFish.Http.Proxy;

internal static class MsHttpClientCache2
{
    private static readonly CacheDictionary<HttpClient> s_httpClients = new CacheDictionary<HttpClient>(false);

    public static HttpClient GetCachedOrCreate(Uri requestUri)
    {
        // 计算缓存KEY
        string cacheKey = GetCacehKey(requestUri);

        // 从缓存中获取实例
        HttpClient client = s_httpClients.Get(cacheKey);
        if( client == null ) {

            lock( s_httpClients ) {
                client = s_httpClients.Get(cacheKey);
                if( client == null ) {

                    // 创建新实例
                    client = CreateClient();

                    if( HttpClientDefaults.HttpClientCacheSeconds > 0 )
                        s_httpClients.Set(cacheKey, client, DateTime.Now.AddSeconds(HttpClientDefaults.HttpClientCacheSeconds));
                    else
                        s_httpClients.Set(cacheKey, client);
                }
            }
        }
        return client;
    }

    private static HttpClient CreateClient()
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.UseProxy = false;
        clientHandler.AutomaticDecompression = DecompressionMethods.None;
        clientHandler.UseCookies = false;
        clientHandler.AllowAutoRedirect = false;

        HttpClient client = new HttpClient(clientHandler);
        client.Timeout = TimeSpan.FromMilliseconds(HttpClientDefaults.HttpProxyTimeout);
        return client;
    }


    private static string GetCacehKey(Uri requestUri)
    {
        return $"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}";
    }

}

#endif
