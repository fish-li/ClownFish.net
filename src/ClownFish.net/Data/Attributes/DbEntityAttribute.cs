namespace ClownFish.Data;

/// <summary>
/// 描述数据实体属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DbEntityAttribute : Attribute
{
    /// <summary>
    /// 别名
    /// </summary>
    public string Alias { get; set; }
}
