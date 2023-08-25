namespace ClownFish.Data;

/// <summary>
/// 定义数据列的描述信息
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class DbColumnAttribute : Attribute
{
    /// <summary>
    /// 数据库字段名（相对于C#属性来说就是别名）
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// 不从数据库加载（仅在内存在指定）
    /// </summary>
    public bool Ignore { get; set; }


    /// <summary>
    /// 是否主键（用于UPDATE操作生成WHERE条件）
    /// 注意：一张表最多只允许一个主键字段，否则将会出现异常。
    /// </summary>
    public bool PrimaryKey { get; set; }

    /// <summary>
    /// 是否为自增列
    /// 注意：一张表最多只允许一个自增列字段，否则将会出现异常。
    /// </summary>
    public bool Identity { get; set; }

    /// <summary>
    /// 字段值是只读的，例如：时间戳，计算列
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 数据库字段的默认值（插入时有效）
    /// </summary>
    public object DefaultValue { get; set; }
}
