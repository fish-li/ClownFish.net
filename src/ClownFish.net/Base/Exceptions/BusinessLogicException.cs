namespace ClownFish.Base.Exceptions;

/// <summary>
/// 表示一个业务逻辑异常
/// </summary>
public sealed class BusinessLogicException : MessageException, IErrorCode
{
    int IErrorCode.GetErrorCode() => this.StatusCode;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public BusinessLogicException(string message) : base(message)
    {
        this.StatusCode = 651;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public BusinessLogicException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
