namespace ClownFish.ImClients;

/// <summary>
/// 表示IM服务端不接受处理而产生的错误，类似于 HTTP400 时的错误
/// </summary>
public sealed class ShitResultException : Exception, IHttpResultString
{
    /// <summary>
    /// Request
    /// </summary>
    public HttpOption Request { get; private set; }

    /// <summary>
    /// ServerResponse，可以为NULL
    /// </summary>
    public HttpResult<string> Response { get; private set; }

    /// <summary>
    /// IImResult，可以为NULL
    /// </summary>
    public IShitResult Result { get; private set; }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <param name="result"></param>
    public ShitResultException(string message, HttpOption request, HttpResult<string> response, IShitResult result) : base(message)
    {
        this.Request = request;
        this.Response = response;
        this.Result = result;
    }

    /// <summary>
    /// Message
    /// </summary>
    public override string Message => this.Result != null 
                                        ? $"errcode: {this.Result.ErrCode}, errmsg: {this.Result.ErrMsg}" 
                                        : base.Message;
}
