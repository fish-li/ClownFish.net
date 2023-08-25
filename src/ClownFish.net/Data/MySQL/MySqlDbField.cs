namespace ClownFish.Data.MySQL;

/// <summary>
/// 描述一个MySQL的字段结构
/// </summary>
public sealed class MySqlDbField
{
    /// <summary>
    /// 字段名,COLUMN_NAME
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 字段的数据类型，例如：varchar
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 字段类型，含长度，例如：varchar(50) 或者  tinyint unsigned
    /// </summary>
    public string ColType { get; set; }

    /// <summary>
    /// Nullable
    /// </summary>
    public string Nullable { get; set; }

    /// <summary>
    /// Key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Default
    /// </summary>
    public string Default { get; set; }

    /// <summary>
    /// Extra
    /// </summary>
    public string Extra { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.Name;
    }


    // 参考链接：https://dev.mysql.com/doc/refman/8.0/en/information-schema-columns-table.html


    /// <summary>
    /// 当前字段是否允许NULL
    /// </summary>
    [DbColumn(Ignore = true)]
    [XmlIgnore]
    [JsonIgnore]
    public bool IsNull => this.Nullable == "YES"; // && this.Default.IsNullOrEmpty();

    /// <summary>
    /// 当前字段的备注描述
    /// </summary>
    [DbColumn(Ignore = true)]
    [XmlIgnore]
    [JsonIgnore]
    public string CommentText => string.IsNullOrEmpty(Comment) ? Name : Comment;

    /// <summary>
    /// 当前字段是不是主键
    /// </summary>
    /// <returns></returns>
    [DbColumn(Ignore = true)]
    [XmlIgnore]
    [JsonIgnore]
    public bool IsPrimaryKey => this.Key.EqualsIgnoreCase("pri");

    /// <summary>
    /// 当前字段是不是“自动递增”
    /// </summary>
    [DbColumn(Ignore = true)]
    [XmlIgnore]
    [JsonIgnore]
    public bool IsAutoIncrement => this.Extra.EqualsIgnoreCase("auto_increment");
}
