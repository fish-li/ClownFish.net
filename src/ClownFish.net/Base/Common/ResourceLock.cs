namespace ClownFish.Base;

/// <summary>
/// 进程内资源锁工具类
/// </summary>
public sealed class ResourceLock
{
    /// <summary>
    /// 根据字符串获取对应的锁对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
#if NET9_0_OR_GREATER
    public Lock GetLock(string key)
    {
        return _dict.GetOrAdd(key, CreateLockObject);
    }

    private readonly TSafeDictionary<string, Lock> _dict = new TSafeDictionary<string, Lock>(255);

    private Lock CreateLockObject(string key)
    {
        return new Lock();
    }
#else
    public object GetLock(string key)
    {
        return _dict.GetOrAdd(key, CreateLockObject);
    }

    private readonly TSafeDictionary<string, object> _dict = new TSafeDictionary<string, object>(255);

    private object CreateLockObject(string key)
    {
        return new object();
    }
#endif

}
