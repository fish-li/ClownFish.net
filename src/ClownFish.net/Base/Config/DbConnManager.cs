namespace ClownFish.Base;

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



    /// <summary>
    /// 获取数据库连接参数
    /// </summary>
    /// <param name="connName"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static DbConfig GetAppDbConfig(string connName, bool checkExist = true)
    {
        if( string.IsNullOrEmpty(connName) )
            throw new ArgumentNullException(nameof(connName));

        DbConfig dbConfig = ConfigClient.Instance.GetAppDbConfig(connName, false)
                            ?? LocalDbConn.GetAppDbConfig(connName);

        if( dbConfig == null && checkExist )
            throw new DatabaseNotFoundException("没有找到指定的数据库连接参数，connName：" + connName);

        return dbConfig;
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
        if( string.IsNullOrEmpty(tenantId) )
            throw new ArgumentNullException(nameof(tenantId));

        if( connType.IsNullOrEmpty() )
            connType = "xsql";

        string flag = readonlyDB ? ReadonlyFlag : null;

        DbConfig dbConfig = ConfigClient.Instance.GetTntDbConfig(connType, tenantId, flag, false)
                            ?? LocalDbConn.GetTntDbConfig(connType, tenantId, flag);


        if( dbConfig == null && checkExist )
            throw new DatabaseNotFoundException($"没有找到指定的数据库连接参数，connType='{connType}', tenantId='{tenantId}', flag='{flag}'");

        return dbConfig;
    }


    /// <summary>
    /// 创建 master 数据库连接
    /// </summary>
    /// <returns></returns>
    public static DbContext CreateMaster()
    {
        return CreateAppDb(DbConnManager.Master);
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
        DbConfig dbConfig = GetAppDbConfig(connName, true);

        return dbConfig.CreateDbContext(longConnection, providerName);
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
        DbConfig dbConfig = GetTntDbConfig(tenantId, null, readonlyDB, true);

        return dbConfig.CreateDbContext(false, providerName);
    }


}
