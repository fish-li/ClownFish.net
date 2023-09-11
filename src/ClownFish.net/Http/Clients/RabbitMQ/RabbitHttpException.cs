namespace ClownFish.Http.Clients.RabbitMQ;

/// <summary>
/// 表示用HTTP方式调用RabbitMQ时出现的异常
/// </summary>
public sealed class RabbitHttpException : Exception
{
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public RabbitHttpException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
