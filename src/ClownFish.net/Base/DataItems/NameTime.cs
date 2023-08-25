namespace ClownFish.Base;

/// <summary>
/// 表示一个 【名称/时间】 数值对
/// </summary>
[Serializable]
public struct NameTime
{
    /// <summary>
    /// 键名
    /// </summary>
    [XmlAttribute]
    public string Name { get; set; }

    /// <summary>
    /// 键值
    /// </summary>
    [XmlAttribute]
    public DateTime Time { get; set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="name"></param>
    public NameTime(string name)
    {
        this.Name = name;
        this.Time = DateTime.Now;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="time"></param>
    public NameTime(string name, DateTime time)
    {
        this.Name = name;
        this.Time = time;
    }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{this.Name}={this.Time.ToTime27String()}";
    }
}
