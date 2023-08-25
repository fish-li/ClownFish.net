namespace ClownFish.Data;

/// <summary>
/// 包含分页操作的2个查询的数据结构
/// </summary>
public struct Page2Query
{
    /// <summary>
    /// 
    /// </summary>
    public CPQuery ListQuery { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public CPQuery CountQuery { get; private set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="listQuery"></param>
    /// <param name="countQuery"></param>
    public Page2Query(CPQuery listQuery, CPQuery countQuery)
    {
        this.ListQuery = listQuery;
        this.CountQuery = countQuery;
    }
}
