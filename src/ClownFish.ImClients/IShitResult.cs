namespace ClownFish.ImClients;

/// <summary>
/// IM服务端返回结果的基础数据接口
/// </summary>
public interface IShitResult
{
    /// <summary>
    /// ErrCode
    /// </summary>
    public int ErrCode { get; }
    /// <summary>
    /// ErrMsg
    /// </summary>
    public string ErrMsg { get; }
}


/// <summary>
/// ImShitResult
/// </summary>
public sealed class ImShitResult : IShitResult
{
    /// <summary>
    /// 
    /// </summary>
    public HttpOption Request { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public HttpResult<string> Response { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public JObject Json { get; private set; }

    int IShitResult.ErrCode => (int)this.Json["errcode"];

    string IShitResult.ErrMsg => (string)this.Json["errmsg"];

    internal ImShitResult(HttpOption request, HttpResult<string> response)
    {
        this.Request = request;
        this.Response = response;
        this.Json = response.Result.FromJson<JObject>();
    }
}
