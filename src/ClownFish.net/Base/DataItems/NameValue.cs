namespace ClownFish.Base;

/// <summary>
/// 表示一个 【名称/值】 数值对
/// </summary>
[Serializable]
public sealed class NameValue
{
    /// <summary>
    /// 键名
    /// </summary>
    [XmlAttribute]
    public string Name { get; set; }

    /// <summary>
    /// 键值
    /// </summary>
    //[XmlAttribute]
    public string Value { get; set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    public NameValue() { }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public NameValue(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }


    /// <summary>
    /// 从一个字符串行中拆分构造NameValue对象
    /// </summary>
    /// <param name="line"></param>
    /// <param name="separator">Name, Value的分隔符，拆分后还会做Trim()</param>
    /// <returns></returns>
    public static NameValue Parse(string line, char separator)
    {
        if( line.IsNullOrEmpty() )
            return null;

        int p = line.IndexOf(separator);
        if( p < 1 )
            return null;

        string name = line.Substring(0, p).Trim();
        string value = line.Substring(p + 1).Trim();

        //if( string.IsNullOrEmpty(name) )   // if( p < 1 ) 可以确保 name 不空
        //    return null;

        return new NameValue(name, value);
    }

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{this.Name}={this.Value}";
    }
}


/// <summary>
/// 扩展方法工具类
/// </summary>
public static class NameValueExtensions
{
    /// <summary>
    /// 根据名称查找匹配项
    /// </summary>
    /// <param name="list"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static NameValue Find(this List<NameValue> list, string name)
    {
        if( list.IsNullOrEmpty() )
            return null;

        return list.FirstOrDefault(x => string.Compare(x.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
    }

    /// <summary>
    /// 根据名称获取匹配项的值
    /// </summary>
    /// <param name="list"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetValue(this List<NameValue> list, string name)
    {
        if( list.IsNullOrEmpty() )
            return null;

        return list.FirstOrDefault(x => string.Compare(x.Name, name, StringComparison.OrdinalIgnoreCase) == 0)?.Value;
    }
}
