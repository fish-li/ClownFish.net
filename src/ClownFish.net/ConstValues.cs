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
        CurrentVersion = AsmHelper.GetFileVersion(typeof(ConstValues)).IfEmpty("8.24.803.2");

        ReLoad();
    }

    internal static void ReLoad()
    {
        // 示例：ClownFish.HttpClient/8.24.803.2/TxClientX/cluster1/dev/fish-debian12/Debian GNU-Linux 12 (bookworm)
        HttpClientUserAgent = $"ClownFish.HttpClient/{CurrentVersion}/{EnvUtils.ApplicationName}/{EnvUtils.ClusterName}/{EnvUtils.RunEnv}/{EnvUtils.GetHostName()}/{OsUtils.GetOsName().Replace('/', '-')}";
    }
}
