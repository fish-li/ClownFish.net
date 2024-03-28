namespace ClownFish.WebClient;

/// <summary>
/// 定义一些与HTTP相关的默认值
/// </summary>
public static class HttpClientDefaults
{
    // 说明：不使用 readonly 是为了在运行时可调整！

    /// <summary>
    /// HTTP客户端调用的超时时间（单位：毫秒），默认值：10 秒
    /// </summary>
    public static /* readonly */ int HttpClientTimeout = LocalSettings.GetInt("ClownFish_HttpClient_HttpTimeout", 10 * 1000);

    /// <summary>
    /// HttpClient实例的缓存时间（单位：秒），默认值：0，永远不过期。
    /// 对于“域名”不需要刷新的情况下，可以不限制缓存时间。
    /// </summary>
    public static /* readonly */ int HttpClientCacheSeconds = LocalSettings.GetInt("ClownFish_HttpClient_InstanceCacheSeconds", 0);

    /// <summary>
    /// HTTP代理调用的超时时间（单位：毫秒），默认值：120 秒
    /// </summary>
    public static /* readonly */ int HttpProxyTimeout = LocalSettings.GetInt("ClownFish_HttpProxy_Timeout", 120 * 1000);

    /// <summary>
    /// Rabbit-HTTP客户端调用的超时时间（单位：毫秒），默认值：30 秒
    /// </summary>
    public static /* readonly */ int RabbitHttpClientTimeout = LocalSettings.GetInt("ClownFish_RabbitHttpClient_Timeout", 30 * 1000);

    /// <summary>
    /// Elasticsearch-HTTP客户端调用的超时时间（单位：毫秒），默认值：30 秒
    /// </summary>
    public static /* readonly */ int EsHttpClientTimeout = LocalSettings.GetInt("ClownFish_EsHttpClient_Timeout", 30 * 1000);

    /// <summary>
    /// HttpJsonWriter-HTTP客户端调用的超时时间（单位：毫秒），默认值：30 秒
    /// </summary>
    public static /* readonly */ int HttpJsonWriterTimeout = LocalSettings.GetInt("ClownFish_HttpJsonWriter_Timeout", 30 * 1000);

    /// <summary>
    /// 
    /// </summary>
    public static readonly bool UseAppExitToken = LocalSettings.GetBool("ClownFish_HttpClient_UseAppExitToken", 1);
}
