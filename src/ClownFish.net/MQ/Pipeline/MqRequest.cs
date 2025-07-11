namespace ClownFish.MQ.Pipeline;
#if NETCOREAPP

/// <summary>
/// 表示一个消息处理请求
/// </summary>
public sealed class MqRequest : ILoggingObject
{
    /// <summary>
    /// MessageId
    /// </summary>
    public string MessageId { get; } = LogIdMaker.GetNewId();

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
    public ReadOnlyMemory<byte> Body { get; init; }


    /// <summary>
    /// 经过反序列化得到的消息对象，它是一个实体或者DTO
    /// </summary>
    public object MessageObject { get; init; }

    string ILoggingObject.ToLoggingText()
    {
        // 此方法由 OprLogScope 调用： this.OprLog.Request = context.GetRequest()?.GetLogText();
        // 一般来说，队列中的消息通常是 JSON 数据，所以只需要将它还原成字符串就可以了
        // 如果消息确实是二 进制数据，可以在 MessageHandler 中设置 this.OprLog.Request = "xxxx" 就可以避免这个调用

        if( this.Body.IsEmpty == false ) {
            try {
                if( this.Body.Length <= LoggingLimit.HttpBodyMaxLen )
                    return Encoding.UTF8.GetString(this.Body.Span);
                else
                    return Encoding.UTF8.GetString(this.Body.Span.Slice(0, LoggingLimit.HttpBodyMaxLen));
            }
            catch {
                //忽略异常
            }
        }

        // MMQ队列时 Body 没有指定，只能使用 MessageObject
        return this.MessageObject?.ToJson();
    }
}


#endif
