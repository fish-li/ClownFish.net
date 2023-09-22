namespace ClownFish.Http.Clients.Elastic;

/// <summary>
/// elasticsearch连接参数
/// </summary>
public sealed class EsConnOption
{
    /// <summary>
    /// 是否采用HTTPS协议访问
    /// </summary>
    public bool Https { get; set; }
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

    private string _url;

    /// <summary>
    /// Url
    /// </summary>
    public string Url {
        get {
            if( _url == null ) {
                string scheme = this.Https ? "https" : "http";
                if( this.Port > 0 )
                    _url = $"{scheme}://{this.Server}:{this.Port}";
                else
                    _url = $"{scheme}://{this.Server}";
            }
            return _url;
        }
    }

    /// <summary>
    /// Set TimeoutMs
    /// </summary>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public EsConnOption SetTimeoutMs(int timeoutMs)
    {
        this.TimeoutMs = timeoutMs;
        return this;
    }

    /// <summary>
    /// Set IndexNameTimeFormat
    /// </summary>
    /// <param name="indexNameTimeFormat"></param>
    /// <returns></returns>
    public EsConnOption SetIndexNameTimeFormat(string indexNameTimeFormat)
    {
        this.IndexNameTimeFormat = indexNameTimeFormat;
        return this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Server={Server};UserName={UserName}";
    }

    /// <summary>
    /// 验证数据成员
    /// </summary>
    /// <exception cref="ConfigurationErrorsException"></exception>
    public void Validate()
    {
        if( this.Server.IsNullOrEmpty() )
            throw new ConfigurationErrorsException("Elasticsearch连接配置中没有指定 Server 参数。");

        if( this.IndexNameTimeFormat.IsNullOrEmpty() )
            this.IndexNameTimeFormat = "-yyyyMMdd";

        if( this.TimeoutMs <= 0 )
            this.TimeoutMs = 30_000;
    }

    /// <summary>
    /// 根据连接名称创建EsConnOption实例
    /// </summary>
    /// <param name="connName"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static EsConnOption Create(string connName, bool checkExist = true)
    {
        EsConnOption opt = Create1(DbConnManager.GetAppDbConfig(connName, false))
                            ?? Settings.GetSetting<EsConnOption>(connName, false);
        if( opt != null )
            return opt;

        if( checkExist )
            throw new DatabaseNotFoundException("没有找到指定的连接配置参数，connName：" + connName);

        return null;
    }


    /// <summary>
    /// 根据DbConfig实例创建EsConnOption实例
    /// </summary>
    /// <param name="dbConfig"></param>
    /// <returns></returns>
    public static EsConnOption Create1(DbConfig dbConfig)
    {
        if( dbConfig == null )
            return null;

        var args = dbConfig.Args.ToKVList(';', '=');
        return new EsConnOption {
            Server = dbConfig.Server,
            Port = dbConfig.Port.GetValueOrDefault(),
            UserName = dbConfig.UserName,
            Password = dbConfig.Password,
            Https = args.GetValue(nameof(Https)).TryToBool(),
            TimeoutMs = args.GetValue(nameof(TimeoutMs)).TryToInt(),
            IndexNameTimeFormat = args.GetValue(nameof(IndexNameTimeFormat))
        };
    }
}
