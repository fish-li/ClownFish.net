namespace ClownFish.Base;


/// <summary>
/// 数据库连接操作接口
/// </summary>
public interface IDbConnManager
{
    /// <summary>
    /// 获取一个数据库连接配置
    /// </summary>
    /// <param name="connName"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    DbConfig GetAppDbConfig(string connName, bool checkExist);

    /// <summary>
    /// 获取租户库的连接配置
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="connType"></param>
    /// <param name="readonlyDB"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    DbConfig GetTntDbConfig(string tenantId, string connType, bool readonlyDB, bool checkExist);

    /// <summary>
    /// 创建主库的数据库连接
    /// </summary>
    /// <returns></returns>
    DbContext CreateMaster();

    /// <summary>
    /// 创建应用库的数据库连接
    /// </summary>
    /// <param name="connName"></param>
    /// <param name="longConnection"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    DbContext CreateAppDb(string connName, bool longConnection, string providerName);

    /// <summary>
    /// 创建租户库的数据库连接
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="readonlyDB"></param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    DbContext CreateTenant(string tenantId, bool readonlyDB, string providerName);
}


internal sealed class DefaultDbConnManagerImpl : IDbConnManager
{
    public static readonly DefaultDbConnManagerImpl Instance = new DefaultDbConnManagerImpl();

    public DbConfig GetAppDbConfig(string connName, bool checkExist)
    {
        if( string.IsNullOrEmpty(connName) )
            throw new ArgumentNullException(nameof(connName));

        DbConfig dbConfig = ConfigClient.Instance.GetAppDbConfig(connName, false)
                            ?? LocalDbConn.GetAppDbConfig(connName);

        if( dbConfig == null && checkExist )
            throw new DatabaseNotFoundException("没有找到指定的数据库连接参数，connName：" + connName);

        return dbConfig;
    }

    public DbConfig GetTntDbConfig(string tenantId, string connType, bool readonlyDB, bool checkExist)
    {
        if( string.IsNullOrEmpty(tenantId) )
            throw new ArgumentNullException(nameof(tenantId));

        if( connType.IsNullOrEmpty() )
            connType = "xsql";

        string flag = readonlyDB ? DbConnManager.ReadonlyFlag : null;

        DbConfig dbConfig = ConfigClient.Instance.GetTntDbConfig(connType, tenantId, flag, false)
                            ?? LocalDbConn.GetTntDbConfig(connType, tenantId, flag);


        if( dbConfig == null && checkExist )
            throw new DatabaseNotFoundException($"没有找到指定的数据库连接参数，connType='{connType}', tenantId='{tenantId}', flag='{flag}'");

        return dbConfig;
    }

    public DbContext CreateMaster()
    {
        return CreateAppDb(DbConnManager.Master, false, null);
    }

    public DbContext CreateAppDb(string connName, bool longConnection, string providerName)
    {
        DbConfig dbConfig = GetAppDbConfig(connName, true);

        return dbConfig.CreateDbContext(longConnection, providerName);
    }

    public DbContext CreateTenant(string tenantId, bool readonlyDB, string providerName)
    {
        DbConfig dbConfig = GetTntDbConfig(tenantId, null, readonlyDB, true);

        return dbConfig.CreateDbContext(false, providerName);
    }
}



/// <summary>
/// 支撑SaaS多租户数据库连接的工具类
/// </summary>
public static class DbConnManager
{
    /// <summary>
    /// string "master"
    /// </summary>
    public static readonly string Master = "master";

    /// <summary>
    /// string "_readonly"
    /// </summary>
    public static readonly string ReadonlyFlag = "_readonly";

    private static IDbConnManager s_instance = DefaultDbConnManagerImpl.Instance;

    /// <summary>
    /// 设置实现方式
    /// </summary>
    /// <param name="instance"></param>
    public static void SetImpl(IDbConnManager instance)
    {
        s_instance = instance ?? DefaultDbConnManagerImpl.Instance;
    }

    /// <summary>
    /// 获取数据库连接参数
    /// </summary>
    /// <param name="connName"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static DbConfig GetAppDbConfig(string connName, bool checkExist = true)
    {
        return s_instance.GetAppDbConfig(connName, checkExist);
    }


    /// <summary>
    /// 获取某个租户库的连接参数
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="connType">可使用 Consts.ConnType 定义的名称</param>
    /// <param name="readonlyDB"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static DbConfig GetTntDbConfig(string tenantId, string connType = null, bool readonlyDB = false, bool checkExist = true)
    {
        return s_instance.GetTntDbConfig(tenantId, connType, readonlyDB, checkExist);
    }


    /// <summary>
    /// 创建 master 数据库连接
    /// </summary>
    /// <returns></returns>
    public static DbContext CreateMaster()
    {
        return s_instance.CreateMaster();
    }


    /// <summary>
    /// 根据 连接名称 创建对应的数据库连接
    /// </summary>
    /// <param name="connName">数据库连接名称</param>
    /// <param name="longConnection">是否用于“长连接”</param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static DbContext CreateAppDb(string connName, bool longConnection = false, string providerName = null)
    {
        return s_instance.CreateAppDb(connName, longConnection, providerName);
    }


    /// <summary>
    /// 根据 租户ID 创建对应的SQL数据库连接
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    /// <param name="readonlyDB">是否连接【只读库】</param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static DbContext CreateTenant(string tenantId, bool readonlyDB = false, string providerName = null)
    {
        return s_instance.CreateTenant(tenantId, readonlyDB, providerName);
    }

}
