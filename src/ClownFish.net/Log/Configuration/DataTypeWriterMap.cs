using ClownFish.Log.Writers;

namespace ClownFish.Log.Configuration;

/// <summary>
/// 日志数据类型与写入器的映射关系
/// </summary>
internal sealed class DataTypeWriterMap
{
    /// <summary>
    /// 日志的数据类型
    /// </summary>
    public Type DataType { get; set; }

    /// <summary>
    /// 数据类型对应的写入器类型
    /// </summary>
    public Type[] WriterTypes { get; set; }

    /// <summary>
    /// 数据类型对应的写入器实例
    /// </summary>
    public ILogWriter[] Writers { get; set; }
}
