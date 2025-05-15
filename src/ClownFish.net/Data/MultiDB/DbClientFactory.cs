using ClownFish.Data.MultiDB.MsSQL;

namespace ClownFish.Data;

/// <summary>
/// DbProviderFactory的辅助工具类
/// </summary>
public static class DbClientFactory
{
    // DatabaseType, ProviderName, ClientProvider 映射关系
    // 一个 ClientProvider 对应 一个 DatabaseType，在抽象类中定义的
    // 多个 ProviderName 可以映射到一个 ClientProvider，可参考 MySqlProviderUtils.RegisterProvider(3)
    // 注意：这里就存在一个问题：一个DatabaseType可能对应多个ProviderName

    internal class ProviderRegisterInfo
    {
        public BaseClientProvider ClientProvider;
        public string ProviderName;
    }

    private static readonly TSafeDictionary<string, ProviderRegisterInfo> s_dict = new TSafeDictionary<string, ProviderRegisterInfo>();
    private static readonly TSafeDictionary<DatabaseType, ProviderRegisterInfo> s_dict2 = new TSafeDictionary<DatabaseType, ProviderRegisterInfo>();

    // 缓存常用的实例，减少字典查找，提升性能。
    private static ProviderRegisterInfo s_mssqlProvider;
    private static ProviderRegisterInfo s_mysqlProvider;
    private static ProviderRegisterInfo s_pgsqlProvider;


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

        ProviderRegisterInfo registerInfo = new ProviderRegisterInfo {
            ClientProvider = provider,
            ProviderName = providerName,
        };

        // 这里直接修改集合，允许多次调用（会覆盖）
        s_dict[providerName] = registerInfo;
        s_dict2[provider.DatabaseType] = registerInfo;

        Console2.Info($"Register DbClient Provider: {providerName} => {provider.GetType().FullName}");

        if( providerName == DatabaseClients.SqlClient ) {
            s_mssqlProvider = registerInfo;
        }

        if( providerName == DatabaseClients.MySqlClient ) {
            s_mysqlProvider = registerInfo;
        }

        if( providerName == DatabaseClients.PostgreSQL ) {
            s_pgsqlProvider = registerInfo;
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
        // 默认就是使用SQLSERVER
        if( providerName.IsNullOrEmpty() )
            providerName = MsSqlProviderUtils.CurrentProviderName;


        // 常用类型就直接返回固定结果，优化性能
        if( s_mssqlProvider != null && providerName == DatabaseClients.SqlClient )
            return s_mssqlProvider.ClientProvider;
        
        if( s_mysqlProvider != null && providerName == DatabaseClients.MySqlClient )
            return s_mysqlProvider.ClientProvider;

        if( s_pgsqlProvider != null && providerName == DatabaseClients.PostgreSQL )
            return s_pgsqlProvider.ClientProvider;


        // 非常见类型就查找字典表
        if( s_dict.TryGetValue(providerName, out ProviderRegisterInfo registerInfo) )
            return registerInfo.ClientProvider;


        throw new NotSupportedException("没有注册的数据提供者类型：" + providerName);
    }


    internal static DbProviderFactory GetDbProviderFactory(string providerName)
    {
        return GetProvider(providerName).ProviderFactory;
    }


    internal static ProviderRegisterInfo GetRegisterInfo(DatabaseType databaseType)
    {
        if( databaseType == DatabaseType.SQLSERVER && s_mssqlProvider != null )
            return s_mssqlProvider;

        if( databaseType == DatabaseType.MySQL && s_mysqlProvider != null )
            return s_mysqlProvider;

        if( databaseType == DatabaseType.PostgreSQL && s_pgsqlProvider != null )
            return s_pgsqlProvider;



        if( s_dict2.TryGetValue(databaseType, out ProviderRegisterInfo registerInfo) )
            return registerInfo;


        throw new NotSupportedException("不支持的数据库类别，DatabaseType=" + databaseType);
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
