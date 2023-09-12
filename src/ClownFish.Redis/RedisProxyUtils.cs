using Castle.DynamicProxy;
using ClownFish.NRedis;

namespace StackExchange.Redis;   // 使用这个命名空间就是为了方便使用

/// <summary>
/// Redis IDatabase 扩展工具类
/// </summary>
public static class RedisProxyUtils
{
    private static readonly ProxyGenerator s_proxyGenerator = new ProxyGenerator();


    /// <summary>
    /// 创建一个IDatabase的代理类
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    public static IDatabase CreateProxy(this IDatabase db)
    {
        return s_proxyGenerator.CreateInterfaceProxyWithTargetInterface<IDatabase>(db, new DatabaseInterceptor());
    }
}
