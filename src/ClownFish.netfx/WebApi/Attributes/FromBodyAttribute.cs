namespace ClownFish.WebApi;

/// <summary>
/// 当请求体以JSON/XML传递数据时，且Action有多个参数申明，
/// 可用 [FromBody] 标记哪个参数值从整体请求体中的JSON/XML反序列化得到参数值，
/// 没有标记 [FromBody] 的参数将尝试从查询字符串或者HTTP上下文中获取数据。
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class FromBodyAttribute : System.Attribute
{
}


