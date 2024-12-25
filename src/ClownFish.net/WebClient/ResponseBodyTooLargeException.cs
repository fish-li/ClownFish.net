namespace ClownFish.WebClient;

/// <summary>
/// 表示响应体长度超过最大限度的异常
/// </summary>
public sealed class ResponseBodyTooLargeException : Exception
{
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    public ResponseBodyTooLargeException(string message) : base(message) { }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="size"></param>
    public ResponseBodyTooLargeException(long size) : base("响应体太大，已超过最大长度限制：" + size.ToString()) { }

}
