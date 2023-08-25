namespace ClownFish.Data;

/// <summary>
/// 指示包含实体代理的程序集，
/// 注意：
/// 1、这个标记仅供工具使用，不要在代码中使用！
/// 2、包含这个标记的程序集，在生成代理类时将不会搜索其中定义的实体类型
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class EntityProxyAssemblyAttribute : Attribute
{
}
