namespace ClownFish.Base;

/// <summary>
/// 一个简单的线程安全的缓存字典。
/// 缓存键的类型固定为string，不区分大小写。
/// 使用场景：以字典方式缓存多个键值。
/// </summary>
/// <typeparam name="T">缓存结果的数据类型</typeparam>
public sealed class CacheDictionary<T> where T : class
{
    private long _lastScanTime = DateTime.Now.Ticks;
    private readonly object _lock = new object();


    private readonly TSafeDictionary<string, CacheItem<T>> _cache;
    private readonly bool _useWeakReference;
    private readonly int _expirationScanFrequency;
    private readonly Action<string> _cleanCallback;

    internal bool IsAutoExpiredClean => _expirationScanFrequency > 0;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="useWeakReference">是否允许使用弱引用来缓存对象</param>
    public CacheDictionary(bool useWeakReference = true)
        : this(300, useWeakReference, GetExpirationScanFrequency(useWeakReference), null)
    {
    }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity">字典的初始容量</param>
    public CacheDictionary(int capacity)
        : this(capacity, true, GetExpirationScanFrequency(true), null)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity">字典的初始容量</param>
    /// <param name="useWeakReference">是否允许使用弱引用来缓存对象</param>
    public CacheDictionary(int capacity, bool useWeakReference)
        : this(capacity, useWeakReference, GetExpirationScanFrequency(useWeakReference), null)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity">字典的初始容量</param>
    /// <param name="useWeakReference">是否允许使用弱引用来缓存对象</param>
    /// <param name="autoExpiredClean">是否启用过期自动清理</param>
    public CacheDictionary(int capacity, bool useWeakReference, bool autoExpiredClean)
        : this(capacity, useWeakReference, GetExpirationScanFrequency(autoExpiredClean), null)
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="capacity">字典的初始容量</param>
    /// <param name="useWeakReference">是否允许使用弱引用来缓存对象</param>
    /// <param name="expirationScanFrequency">过期自动清理扫描的频率，单位：秒，大于零表示启用过期自动清理。</param>
    /// <param name="cleanCallback">自动清理过期缓存条目时的回调味委托</param>
    public CacheDictionary(int capacity, bool useWeakReference, int expirationScanFrequency, Action<string> cleanCallback)
    {
        _useWeakReference = useWeakReference;
        _expirationScanFrequency = expirationScanFrequency;
        _cleanCallback = cleanCallback;
        
        _cache = new TSafeDictionary<string, CacheItem<T>>(capacity, StringComparer.OrdinalIgnoreCase);
    }


    private static int GetExpirationScanFrequency(bool autoExpiredClean)
    {
        // 如果启用【弱引用】就意味需要着缓存大对象，那么应该是需要时启用【自动清理】的，反之则不启用自动清理

        return autoExpiredClean ? CacheOption.ExpirationScanFrequency : 0;
    }

    /// <summary>
    /// 添加一个缓存项，如果存在就覆盖。
    /// 此缓存项永不过期。
    /// </summary>
    /// <param name="key">缓存键名称</param>
    /// <param name="value">缓存结果</param>
    public void Set(string key, T value)
    {
        Set(key, value, DateTime.MaxValue);
    }


    /// <summary>
    /// 添加一个缓存项，如果存在就覆盖
    /// </summary>
    /// <param name="key">缓存键名称</param>
    /// <param name="value">缓存结果</param>
    /// <param name="expiration">过期时间</param>
    public void Set(string key, T value, DateTime expiration)
    {
        if( string.IsNullOrEmpty(key) )
            throw new ArgumentNullException(nameof(key));

        if( key.Length > 256 )  // 限制长度，避免瞎搞！
            throw new ArgumentOutOfRangeException(nameof(key), "缓存key的长度不允许超过256个字符。");

        if( value == null )
            return;     // NULL值就不保存了，后面GET拿到NULL也会认为是没有缓存过的

        CacheItem<T> item = new CacheItem<T>(value, expiration, _useWeakReference);
        _cache[key] = item;


        // 如果不启用自动清理，或者缓存项是无限期的，就不启动自动清理
        if( _expirationScanFrequency <= 0 || expiration == DateTime.MaxValue )
            return;

        // 说明：触发检查主动清理的时机在 Set 操作时，
        // Get 操作不触发，因为 Get 方法内部有 延迟清理 机制。
        CheckExpiredItems();
    }

    /// <summary>
    /// 检查要不要做主动过期清理
    /// </summary>
    internal void CheckExpiredItems()
    {
        // 由于这个类型的实例通常被用于静态引用，所以这个方法一定要考虑线程安全

        long now = DateTime.Now.Ticks;
        long lastTime = Interlocked.Read(ref _lastScanTime);

        if( (new DateTime(lastTime)).AddSeconds(_expirationScanFrequency).Ticks <= now ) {

            lock( _lock ) {

                if( lastTime == Interlocked.Read(ref _lastScanTime) ) {

                    // 修改最后扫描时间，并开始扫描
                    Interlocked.Exchange(ref _lastScanTime, now);

                    ThreadUtils.Run("CacheDictionary_ClearExpiredItems", ClearExpiredItems);
                }
            }
        }
    }

    /// <summary>
    /// 清理已过期的缓存项（主动清除）
    /// </summary>
    private void ClearExpiredItems()
    {
        foreach( var kv in _cache.Clone() ) {

            if( kv.Value.IsExpired() ) {

                _cleanCallback?.Invoke(kv.Key);
                Remove(kv.Key);                
            }
        }
    }

    /// <summary>
    /// 从缓存中获取结果
    /// </summary>
    /// <param name="key">缓存键名称</param>
    /// <returns>已缓存的结果。如果缓存中不存在或者超过了过期时间，则返回结果类型的默认值</returns>
    public T Get(string key)
    {
        if( string.IsNullOrEmpty(key) )
            throw new ArgumentNullException(nameof(key));

        return _cache.TryGetValue(key, out CacheItem<T> item) 
            ? item.Value
            : default(T);
    }


    /// <summary>
    /// 删除指定键对应的缓存对象
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key)
    {
        if( string.IsNullOrEmpty(key) )
            throw new ArgumentNullException(nameof(key));

        _cache.TryRemove(key, out CacheItem<T> item);

        item?.Dispose();
    }

    /// <summary>
    /// 清除所有缓存
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetCount()
    {
        return _cache.Count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, T> DumpData()
    {
        var dict = _cache.Clone();
        return dict.ToDictionary2(dict.Count, x => x.Key, x => x.Value.Get());
    }


    /// <summary>
    /// 获取所有的缓存键
    /// </summary>
    /// <returns></returns>
    public string[] GetKeys()
    {
        return _cache.GetKeys();
    }
}
