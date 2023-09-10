namespace ClownFish.MQ.Pipeline;

#if NET6_0_OR_GREATER

/// <summary>
/// “死消息”相关工具类
/// </summary>
public static class DeadMessageUtils
{
    /// <summary>
    /// “死消息”产生事件
    /// </summary>
    public static event EventHandler<DeadMessageArgs> OnDeadMessage;

    internal static void HandlerDeadMessage(object message)
    {
        EventHandler<DeadMessageArgs> handler = OnDeadMessage;
        if( handler != null ) {
            DeadMessageArgs e = new DeadMessageArgs {
                Message = message
            };
            handler(null, e);
        }
    }

}


/// <summary>
/// “死消息”事件参数
/// </summary>
public sealed class DeadMessageArgs : EventArgs
{
    /// <summary>
    /// “死消息”对象
    /// </summary>
    public object Message { get; init; }
}

#endif

