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


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Message {
        get {
            RemoteWebException ex2 = this.InnerException as RemoteWebException;
            if( ex2 == null )
                return base.Message;

            if( ex2.ResponseText.IsNullOrEmpty() )
                return base.Message;

            return base.Message + ", " + ex2.ResponseText;
        }
    }
}
