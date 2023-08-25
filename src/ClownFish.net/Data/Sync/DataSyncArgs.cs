#if NET6_0_OR_GREATER
namespace ClownFish.Data.Sync;

/// <summary>
/// 数据同步参数
/// </summary>
public sealed class DataSyncArgs
{
    /// <summary>
    /// 业务场景名称。此名称【必须保证唯一】
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 源表的数据连接对象
    /// </summary>
    public DbContext SrcDbContext { get; set; }

    /// <summary>
    /// 目标表的数据连接对象
    /// </summary>
    public DbContext DestDbContext { get; set; }

    /// <summary>
    /// 是否启用数据库事务，默认值：false
    /// </summary>
    public bool WithTranscation { get; set; }

    /// <summary>
    /// 源表名称
    /// </summary>
    public string SrcTableName { get; set; }

    /// <summary>
    /// 目标表名称
    /// </summary>
    public string DestTableName { get; set; }

    /// <summary>
    /// 源表的主键字段
    /// </summary>
    public string SrcKeyField { get; set; }

    /// <summary>
    /// 目标表中，与源表的主键对应字段，可以是【外键】，也可以是【主键】
    /// </summary>
    public string DestRelatedKey { get; set; }

    /// <summary>
    /// Where 语句片段，用于获取源表数据行
    /// </summary>
    public string SrcFilterSql { get; set; }

    /// <summary>
    /// 字段映射，默认找到所有同名字段形成映射关系
    /// </summary>
    public List<FieldMap> DataFieldsMap { get; set; }


    internal void Validate()
    {
        if( this.BizName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.BizName));

        if( this.SrcDbContext == null )
            throw new ArgumentNullException(nameof(this.SrcDbContext));

        if( this.DestDbContext == null )
            throw new ArgumentNullException(nameof(this.DestDbContext));        

        if( this.SrcTableName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.SrcTableName));

        if( this.DestTableName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.DestTableName));

        if( this.SrcKeyField.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.SrcKeyField));

        if( this.DestRelatedKey.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.DestRelatedKey));

        if( this.SrcFilterSql.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.SrcFilterSql));
    }
}


/// <summary>
/// 字段名称映射关系
/// </summary>
/// <param name="SrcField">源表字段名</param>
/// <param name="DestField">目标表字段名</param>
public record struct FieldMap(string SrcField, string DestField);

#endif