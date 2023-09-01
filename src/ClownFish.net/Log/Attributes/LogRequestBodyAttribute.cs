namespace ClownFish.Log.Attributes;

/// <summary>
/// 标记当前请求在记录日志时，将请求体也记录到日志中
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class LogRequestBodyAttribute : Attribute
{
}
