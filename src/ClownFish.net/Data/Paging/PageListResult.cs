namespace ClownFish.Data;

/// <summary>
/// 表示一个分页的查询结果
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public sealed class PageListResult<T>
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public PagingInfo PagingInfo { get; set; }

    /// <summary>
    /// 查询结果列表
    /// </summary>
    public List<T> Data { get; set; }
}
