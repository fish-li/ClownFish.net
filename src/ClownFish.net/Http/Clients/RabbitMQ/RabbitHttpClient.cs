using ClownFish.MQ;

namespace ClownFish.Http.Clients.RabbitMQ;


/// <summary>
/// RabbitMQ Client, 使用 HTTP 协议，适合于发送单条小消息
/// </summary>
public sealed class RabbitHttpClient : IDisposable
{
    private static readonly Dictionary<string, object> s_emptyArguments = new Dictionary<string, object>();

    private static readonly Dictionary<string, object> s_defaultBasicProperties = new Dictionary<string, object> {
        { "delivery_mode" , 2 }
    };

    private readonly RabbitOption _option;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="option">RabbitMQ连接参数</param>
    public RabbitHttpClient(RabbitOption option)
    {
        if( option == null )
            throw new ArgumentNullException(nameof(option));

        option.Validate();
        _option = option;
    }

    /// <summary>
    /// 创建队列并绑定到交换机
    /// </summary>
    /// <param name="queue">队列名称</param>
    /// <param name="exchange">交换机名称，默认值： "amq.direct"</param>
    /// <param name="routing">从交换机到队列的路由键</param>
    /// <param name="argument">调用QueueDeclare时传递的argument参数</param>
    public void CreateQueueBind(string queue, string exchange = null, string routing = null, IDictionary<string, object> argument = null)
    {
        if( queue.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(queue));

        if( exchange == null )
            exchange = "amq.direct";

        if( routing.IsNullOrEmpty() )
            routing = queue;

        try {
            QueueDeclare(queue, true, false, false, argument);
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 QueueDeclare 失败。", ex);
        }

        try {
            QueueBind(queue, exchange, routing);
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 QueueBind 失败。", ex);
        }
    }

    private void QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
    {
        string vhost = _option.VHost.UrlEncode();

        //  {"auto_delete":false,"durable":true,"arguments":{},"node":"rabbit@smacmullen"}
        HttpOption httpOption = _option.GetHttpOption($"/api/queues/{vhost}/{queue}");
        httpOption.Method = "PUT";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Data = new {
            durable,
            exclusive,
            auto_delete = autoDelete,
            arguments = (arguments ?? s_emptyArguments)
        };
        httpOption.GetResult();
    }

    private void QueueBind(string queue, string exchange, string routingKey)
    {
        string vhost = _option.VHost.UrlEncode();

        // { "routing_key":"my_routing_key","arguments":{ } }
        HttpOption httpOption = _option.GetHttpOption($"/api/bindings/{vhost}/e/{exchange}/q/{queue}");
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Data = new {
            routing_key = routingKey
        };
        httpOption.GetResult();
    }


    private HttpOption GetPublishHttpOption(object data, string exchange = null, string routing = null, Dictionary<string, object> basicProperties = null)
    {
        // 说明：这里不再使用 IBasicProperties 类型，
        // 因为它的一个实现类型 RabbitMQ.Client.Framing.BasicProperties 不能做JSON序列化，会出现下面的异常：
        //   Newtonsoft.Json.JsonSerializationException
        //     HResult=0x80131500
        //     Message=Error getting value from 'ReplyToAddress' on 'RabbitMQ.Client.Framing.BasicProperties'.
        //     ArgumentNullException: Value cannot be null. 

        if( routing.IsNullOrEmpty() )
            routing = data.GetType().GetQueueName();

        if( exchange == null )
            exchange = "amq.direct";

        if( basicProperties == null )
            basicProperties = s_defaultBasicProperties;

        // 文档摘抄
        // /api/exchanges/vhost/name/publish
        // {"properties":{},"routing_key":"my key","payload":"my body","payload_encoding":"string"}
        // All keys are mandatory.  
        // 注意：当basicProperties=null时，这里会强制给一个对象，保证能生成 "properties":{}
        // 如果没有这个成员，将出现 HTTP 400 的错误

        // As the default virtual host is called "/", this will need to be encoded as "%2F".
        string vhost = _option.VHost.UrlEncode();

        string body = data.ToJson();
        if( body.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(data));

        HttpOption httpOption = _option.GetHttpOption($"/api/exchanges/{vhost}/{exchange}/publish");
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Data = new {
            properties = basicProperties,
            routing_key = routing,
            payload = body,
            payload_encoding = "string"
        };

        return httpOption;
    }

    /// <summary>
    /// 往队列中发送一条消息。
    /// </summary>
    /// <param name="data">要发送的消息数据</param>
    /// <param name="exchange">交换机名称，默认值： "amq.direct"</param>
    /// <param name="routing">从交换机到队列的路由键</param>
    /// <param name="basicProperties"></param>
    public void SendMessage(object data, string exchange = null, string routing = null, Dictionary<string, object> basicProperties = null)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        HttpOption httpOption = GetPublishHttpOption(data, exchange, routing, basicProperties);

        try {
            httpOption.GetResult();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 SendMessage 失败。", ex);
        }
    }

    /// <summary>
    /// 往队列中发送一条消息。
    /// </summary>
    /// <param name="data">要发送的消息数据</param>
    /// <param name="exchange">交换机名称，默认值： "amq.direct"</param>
    /// <param name="routing">从交换机到队列的路由键</param>
    /// <param name="basicProperties"></param>
    public async Task SendMessageAsync(object data, string exchange = null, string routing = null, Dictionary<string, object> basicProperties = null)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        HttpOption httpOption = GetPublishHttpOption(data, exchange, routing, basicProperties);

        try {
            await httpOption.GetResultAsync();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 SendMessageAsync 失败。", ex);
        }
    }

#if NETCOREAPP
    /// <summary>
    /// 获取指定数据类型对应的队列中的消息数量
    /// </summary>
    /// <param name="queue"></param>
    /// <returns></returns>
    public uint MessageCount(string queue)
    {
        // As the default virtual host is called "/", this will need to be encoded as "%2F".
        string vhost = _option.VHost.UrlEncode();

        HttpOption httpOption = _option.GetHttpOption($"/api/queues/{vhost}/{queue}");

        dynamic result = null;

        try {
            result = httpOption.GetResult<ExpandoObject>();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 MessageCount 失败。", ex);
        }

        return (uint)result.messages;
    }
#endif


    void IDisposable.Dispose()
    {
        // do nothing
    }


    /// <summary>
    /// 获取队列的消息
    /// </summary>
    /// <param name="queue">队列名称</param>
    /// <param name="autoAck">是否自动ACK</param>
    /// <returns></returns>
    public async Task<string> GetMessageAsync(string queue, bool autoAck)
    {
        string vhost = _option.VHost.UrlEncode();

        HttpOption httpOption = _option.GetHttpOption($"/api/queues/{vhost}/{queue}/get");
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Data = new {
            count = 1,
            ackmode = (autoAck ? "ack_requeue_false" : "ack_requeue_true"),
            encoding = "auto"
        };

        return await httpOption.GetResultAsync();
    }


    /// <summary>
    /// 确认最近收到的消息
    /// </summary>
    /// <param name="queue"></param>
    /// <returns></returns>
    public async Task AckLast(string queue)
    {
        string vhost = _option.VHost.UrlEncode();

        HttpOption httpOption = _option.GetHttpOption($"/api/queues/{vhost}/{queue}/get");
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Data = new {
            count = 1,
            ackmode = "ack_requeue_false",
            encoding = "auto"
        };

        await httpOption.SendAsync();
    }


    /// <summary>
    /// 测试连接参数是否有效
    /// </summary>
    public void TestConnection()
    {
        HttpOption httpOption = _option.GetHttpOption("/api/overview");
        httpOption.Send();
    }
}
