namespace ClownFish.Data;

/// <summary>
/// 定义IDbConfig相关的扩展方法
/// </summary>
public static class DbConfigExtensions
{
    /// <summary>
    /// 根据IDbConfig的实例创建对应的DbContext实例
    /// </summary>
    /// <param name="dbConfig"></param>
    /// <param name="longConnection">是否用于“长连接”</param>
    /// <param name="providerName"></param>
    /// <returns></returns>
    public static DbContext CreateDbContext(this IDbConfig dbConfig, bool longConnection = false, string providerName = null)
    {
        if( dbConfig == null )
            throw new ArgumentNullException(nameof(dbConfig));

        if( longConnection ) {
            string connectionString = dbConfig.GetConnectionString(true);       // 连接字符串包含 “数据库名称”

            if( providerName.IsNullOrEmpty() )
                providerName = dbConfig.GetProviderName();

            return DbContext.Create(connectionString, providerName);
        }
        else {
            string connectionString = dbConfig.GetConnectionString(false);      // 连接字符串【不包含】 “数据库名称”

            if( providerName.IsNullOrEmpty() )
                providerName = dbConfig.GetProviderName();

            DbContext dbContext = DbContext.Create(connectionString, providerName);
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

        return dbConfig.DbType switch {
            DatabaseType.SQLSERVER => ClownFish.Data.DatabaseClients.SqlClient,
            DatabaseType.MySQL => ClownFish.Data.DatabaseClients.MySqlClient,
            DatabaseType.PostgreSQL => ClownFish.Data.DatabaseClients.PostgreSQL,
            DatabaseType.DaMeng => ClownFish.Data.DatabaseClients.DaMeng,
            _ => throw new NotSupportedException($"不支持的 DbType: {dbConfig.DbType}")
        };
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

        return dbConfig.DbType switch {
            DatabaseType.SQLSERVER => GetMsSqlConnectionString(dbConfig, includeDatabase),
            DatabaseType.MySQL => GetMySqlConnectionString(dbConfig, includeDatabase),
            DatabaseType.PostgreSQL => GetPostgreSQLConnectionString(dbConfig, includeDatabase),
            DatabaseType.MongoDB => GetMongoDbConnectionString(dbConfig, includeDatabase),
            DatabaseType.DaMeng => GetDamengConnectionString(dbConfig, includeDatabase),
            _ => throw new NotSupportedException($"不支持的 DbType: {dbConfig.DbType}")
        };
    }

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


    private static string GetMsSqlConnectionString(IDbConfig db, bool includeDatabase)
    {
        // SQLSERVER端口号格式： server=192.168.11.100,1433;
        // https://docs.microsoft.com/zh-cn/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring
        // format: server=xxx,port;database=xxxx;uid=xxx

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("Server=").Append(db.Server);

            if( db.Port.HasValue && db.Port.Value > 0 )
                sb.Append(',').Append(db.Port.Value);

            if( includeDatabase )
                sb.Append(";Database=").Append(db.Database);

            sb.Append(";Uid=").Append(db.UserName)
                .Append(";Pwd=").Append(db.Password)
                .Append(";Application Name=").Append(EnvUtils.GetAppName())
                .Append(';').Append(db.Args);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    private static string GetMySqlConnectionString(IDbConfig db, bool includeDatabase)
    {
        // 参考：https://www.connectionstrings.com/mysql/

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("Server=").Append(db.Server);

            if( db.Port.HasValue && db.Port.Value > 0 )
                sb.Append(";Port=").Append(db.Port.Value);

            if( includeDatabase )
                sb.Append(";Database=").Append(db.Database);

            sb.Append(";Uid=").Append(db.UserName)
                .Append(";Pwd=").Append(db.Password)
                .Append(';').Append(db.Args);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    private static string GetPostgreSQLConnectionString(IDbConfig db, bool includeDatabase)
    {
        // PostgreSQL 连接参数
        // https://www.npgsql.org/doc/connection-string-parameters.html

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("Host=").Append(db.Server);

            if( db.Port.HasValue && db.Port.Value > 0 )
                sb.Append(";Port=").Append(db.Port.Value);

            if( includeDatabase )
                sb.Append(";Database=").Append(db.Database);

            sb.Append(";Username=").Append(db.UserName)
                .Append(";Password=").Append(db.Password)
                .Append(";Application Name=").Append(EnvUtils.GetAppName())
                .Append(';').Append(db.Args);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
        

    private static string GetMongoDbConnectionString(IDbConfig db, bool includeDatabase)
    {
        // https://docs.mongodb.com/manual/reference/connection-string/
        // mongodb://[username:password@]host1[:port1][,...hostN[:portN]][/[database][?options]]


        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("mongodb://");

            if( db.UserName.IsNullOrEmpty() == false )
                sb.Append(db.UserName).Append(':').Append(db.Password).Append('@');

            sb.Append(db.Server);

            if( db.Port.HasValue && db.Port.Value > 0 )
                sb.Append(":").Append(db.Port.Value);

            sb.Append('/');

            if( includeDatabase )
                sb.Append(db.Database);

            if( db.Args.IsNullOrEmpty() == false )
                sb.Append('?').Append(db.Args);


            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    private static string GetDamengConnectionString(IDbConfig db, bool includeDatabase)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append("server=").Append(db.Server);

            if( db.Port.HasValue && db.Port.Value > 0 )
                sb.Append(";port=").Append(db.Port.Value);

            if( includeDatabase )
                sb.Append(";schema=").Append(db.Database);

            sb.Append(";user=").Append(db.UserName)
                .Append(";password=").Append(db.Password)
                .Append(';').Append(db.Args);

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
}
