namespace ClownFish.Base;

/// <summary>
/// 一些常量值
/// </summary>
public static class ConstValues
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly string CurrentVersion = "10.26.317.1";

    /// <summary>
    /// 
    /// </summary>
    public static string HttpClientUserAgent { get; internal set; }

    static ConstValues()
    {
        // 示例：ClownFish.HttpClient/10.26.317.1
        HttpClientUserAgent = $"ClownFish.HttpClient/{CurrentVersion}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userAgent"></param>
    public static void SetHttpClientUserAgent(string userAgent)
    {
        if( userAgent.HasValue() ) {
            HttpClientUserAgent = userAgent;
        }
    }

}
