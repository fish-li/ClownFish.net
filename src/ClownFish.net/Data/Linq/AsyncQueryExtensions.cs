using System.Linq.Expressions;
using ClownFish.Data.Linq;

namespace ClownFish.Data;

/// <summary>
/// 为LINQ异步查询提供的扩展方法。
/// </summary>
public static class AsyncQueryExtensions
{
    private async static Task<TResult> ExecuteAsync<TSource, TResult>(IQueryable<TSource> source, string methodName)
    {
        if( source == null )
            throw new ArgumentNullException(nameof(source));

        EntityLinqProvider provider = source.Provider as EntityLinqProvider;
        if( provider == null )
            throw new NotSupportedException();

        MethodInfo method = typeof(AsyncQueryExtensions).GetMethod(methodName).MakeGenericMethod(typeof(TSource));

        return await provider.ExecuteAsync<TResult>(
                                Expression.Call(null, method, source.Expression));
    }


    /// <summary>
    /// 以异步方式执行Count查询
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public async static Task<int> CountAsync<TSource>(this IQueryable<TSource> source)
    {
        return await ExecuteAsync<TSource, int>(source, "CountAsync");
    }

    /// <summary>
    /// 以异步方式执行查询并生成单个实体对象
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public async static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source)
    {
        return await ExecuteAsync<TSource, TSource>(source, "FirstOrDefaultAsync");
    }

    /// <summary>
    /// 以异步方式执行查询并生成单个实体对象
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public async static Task<TSource> ToSingleAsync<TSource>(this IQueryable<TSource> source)
    {
        return await ExecuteAsync<TSource, TSource>(source, "ToSingleAsync");
    }

    /// <summary>
    /// 以异步方式执行查询并生成实体列表
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public async static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source)
    {
        return await ExecuteAsync<TSource, List<TSource>>(source, "ToListAsync");
    }

    /// <summary>
    /// 以异步方式执行查询
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public async static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source)
    {
        return await ExecuteAsync<TSource, bool>(source, "AnyAsync");
    }
        
}
