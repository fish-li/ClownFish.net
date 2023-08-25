namespace ClownFish.Data;

/// <summary>
/// 基本的分页信息。
/// </summary>
public sealed class PagingInfo
{
    /// <summary>
    /// 分页序号，从0开始计数
    /// </summary>
    public int PageIndex { get; set; }
    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// 从相关查询中获取到的符合条件的总记录数
    /// </summary>
    public int TotalRows { get; set; }


    /// <summary>
    /// 是否需要计算分页数。
    /// 如果此属性设置为 false，那么在执行类似ToPageList方法时，将不执行count查询。
    /// 使用建议：在导出数据时，可以将此属性设置为 false
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public bool NeedCount { get; set; } = true;


    /// <summary>
    /// 计算总页数
    /// </summary>
    /// <returns>总页数</returns>
    public int CalcPageCount()
    {
        if( this.PageSize == 0 || this.TotalRows == 0 )
            return 0;

        return (int)Math.Ceiling((double)this.TotalRows / (double)this.PageSize);
    }
}
