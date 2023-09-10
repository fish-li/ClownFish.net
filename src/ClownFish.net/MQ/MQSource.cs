namespace ClownFish.MQ;
#if NET6_0_OR_GREATER
/// <summary>
/// 消息队列源的常量定义
/// </summary>
public static class MQSource
{
    /// <summary>
    /// MMQ
    /// </summary>
    public const string MMQ = "MMQ";

    /// <summary>
    /// RabbitMQ
    /// </summary>
    public const string RabbitMQ = "RabbitMQ";

    /// <summary>
    /// Kafka
    /// </summary>
    public const string Kafka = "Kafka";

    /// <summary>
    /// Redis
    /// </summary>
    public const string Redis = "Redis";

    /// <summary>
    /// Pulsar
    /// </summary>
    public const string Pulsar = "Pulsar";
}
#endif
