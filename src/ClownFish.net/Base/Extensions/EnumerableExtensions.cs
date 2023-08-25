namespace ClownFish.Base;

/// <summary>
/// 工具类
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 判断一个集合是否NULL或空
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this ICollection collection)
    {
        return collection == null || collection.Count == 0;
    }


    /// <summary>
    /// 判断一个集合不NULL且有元素
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static bool HasValue(this ICollection collection)
    {
        return collection != null && collection.Count > 0;
    }


    /// <summary>
    /// 将指定集合的元素添加到集合的的末尾，并返回集合本身。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dest"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static List<T> AddRange2<T>(this List<T> dest, IEnumerable<T> collection)
    {
        if( dest == null )
            throw new ArgumentNullException(nameof(dest));

        dest.AddRange(collection);
        return dest;
    }



    /// <summary>
    /// 将一个大列表再拆分成多个子列表
    /// </summary>
    /// <typeparam name="T">列表中元素的数据类型</typeparam>
    /// <param name="list">需要拆分的大列表</param>
    /// <param name="maxBatch">最大分组数量，也就是返回结果列表的最大长度</param>
    /// <param name="batchSize">每个子列表的元素数量。这个参数是建议值，当达到maxBatch时就有可能会超过这个建议值。</param>
    /// <returns></returns>
    public static List<List<T>> SplitList<T>(this List<T> list, int maxBatch, int batchSize)
    {
        if( maxBatch <= 1 )
            throw new ArgumentOutOfRangeException(nameof(maxBatch));
        if( batchSize <= 1 )
            throw new ArgumentOutOfRangeException(nameof(batchSize));


        // 有3种场景：
        // 1，list.Count <= batchSize，此时可以直接返回结果
        // 2，maxBatch * batchSize >= list.Count，可以按照 batchSize 参数来拆分
        // 3，maxBatch * batchSize <= list.Count，此时需要调整(放大) batchSize，并保证不超过 maxBatch

        if( list.Count <= batchSize ) {
            return new List<List<T>> { list };
        }

        // 注意：maxBatch 有可能是int.MaxValue，此时需要转成 long 再做比较，
        // 否则2个int的乘积是负数了（溢出），然后进入else分支，在创建List时出现OOM
        if( 1L * maxBatch * batchSize >= list.Count ) {
            // 计算实际所需的分组数量
            int batchCount = ((int)Math.Ceiling((double)list.Count / batchSize));
            return SplitList0(list, batchCount, batchSize);
        }
        else {
            // 计算为了保证不超过 maxBatch 的每组长度
            int realBatchSize = ((int)Math.Ceiling((double)list.Count / maxBatch));
            return SplitList0(list, maxBatch, realBatchSize);
        }

        List<List<T>> SplitList0(List<T> list, int batchCount, int batchSize)
        {
            List<List<T>> result = new List<List<T>>(batchCount);

            for( int i = 0; i < list.Count; i += batchSize ) {
                result.Add(list.Skip(i).Take(batchSize).ToList());
            }
            return result;
        }
    }




    /// <summary>
    /// System.Linq.Enumerable.Cast方法的增强版本，允许在遍历集合时做元素的类型转换
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> Cast2<TResult>(this IEnumerable source)
    {
        IEnumerable<TResult> enumerable = source as IEnumerable<TResult>;
        if( enumerable != null ) {
            return enumerable;
        }
        if( source == null ) {
            // System.Linq.Enumerable: throw Error.ArgumentNull("source");
            return Empty.Array<TResult>();
        }

        return CastIterator2<TResult>(source);
    }

    private static IEnumerable<TResult> CastIterator2<TResult>(IEnumerable source)
    {
        foreach( object item in source ) {
            // System.Linq.Enumerable： yield return (TResult)item;

            if( item is TResult )
                yield return (TResult)item;

            else if( item is string text )
                yield return (TResult)StringConverter.ChangeType(text, typeof(TResult));

            else
                yield return (TResult)Convert.ChangeType(item, typeof(TResult));
        }
    }


}
