namespace ClownFish.MQ.MMQ;

#if NETCOREAPP

/// <summary>
/// MMQ（内存队列）消息订阅工具类
/// </summary>
public static class MmqSubscriber
{
    private static readonly List<Task> s_workers = new List<Task>();

    internal static void CheckArgs<TData>(MmqSubscriberArgs<TData> args) where TData : class
    {
        if( args == null )
            throw new ArgumentNullException(nameof(args));

        if( args.Queue == null )
            throw new ArgumentNullException(nameof(args.Queue));

        if( args.SubscriberCount <= 0 )
            throw new ArgumentOutOfRangeException(nameof(args.SubscriberCount));

        if( args.RetryCount < 0 )
            throw new ArgumentOutOfRangeException(nameof(args.RetryCount));

        if( args.RetryWaitMilliseconds < 0 )
            throw new ArgumentOutOfRangeException(nameof(args.RetryWaitMilliseconds));

        PipelineUtils.EnsureIsRootCode();
    }

    /// <summary>
    /// 创建订阅者并开始执行
    /// </summary>
    /// <param name="args"></param>
    public static void Start<TData, THandler>(MmqSubscriberArgs<TData> args)
                            where TData : class
                            where THandler : BaseMessageHandler<TData>, new()
    {
        CheckArgs(args);

        using( ExecutionContext.SuppressFlow() ) {
            // 为每个订阅者创建对应的线程
            for( int i = 0; i < args.SubscriberCount; i++ ) {

                Task task = Task.Run(() => {
                    try {
                        THandler handler = new THandler();
                        MmqSubscriberSync<TData> subscriber = new MmqSubscriberSync<TData>(handler, args);
                        subscriber.Start();
                    }
                    catch( Exception ex ) {
                        Console2.Error(ex);
                    }
                });
                s_workers.Add(task);
            }
        }
    }



    /// <summary>
    /// 创建订阅者并开始执行
    /// </summary>
    /// <param name="args"></param>
    public static void StartAsync<TData, THandler>(MmqSubscriberArgs<TData> args)
                            where TData : class
                            where THandler : AsyncBaseMessageHandler<TData>, new()
    {
        CheckArgs(args);

        using( ExecutionContext.SuppressFlow() ) {
            // 为每个订阅者创建对应的线程
            for( int i = 0; i < args.SubscriberCount; i++ ) {

                Task task = Task.Run(async () => {
                    try {
                        THandler handler = new THandler();
                        MmqSubscriberAsync<TData> subscriber = new MmqSubscriberAsync<TData>(handler, args);
                        await subscriber.Start();
                    }
                    catch( Exception ex ) {
                        Console2.Error(ex);
                    }
                });
                s_workers.Add(task);
            }
        }
    }
}
#endif
