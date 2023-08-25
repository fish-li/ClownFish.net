using System.Linq.Expressions;
using ClownFish.Data.Linq;


namespace ClownFish.Data;
/// <summary>
/// 为LINQ异步查询提供的扩展方法。
/// </summary>
public static class QueryExtensions
{
    private static TResult Execute<TSource, TResult>(IQueryable<TSource> source, string methodName)
    {
        if( source == null )
            throw new ArgumentNullException(nameof(source));

        EntityLinqProvider provider = source.Provider as EntityLinqProvider;
        if( provider == null )
            throw new NotSupportedException();

        MethodInfo method = typeof(QueryExtensions).GetMethod(methodName).MakeGenericMethod(typeof(TSource));

        return provider.Execute<TResult>(
                                Expression.Call(null, method, source.Expression));
    }


    /// <summary>
    /// 将linq查询转换成CPQuery对象
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static CPQuery GetQuery<TSource>(this IQueryable<TSource> source)
    {
        return Execute<TSource, CPQuery>(source, "GetQuery");
    }



    /// <summary>
    /// 执行查询并生成单个实体对象
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static TSource ToSingle<TSource>(this IQueryable<TSource> source)
    {
        return Execute<TSource, TSource>(source, "ToSingle");
    }
}
