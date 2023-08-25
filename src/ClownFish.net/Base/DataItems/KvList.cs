namespace ClownFish.Base;

/// <summary>
/// 一个有序的KV列表。
/// 和Dictionary类似的用法，但结果和插入顺序保持一致。
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class KvList<TKey, TValue> : IDictionary<TKey, TValue>, ICollection
{
    private readonly IEqualityComparer<TKey> _comparer;
    private readonly List<KeyValuePair<TKey, TValue>> _list;

    /// <summary>
    /// 构造方法
    /// </summary>
    public KvList() : this(16, (IEqualityComparer<TKey>)null)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity"></param>
    public KvList(int capacity) : this(capacity, (IEqualityComparer<TKey>)null)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="comparer"></param>
    public KvList(int capacity, IEqualityComparer<TKey> comparer)
    {
        _list = new List<KeyValuePair<TKey, TValue>>(capacity);

        _comparer = (comparer ?? EqualityComparer<TKey>.Default);
    }

    /// <summary>
    /// 读写索引器
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue this[TKey key] {
        get {
            TValue value;
            if( TryGetValue(key, out value) == false )
                throw new ArgumentOutOfRangeException(nameof(key));

            return value;
        }
        set {
            SetValue(key, value);
        }
    }

    /// <summary>
    /// 获取所有KEY
    /// </summary>
    public ICollection<TKey> Keys => _list.Select(x => x.Key).ToArray();

    /// <summary>
    /// 获取所有Value
    /// </summary>
    public ICollection<TValue> Values => _list.Select(x => x.Value).ToArray();

    /// <summary>
    /// 获取集合中元素数量
    /// </summary>
    public int Count => _list.Count;

    /// <summary>
    /// 是否只读，永远 false
    /// </summary>
    public bool IsReadOnly => false;

    int ICollection.Count => _list.Count;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

    /// <summary>
    /// 添加一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));

        _list.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    /// <summary>
    /// 未实现这个方法
    /// </summary>
    /// <param name="item"></param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 清除所有元素
    /// </summary>
    public void Clear()
    {
        _list.Clear();
    }

    /// <summary>
    /// 未实现这个方法
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 判断是否包含指定的KEY
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(TKey key)
    {
        if( key == null )
            return false;

        return _list.FindIndex(x => _comparer.Equals(key, x.Key)) >= 0;
    }

    /// <summary>
    /// 未实现这个方法
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取一个迭代器
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach( var x in _list )
            yield return x;
    }

    /// <summary>
    /// 移除一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Remove(TKey key)
    {
        if( key == null )
            return false;

        int index = _list.FindIndex(x => _comparer.Equals(key, x.Key));
        if( index >= 0 )
            _list.RemoveAt(index);

        return index >= 0;
    }

    /// <summary>
    /// 未实现这个方法
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取每个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        value = default(TValue);

        if( key == null )
            return false;


        int index = _list.FindIndex(x => _comparer.Equals(key, x.Key));

        if( index < 0 ) {
            return false;
        }
        else {
            KeyValuePair<TKey, TValue> item = _list[index];
            value = item.Value;
            return true;
        }
    }

    /// <summary>
    /// 设置KEY,VALUE
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetValue(TKey key, TValue value)
    {
        if( key == null )
            throw new ArgumentNullException(nameof(key));


        int index = _list.FindIndex(x => _comparer.Equals(key, x.Key));

        if( index < 0 ) {
            Add(key, value);
        }
        else {
            _list[index] = new KeyValuePair<TKey, TValue>(key, value);
        }
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 克隆一个对象副本
    /// </summary>
    /// <returns></returns>
    public KvList<TKey, TValue> Clone()
    {
        KvList<TKey, TValue> list = new KvList<TKey, TValue>(_list.Count, _comparer);

        foreach( var x in _list )
            list.Add(x.Key, x.Value);

        return list;
    }
}



