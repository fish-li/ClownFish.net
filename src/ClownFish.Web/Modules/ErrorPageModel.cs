namespace ClownFish.Web.Aspnetcore;

/// <summary>
/// 异常页面的数据结构
/// </summary>
public sealed class ErrorPageModel
{
    /// <summary>
    /// 异常的消息内容
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 异常类型
    /// </summary>
    public string ExceptionType { get; set; }

    /// <summary>
    /// 异常状态码
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// 当前请求ID
    /// </summary>
    public string RequestId { get; set; }

}

