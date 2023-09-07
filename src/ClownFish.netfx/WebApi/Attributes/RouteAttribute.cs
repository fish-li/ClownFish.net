// 尽量兼容 AP.NET WEBAPI 的路由方式

namespace ClownFish.WebApi;

/// <summary>
/// URL路由标记。
/// 常见用法：在Controller类型是指定URL的前缀部分，在Action方法上指定剩余部分。
/// </summary>
/// <example>
/// 例如：
/// 在Controller类型上 [Route("/v20/api/demoservice/customer/")]
/// 在 Action方法上 [Route("{id}.aspx")]  or  [Route("findByTel.aspx")]
/// </example>
/// <remarks>
/// 注意：如果在Action方法上指定的 URL 是一个绝对路径，此时会忽略Controller类型上指定的URL前缀。
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class RouteAttribute : Attribute
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public RouteAttribute()
    {

    }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="urlPart">URL片段或模板，允许包含“参数占位符”</param>
    public RouteAttribute(string urlPart)
    {
        this.Url = urlPart;
    }

    /// <summary>
    /// 在类型上标记，要匹配的 URL 前缀部分
    /// </summary>
    public string Url { get; private set; }
}
