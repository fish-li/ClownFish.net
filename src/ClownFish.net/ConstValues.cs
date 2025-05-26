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
        CurrentVersion = AsmHelper.GetFileVersion(typeof(ConstValues)).IfEmpty("9.25.526.1");

        // 示例：ClownFish.HttpClient/9.25.526.1
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
