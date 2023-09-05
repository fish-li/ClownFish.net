namespace ClownFish.Web.Security.Attributes;

/// <summary>
/// 指示某个Controller或者Action允许匿名访问
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class AllowAnonymousAttribute : Attribute
{
}
