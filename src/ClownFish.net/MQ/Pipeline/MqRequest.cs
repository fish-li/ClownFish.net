namespace ClownFish.MQ.Pipeline;
#if NETCOREAPP

/// <summary>
/// 表示一个消息处理请求
/// </summary>
public sealed class MqRequest
{
    /// <summary>
    /// MessageId
    /// </summary>
    public string MessageId { get; } = OprLog.GetNewId();

    /// <summary>
    /// 消息队列类别，例如：RabbitMQ, Kafka
    /// </summary>
    public string MqKind { get; init; }

    /// <summary>
    /// 原始的消息数据，各种 MQ-client 的消息对象
    /// </summary>
    public object Original { get; init; }

    /// <summary>
    /// 消息的二进制形式
    /// </summary>
    public ReadOnlyMemory<byte>? Body { get; init; }


    /// <summary>
    /// 消息的二进制长度
    /// </summary>
    public long BodyLen => this.Body.HasValue ? this.Body.Value.Length : 0;

    /// <summary>
    /// 经过反序列化得到的消息对象，它是一个实体或者DTO
    /// </summary>
    public object MessageObject { get; init; }
}


#endif
