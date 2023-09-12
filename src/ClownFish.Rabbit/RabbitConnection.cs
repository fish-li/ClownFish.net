namespace ClownFish.Rabbit;

/// <summary>
/// 封装Rabbit连接与会话
/// </summary>
internal sealed class RabbitConnection : IDisposable
{
    private readonly RabbitOption _rabbitOption;
    private readonly string _clientName;


    private IConnection _innerConnection;
    private IModel _channel;


    // 关于 connection, channel 相关的链接参数：
    // https://stackoverflow.com/questions/10407760/is-there-a-performance-difference-between-pooling-connections-or-channels-in-rab
    // https://stackoverflow.com/questions/12024241/c-sharp-rabbitmq-client-thread-safety       
    // https://www.rabbitmq.com/dotnet-api-guide.html#connection-and-channel-lifspan

    // 基本结论是：IConnection is thread safe, IModel is not. 
    // connection, channel 都应该被反复使用，而不是每个操作都打开一次

    // 虽然 Rabbit的官方上说 channel 会由于一些异常而关闭，然而通过一些参数的指定，channel 是可以做法自动恢复的，
    // 如果真的遇到故障，可以调用 Close() 方法关闭连接，下次访问 Channel 属性时将重新打开连接


    /// <summary>
    /// RabbitConnection的实例可能会被缓存起来共享使用，所以在使用共享连接时需要加锁。
    /// </summary>
    public readonly object SyncLock = new object();


    /// <summary>
    /// In order to use this dispatcher, set the ConnectionFactory.DispatchConsumersAsync property to true:
    /// https://www.rabbitmq.com/dotnet-api-guide.html#consuming-async
    /// </summary>
    public bool? DispatchConsumersAsync { get; set; }


    internal RabbitConnection(string settingName, string clientName)
    {
        if( clientName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(clientName));


        _rabbitOption = RabbitCache.GetOption(settingName);
        _clientName = clientName;
    }


    internal RabbitConnection(RabbitOption option, string clientName)
    {
        if( option == null)
            throw new ArgumentNullException(nameof(option));
        if( clientName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(clientName));


        _rabbitOption = option;
        _clientName = clientName;
    }


    public IModel GetChannel()
    {
        if( _channel == null ) {

            if( _innerConnection == null ) {
                // return type: RabbitMQ.Client.Framing.Impl.AutorecoveringConnection
                _innerConnection = CreateConnection(_rabbitOption);   // 这里会打开连接
            }

            // return type: RabbitMQ.Client.Impl.AutorecoveringModel
            _channel = _innerConnection.CreateModel();
        }

        return _channel;
    }

    internal IConnection CreateConnection(RabbitOption option)
    {
        ConnectionFactory factory = CreateConnectionFactory();

        factory.HostName = option.Server;
        factory.UserName = option.Username;
        factory.Password = option.Password;

        factory.VirtualHost = option.VHost;

        if( option.Port > 0 )
            factory.Port = option.Port;

        if( this.DispatchConsumersAsync.HasValue )
            factory.DispatchConsumersAsync = this.DispatchConsumersAsync.Value;


        //if( option.IsAliyunMQ() ) {
        //    factory.AuthMechanisms = new List<AuthMechanismFactory>() { new AliyunMechanismFactory() };
        //}

        // 显示在 Connections 表格中
        // 发送方显示：client-SettingName ，订阅方显示：Subscriber-QueueName
        string clientProvidedName = $"{EnvUtils.GetAppName()}({EnvUtils.GetHostName()})-{_clientName}";
        try {
            return factory.CreateConnection(clientProvidedName);
        }
        catch {
            Console2.Error($"连接RabbitMQ失败，Server={factory.HostName};Username={factory.UserName};VHost={factory.VirtualHost};Port={factory.Port};");
            throw;
        }
    }


    internal static ConnectionFactory CreateConnectionFactory()
    {
        // https://www.rabbitmq.com/dotnet-api-guide.html

        ConnectionFactory factory = new ConnectionFactory();

        factory.AutomaticRecoveryEnabled = true;
        factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(1);
        //factory.ContinuationTimeout = TimeSpan.FromSeconds(10);
        //factory.RequestedConnectionTimeout = TimeSpan.FromSeconds(10);

        return factory;
    }

    public void Close()
    {
        if( _channel != null ) {
            _channel.Close();
            _channel = null;
        }

        if( _innerConnection != null ) {
            _innerConnection.Close();
            _innerConnection = null;
        }
    }


    public void Dispose()
    {
        this.Close();
    }


}
