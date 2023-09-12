namespace ClownFish.Rabbit;

/// <summary>
/// SendMessageEventArgs
/// </summary>
public sealed class SendRabbitMessageEventArgs : EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public string Server { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    public string Exchange { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    public string RoutingKey { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    public object Data { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    public int DataLen { get; internal set; }

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
