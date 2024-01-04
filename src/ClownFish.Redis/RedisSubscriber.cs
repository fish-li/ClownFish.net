namespace ClownFish.NRedis;

/// <summary>
/// Redis消息订阅工具类
/// </summary>
public static class RedisSubscriber
{
    /// <summary>
    /// 所有创建的订阅者引用，防止对象被释放
    /// </summary>
    private static readonly List<object> s_objects = new List<object>();

    private static void CheckArgs<TData>(RedisSubscriberArgs args)
    {
        if( args == null )
            throw new ArgumentNullException(nameof(args));

        if( args.SettingName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(args.SettingName));

        if( args.Channel.IsNullOrEmpty() )
            args.Channel = typeof(TData).GetQueueName();

        if( args.RetryCount < 0 )
            args.RetryCount = 0;

        if( args.RetryWaitMilliseconds < 0 )
            args.RetryWaitMilliseconds = 0;
    }

    /// <summary>
    /// 启动消息队列的订阅。
    /// 说明：当前方法并不会阻塞当前线程，仅仅是启动订阅过程而已。
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="args"></param>
    public static void Start<TData, THandler>(RedisSubscriberArgs args)
                            where TData : class
                            where THandler : BaseMessageHandler<TData>, new()
    {
        CheckArgs<TData>(args);

        Console2.Info($"Start 1 Redis MessageHandler: {typeof(THandler).FullName}, Channel: {args.Channel}");

        using( ExecutionContext.SuppressFlow() ) {
            THandler handler = new THandler();
            RedisSubscriberSync<TData> subscriber = new RedisSubscriberSync<TData>(handler, args);
            subscriber.Start();

            s_objects.Add(subscriber);
        }
    }


    /// <summary>
    /// 启动消息队列的订阅。
    /// 说明：当前方法开启的消息管道将以异步方式运行，当前方法本身没有不涉及异步操作。
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="args"></param>
    public static void StartAsync<TData, THandler>(RedisSubscriberArgs args)
                            where TData : class
                            where THandler : AsyncBaseMessageHandler<TData>, new()
    {
        CheckArgs<TData>(args);

        Console2.Info($"Start 1 Redis MessageHandler: {typeof(THandler).FullName}, Channel: {args.Channel}");

        using( ExecutionContext.SuppressFlow() ) {
            THandler handler = new THandler();
            RedisSubscriberAsync<TData> subscriber = new RedisSubscriberAsync<TData>(handler, args);
            subscriber.Start();

            s_objects.Add(subscriber);
        }
    }

}
