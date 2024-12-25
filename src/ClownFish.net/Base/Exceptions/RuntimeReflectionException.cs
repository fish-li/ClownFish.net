namespace ClownFish.Base.Exceptions;

/// <summary>
/// 表示执行反射时出现的异常
/// </summary>
public sealed class RuntimeReflectionException : Exception
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="message"></param>
    public RuntimeReflectionException(string message) : base(message) { }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ex"></param>
    public RuntimeReflectionException(string  message, Exception ex) : base(message, ex) { }
}
