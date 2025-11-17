namespace ClownFish.Base;

/// <summary>
/// 一些常量值
/// </summary>
public static class ConstValues
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly string CurrentVersion;

    /// <summary>
    /// 
    /// </summary>
    public static string HttpClientUserAgent { get; internal set; }

    static ConstValues()
    {
        CurrentVersion = AsmHelper.GetFileVersion(typeof(ConstValues)).IfEmpty("10.25.1117.1");

        // 示例：ClownFish.HttpClient/10.25.1117.1
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
