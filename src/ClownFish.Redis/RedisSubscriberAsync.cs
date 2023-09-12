namespace ClownFish.NRedis;

/// <summary>
/// Redis消息订阅管道
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class RedisSubscriberAsync<T> where T : class
{
    private readonly RedisSubscriberArgs _args;
    private readonly AsyncMessagePipeline<T> _pipeline;
    
    private ChannelMessageQueue _channelMessageQueue;
    private ISubscriber _subscriber;


    internal RedisSubscriberAsync(AsyncBaseMessageHandler<T> handler, RedisSubscriberArgs args)
    {
        _args = args;
        _pipeline = new AsyncMessagePipeline<T>(handler, args.RetryCount, args.RetryWaitMilliseconds);
    }


    /// <summary>
    /// 开始订阅消息
    /// </summary>
    public void Start()
    {
        RedisClient client = Redis.GetClient(_args.SettingName);
        _subscriber = client.GetConnection().GetSubscriber(this);

        // https://github.com/StackExchange/StackExchange.Redis/issues/639
        _channelMessageQueue = _subscriber.Subscribe(_args.Channel);
        _channelMessageQueue.OnMessage(HandleMessage);

        ClownFishInit.AppExitToken.Register(OnAppEnd);
    }

    private void OnAppEnd()
    {
        _subscriber?.Unsubscribe(_args.Channel);
        Console2.WriteLine("Application exit, stop RedisSubscriber: " + _pipeline.HandlerInstance.GetType().FullName);
    }

    private async Task HandleMessage(ChannelMessage channelMessage)
    {
        MqRequest request = channelMessage.CreateMqRequest<T>(_args.MaxMessageLength);

        try {
            await _pipeline.PushMessage(request);
        }
        catch( Exception ex ) {
            OnError("处理消息失败", ex);
        }
    }


    private void OnError(string subject, Exception e)
    {
        Console2.Error($"RedisSubscriber {subject}。", e);
    }
}
