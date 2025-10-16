namespace ClownFish.Log.Attributes;

/// <summary>
/// 指标某个消息类型在日志写入时【可以】做批次操作。
/// 这个标记只是一个建议设置，最终如何执行写入操作与具体的Writer有关。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class BatchWritableAttribute : Attribute
{
    ///// <summary>
    ///// 每个批次的大小
    ///// </summary>
    //public int BatchSize { get; set; } = 500;

    // 每个批次包含多少个元素由 LoggingOptions.WriteListBatchSize 决定，可参考 CacheQueue.Flush()

    /// <summary>
    /// 列表序列化时是否采用 ndjson 格式
    /// </summary>
    public bool Ndjson { get; set; }
}
