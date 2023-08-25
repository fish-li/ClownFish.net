namespace ClownFish.Data;

// 新增一种数据库支持的工作项
// https://note.youdao.com/ynoteshare/index.html?id=7de2f73d4190e9dcef528ed0f3a75272


/// <summary>
/// 数据库类型
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

    /// <summary>
    /// HBase
    /// </summary>
    HBase = 6,

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
    /// 未知的数据库类型
    /// </summary>
    Unknow = int.MaxValue
}
