namespace ClownFish.Log.Logging;

/// <summary>
/// 记录日志相关的一些预定义名称
/// </summary>
public static class LoggingKeys
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


    internal static readonly string HttpOptionId = "Clownfish-HttpOption-Id";
}
