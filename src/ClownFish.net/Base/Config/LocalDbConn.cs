namespace ClownFish.Base;
internal static class LocalDbConn
{
    public static DbConfig GetAppDbConfig(string name)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        return MemoryConfig.GetDbConfig(name)
                ?? AppConfig.GetDbConfig(name);
    }


    public static DbConfig GetTntDbConfig(string connType, string tenantId, string flag)
    {
        if( tenantId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(tenantId));

        // 本地配置文件中管理数据库连接，为了简化数据库的连接配置，租户库的连接采用 “约定命名” 方式来处理，
        // 格式：tenant_{connType}_{tenantId}
        //       SQLSERVER/MySQL/PostgreSQL 可读可写的连接名称: tenant_xsql_{tenantId}
        //       SQLSERVER/MySQL/PostgreSQL 只读库的连接名称:   tenant_xsql_{tenantId}_readonly
        //       InfluxDB 的连接名称：       tenant_influx_{tenantId}
        //       VictoriaMetrics 的连接名称：tenant_vm_{tenantId}

        string connName = flag.HasValue()
                            ? $"tenant_{connType}_{tenantId}{flag}"
                            : $"tenant_{connType}_{tenantId}";

        return MemoryConfig.GetDbConfig(connName)
                ?? AppConfig.GetDbConfig(connName);
    }


}
