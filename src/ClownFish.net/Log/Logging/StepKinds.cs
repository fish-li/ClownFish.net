namespace ClownFish.Log.Logging;

/// <summary>
/// 一些预定义的 步骤类型 StepKind
/// </summary>
public static class StepKinds
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly string SQLConn = "sqlconn";

    /// <summary>
    /// 
    /// </summary>
    public static readonly string SQLExec = "sqlcommand";


    /// <summary>
    /// 
    /// </summary>
    public static readonly string SQLBatch = "sqlbatch";

    /// <summary>
    /// 
    /// </summary>
    public static readonly string SQLTrans = "sqltrans";

    /// <summary>
    /// 
    /// </summary>
    public static readonly string HttpRpc = "httprpc";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Redis = "redis";

    /// <summary>
    /// 
    /// </summary>
    public static readonly string Mail = "mail";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Rabbit = "rabbit";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Kafka = "kafka";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Pulsar = "pulsar";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string ElasticSearch = "es";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string InfluxDB = "influx";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Memcache = "memcache";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string MongoDB = "mongo";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Hbase = "hbase";
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Cassandra = "cassandra";
}
