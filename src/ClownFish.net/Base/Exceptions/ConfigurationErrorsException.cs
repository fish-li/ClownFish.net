namespace ClownFish.Base.Exceptions;

/// <summary>
/// 表示一个配置相关的错误
/// </summary>
public sealed class ConfigurationErrorsException : Exception, IErrorCode
{
    int IErrorCode.GetErrorCode() => 500;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public ConfigurationErrorsException(string message) : base(message)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ConfigurationErrorsException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
