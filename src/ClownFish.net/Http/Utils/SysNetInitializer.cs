namespace ClownFish.Http.Utils;

/// <summary>
/// 初始化系统网络相关设置
/// </summary>
internal static class SysNetInitializer
{
    static SysNetInitializer()
    {
        // .net 默认值： ASP.NET 10， others 2
        if( ServicePointManager.DefaultConnectionLimit < 128 )
            ServicePointManager.DefaultConnectionLimit = 128;

        // 设置无效证书的处理方式：忽略错误
        ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
    }

    private static bool RemoteCertificateValidationCallback(
        Object sender,
        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
        System.Security.Cryptography.X509Certificates.X509Chain chain,
        System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        // 忽略证书错误。
        // HttpClient定位在后台代码调用，因此对方网站应该是确定的，
        // 因此，发生证书错误时，通常是由于证书过期导致的，所以这里就直接忽略这类错误。
        return true;
    }

    /// <summary>
    /// 触发执行初始化（可多次调用，只有第一次有效）
    /// </summary>
    public static void Init()
    {
        // 触发静态构造方法，设置与HttpWebRequest相关的参数
    }
}
