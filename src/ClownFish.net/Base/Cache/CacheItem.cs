namespace ClownFish.Base;

/// <summary>
/// 一个包含过期时间描述的缓存结果的类型。
/// 使用场景：缓存单个对象。
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class CacheItem<T> : IDisposable where T : class
{
    private T _value;
    private WeakReference<T> _weakObject;

    private readonly object _syncLock = new object();
    private long _expiration;

    private readonly bool _useWeakReference;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="value">缓存结果</param>
    /// <param name="expiration">缓存的过期时间，如果访问Value属性时【超过】过期时间，则返回结果类型的默认值</param>
    /// <param name="useWeakReference">是否允许使用弱引用来缓存对象</param>
    public CacheItem(T value, DateTime expiration, bool useWeakReference = true)
    {
        _useWeakReference = useWeakReference;
        SetValue(value, expiration);
    }

    /// <summary>
    /// 获取结果。如果当前时间超过过期时间，刚返回结果类型的默认值。
    /// </summary>
    public T Value => Get();

    internal bool IsExpired()
    {
        return DateTime.Now.Ticks > Interlocked.Read(ref _expiration);
    }

    private void SetValue(T value, DateTime expiration)
    {
        lock( _syncLock ) {

            if( value == null ) {
                _value = null;
                _weakObject = null;
                Interlocked.Exchange(ref _expiration, DateTime.MinValue.Ticks);
            }
            else {
                if( UseWeakReference(value, expiration) ) {
                    // 防止开发人员缓存大对象，长时间运行会导致OOM，所以这里使用【弱引用】
                    _value = null;
                    _weakObject = new WeakReference<T>(value);
                }
                else {
                    _value = value;
                    _weakObject = null;
                }
                Interlocked.Exchange(ref _expiration, expiration.Ticks);
            }
        }
    }


    private bool UseWeakReference(T value, DateTime expiration)
    {
        if( _useWeakReference == false )
            return false;

        // 永不过期的对象，不用弱引用保持
        if( expiration == DateTime.MaxValue )
            return false;


        // 【小字符串】，不用弱引用保持。 这里的【大/小】和GC的85K大对象不是一回事。
        if( (value is string text) && text.Length < 1024 )
            return false;


        return true;
    }

    /// <summary>
    /// 修改缓存值
    /// </summary>
    /// <param name="value">缓存结果</param>
    /// <param name="expiration">过期时间</param>
    public void Set(T value, DateTime expiration)
    {
        SetValue(value, expiration);
    }


    /// <summary>
    /// 从缓存中获取结果
    /// </summary>
    /// <returns>已缓存的结果。如果缓存中不存在或者超过了过期时间，则返回结果类型的默认值</returns>
    public T Get()
    {
        lock( _syncLock ) {

            if( _value != null ) {
                if( IsExpired() ) {
                    Dispose();    // 清除缓存引用（延迟清理）
                    return default(T);
                }
                else {
                    return _value;
                }
            }

            if( _weakObject != null ) {
                if( IsExpired() ) {
                    Dispose();    // 清除缓存引用（延迟清理）
                    return default(T);
                }
                else {
                    T data = default(T);
                    _weakObject.TryGetTarget(out data);
                    return data;
                }
            }
        }

        return default(T);
    }


    /// <summary>
    /// 清除缓存引用
    /// </summary>
    public void Dispose()
    {
        lock( _syncLock ) {

            if( _value != null ) {

                if( _value is IDisposable disposable ) {
                    disposable.Dispose();
                }

                // 释放引用
                _value = null;
            }

            if( _weakObject != null ) {

                T weakValue = default(T);
                _weakObject.TryGetTarget(out weakValue);

                if( weakValue != null ) {
                    if( weakValue is IDisposable disposable ) {
                        disposable.Dispose();
                    }
                }

                // 释放引用
                _weakObject.SetTarget(null);
                _weakObject = null;
            }
        }
    }




}
