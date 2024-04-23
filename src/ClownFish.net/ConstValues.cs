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
        CurrentVersion = AsmHelper.GetFileVersion(typeof(ConstValues)).IfEmpty("8.24.411.17");

        HttpClientUserAgent = "ClownFish.HttpClient/" + CurrentVersion;
    }
}
