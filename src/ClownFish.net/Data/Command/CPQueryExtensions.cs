namespace ClownFish.Data;

/// <summary>
/// 提供CPQuery扩展方法的工具类
/// </summary>
public static class CPQueryExtensions
{
    /// <summary>
    /// 将指定的字符串（T-SQL的片段）转成CPQuery对象
    /// </summary>
    /// <param name="sql">T-SQL的片段的字符串</param>
    /// <returns>包含T-SQL的片段的CPQuery对象</returns>
    public static CPQuery AsCPQuery(this string sql)
    {
        return CPQuery.Create(sql);
    }




    /// <summary>
    /// 将string转换成QueryParameter对象
    /// </summary>
    /// <param name="value">要转换成QueryParameter的原对象</param>
    /// <returns>转换后的QueryParameter对象</returns>
    public static QueryParameter AsQueryParameter(this string value)
    {
        return new QueryParameter(value);
    }


    /// <summary>
    /// SqlFragment
    /// </summary>
    /// <param name="text">T-SQL的片段的字符串</param>
    /// <returns></returns>
    public static SqlFragment AsSql(this string text)
    {
        return new SqlFragment(text);
    }

}
