namespace ClownFish.NRedis;

internal static class RedisUtils
{
    internal static MqRequest CreateMqRequest<T>(this ChannelMessage channelMessage, int maxMessageLength) where T : class
    {
        if( ClownFishInit.AppExitToken.IsCancellationRequested )
            return null;

        RedisValue value = channelMessage.Message;
        if( value.IsNullOrEmpty )
            return null;

        ReadOnlyMemory<byte> payload = value;
        if( payload.Length < QueueUtils.MinMessageLength )
            return null;


        if( payload.Length >= maxMessageLength ) {
            Console2.Info("RedisSubscriber 已丢弃过大的消息，长度：" + payload.Length.ToString());
            return null;
        }

        try {
            T msgObject = MessageBinSerializer.Instance.Deserialize<T>(payload);

            return new MqRequest {
                MqKind = MQSource.Redis,
                Original = channelMessage,
                Body = payload,
                MessageObject = msgObject
            };
        }
        catch( Exception ex ) {
            Console2.Error($"RedisSubscriber 消息对象反序列化失败。", ex);
            return null;
        }
    }


}
