namespace ClownFish.Base;

/// <summary>
/// 缓存工具类，供应用程序所有业务代码共用
/// </summary>
public static class AppCache
{
    private static readonly CacheDictionary<object> s_cacheDict = new CacheDictionary<object>();

    private static readonly ResourceLock s_syncLock = new ResourceLock();

    internal static int GetCount()
    {
        return s_cacheDict.GetCount();
    }

    /// <summary>
    /// 从缓存容器中获取一个对象，如果对象不存在，则调用“加载委托”进行加载并存入缓存。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="loadFunc">对象“加载委托”，用于当缓存对象不存在时获取对象，调用结束后，新产生的对象将插入缓存。此委托在 lock 语句块中运行，不会有并发问题。</param>
    /// <param name="cacheMs">当缓存对象不存在时，loadFunc加载的对象 放入缓存容器的 缓存时间长度，单位：毫秒。</param>
    /// <returns></returns>
    public static T GetObject<T>(string key, Func<T> loadFunc = null, int cacheMs = 0) where T : class
    {
        if( key.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(key));

        T value = (T)s_cacheDict.Get(key);

        if( value == null ) {

            if( loadFunc != null ) {

                var loadSyncLock = s_syncLock.GetLock(key);

                // 调用委托加载对象
                lock( loadSyncLock ) {

                    value = (T)s_cacheDict.Get(key);

                    if( value == null ) {
                        value = loadFunc();

                        if( cacheMs > 0 ) {
                            s_cacheDict.Set(key, value, DateTime.Now.AddMilliseconds(cacheMs));
                        }
                        else {
                            s_cacheDict.Set(key, value, DateTime.Now.AddSeconds(CacheOption.AppCacheSeconds));
                        }
                    }
                }                
            }
        }

        return value;
    }


    /// <summary>
    /// 将一个对象添加到缓存容器中
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="value">需要缓存的对象</param>
    /// <param name="expiration">缓存的过期时间</param>
    public static void SetObject(string key, object value, DateTime expiration)
    {
        if( key.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(key));

        s_cacheDict.Set(key, value, expiration);
    }


    /// <summary>
    /// 删除指定的缓存对象
    /// </summary>
    /// <param name="key">缓存键</param>
    public static void RemoveObject(string key)
    {
        if( key.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(key));

        s_cacheDict.Remove(key);
    }


}
