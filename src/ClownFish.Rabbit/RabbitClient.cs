namespace ClownFish.Rabbit;

/// <summary>
/// RabbitMQ Client, 使用 AMQP 协议，适合于批量发送消息，或者发送消息体比较大的消息。
/// </summary>
public sealed class RabbitClient : IDisposable
{
    /// <summary>
    /// 应用程序共享连接的名称
    /// </summary>
    public static readonly string ShareConnName = "AppShare";

    private readonly string _settingName;
    private readonly string _connectionName;

    private readonly RabbitOption _option;    
    private RabbitConnection _connection;

    private readonly bool _autoClose;
    private readonly string _clientShowName;

    private static readonly ConstructorInfo s_ctor;
    private static readonly IBasicProperties s_defaultBasicProperties;

    static RabbitClient()
    {
        Type type = Type.GetType("RabbitMQ.Client.Framing.BasicProperties, RabbitMQ.Client", true);
        s_ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        s_defaultBasicProperties = CreateBasicProperties0();
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="settingName">配置服务中的Rabbit连接名称</param>
    /// <param name="shareConnectionName">共享连接名称，
    /// 如果不指定，表示连接在使用结束后关闭， 
    /// 如果指定，那么连接将会一直打开，供后续同名的connectionName使用。</param>
    public RabbitClient(string settingName, string shareConnectionName = null)
    {
        _option = RabbitCache.GetOption(settingName);
        _settingName = settingName;

        _autoClose = shareConnectionName.IsNullOrEmpty();
        _clientShowName = "RabbitClient-" + settingName;
        _connectionName = shareConnectionName.HasValue() ? (settingName + "#" + shareConnectionName) : null;
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="rabbitOption">连接参数</param>
    /// <param name="clientName">一个客户端的名称，它将显示在RabbitMQ的连接列表界面上</param>
    public RabbitClient(RabbitOption rabbitOption, string clientName = null)
    {
        if( rabbitOption == null )
            throw new ArgumentNullException(nameof(rabbitOption));

        rabbitOption.Validate();
        _option = rabbitOption;

        _autoClose = true;   // 总是自动关闭连接
        _clientShowName = clientName;
    }

    private RabbitConnection GetConnection()
    {
        if( _connection == null ) {

            if( _connectionName != null )  // 使用共享连接
                _connection = RabbitCache.GetConnection(_settingName, _connectionName);

            else // 直接打开连接，Dispose时释放
                _connection = new RabbitConnection(_option, _clientShowName);
        }

        return _connection;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    void IDisposable.Dispose()
    {
        if( _autoClose && _connection != null ) {
            _connection.Dispose();
            _connection = null;
        }
    }


    private void CloseConnection()
    {
        if( _connection != null ) {
            _connection.Dispose();
            _connection = null;
        }
    }

    /// <summary>
    /// 创建队列并绑定到交换机
    /// </summary>
    /// <param name="queue">队列名称</param>
    /// <param name="exchange">交换机名称，默认值： "amq.direct"</param>
    /// <param name="bindingKey">从交换机到队列的映射标记</param>
    /// <param name="argument">调用QueueDeclare时传递的argument参数</param>
    /// <param name="durable">Should this queue will survive a broker restart?</param>
    /// <param name="exclusive">Should this queue use be limited to its declaring connection?  Such a queue will be deleted when its declaring connection closes.</param>
    /// <param name="autoDelete">Should this queue be auto-deleted when its last consumer (if any) unsubscribes?</param>
    public void CreateQueueBind(string queue, string exchange = null, string bindingKey = null, IDictionary<string, object> argument = null,
                                bool durable = true, bool exclusive = false, bool autoDelete = false)
    {
        if( queue.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(queue));

        if( exchange == null )
            exchange = Exchanges.Direct;

        if( bindingKey.IsNullOrEmpty() )
            bindingKey = queue;


        RabbitConnection conn = this.GetConnection();  // 获取一个连接包装对象，有可能是共享的，所以在使用时要加锁
        lock( conn.SyncLock ) {
            IModel channel = conn.GetChannel();  // 获取连接通道（内部会打开连接）

            // 申明队列，将队列绑定到交换机
            channel.QueueDeclare(queue, durable, exclusive, autoDelete, argument);
            channel.QueueBind(queue, exchange, bindingKey);
        }
    }

    /// <summary>
    /// 创建队列并绑定到交换机
    /// </summary>
    /// <param name="dataType">消息的数据类型，最终创建的队列名称就是消息数据类型的全名，bindingKey与队列同名</param>
    /// <param name="exchange">交换机名称，默认值： "amq.direct"</param>
    /// <param name="argument">调用QueueDeclare时传递的argument参数</param>
    public void CreateQueueBind(Type dataType, string exchange = null, IDictionary<string, object> argument = null)
    {
        if( dataType == null )
            throw new ArgumentNullException(nameof(dataType));

        string queue = dataType.GetQueueName();
        string bindingKey = queue;

        CreateQueueBind(queue, exchange, bindingKey, argument);
    }

    /// <summary>
    /// Construct a completely empty content header for use with the Basic content class.
    /// </summary>
    /// <returns></returns>
    public IBasicProperties GetBasicProperties()
    {
        //var props = new RabbitMQ.Client.Framing.BasicProperties();     // 这行代码不能运行，因为类型是 internal

        //var props = this.Channel.CreateBasicProperties();   // 这种写法不错，但是会触发打开连接，会涉及锁问题，所以放弃！

        //IBasicProperties props = (IBasicProperties)s_ctor.Invoke(null);
        //props.ContentType = "text/plain";
        //props.Persistent = true;
        //return props;

        return CreateBasicProperties0();
    }


    private static IBasicProperties CreateBasicProperties0()
    {
        IBasicProperties props = (IBasicProperties)s_ctor.Invoke(null);
        props.ContentType = "text/plain";
        props.Persistent = true;
        return props;
    }

    /// <summary>
    /// 往队列中发送一条消息。
    /// </summary>
    /// <param name="data">要发送的消息数据</param>
    /// <param name="exchange">交换机名称，默认值： "amq.direct"</param>
    /// <param name="routingKey">消息的路由键</param>
    /// <param name="basicProperties"></param>
    /// <returns>消息体长度</returns>
    public int SendMessage(object data, string exchange = null, string routingKey = null, IBasicProperties basicProperties = null)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        byte[] body = MessageBinSerializer.Instance.Serialize(data);

        if( body == null || body.Length == 0 )
            throw new ArgumentNullException(nameof(data));


        if( routingKey.IsNullOrEmpty() )
            routingKey = data.GetType().GetQueueName();

        if( exchange == null )
            exchange = Exchanges.Direct;

        if( basicProperties == null ) {
            basicProperties = s_defaultBasicProperties;
        }

        DateTime start = DateTime.Now;
        Exception lastException = null;
        try {
            RabbitConnection conn = this.GetConnection();  // 获取一个连接包装对象，有可能是共享的，所以在使用时要加锁

            lock( conn.SyncLock ) {

                IModel channel = conn.GetChannel();  // 获取连接通道（内部会打开连接）

                try {
                    channel.BasicPublish(exchange, routingKey, basicProperties, body);
                }
                catch( RabbitMQ.Client.Exceptions.AlreadyClosedException ) {
                    // 先把连接关闭，否则这个无效的连接一直存在~~~
                    CloseConnection();
                    throw;
                }
            }
        }
        catch( Exception ex ) {
            lastException = ex;
            throw;
        }
        finally {
            RabbitClientEvent.SendMessage(this, _option.Server, exchange, routingKey, data, body.Length, start, lastException);
        }

        return body.Length;
    }

#region 主动拉取模式

    /// <summary>
    /// 从消息队列中获取一条消息。
    /// 如果队列不空，立即返回最早消息。如果队列为空，刚返回 null
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="autoAck"></param>
    /// <param name="deliveryTag"></param>
    /// <returns></returns>
    public T GetMessage<T>(string queue, bool autoAck, out string deliveryTag)
    {
        if( queue.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(queue));

        BasicGetResult result = null;
        RabbitConnection conn = this.GetConnection();  // 获取一个连接包装对象，有可能是共享的，所以在使用时要加锁

        lock( conn.SyncLock ) {
            IModel channel = conn.GetChannel();  // 获取连接通道（内部会打开连接）

            try {
                result = channel.BasicGet(queue, autoAck);
            }
            catch( RabbitMQ.Client.Exceptions.AlreadyClosedException ) {
                // 先把连接关闭，否则这个无效的连接一直存在~~~
                CloseConnection();
                throw;
            }
        }

        if( result == null || result.Body.Length == 0 ) {
            deliveryTag = null;
            return default(T);
        }
        else {
            deliveryTag = result.DeliveryTag.ToString();   // 可能需要跨语言使用，所以使用string类型会方便些
            return MessageBinSerializer.Instance.Deserialize<T>(result.Body);
        }
    }

    // RabbitMQ BasicGet/BasicAck 小结：
    // 1，BasicGet有结果时，deliveryTag 是【当前连接】的结果序号
    // 2，BasicGet结束后，返回的消息对其它连接不可见（其它连接获取不到这个消息）
    // 3，BasicGet结束后，如果一直没有BasicAck，最后连接关闭时，消息会恢复状态（允许其它连接获取）
    // 4，由于第1点，BasicAck的连接必须是BasicGet的连接，否则deliveryTag的序号就是错误的
    // 5，错误的BasicAck调用，会导致连接无效，RabbitMQ.Client.Exceptions.AlreadyClosedException: Already closed: The AMQP operation was interrupted: AMQP close-reason, initiated by Peer, code=406, text='PRECONDITION_FAILED - unknown delivery tag 1', classId=60, methodId=80


    /// <summary>
    /// 发送一个确认信息到MQ，通知【某条消息】已处理。
    /// 此方法必须在 GetMessage(autoAck=false)之后调用。
    /// </summary>
    /// <param name="deliveryTag"></param>
    public void AckMessage(string deliveryTag)
    {
        if( deliveryTag.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(deliveryTag));

        ulong deliveryTagValue = ulong.Parse(deliveryTag);

        RabbitConnection conn = this.GetConnection();  // 获取一个连接包装对象，有可能是共享的，所以在使用时要加锁
        lock( conn.SyncLock ) {
            IModel channel = conn.GetChannel();  // 获取连接通道（内部会打开连接）
            channel.BasicAck(deliveryTagValue, false);
        }
    }

#endregion


    /// <summary>
    /// 获取指定数据类型对应的队列中的消息数量
    /// </summary>
    /// <returns></returns>
    public uint MessageCount(string queue)
    {
        if( queue.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(queue));

        RabbitConnection conn = this.GetConnection();  // 获取一个连接包装对象，有可能是共享的，所以在使用时要加锁
        lock( conn.SyncLock ) {
            IModel channel = conn.GetChannel();  // 获取连接通道（内部会打开连接）
            return channel.MessageCount(queue);
        }
    }

    /// <summary>
    /// 清空指定数据类型对应的队列中的所有消息
    /// </summary>
    public void ClearQueue(string queue)
    {
        if( queue.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(queue));

        RabbitConnection conn = this.GetConnection();  // 获取一个连接包装对象，有可能是共享的，所以在使用时要加锁
        lock( conn.SyncLock ) {
            IModel channel = conn.GetChannel();  // 获取连接通道（内部会打开连接）
            channel.QueuePurge(queue);
        }
    }

    
    /// <summary>
    /// 测试连接是否能打开
    /// </summary>
    public void TestConnection()
    {
        RabbitConnection conn = this.GetConnection();  // 获取一个连接包装对象，有可能是共享的，所以在使用时要加锁
        lock( conn.SyncLock ) {
            IModel channel = conn.GetChannel();  // 获取连接通道（内部会打开连接）

            // 其实这二行代码没什么用，不写就担心编译器优化时把整块代码去掉了。
            if( channel == null )
                Console2.Info("RabbitClient打开连接失败？？");
        }
    }



}
