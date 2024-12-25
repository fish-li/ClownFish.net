namespace ClownFish.ImClients;

/// <summary>
/// 表示IM服务器出现异常，或者没有按照文档响应
/// </summary>
public sealed class ImServerException : Exception
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="message"></param>
    public ImServerException(string message) : base(message) { }
}
