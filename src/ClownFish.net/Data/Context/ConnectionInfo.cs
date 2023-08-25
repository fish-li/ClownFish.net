namespace ClownFish.Data;

/// <summary>
/// 数据库连接的描述信息
/// </summary>
public sealed class ConnectionInfo
{
    internal ConnectionInfo(string connectionString, string providerName)
    {
        if( string.IsNullOrEmpty(connectionString) )
            throw new ArgumentNullException("connectionString");

        this.ConnectionString = connectionString;

        this.ProviderName = string.IsNullOrEmpty(providerName)
                                ? "System.Data.SqlClient"   // 默认连接到SQLSERVER
                                : providerName;
    }

    internal ConnectionInfo(ConnectionStringSetting setting)
        : this(setting.ConnectionString, setting.ProviderName)
    { }

    internal ConnectionInfo(DbConfig dbConfig)
        : this(dbConfig.GetConnectionString(true), dbConfig.GetProviderName())
    { }


    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string ConnectionString { get; private set; }
    /// <summary>
    /// 数据库提供者类型名称
    /// </summary>
    public string ProviderName { get; private set; }


    private string _text;

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        // 这个方法可能会被多次调用，所以加个变量缓存结果
        if( _text == null )
            _text = this.ProviderName + "\n" + this.ConnectionString;

        return _text;
    }


}
