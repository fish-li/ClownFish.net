namespace ClownFish.Base.Exceptions;

/// <summary>
/// 表示数据库的重复插入异常
/// </summary>
public sealed class DuplicateInsertException : Exception, IErrorCode
{
    /// <summary>
    /// StatusCode, default value: 500
    /// </summary>
    public int StatusCode { get; set; } = 500;

    int IErrorCode.GetErrorCode() => this.StatusCode;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message">解释异常原因的错误信息。</param>
    /// <param name="innerException"></param>
    public DuplicateInsertException(string message, Exception innerException) : base(message, innerException)
    {

    }

}
