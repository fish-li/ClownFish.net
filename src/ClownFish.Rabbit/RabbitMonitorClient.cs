namespace ClownFish.Rabbit;

/// <summary>
/// 交换机信息
/// </summary>
public sealed class ExchangeInfo
{
    /// <summary>
    /// 交换机名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 消息进入数量
    /// </summary>
    public double Incoming { get; set; }
    /// <summary>
    /// 消息发出数量
    /// </summary>
    public double Deliver { get; set; }

}

/// <summary>
/// 队列信息
/// </summary>
public sealed class QueueInfo
{
    /// <summary>
    /// 队列名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 队列状态
    /// </summary>
    public string State { get; set; }
    /// <summary>
    /// 队列长度
    /// </summary>
    public long Length { get; set; }
    /// <summary>
    /// 消费者数量
    /// </summary>
    public long Consumers { get; set; }
    /// <summary>
    /// 消息进入数量
    /// </summary>
    public double Incoming { get; set; }
    /// <summary>
    /// 消息发出数量
    /// </summary>
    public double Deliver { get; set; }
}


/// <summary>
/// 表示RabbitMQ结果无效导致的异常
/// </summary>
[Serializable]
public sealed class RabbitResultException : Exception
{
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public RabbitResultException(string message, Exception innerException) : base(message, innerException) { }
}



/// <summary>
/// RabbitMQ 监控客户端
/// </summary>
public sealed class RabbitMonitorClient
{
    private readonly RabbitOption _option;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="settingName"></param>
    public RabbitMonitorClient(string settingName)
    {
        if( settingName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(settingName));

        _option = Settings.GetSetting<RabbitOption>(settingName, true);
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="option"></param>
    public RabbitMonitorClient(RabbitOption option)
    {
        if( option == null )
            throw new ArgumentNullException(nameof(option));

        _option = option;
    }


    /// <summary>
    /// 获取所有交换机信息
    /// </summary>
    /// <param name="timout">超时时间，单位：毫秒</param>
    /// <param name="retry">出现异常的重试策略</param>
    /// <returns></returns>
    public List<ExchangeInfo> GetExchanges(int? timout, Retry retry = null)
    {
        string url = "/api/exchanges/" + System.Web.HttpUtility.UrlEncode(_option.VHost);
        HttpOption httpOption = _option.GetHttpOption(url);

        if( timout.HasValue )
            httpOption.Timeout = timout.Value;

        string responseJson = httpOption.GetResult(retry);
        try {
            return GetExchangeList(responseJson);
        }
        catch( Exception ex ) {
            WriteTempFile("rabbit_exchanges_error_data.json", responseJson);

            // RabbitMQ的返回结果很“随意”，没有很好的方式判断结果是否为“正常结果”
            throw new RabbitResultException($"RabbitMQ 没有正确响应， {url} 返回结果非预期", ex);
        }
    }


    private List<ExchangeInfo> GetExchangeList(string responseJson)
    {
        List<ExchangeInfo> list = new List<ExchangeInfo>();
        List<ExpandoObject> rows = responseJson.FromJson<List<ExpandoObject>>();

        foreach( dynamic x in rows ) {

            ExchangeInfo info = new ExchangeInfo();
            info.Name = x.name;

            if( info.Name.IsNullOrEmpty() )
                info.Name = "(AMQP default)";

            try {
                info.Incoming = Convert.ToDouble(x.message_stats.publish_in_details.rate);
                info.Deliver = Convert.ToDouble(x.message_stats.publish_out_details.rate);
            }
            catch { }

            list.Add(info);
        }
        return list;
    }


    /// <summary>
    /// 获取所有队列信息
    /// </summary>
    /// <param name="timout"></param>
    /// <param name="retry"></param>
    /// <returns></returns>
    public List<QueueInfo> GetQueues(int? timout, Retry retry = null)
    {
        string url = "/api/queues/" + System.Web.HttpUtility.UrlEncode(_option.VHost);
        HttpOption httpOption = _option.GetHttpOption(url);

        if( timout.HasValue )
            httpOption.Timeout = timout.Value;

        string responseJson = httpOption.GetResult(retry);

        try {
            return GetQueueList(responseJson);
        }
        catch( Exception ex ) {
            WriteTempFile("rabbit_queues_error_data.json", responseJson);

            // RabbitMQ的返回结果很“随意”，没有很好的方式判断结果是否为“正常结果”
            throw new RabbitResultException($"RabbitMQ 没有正确响应， {url} 返回结果非预期", ex);
        }
    }

    private static List<QueueInfo> GetQueueList(string responseJson)
    {
        List<QueueInfo> list = new List<QueueInfo>();
        List<ExpandoObject> rows = responseJson.FromJson<List<ExpandoObject>>();

        foreach( dynamic x in rows ) {

            QueueInfo info = new QueueInfo();
            info.Name = x.name;

            // RabbitMQ 刚刚重启后立即访问时，返回的信息很不完整
            try {
                info.State = x.state;
            }
            catch {
                info.State = "?";
            }
            try {
                info.Length = Convert.ToInt64(x.messages);
            }
            catch { }
            try {
                info.Consumers = Convert.ToInt64(x.consumers);
            }
            catch { }
            try {
                info.Incoming = Convert.ToDouble(x.message_stats.publish_details.rate);
                info.Deliver = Convert.ToDouble(x.message_stats.deliver_details.rate);
            }
            catch { }
            list.Add(info);
        }
        return list;
    }


    private static void WriteTempFile(string filename, string text)
    {
        string tempPath = EnvUtils.GetTempPath();
        string filePath = Path.Combine(tempPath, filename);

        try {
            RetryFile.AppendAllText(filePath, text, Encoding.UTF8);
        }
        catch {
            // 这里只能吃掉异常。
        }
    }


}
