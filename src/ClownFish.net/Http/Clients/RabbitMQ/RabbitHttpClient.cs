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
            QueueBind(queue, exchange, routing);
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 CreateQueueBind 失败！", ex);
        }
    }

    private void QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
    {
        string vhost = _option.VHost.UrlEncode();

        //  {"auto_delete":false,"durable":true,"arguments":{},"node":"rabbit@smacmullen"}
        HttpOption httpOption = _option.GetHttpOption($"/api/queues/{vhost}/{queue}");
        httpOption.Id = "ClownFish_RabbitHttpClient_QueueDeclare";
        httpOption.Method = "PUT";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Timeout = HttpClientDefaults.RabbitHttpClientTimeout;
        httpOption.Data = new {
            durable,
            exclusive,
            auto_delete = autoDelete,
            arguments = (arguments ?? s_emptyArguments)
        };
        httpOption.Send();
    }

    private void QueueBind(string queue, string exchange, string routingKey)
    {
        string vhost = _option.VHost.UrlEncode();

        // { "routing_key":"my_routing_key","arguments":{ } }
        HttpOption httpOption = _option.GetHttpOption($"/api/bindings/{vhost}/e/{exchange}/q/{queue}");
        httpOption.Id = "ClownFish_RabbitHttpClient_QueueBind";
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Timeout = HttpClientDefaults.RabbitHttpClientTimeout;
        httpOption.Data = new {
            routing_key = routingKey
        };
        httpOption.Send();
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
        httpOption.Id = "ClownFish_RabbitHttpClient_SendMessage";
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Timeout = HttpClientDefaults.RabbitHttpClientTimeout;
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
            httpOption.Send();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 SendMessage 失败！", ex);
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
            await httpOption.SendAsync();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 SendMessageAsync 失败！", ex);
        }
    }

    /// <summary>
    /// 获取指定数据类型对应的队列中的消息数量。
    /// 【注意】：由于RabbitMQ服务端的原因（响应体数据结构不固定），当此方法返回 -1 时表示没有取到结果，调用方可采用循环的方式发起调用，有效结果范围：大于或等于 0
    /// </summary>
    /// <param name="queue"></param>
    /// <returns></returns>
    public long MessageCount(string queue)
    {
        string vhost = _option.VHost.UrlEncode();
        HttpOption httpOption = _option.GetHttpOption($"/api/queues/{vhost}/{queue}");
        httpOption.Id = "ClownFish_RabbitHttpClient_MessageCount";
        httpOption.Timeout = HttpClientDefaults.RabbitHttpClientTimeout;

        // RabbitMQ 这个接口的返回结构很不固定，很有可能会导致取不到结果，
        // 所以暂时也没有很好的办法对付这个老六
        // 下面是5个返回结果的样例，而且还不是5个轮流换，结果很随机！！
        //{ "consumer_details":[],"arguments":{},"auto_delete":false,"backing_queue_status":{"avg_ack_egress_rate":0.0,"avg_ack_ingress_rate":0.0,"avg_egress_rate":0.0,"avg_ingress_rate":0.0,"delta":["delta","undefined",0,0,"undefined"],"len":0,"mode":"default","next_seq_id":0,"q1":0,"q2":0,"q3":0,"q4":0,"target_ram_count":"infinity"},"consumer_utilisation":null,"consumers":0,"deliveries":[],"durable":true,"effective_policy_definition":{"max-length":100000,"overflow":"reject-publish"},"exclusive":false,"exclusive_consumer_tag":null,"garbage_collection":{"fullsweep_after":65535,"max_heap_size":0,"min_bin_vheap_size":46422,"min_heap_size":233,"minor_gcs":5},"head_message_timestamp":null,"incoming":[],"memory":34668,"message_bytes":0,"message_bytes_paged_out":0,"message_bytes_persistent":0,"message_bytes_ram":0,"message_bytes_ready":0,"message_bytes_unacknowledged":0,"message_stats":{"ack":0,"ack_details":{"rate":0.0},"deliver":0,"deliver_details":{"rate":0.0},"deliver_get":3,"deliver_get_details":{"rate":0.0},"deliver_no_ack":0,"deliver_no_ack_details":{"rate":0.0},"get":1,"get_details":{"rate":0.0},"get_empty":0,"get_empty_details":{"rate":0.0},"get_no_ack":2,"get_no_ack_details":{"rate":0.0},"redeliver":1,"redeliver_details":{"rate":0.0}},"messages_paged_out":0,"messages_persistent":0,"messages_ram":0,"messages_ready_ram":0,"messages_unacknowledged_ram":0,"name":"test1","node":"rabbit@my-rabbit","operator_policy":null,"policy":"10W","recoverable_slaves":null,"single_active_consumer_tag":null,"state":"running","type":"classic","vhost":"nbtest"}
        //{ "consumer_details":[],"arguments":{},"auto_delete":false,"backing_queue_status":{"avg_ack_egress_rate":0.0,"avg_ack_ingress_rate":0.0,"avg_egress_rate":0.0,"avg_ingress_rate":0.0,"delta":["delta","undefined",0,0,"undefined"],"len":0,"mode":"default","next_seq_id":0,"q1":0,"q2":0,"q3":0,"q4":0,"target_ram_count":"infinity"},"consumer_utilisation":null,"consumers":0,"deliveries":[],"durable":true,"effective_policy_definition":{"max-length":100000,"overflow":"reject-publish"},"exclusive":false,"exclusive_consumer_tag":null,"garbage_collection":{"fullsweep_after":65535,"max_heap_size":0,"min_bin_vheap_size":46422,"min_heap_size":233,"minor_gcs":5},"head_message_timestamp":null,"incoming":[],"memory":34668,"message_bytes":0,"message_bytes_paged_out":0,"message_bytes_persistent":0,"message_bytes_ram":0,"message_bytes_ready":0,"message_bytes_unacknowledged":0,"message_stats":{"ack":0,"ack_details":{"rate":0.0},"deliver":0,"deliver_details":{"rate":0.0},"deliver_get":3,"deliver_get_details":{"rate":0.0},"deliver_no_ack":0,"deliver_no_ack_details":{"rate":0.0},"get":1,"get_details":{"rate":0.0},"get_empty":0,"get_empty_details":{"rate":0.0},"get_no_ack":2,"get_no_ack_details":{"rate":0.0},"publish":2,"publish_details":{"rate":0.0},"redeliver":1,"redeliver_details":{"rate":0.0}},"messages_paged_out":0,"messages_persistent":0,"messages_ram":0,"messages_ready_ram":0,"messages_unacknowledged_ram":0,"name":"test1","node":"rabbit@my-rabbit","operator_policy":null,"policy":"10W","recoverable_slaves":null,"single_active_consumer_tag":null,"state":"running","type":"classic","vhost":"nbtest"}
        //{ "garbage_collection":{ "max_heap_size":-1,"min_bin_vheap_size":-1,"min_heap_size":-1,"fullsweep_after":-1,"minor_gcs":-1},"consumer_details":[],"arguments":{ },"auto_delete":false,"deliveries":[],"durable":true,"exclusive":false,"incoming":[],"name":"test1","node":"rabbit@my-rabbit","type":"classic","vhost":"nbtest"}
        //{ "garbage_collection":{ "max_heap_size":-1,"min_bin_vheap_size":-1,"min_heap_size":-1,"fullsweep_after":-1,"minor_gcs":-1},"consumer_details":[],"arguments":{ },"auto_delete":false,"deliveries":[],"durable":true,"exclusive":false,"incoming":[],"message_stats":{ "publish":2,"publish_details":{ "rate":0.0} },"name":"test1","node":"rabbit@my-rabbit","type":"classic","vhost":"nbtest"}
        //{ "garbage_collection":{ "max_heap_size":-1,"min_bin_vheap_size":-1,"min_heap_size":-1,"fullsweep_after":-1,"minor_gcs":-1},"consumer_details":[],"arguments":{ },"auto_delete":false,"deliveries":[],"durable":true,"exclusive":false,"incoming":[],"message_stats":{ "publish":2,"publish_details":{ "rate":0.0} },"messages":0,"messages_details":{ "rate":0.0},"messages_ready":0,"messages_ready_details":{ "rate":0.0},"messages_unacknowledged":0,"messages_unacknowledged_details":{ "rate":0.0},"name":"test1","node":"rabbit@my-rabbit","reductions":10546,"reductions_details":{ "rate":0.0},"type":"classic","vhost":"nbtest"}

        try {
            string json = httpOption.GetResult();
            MessageCountRespnonse result = json.FromJson<MessageCountRespnonse>();

            if( result != null && result.Messages.HasValue )
                return result.Messages.Value;
            else
                return -1;
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 MessageCount 失败！", ex);
        }
    }


    private sealed class MessageCountRespnonse
    {
        public int? Messages {  get; set; }
    }

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
        httpOption.Id = "ClownFish_RabbitHttpClient_GetMessage";
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Timeout = HttpClientDefaults.RabbitHttpClientTimeout;
        httpOption.Data = new {
            count = 1,
            ackmode = (autoAck ? "ack_requeue_false" : "ack_requeue_true"),
            encoding = "auto"
        };

        try {
            return await httpOption.GetResultAsync();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 GetMessageAsync 失败！", ex);
        }
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
        httpOption.Id = "ClownFish_RabbitHttpClient_AckLast";
        httpOption.Method = "POST";
        httpOption.Format = SerializeFormat.Json;
        httpOption.Timeout = HttpClientDefaults.RabbitHttpClientTimeout;
        httpOption.Data = new {
            count = 1,
            ackmode = "ack_requeue_false",
            encoding = "auto"
        };

        try {
            await httpOption.SendAsync();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 AckLast 失败！", ex);
        }
    }


    /// <summary>
    /// 测试连接参数是否有效
    /// </summary>
    public void TestConnection()
    {
        HttpOption httpOption = _option.GetHttpOption("/api/overview");
        httpOption.Id = "ClownFish_RabbitHttpClient_TestConnection";

        try {
            httpOption.Send();
        }
        catch( Exception ex ) {
            throw new RabbitHttpException("执行 TestConnection 失败！", ex);
        }
    }


    //internal void DeleteQueue(string queue)
    //{
    //    string vhost = _option.VHost.UrlEncode();

    //    HttpOption httpOption = _option.GetHttpOption($"/api/queues/{vhost}/{queue}");
    //    httpOption.Id = "ClownFish_RabbitHttpClient_DeleteQueue";
    //    httpOption.Method = "DELETE";
    //    httpOption.Timeout = HttpClientDefaults.RabbitHttpClientTimeout;

    //    httpOption.Send();
    //}
}
