namespace ClownFish.Data;

// 新增一种数据库支持的工作项
// https://note.youdao.com/ynoteshare/index.html?id=7de2f73d4190e9dcef528ed0f3a75272


// DatabaseType 有2个用途：
//   1，实现 BaseClientProvider 时使用，可用于在代码中判断 dbContext.DatabaseType == xxx  编写特定的数据库代码
//   2，Venus 区分某个配置是什么数据库，可以针对性的监控


// DatabaseType, ProviderName, ClientProvider 映射关系
// 一个 ClientProvider 对应 一个 DatabaseType，在抽象类中定义的
// 多个 ProviderName 可以映射到一个 ClientProvider，可参考 MySqlProviderUtils.RegisterProvider(3)
// 注意：这里就存在一个问题：一个DatabaseType可能对应多个ProviderName


/// <summary>
/// 数据库类别
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// SQLSERVER
    /// </summary>
    SQLSERVER = 0,

    /// <summary>
    /// MySQL
    /// </summary>
    MySQL = 1,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSQL = 2,

    /// <summary>
    /// Oracle
    /// </summary>
    Oracle = 3,

    /// <summary>
    /// MongoDB
    /// </summary>
    MongoDB = 4,

    /// <summary>
    /// SQLite
    /// </summary>
    SQLite = 5,

    //HBase = 6,

    /// <summary>
    /// InfluxDB
    /// </summary>
    InfluxDB = 7,

    /// <summary>
    /// Elasticsearch
    /// </summary>
    Elasticsearch = 8,

    /// <summary>
    /// VictoriaMetrics
    /// </summary>
    VictoriaMetrics = 9,

    /// <summary>
    /// 达梦
    /// </summary>
    DaMeng = 10,




    /// <summary>
    /// 兼容 Odbc 连接协议的数据库
    /// </summary>
    Odbc = 10001,

    /// <summary>
    /// 兼容 OleDb 连接协议的数据库
    /// </summary>
    OleDb = 10002,

    
    /// <summary>
    /// 未知的数据库类型
    /// </summary>
    Unknow = int.MaxValue
}
