namespace ClownFish.Base.Exceptions;

/// <summary>
/// 表示错误的代码异常
/// </summary>
public sealed class InvalidCodeException : Exception, IErrorCode
{
    int IErrorCode.GetErrorCode() => 500;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message">解释异常原因的错误信息。</param>
    public InvalidCodeException(string message) : base(message)
    {

    }

}
