namespace ClownFish.Base;

/// <summary>
/// 租户没找到异常
/// </summary>
public sealed class TenantNotFoundException : Exception, IErrorCode
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
    public TenantNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public TenantNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
