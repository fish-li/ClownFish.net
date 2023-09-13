using ClownFish.Data.MultiDB.MsSQL;

namespace ClownFish.Data;

/// <summary>
/// DbProviderFactory的辅助工具类
/// </summary>
public static class DbClientFactory
{
    private static readonly TSafeDictionary<string, BaseClientProvider> s_dict = new TSafeDictionary<string, BaseClientProvider>();

    // 缓存常用的实例，减少字典查找，提升性能。
    private static BaseClientProvider s_mssqlClientProvider;
    private static BaseClientProvider s_mysqlClientProvider;


    static DbClientFactory()
    {
#if NETFRAMEWORK
        RegisterProvider(DatabaseClients.SqlClient, MsSqlClientProvider.Instance);
        RegisterProvider("System.Data.OleDb", OledbClientProvider.Instance);
        RegisterProvider("System.Data.Odbc", OdbcClientProvider.Instance);
#endif
    }

    /// <summary>
    /// 注册数据客户端提供者实例
    /// </summary>
    /// <param name="providerName">客户端提供者名称</param>
    /// <param name="provider">提供者实例</param>
    public static void RegisterProvider(string providerName, BaseClientProvider provider)
    {
        if( providerName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(providerName));

        if( provider == null )
            throw new ArgumentNullException(nameof(provider));

        s_dict[providerName] = provider;

        if( providerName == DatabaseClients.SqlClient ) {
            s_mssqlClientProvider = provider;
        }

        if( providerName == DatabaseClients.MySqlClient ) {
            s_mysqlClientProvider = provider;
        }
    }

    /// <summary>
    /// 根据指定的数据提供者名称创建对应的 BaseClientProvider 实例，
    /// 如果找不到匹配的结果，将会抛出异常。
    /// </summary>
    /// <param name="providerName">数据提供者名称</param>
    /// <returns>与数据提供者名称对应的DbProviderFactory实例</returns>
    public static BaseClientProvider GetProvider(string providerName)
    {
        if( string.IsNullOrEmpty(providerName) || providerName == DatabaseClients.SqlClient )
            // 默认就是使用SQLSERVER
            return s_mssqlClientProvider;


        // 常用类型就直接返回固定结果，优化性能
        if( s_mysqlClientProvider != null && providerName == DatabaseClients.MySqlClient )
            return s_mysqlClientProvider;


        if( s_dict.TryGetValue(providerName, out BaseClientProvider provider) )
            return provider;


        throw new NotSupportedException("不支持的数据提供者类型：" + providerName);
    }


    internal static DbProviderFactory GetDbProviderFactory(string providerName)
    {
        return GetProvider(providerName).ProviderFactory;
    }


    ///// <summary>
    ///// 当DbProviderFactories.GetFactory的注册机制无效时，再尝试使用反射方式查找DbProviderFactory
    ///// </summary>
    ///// <param name="providerName">数据提供者名称</param>
    ///// <returns>与数据提供者名称对应的DbProviderFactory实例</returns>
    //private static DbProviderFactory GetDbProviderFactoryViaReflection(string providerName)
    //{
    //    Type factoryType = (from asm in AsmHelper.GetLoadAssemblies(true)
    //                        from t in asm.GetPublicTypes()
    //                        where t.Namespace == providerName && typeof(DbProviderFactory).IsAssignableFrom(t)
    //                        select t).FirstOrDefault();

    //    if( factoryType == null )
    //        return null;

    //    return (DbProviderFactory)factoryType.InvokeMember("Instance",
    //                            BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);
    //}
}
