namespace ClownFish.WebApi.Result;

/// <summary>
/// 表示Action结果的接口
/// </summary>
public interface IActionResult
{
    /// <summary>
    /// 执行输出操作
    /// </summary>
    /// <param name="httpContext"></param>
    Task OuputAsync(NHttpContext httpContext);
}
