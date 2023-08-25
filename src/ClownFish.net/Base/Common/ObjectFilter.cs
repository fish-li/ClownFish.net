namespace ClownFish.Base;

/// <summary>
/// 对象过滤器，用于判断某个对象在“一段时间间隔”内是否重复出现。
/// </summary>
public sealed class ObjectFilter<T> where T : class
{
    private readonly CacheDictionary<string> _cache;

    private readonly int _intervalSeconds;
    private readonly Func<T, string> _keyFunc;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="keyFunc">一个计算KEY的委托，用于在执行 IsExist 方法时计算KEY，再根据KEY来判断元素是否在一段时间内出现过。</param>
    /// <param name="intervalSeconds">用于判断元素是否出现过的时间间隔，单位：秒。</param>
    public ObjectFilter(Func<T, string> keyFunc, int intervalSeconds)
    {
        if( keyFunc == null )
            throw new ArgumentNullException(nameof(keyFunc));
        if( intervalSeconds <= 0 )
            throw new ArgumentOutOfRangeException(nameof(intervalSeconds));

        _intervalSeconds = intervalSeconds;
        _keyFunc = keyFunc;

        _cache = new CacheDictionary<string>();
    }


    /// <summary>
    /// 检查指定的对象在"一段时间间隔"内是否已存在。
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <returns>如果对象存在（没有被过期清除），返回 true, 否则返回 false</returns>
    public bool IsExist(T element)
    {
        if( element == null )
            throw new ArgumentNullException(nameof(element));


        // 将元素生成 string key，然后放入缓存集合，后面检查是否存在时，判断KEY是否存在就可以了，
        // 这里生成KEY时，要再做一次MD5，好处是避免一些大的KEY占用大量内存，可以理解为：用CPU的计算时间换内存空间

        string key = HashHelper.Md5(_keyFunc(element));

        if( _cache.Get(key) == null ) {

            // 将 key 放入缓存集合，VALUE不重要，所以随便指定
            // “一段时间间隔” 用过期时间来实现。

            _cache.Set(key, "xx", DateTime.Now.AddSeconds(_intervalSeconds));
            return false;
        }

        return true;

    }
}
