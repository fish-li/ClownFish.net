namespace ClownFish.Log;

/// <summary>
/// 日志时出现异常不能被处理时引用的事件参数
/// </summary>
public sealed class ExceptionEventArgs : System.EventArgs
{
    /// <summary>
    /// 新产生的异常实例
    /// </summary>
    public Exception Exception { get; private set; }

    internal ExceptionEventArgs(Exception ex)
    {
        this.Exception = ex;
    }
}
