namespace ClownFish.WebClient;

/// <summary>
/// 定义一些与HTTP相关的默认值
/// </summary>
public static class HttpClientDefaults
{
    /// <summary>
    /// HTTP调用的超时时间（单位：毫秒），默认值：10 秒
    /// </summary>
    public static readonly int HttpTimeout = LocalSettings.GetInt("ClownFish_HttpClient_HttpTimeout", 10 * 1000);

    /// <summary>
    /// HTTP调用的超时时间（单位：毫秒），默认值：120 秒
    /// </summary>
    public static readonly int ProxyHttpTimeout = LocalSettings.GetInt("ClownFish_HttpClient_ProxyHttpTimeout", 120 * 1000);

    /// <summary>
    /// HttpClient实例的缓存时间（单位：秒），默认值：0，永远不过期。
    /// 对于“域名”不需要刷新的情况下，可以不限制缓存时间。
    /// </summary>
    public static readonly int HttpClientCacheSeconds = LocalSettings.GetInt("ClownFish_HttpClient_InstanceCacheSeconds", 0);

    ///// <summary>
    ///// 在记录HTTP请求日志时，允许最大的请求体长度，默认值：4K
    ///// </summary>
    //public static readonly int LogMaxRequestBodyLen = LocalSettings.GetInt("ClownFish_HttpClient_LogMaxRequestBodyLen", 4 * 1024);


    public static CancellationToken DefaultCancellationToken = CancellationToken.None;


}
