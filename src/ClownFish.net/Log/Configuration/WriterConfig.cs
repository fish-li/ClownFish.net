using ClownFish.Log.Writers;

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
    /// 日志写入器的工作参数
    /// </summary>
    [XmlElement("Option")]
    public WriterOption[] Options { get; set; }

    /// <summary>
    /// 写入器的 Type 实例
    /// </summary>
    internal Type TypeObject;

    /// <summary>
    /// 写入器的实例
    /// </summary>
    internal ILogWriter WriterInstnace;

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.Name + " = " + this.Type;
    }

    /// <summary>
    /// 获取指定的配置参数值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetOptionValue(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        if( this.Options == null || this.Options.Length == 0 )
            return null;

        var option = this.Options.FirstOrDefault(x => x.Key.EqualsIgnoreCase(name));
        return option?.Value;
    }
}


/// <summary>
/// 日志写入器的工作参数
/// </summary>
public sealed class WriterOption
{
    /// <summary>
    /// 参数名称
    /// </summary>
    [XmlAttribute]
    public string Key { get; set; }

    /// <summary>
    /// 参数值
    /// </summary>
    [XmlAttribute]
    public string Value { get; set; }
}
