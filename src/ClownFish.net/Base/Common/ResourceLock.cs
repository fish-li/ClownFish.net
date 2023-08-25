namespace ClownFish.Base;

/// <summary>
/// 进程内资源锁工具类
/// </summary>
public sealed class ResourceLock
{
    private readonly TSafeDictionary<string, object> _dict = new TSafeDictionary<string, object>(255);

    /// <summary>
    /// 根据字符串获取对应的锁对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetLock(string key)
    {
        return _dict.GetOrAdd(key, CreateLockObject);
    }

    private object CreateLockObject(string key)
    {
        return new object();
    }
}
