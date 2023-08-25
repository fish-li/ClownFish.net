namespace ClownFish.Data.Internals;

/// <summary>
/// 实体的附加描述标记，仅供框架内部使用（不考虑升级兼容问题）！
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EntityAdditionAttribute : Attribute
{
    /// <summary>
    /// 相关联的实体代理类型
    /// </summary>
    public Type ProxyType { get; set; }

}
