namespace ClownFish.Rabbit;

/// <summary>
/// 订阅者参数
/// </summary>
public sealed class RabbitSubscriberArgs
{
    /// <summary>
    /// RabbitMQ的连接配置名称
    /// </summary>
    public string SettingName { get; set; }
    /// <summary>
    /// 队列的名称
    /// </summary>
    public string QueueName { get; set; }
    /// <summary>
    /// 订阅者数量
    /// </summary>
    public int SubscriberCount { get; set; } = 1;
    /// <summary>
    /// 消息处理失败后的重试次数
    /// </summary>
    public int RetryCount { get; set; }
    /// <summary>
    /// 消息处理失败后与下次重试之间的时间间隔，单位毫秒
    /// </summary>
    public int RetryWaitMilliseconds { get; set; }

    /// <summary>
    /// 最大允许的消息长度。如果超过这个值，消息会直接丢弃。
    /// </summary>
    public int MaxMessageLength { get; set; } = 512 * 1024;

    /// <summary>
    /// 消息本身的大小 如果设置为0 那么表示对消息本身的大小不限制
    /// </summary>
    public uint PrefetchSize { get; set; } = 0;

    /// <summary>
    /// 告诉rabbitmq不要一次性给消费者推送大于N个消息
    /// </summary>
    public ushort PrefetchCount { get; set; } = 1;
}
