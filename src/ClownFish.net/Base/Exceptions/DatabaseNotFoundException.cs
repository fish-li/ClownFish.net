namespace ClownFish.Base.Exceptions;

/// <summary>
/// 数据库没找到异常
/// </summary>
public sealed class DatabaseNotFoundException : Exception, IErrorCode
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
    public DatabaseNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public DatabaseNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
