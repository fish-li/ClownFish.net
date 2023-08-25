namespace ClownFish.Log.Configuration;

/// <summary>
/// 表示日志配置中存在的错误
/// </summary>
[Serializable]
public sealed class LogConfigException : Exception
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message"></param>
    public LogConfigException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public LogConfigException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public LogConfigException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
