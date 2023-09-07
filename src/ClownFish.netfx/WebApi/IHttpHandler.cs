namespace ClownFish.WebApi;

/// <summary>
/// 
/// </summary>
public interface IHttpHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    void ProcessRequest(NHttpContext context);
}
