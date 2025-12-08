namespace ClownFish.Data;

/// <summary>
/// 定义IDbConfig相关的扩展方法
/// </summary>
public static class DbConfigExtensions
{
    // 说明
    // 在 MySQL，SQLSERVER 的客户端实现中，都有连接池的设计，
    // 然而它们的连接池不是单一实例存在，而是会根据不同的 “连接字符串” 创建不同的 “连接池” 实例，
    // 这种设计对于大多数程序来说是可行的。

    // 但是对于SaaS程序来说，不同的租户对应不同的数据库后，如果继续按照常规模式构造数据库连接字符串，
    // 例如：server=xxxx;database=[租户数据库名称];uid=xxx;pwd=xxx
    // 将会产生很多不同的连接字符串，假设有一千个租户，那么就会有一千个不同的连接字符串，它们将会产生一千个“连接池”实例！
    // 而每个连接池中的数据库连接又有多个实例，默认最大值可达到100个，最终会产生大量的连接对象（理论最大值可达 10W），消耗数据库的资源，甚至影响程序的稳定运行。

    // 解决办法：
    // 1、构造连接字符串时，不包含 Database=xxxx 这部分
    // 2、连接打开后，再切换到指定的租户库上。


    // ####################################################################
    // 注意：上面的思路对于 Npgsql/PostgreSQL 来说就很不合适了~~~
    // PostgreSQL 的 wire protocol 在连接初始化阶段就确定了目标数据库。一旦连接建立，就没有类似于 USE database 的协议命令来改变这个绑定。
    // Npgsql的切换数据库是先“关闭连接”再修改连接字符串，再“打开连接”，所以显得非常SB~~~~~ 
    // 所以，在基于PostgreSQL数据库设计SaaS架构时，可采用  schema 替代多个数据库，例如：
    // CREATE SCHEMA tenant1;  CREATE SCHEMA tenant2;
    // 然后在连接内切换 schema 来实现 **切换租户库** 的需求，SET search_path TO tenant1;
    // 使用切换 schema 只能解决连接问题，这样做又将面临新的挑战，例如：备份恢复~~~


    /// <summary>
    /// 根据IDbConfig的实例创建对应的DbContext实例
    /// </summary>
    /// <param name="dbConfig"></param>
    /// <param name="includeDatabase">在连接数据库时是否包含“数据库名”</param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static DbContext CreateDbContext(this IDbConfig dbConfig, bool includeDatabase = false, string providerName = null)
    {
        if( dbConfig == null )
            throw new ArgumentNullException(nameof(dbConfig));

        var regInfo = DbClientFactory.GetRegisterInfo(dbConfig.DbType);

        // PostgreSQL不支持 USE database 协议命令，所以在构造连接字符串时始终包含数据库名称
        if( dbConfig.DbType == DatabaseType.PostgreSQL )
            includeDatabase = true;

        if( includeDatabase ) {
            string connectionString = regInfo.ClientProvider.GetConnectionString(dbConfig, true);   // 连接字符串【包含】 “数据库名称”

            if( providerName.IsNullOrEmpty() )
                providerName = regInfo.ProviderName;

            return DbContext.Create(connectionString, providerName, dbConfig.Name);
        }
        else {
            string connectionString = regInfo.ClientProvider.GetConnectionString(dbConfig, false);  // 连接字符串【不包含】 “数据库名称”

            if( providerName.IsNullOrEmpty() )
                providerName = regInfo.ProviderName;

            DbContext dbContext = DbContext.Create(connectionString, providerName, dbConfig.Name);
            dbContext.ChangeDatabase(dbConfig.Database);
            return dbContext;
        }
    }


    /// <summary>
    /// 根据IDbConfig实例的属性信息获取对应的数据库驱动提供者名称
    /// </summary>
    /// <param name="dbConfig"></param>
    /// <returns></returns>
    public static string GetProviderName(this IDbConfig dbConfig)
    {
        if( dbConfig == null )
            throw new ArgumentNullException(nameof(dbConfig));

        return DbClientFactory.GetRegisterInfo(dbConfig.DbType).ProviderName;
    }


    /// <summary>
    /// 根据IDbConfig实例的属性信息获取对应的数据库连接字符串
    /// </summary>
    /// <param name="dbConfig"></param>
    /// <param name="includeDatabase">产生的连接字符串中是否包含数据库部分</param>
    /// <returns></returns>
    public static string GetConnectionString(this IDbConfig dbConfig, bool includeDatabase = false)
    {
        if( dbConfig == null )
            throw new ArgumentNullException(nameof(dbConfig));

        // PostgreSQL不支持 USE database 协议命令，所以在构造连接字符串时始终包含数据库名称
        if( dbConfig.DbType == DatabaseType.PostgreSQL )
            includeDatabase = true;

        return DbClientFactory.GetRegisterInfo(dbConfig.DbType).ClientProvider.GetConnectionString(dbConfig, includeDatabase);
    }


    //private static string GetMongoDbConnectionString(IDbConfig db, bool includeDatabase)
    //{
    //    // https://docs.mongodb.com/manual/reference/connection-string/
    //    // mongodb://[username:password@]host1[:port1][,...hostN[:portN]][/[database][?options]]


    //    StringBuilder sb = StringBuilderPool.Get();
    //    try {
    //        sb.Append("mongodb://");

    //        if( db.UserName.IsNullOrEmpty() == false )
    //            sb.Append(db.UserName).Append(':').Append(db.Password).Append('@');

    //        sb.Append(db.Server);

    //        if( db.Port.HasValue && db.Port.Value > 0 )
    //            sb.Append(":").Append(db.Port.Value);

    //        sb.Append('/');

    //        if( includeDatabase && db.Database.HasValue() )
    //            sb.Append(db.Database);

    //        if( db.Args.IsNullOrEmpty() == false )
    //            sb.Append('?').Append(db.Args);


    //        return sb.ToString();
    //    }
    //    finally {
    //        StringBuilderPool.Return(sb);
    //    }
    //}




}
