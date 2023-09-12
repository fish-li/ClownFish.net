namespace ClownFish.NRedis;

/// <summary>
/// Redis Client
/// </summary>
public sealed class RedisClient //: IDisposable
{
    // 参考链接：https://stackexchange.github.io/StackExchange.Redis/

    // Because the ConnectionMultiplexer does a lot, it is designed to be shared and reused between callers. 
    // You should not create a ConnectionMultiplexer per operation. 
    // It is fully thread-safe and ready for this usage.
    private readonly ConnectionMultiplexer _redisConnection;

    internal RedisClient(string settingName)
    {
        // eg.   10.5.11.29:16379,password=myredis,connectRetry=10,connectTimeout=3000
        // ref:  https://stackexchange.github.io/StackExchange.Redis/Configuration.html
        string connectionString = Settings.GetSetting(settingName);

        if( connectionString.IsNullOrEmpty() )
            throw new ConfigurationErrorsException("没有在配置服务中找到参数项：" + settingName);


        // https://stackexchange.github.io/StackExchange.Redis/ThreadTheft
        ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);


        // 连接 Redis
        // 默认采用 .net 线程池的连接方式
        if( Settings.GetBool($"RedisSocket_UseThreadPool_{settingName}", 1) ) {
            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
            options.SocketManager = SocketManager.ThreadPool;   // 采用线程池做为调度器
            _redisConnection = ConnectionMultiplexer.Connect(options);

        }
        else {
            // StackExchange.Redis的默认行为：使用 SocketManager.Shared，它里面会创建10个后台线程，太浪费了~~
            _redisConnection = ConnectionMultiplexer.Connect(connectionString);
        }
    }


    /// <summary>
    /// 获取 Redis 的 IDatabase 实例
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    public IDatabase GetDatabase(int db = 0)
    {
        return _redisConnection.GetDatabase(db).CreateProxy();
    }

    /// <summary>
    /// 获取ConnectionMultiplexer实例
    /// </summary>
    /// <returns></returns>
    public ConnectionMultiplexer GetConnection()
    {
        return _redisConnection;
    }

    ///// <summary>
    ///// Dispose
    ///// </summary>
    //public void Dispose()
    //{
    //    if( _redisConnection != null ) {
    //        _redisConnection.Dispose();
    //        _redisConnection = null;
    //    }
    //}


    /// <summary>
    /// 将一个数据对象做为消息发送到Redis
    /// </summary>
    /// <param name="data">消息对象</param>
    /// <returns></returns>
    public int SendMessage(object data)
    {
        return SendMessage(null, data);
    }

    /// <summary>
    /// 将一个数据对象做为消息发送到Redis
    /// </summary>
    /// <param name="channel">channel，可以为null</param>
    /// <param name="data">消息对象</param>
    /// <returns></returns>
    public int SendMessage(string channel, object data)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( channel.IsNullOrEmpty() )
            channel = data.GetType().GetQueueName();


        byte[] body = MessageBinSerializer.Instance.Serialize(data);

        if( body == null || body.Length == 0 )
            throw new ArgumentNullException(nameof(data));

        IDatabase db = GetDatabase();
        db.Publish(channel, body);

        return body.Length;
    }
}
