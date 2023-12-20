namespace ClownFish.Http.Clients.Elastic;

/// <summary>
/// ES调用时产生的异常
/// </summary>
public sealed class EsHttpException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public string Response { get; private set; }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="response"></param>
    public EsHttpException(string message, string response) : base(message)
    {
        Response = response;
    }

    /// <summary>
    /// 
    /// </summary>
    public override string Message => base.Message;
}
