namespace ClownFish.Log.Logging;

/// <summary>
/// 日志记录时“忽略操作”的一些预定义名称
/// </summary>
public static class LoggingIgnoreNames
{
    /// <summary>
    /// 请求头名称
    /// </summary>
    public static readonly string HeaderName = "x-cf-logflags";


    /// <summary>
    /// "IgnoreRequestBody"
    /// </summary>
    public static readonly string IgnoreRequestBody = "IgnoreRequestBody";

    /// <summary>
    /// "IgnoreResponseBody"
    /// </summary>
    public static readonly string IgnoreResponseBody = "IgnoreResponseBody";
}
