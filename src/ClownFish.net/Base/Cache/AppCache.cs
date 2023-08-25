namespace ClownFish.Base;

/// <summary>
/// 简单的缓存工具类，供应用程序所有业务代码共用
/// </summary>
public static class AppCache
{
    private static readonly CacheDictionary<object> s_cacheDict = new CacheDictionary<object>();


    internal static int GetCount()
    {
        return s_cacheDict.GetCount();
    }

    /// <summary>
    /// 尝试从缓存中获取一个对象，如果缓存对象不存在，则调用“加载委托”进行加载并存入缓存。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="loadFunc">对象“加载委托”，用于当缓存对象不存时获取对象，调用结束后，新产生的对象将插入缓存。</param>
    /// <returns></returns>
    public static T GetObject<T>(string key, Func<T> loadFunc = null) where T : class
    {
        if( key.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(key));

        T value = (T)s_cacheDict.Get(key);

        if( value == null ) {

            if( loadFunc != null ) {
                // 调用委托加载对象
                value = loadFunc();

                s_cacheDict.Set(key, value, DateTime.Now.AddSeconds(CacheOption.CacheSeconds));
            }
        }

        return value;
    }


    /// <summary>
    /// 将一个对象添加到缓存中。
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
    /// 删除指定键对应的缓存对象
    /// </summary>
    /// <param name="key">缓存键</param>
    public static void RemoveObject(string key)
    {
        if( key.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(key));

        s_cacheDict.Remove(key);
    }


}
