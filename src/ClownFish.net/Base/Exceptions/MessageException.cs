namespace ClownFish.Base.Exceptions;

/// <summary>
/// 表示一个普通的消息异常，用于从代码中快速退出。
/// </summary>
public class MessageException : Exception, IErrorCode
{
    /// <summary>
    /// StatusCode, default value: 500
    /// </summary>
    public int StatusCode { get; set; } = 500;

    int IErrorCode.GetErrorCode() => this.StatusCode;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public MessageException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public MessageException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
