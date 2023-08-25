#if NETCOREAPP
using Microsoft.Extensions.ObjectPool;
#endif

namespace ClownFish.Base;

/// <summary>
/// StringBuilder对象池
/// </summary>
public static class StringBuilderPool
{
#if NETCOREAPP

    private static readonly DefaultObjectPool<StringBuilder> s_pool = new DefaultObjectPool<StringBuilder>(
            new StringBuilderPooledObjectPolicy {
                // StringBuilder 实例的初始容量
                InitialCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_InitialCapacity", 32 * 1024),

                // 归还时接受的 StringBuilder 实例最大容量，如果超过将不接受（放弃）
                MaximumRetainedCapacity = LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetainedCapacity", 1024 * 512)
            },
            // 缓存池中最多保留多少个 StringBuilder 实例
            LocalSettings.GetUInt("ClownFish_StringBuilderPool_MaximumRetained", Environment.ProcessorCount * 6)
        );


    /// <summary>
    /// 从缓存池中获取一个StringBuilder实例
    /// </summary>
    /// <returns></returns>
    public static StringBuilder Get()
    {
        return s_pool.Get();
    }


    /// <summary>
    /// 将StringBuilder实例归还到缓存池
    /// </summary>
    /// <param name="sb"></param>
    public static void Return(StringBuilder sb)
    {
        if( sb == null )
            return;

        s_pool.Return(sb);
    }

#else
    /// <summary>
    /// 从缓存池中获取一个StringBuilder实例
    /// </summary>
    /// <returns></returns>
    public static StringBuilder Get()
    {
        return new StringBuilder();
    }


    /// <summary>
    /// 将StringBuilder实例归还到缓存池
    /// </summary>
    /// <param name="sb"></param>
    public static void Return(StringBuilder sb)
    {
        // 不实现
    }
#endif

}
