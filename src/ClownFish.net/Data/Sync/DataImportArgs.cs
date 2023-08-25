#if NET6_0_OR_GREATER

namespace ClownFish.Data.Sync;

/// <summary>
/// 数据导入参数
/// </summary>
public sealed class DataImportArgs
{
    /// <summary>
    /// 目标表的数据连接对象
    /// </summary>
    public DbContext DestDbContext { get; set; }

    /// <summary>
    /// 目标表的名称
    /// </summary>
    public string DestTableName { get; set; }


    /// <summary>
    /// 是否允许导入自增列的主键列，默认值：false
    /// </summary>
    public bool AllowAutoIncrement { get; set; }

    /// <summary>
    /// 要导入的数据
    /// </summary>
    public DataTable Data { get; set; }

    /// <summary>
    /// 是否启用数据库事务，默认值：false
    /// </summary>
    public bool WithTranscation { get; set; }



    internal void Validate()
    {
        if( this.DestDbContext == null )
            throw new ArgumentNullException(nameof(this.DestDbContext));

        if( this.DestTableName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(this.DestTableName));

        if( this.Data == null )
            throw new ArgumentNullException(nameof(this.Data));

        if( this.Data.Rows.Count == 0 || this.Data.Columns.Count == 0 )
            throw new ArgumentException("数据表没有包含行或者列！");
    }
}

#endif