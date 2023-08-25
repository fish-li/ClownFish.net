namespace ClownFish.Base;

// 开发这个类的原因：
// ConcurrentDictionary这个类的GetOrAdd方法有个大坑！
// 它虽然保证集合是线程安全，但不保证 valueFactory 在多线程并发时只调用一次，这样会造成很大的问题，完全不能接受！
// 所以，这个类就是用来代替ConcurrentDictionary


/// <summary>
/// 线程安全的字典集合，类似于 ConcurrentDictionary，但无坑！
/// </summary>
public sealed class TSafeDictionary<TKey, TValue>
{
    private readonly ConcurrentDictionary<TKey, TValue> _dict;

    private readonly object _syncLock = new object();

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity"></param>
    public TSafeDictionary(int capacity = 63)
    {
        _dict = new ConcurrentDictionary<TKey, TValue>(Environment.ProcessorCount, capacity);
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="comparer"></param>
    public TSafeDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
        _dict = new ConcurrentDictionary<TKey, TValue>(Environment.ProcessorCount, capacity, comparer);
    }

    /// <summary>
    /// 获取集合的元素数量
    /// </summary>
    public int Count => _dict.Count;


    /// <summary>
    /// 从集合中获取一个元素，如果不存在则调用valueFactory来创建。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="valueFactory"></param>
    /// <returns></returns>
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));
        if( valueFactory == null )
            throw new ArgumentNullException(nameof(valueFactory));

        TValue value;
        if( _dict.TryGetValue(key, out value) )
            return value;

        lock( _syncLock ) {

            if( _dict.TryGetValue(key, out value) )
                return value;

            value = valueFactory.Invoke(key);
            _dict[key] = value;
            return value;
        }
    }


    /// <summary>
    /// 索引器属性
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue this[TKey key] {

        get => _dict[key];

        set => _dict[key] = value;
    }


    /// <summary>
    /// 给集合添加一个元素，如果KEY存在则修改。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Set(TKey key, TValue value)
    {
        _dict[key] = value;
    }


    /// <summary>
    /// 尝试获取KEY对应的元素，如果没有则返回类型的默认值。
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue TryGet(TKey key)
    {
        return _dict.TryGet(key);
    }


    /// <summary>
    /// 给集合添加一个元素，如果指定的KEY已存在则抛出异常，在异常中会报告是哪个KEY
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddValue(TKey key, TValue value)
    {
        _dict.AddValue(key, value);
    }

    /// <summary>
    /// 尝试给集合添加一个元素，如果指定的KEY已存在则返回false，否则返回true表示添加成功。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryAdd(TKey key, TValue value)
    {
        return _dict.TryAdd(key, value);
    }


    /// <summary>
    /// 尝试获取KEY对应的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dict.TryGetValue(key, out value);
    }


    /// <summary>
    /// 尝试删除KEY对应的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryRemove(TKey key, out TValue value)
    {
        return _dict.TryRemove(key, out value);
    }


    /// <summary>
    /// 清空集合
    /// </summary>
    public void Clear()
    {
        _dict.Clear();
    }


    /// <summary>
    /// 克隆集合中的数据到一个非线程安全的集合，用于枚举场景。
    /// </summary>
    /// <returns></returns>
    public Dictionary<TKey, TValue> Clone()
    {
        Dictionary<TKey, TValue> newDict = new Dictionary<TKey, TValue>(_dict.Count);

        KeyValuePair<TKey, TValue>[] kvs = _dict.ToArray();
        foreach( var kv in kvs )
            newDict[kv.Key] = kv.Value;

        return newDict;
    }


    /// <summary>
    /// GetKeys
    /// </summary>
    /// <returns></returns>
    public TKey[] GetKeys()
    {
        return _dict.Keys.ToArray();
    }

}



