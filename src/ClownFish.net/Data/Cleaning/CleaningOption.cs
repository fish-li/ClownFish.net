namespace ClownFish.Data.Cleaning;

/// <summary>
/// 数据表清理参数
/// </summary>
public sealed class CleaningOption
{
    /// <summary>
    /// 数据库连接参数
    /// </summary>
    public DbConfig DbConfig { get; set; }

    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 时间字段名
    /// </summary>
    public string TimeFieldName { get; set; }

    /// <summary>
    /// 清理多少小时之前的数据，默认值：24
    /// </summary>
    public int HoursAgo { get; set; }

    /// <summary>
    /// 每次清理的记录最大条数，如果超过这个数量，将拆分成多次执行清理和归档。，默认值：500
    /// </summary>
    public int BatchRows { get; set; }


    /// <summary>
    /// 重试次数，默认值：20
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 数据库操作的超时时间，单位：秒，默认值：200
    /// </summary>
    public int DbTimeout { get; set; }


    internal void Validate()
    {
        if( this.DbConfig == null )
            throw new ArgumentNullException(nameof(DbConfig));

        if( this.TableName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(TableName));

        if( this.TimeFieldName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(TimeFieldName));

        if( this.HoursAgo <= 0 )
            this.HoursAgo = 24;

        if( this.BatchRows <= 0 )
            this.BatchRows = 500;

        if( this.RetryCount <= 0 )
            this.RetryCount = 20;

        if( this.DbTimeout <= 0 )
            this.DbTimeout = 200;
    }
}

