namespace ClownFish.Data;

/// <summary>
/// 指示包含实体的程序集
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class EntityAssemblyAttribute : Attribute
{
}
