namespace ClownFish.Log.Logging;

/// <summary>
/// 日志记录时“忽略操作”的一些预定义名称
/// </summary>
public static class LoggingIgnoreNames
{
    /// <summary>
    /// "Logging-Ignore-RequestBody"
    /// </summary>
    /// <example>
    /// httpOption.OnSetRequest = req => req.Options.AddValue(LoggingIgnoreNames.IgnoreRequestBody, "1");
    /// </example>
    public static readonly string IgnoreRequestBody = "Logging-Ignore-RequestBody";

    /// <summary>
    /// "Logging-Ignore-ResponseBody"
    /// </summary>
    /// <example>
    /// httpOption.OnSetRequest = req => req.Options.AddValue(LoggingIgnoreNames.IgnoreResponseBody, "1");
    /// </example>
    public static readonly string IgnoreResponseBody = "Logging-Ignore-ResponseBody";
}
