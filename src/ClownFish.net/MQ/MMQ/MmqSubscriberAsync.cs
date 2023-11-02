namespace ClownFish.MQ.MMQ;

#if NETCOREAPP

internal class MmqSubscriberAsync<T> where T : class
{
    private readonly MmqSubscriberArgs<T> _args;
    private readonly CancellationToken _cancellationToken;
    private readonly AsyncMessagePipeline<T> _pipeline;

    private readonly MemoryMesssageQueue<T> _channel;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="args"></param>
    internal MmqSubscriberAsync(AsyncBaseMessageHandler<T> handler, MmqSubscriberArgs<T> args)
    {
        _args = args;
        _channel = args.Queue;

        _pipeline = new AsyncMessagePipeline<T>(handler, args.RetryCount, args.RetryWaitMilliseconds);

        if( _args.CancellationToken.HasValue == false )
            _cancellationToken = ClownFishInit.AppExitToken;
        else
            _cancellationToken = _args.CancellationToken.Value;
    }

    /// <summary>
    /// 开启订阅消息
    /// </summary>
    public async Task Start()
    {
        _cancellationToken.Register(OnAppExit);

        await MainLoop();
    }


    private async Task MainLoop()
    {
        while( true ) {
            T message = null;
            try {
                message = await _channel.ReadAsync(_cancellationToken);
            }
            catch( OperationCanceledException ) { // Appliecation Exit
                return;
            }

            await HandleMessage(message);
        }
    }

    private void OnAppExit()
    {
        Console2.WriteLine("Application exit, stop MmqSubscriber: " + _pipeline.HandlerInstance.GetType().FullName);
    }


    /// <summary>
    /// 当出现异常时的回调方法，默认处理方式是输出到控制台。
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="e"></param>
    private void OnError(string subject, Exception e)
    {
        Console2.Error($"MmqSubscriber {subject}", e);
    }


    internal async Task<int> HandleMessage(T message)
    {
        if( message == null )
            return 0;

        MqRequest request = new MqRequest {
            MqKind = MQSource.MMQ,
            MessageObject = message
        };

        try {
#if DEBUG
            TestHelper.TryThrowException();
#endif
            await _pipeline.PushMessage(request);
            return 1;
        }
        catch( Exception ex ) {
            OnError("处理消息失败", ex);
            return -1;
        }
    }


}

#endif
