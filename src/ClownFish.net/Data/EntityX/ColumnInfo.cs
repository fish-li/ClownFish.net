namespace ClownFish.Data;

/// <summary>
/// 
/// </summary>
public sealed class ColumnInfo
{
    /// <summary>
    /// 
    /// </summary>
    public string DbName {
        get {
            return Attr == null || string.IsNullOrEmpty(Attr.Alias)
            ? PropertyInfo.Name
            : Attr.Alias;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public PropertyInfo PropertyInfo { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public DbColumnAttribute Attr { get; private set; }

    /// <summary>
    /// PropertyInfo的实际数据类型（可空类型返回对应的值类型）
    /// </summary>
    public Type DataType { get; set; }
    /// <summary>
    /// 属性的出现次序，主要用于映射状态变更数组中的序号
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="attr"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ColumnInfo(PropertyInfo propertyInfo, DbColumnAttribute attr)
    {
        if( propertyInfo == null )
            throw new ArgumentNullException("propertyInfo");

        this.Attr = attr;
        this.PropertyInfo = propertyInfo;
    }
}
