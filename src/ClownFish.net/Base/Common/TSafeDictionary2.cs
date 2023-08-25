// 这个类型可用于 .NET35 移植

namespace ClownFish.Base;

// 开发这个类的原因：
// ConcurrentDictionary这个类的GetOrAdd方法有个大坑！
// 它虽然保证集合是线程安全，但不保证 valueFactory 在多线程并发时只调用一次，这样会造成很大的问题，完全不能接受！
// 所以，这个类就是用来代替ConcurrentDictionary


/// <summary>
/// 线程安全的字典集合，类似于 ConcurrentDictionary，但无坑！
/// </summary>
internal sealed class TSafeDictionary2<TKey, TValue> : IDisposable
{
    private readonly Dictionary<TKey, TValue> _dict;

    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity"></param>
    public TSafeDictionary2(int capacity = 63)
    {
        _dict = new Dictionary<TKey, TValue>(capacity);
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="comparer"></param>
    public TSafeDictionary2(int capacity, IEqualityComparer<TKey> comparer)
    {
        _dict = new Dictionary<TKey, TValue>(capacity, comparer);
    }

    /// <summary>
    /// 获取集合的元素数量
    /// </summary>
    public int Count {
        get {
            _lock.EnterWriteLock();
            try {
                return _dict.Count;
            }
            finally {
                _lock.ExitWriteLock();
            }
        }
    }


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

        TValue value = default(TValue);

        _lock.EnterUpgradeableReadLock();

        try {
            if( _dict.TryGetValue(key, out value) == false ) {


                _lock.EnterWriteLock();
                try {
                    value = valueFactory.Invoke(key);
                    _dict[key] = value;
                }
                finally {
                    _lock.ExitWriteLock();
                }
            }
        }
        finally {
            _lock.ExitUpgradeableReadLock();
        }

        return value;
    }


    /// <summary>
    /// 索引器属性
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue this[TKey key] {

        get => TryGet(key);

        set => Set(key, value);
    }


    /// <summary>
    /// 给集合添加一个元素，如果KEY存在则修改。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Set(TKey key, TValue value)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        _lock.EnterWriteLock();
        try {
            _dict[key] = value;
        }
        finally {
            _lock.ExitWriteLock();
        }
    }


    /// <summary>
    /// 尝试获取KEY对应的元素，如果没有则返回类型的默认值。
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue TryGet(TKey key)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        _lock.EnterReadLock();
        try {
            TValue value = default(TValue);
            _dict.TryGetValue(key, out value);
            return value;
        }
        finally {
            _lock.ExitReadLock();
        }
    }



    /// <summary>
    /// 给集合添加一个元素，如果指定的KEY已存在则抛出异常，在异常中会报告是哪个KEY
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddValue(TKey key, TValue value)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        _lock.EnterWriteLock();
        try {
            _dict.AddValue(key, value);
        }
        finally {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// 尝试给集合添加一个元素，如果指定的KEY已存在则返回false，否则返回true表示添加成功。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryAdd(TKey key, TValue value)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        _lock.EnterWriteLock();
        try {
            try {
                _dict.Add(key, value);
                return true;
            }
            catch {
                return false;
            }
        }
        finally {
            _lock.ExitWriteLock();
        }
    }



    /// <summary>
    /// 尝试获取KEY对应的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        _lock.EnterReadLock();
        try {
            value = default(TValue);
            return _dict.TryGetValue(key, out value);
        }
        finally {
            _lock.ExitReadLock();
        }
    }


    /// <summary>
    /// 尝试删除KEY对应的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryRemove(TKey key, out TValue value)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        value = default(TValue);

        _lock.EnterUpgradeableReadLock();

        try {
            if( _dict.TryGetValue(key, out value) ) {

                _lock.EnterWriteLock();
                try {
                    return _dict.Remove(key);
                }
                finally {
                    _lock.ExitWriteLock();
                }
            }
            else {
                return false;
            }
        }
        finally {
            _lock.ExitUpgradeableReadLock();
        }
    }



    /// <summary>
    /// 清空集合
    /// </summary>
    public void Clear()
    {
        _lock.EnterWriteLock();
        try {
            _dict.Clear();
        }
        finally {
            _lock.ExitWriteLock();
        }
    }


    /// <summary>
    /// 克隆集合中的数据到一个非线程安全的集合，用于枚举场景。
    /// </summary>
    /// <returns></returns>
    public Dictionary<TKey, TValue> Clone()
    {
        _lock.EnterReadLock();
        try {
            Dictionary<TKey, TValue> newDict = new Dictionary<TKey, TValue>(_dict.Count);
            foreach( var kv in _dict )
                newDict[kv.Key] = kv.Value;

            return newDict;
        }
        finally {
            _lock.ExitReadLock();
        }
    }


    ///// <summary>
    ///// GetKeys
    ///// </summary>
    ///// <returns></returns>
    //public TKey[] GetKeys()
    //{
    //    _lock.EnterReadLock();
    //    try {
    //        return _dict.Keys.ToArray();
    //    }
    //    finally {
    //        _lock.ExitReadLock();
    //    }
    //}

    void IDisposable.Dispose()
    {
        _lock.Dispose();
    }
}
