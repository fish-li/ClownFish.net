namespace ClownFish.Rabbit;

internal sealed class RabbitMessage
{
    public string ConsumerTag { get; init; }

    public ulong DeliveryTag { get; init; }

    public bool Redelivered { get; init; }

    public string Exchange { get; init; }

    public string RoutingKey { get; init; }

    public IBasicProperties Properties { get; init; }

    public ReadOnlyMemory<byte> Payload { get; init; }

    public RabbitMessage(string consumerTag, ulong deliveryTag, bool redelivered,
        string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        this.ConsumerTag = consumerTag;
        this.DeliveryTag = deliveryTag;
        this.Redelivered = redelivered;
        this.Exchange = exchange;
        this.RoutingKey = routingKey;
        this.Properties = properties;
        this.Payload = body;
    }
}
