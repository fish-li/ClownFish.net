namespace ClownFish.Http.Clients.RabbitMQ;

/// <summary>
/// RabbitMQ的连接信息
/// </summary>
public sealed class RabbitOption
{
    /// <summary>
    /// VHost，默认值："/"
    /// </summary>
    public string VHost { get; set; } = "/";
    /// <summary>
    /// RabbitMQ服务地址。【必填】
    /// </summary>
    public string Server { get; set; }
    /// <summary>
    /// 连接端口（大于 0 时有效）
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// HTTP协议的连接端口（大于 0 时有效）
    /// </summary>
    public int HttpPort { get; set; }
    /// <summary>
    /// 登录名。【必填】
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// 登录密码。
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Http请求超时时间，单位：毫秒
    /// </summary>
    public int? HttpTimeoutMs { get; set; }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string port = this.Port != 0 ? this.Port.ToString() : "";
        string hport = this.HttpPort != 0 ? this.HttpPort.ToString() : "";
        string vhost = (this.VHost.IsNullOrEmpty() || this.VHost == "/") ? "" : this.VHost;

        return $"Server={Server};Port={port};HttpPort={hport};Username={Username};VHost={vhost}";
    }

    /// <summary>
    /// 创建一个HttpOption用于调用RabbitMQ
    /// </summary>
    /// <param name="urlPath"></param>
    /// <returns></returns>
    public HttpOption GetHttpOption(string urlPath)
    {
        int port = this.HttpPort > 0 ? this.HttpPort : 15672;
        int timeout = this.HttpTimeoutMs.HasValue && this.HttpTimeoutMs.Value > 0 
                        ? this.HttpTimeoutMs.Value : HttpClientDefaults.RabbitHttpClientTimeout;

        HttpOption httpOption = new HttpOption {
            Url = $"http://{this.Server}:{port}{urlPath}",
            Timeout = timeout,
        };

        if( this.Username.HasValue() ) {
            httpOption.SetBasicAuthorization(this.Username, this.Password);
        }
        return httpOption;
    }

    /// <summary>
    /// 验证数据成员
    /// </summary>
    public void Validate()
    {
        if( this.Server.IsNullOrEmpty() )
            throw new ConfigurationErrorsException("RabbitMQ连接配置中没有指定 Server 参数。");

        if( this.Username.IsNullOrEmpty() )
            throw new ConfigurationErrorsException("RabbitMQ连接配置中没有指定 Username 参数。");

        if( this.VHost.IsNullOrEmpty() )
            this.VHost = "/";
    }

}
