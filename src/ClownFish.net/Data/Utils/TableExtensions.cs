namespace ClownFish.Data;

/// <summary>
/// 从DataTable加载数据的扩展工具类
/// </summary>
public static class TableExtensions
{
    /// <summary>
    /// 从DataTable中加载一个实体列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="table"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(this DataTable table) where T : class, new()
    {
        if( table == null )
            throw new ArgumentNullException("table");

        IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
        return loader.ToList(table);
    }

    /// <summary>
    /// 从DataRow加载一个实体对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="row"></param>
    /// <returns></returns>
    public static T ToSingle<T>(this DataRow row) where T : class, new()
    {
        if( row == null )
            throw new ArgumentNullException("row");

        IDataLoader<T> loader = DataLoaderFactory.GetLoader<T>();
        return loader.ToSingle(row);
    }
}
