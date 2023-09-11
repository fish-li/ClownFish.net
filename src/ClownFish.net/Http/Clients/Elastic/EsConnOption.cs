namespace ClownFish.Http.Clients.Elastic;

/// <summary>
/// elasticsearch连接参数
/// </summary>
public sealed class EsConnOption
{
    /// <summary>
    /// 是否采用HTTPS协议访问
    /// </summary>
    public bool IsHttps { get; set; }
    /// <summary>
    /// elasticsearch服务地址。【必填】
    /// </summary>
    public string Server { get; set; }
    /// <summary>
    /// 连接端口（大于 0 时有效）
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 登录用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 登录密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 请求超时时间，单位：毫秒
    /// </summary>
    public int TimeoutMs { get; set; } = 30_000;

    /// <summary>
    /// 索引名称后缀的时间格式，例如："-yyyyMMdd"
    /// </summary>
    public string IndexNameTimeFormat { get; set; } = "-yyyyMMdd";

    internal string Url;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Server={Server};UserName={UserName}";
    }

    internal void Validate()
    {
        if( this.Server.IsNullOrEmpty() )
            throw new ConfigurationErrorsException("Elasticsearch连接配置中没有指定 Server 参数。");

        if( this.IndexNameTimeFormat.IsNullOrEmpty() )
            this.IndexNameTimeFormat = "-yyyyMMdd";

        if( this.TimeoutMs <= 0 )
            this.TimeoutMs = 30_000;

        string scheme = this.IsHttps ? "https" : "http";
        if( this.Port > 0 )
            this.Url = $"{scheme}://{this.Server}:{this.Port}";
        else
            this.Url = $"{scheme}://{this.Server}";
    }

    /// <summary>
    /// 根据连接名称创建EsConnOption实例
    /// </summary>
    /// <param name="dbConnName"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public static EsConnOption Create(string dbConnName, int timeoutMs = 30_000)
    {
        DbConfig dbConfig = DbConnManager.GetAppDbConfig(dbConnName, true);
        return new EsConnOption {
            Server = dbConfig.Server,
            Port = dbConfig.Port.GetValueOrDefault(),
            UserName = dbConfig.UserName,
            Password = dbConfig.Password,
            TimeoutMs = timeoutMs
        };
    }
}
