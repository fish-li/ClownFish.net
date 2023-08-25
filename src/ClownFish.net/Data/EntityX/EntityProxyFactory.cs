namespace ClownFish.Data;

/// <summary>
/// 实体代理的工厂类型
/// </summary>
internal static class EntityProxyFactory
{
    private static readonly Hashtable s_table = Hashtable.Synchronized(new Hashtable(2048));

    /// <summary>
    /// 获取已加载的实体类型
    /// </summary>
    /// <returns></returns>
    internal static List<Type> GetEntityTypes()
    {
        List<Type> list = new List<Type>();

        foreach( DictionaryEntry kv in s_table ) {
            list.Add(kv.Key as Type);
        }

        return list;
    }

    /// <summary>
    /// 获取某个实体类型的代理类型
    /// </summary>
    /// <returns></returns>
    public static Type GetProxy(Type entityType)
    {
        return s_table[entityType] as Type;
    }


    /// <summary>
    /// 注册实体的代理类型
    /// </summary>
    /// <param name="proxyType"></param>
    public static void Register(Type proxyType)
    {
        if( proxyType == null )
            throw new ArgumentNullException(nameof(proxyType));

        Type entityType = proxyType.BaseType;
        if( entityType.BaseType != TypeList.Entity )
            throw new ArgumentException("无效的实体代理类型：" + proxyType.FullName);

        if( TypeList.IEntityProxy.IsAssignableFrom(proxyType) == false )
            throw new ArgumentException("无效的实体代理类型：" + proxyType.FullName);

        lock( ((ICollection)s_table).SyncRoot ) {
            s_table[entityType] = proxyType;
        }
    }


    /// <summary>
    /// （内部使用的）批量注册EntityProxy
    /// </summary>
    /// <param name="proxyTypes"></param>
    internal static void BatchRegister(IEnumerable<Type> proxyTypes)
    {
        // 内部使用版本，不做参数检查

        lock( ((ICollection)s_table).SyncRoot ) {

            foreach( Type proxyType in proxyTypes ) {

                Type entityType = proxyType.BaseType;
                s_table[entityType] = proxyType;
            }
        }
    }
}
