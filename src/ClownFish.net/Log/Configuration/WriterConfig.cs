namespace ClownFish.Log.Configuration;

/// <summary>
/// 描述一个日志写入器的配置信息
/// </summary>
public sealed class WriterConfig
{
    /// <summary>
    /// 写入器的名称
    /// </summary>
    [XmlAttribute]
    public string Name { get; set; }

    /// <summary>
    /// 写入器的实现类型字符串
    /// </summary>
    [XmlAttribute]
    public string Type { get; set; }


    /// <summary>
    /// 写入器的 Type 实例
    /// </summary>
    internal Type TypeObject;


    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name}={Type}";
    }

}
