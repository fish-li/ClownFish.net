namespace ClownFish.Base;

/// <summary>
/// 一个KV数据类型
/// </summary>
public sealed class NameInt64
{
    /// <summary>
    /// Name
    /// </summary>
    [XmlAttribute]
    public string Name { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [XmlAttribute]
    public long Value { get; set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public NameInt64(string name, long value)
    {
        this.Name = name;
        this.Value = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{this.Name}={this.Value.ToWString()}";
    }
}
