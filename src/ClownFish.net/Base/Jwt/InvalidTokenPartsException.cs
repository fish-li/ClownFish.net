namespace ClownFish.Base.Jwt;

internal sealed class InvalidTokenPartsException : Exception
{
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public InvalidTokenPartsException(string message) : base(message)
    {
    }
}


internal sealed class SignatureVerificationException : Exception
{
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public SignatureVerificationException(string message) : base(message)
    {
    }
}
