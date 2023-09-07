namespace ClownFish.WebClient;

/// <summary>
/// 包含有服务端响应的接口
/// </summary>
public interface IHttpResultString
{
    /// <summary>
    /// 服务端响应
    /// </summary>
    HttpResult<string> Response { get; }
}
