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
    public static readonly string HttpClientUserAgent;

    static ConstValues()
    {
        CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(ConstValues).Assembly.Location).FileVersion;

        HttpClientUserAgent = "ClownFish.HttpClient/" + CurrentVersion;
    }
}
