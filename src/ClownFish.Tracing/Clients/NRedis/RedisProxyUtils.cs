using Castle.DynamicProxy;
using ClownFish.Tracing;

namespace StackExchange.Redis;   // 使用这个命名空间就是为了方便使用

public static class RedisProxyUtils
{
    private static readonly ProxyGenerator s_proxyGenerator = new ProxyGenerator();

    public static IDatabase CreateProxy(this IDatabase db)
    {
        return s_proxyGenerator.CreateInterfaceProxyWithTargetInterface<IDatabase>(db, new DatabaseInterceptor());
    }
}
