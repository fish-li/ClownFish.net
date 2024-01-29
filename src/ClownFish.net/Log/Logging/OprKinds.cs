namespace ClownFish.Log.Logging;

/// <summary>
/// 一些预定义的 操作类型 OprKind
/// </summary>
public static class OprKinds
{
    /// <summary>
    /// string "http"，表示一次HTTP请求的处理过程
    /// </summary>
    public static readonly string Http = "http";

    /// <summary>
    /// string "proxy"，表示一次HTTP代理转发
    /// </summary>
    public static readonly string Proxy = "proxy";

    /// <summary>
    /// string "msg"，表示一次队列消息的处理过程
    /// </summary>
    public static readonly string Msg = "msg";

    /// <summary>
    /// string "btask"，表示一次后台任务的执行过程
    /// </summary>
    public static readonly string Btask = "btask";

    /// <summary>
    /// string "gevent"，表示处理了一个全局事件
    /// </summary>
    public static readonly string GlobalEvent = "gevent";

    /// <summary>
    /// string "error"，表示一条独立的异常日志
    /// </summary>
    public static readonly string Error = "error";

    /// <summary>
    /// string "info"，表示一条独立的消息日志
    /// </summary>
    public static readonly string Info = "info";

}
