namespace ClownFish.Rabbit;

internal static class RabbitUtils
{
    internal static MqRequest CreateMqRequest<T>(this RabbitMessage message, int maxMessageLength) where T: class
    {
        if( message.Payload.Length < QueueUtils.MinMessageLength ) {
            return null;
        }

        if( message.Payload.Length >= maxMessageLength ) {
            Console2.Info("RabbitSubscriber 已丢弃过大的消息，长度：" + message.Payload.Length.ToString());
            return null;
        }

        try {
            T msgObject = MessageBinSerializer.Instance.Deserialize<T>(message.Payload);

            return new MqRequest {
                MqKind = MQSource.RabbitMQ,
                Original = message,
                Body = message.Payload,
                MessageObject = msgObject
            };
        }
        catch( Exception ex ) {
            Console2.Error($"RabbitSubscriber 消息对象反序列化失败。", ex);
            return null;
        }
    }
}
