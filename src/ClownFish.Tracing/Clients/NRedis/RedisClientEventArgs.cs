namespace ClownFish.Tracing;

/// <summary>
/// Redis客户端执行时的事件参数
/// </summary>
public sealed class RedisClientEventArgs : EventArgs
{
    /// <summary>
    /// 当前调用的方法
    /// </summary>
    public MethodInfo Method { get; internal set; }

    /// <summary>
    /// 方法的调用参数
    /// </summary>
    public object[] Arguments { get; internal set; }

    /// <summary>
    /// StartTime
    /// </summary>
    public DateTime StartTime { get; internal set; }

    /// <summary>
    /// EndTime
    /// </summary>
    public DateTime EndTime { get; internal set; }

    /// <summary>
    /// 与执行相关的异常对象
    /// </summary>
    public Exception Exception { get; internal set; }

}
