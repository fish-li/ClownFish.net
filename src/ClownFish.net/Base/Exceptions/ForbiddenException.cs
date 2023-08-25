namespace ClownFish.Base;

/// <summary>
/// 表示一个禁止访问的异常
/// </summary>
public sealed class ForbiddenException : Exception, IErrorCode
{
    /// <summary>
    /// StatusCode, default value: 403
    /// </summary>
    public int StatusCode { get; set; } = 403;

    int IErrorCode.GetErrorCode() => this.StatusCode;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public ForbiddenException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ForbiddenException(string message, Exception innerException) : base(message, innerException)
    {
    }


}
