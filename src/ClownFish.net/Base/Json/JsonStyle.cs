namespace ClownFish.Base;

/// <summary>
/// JSON序列化风格
/// </summary>
[Flags]
public enum JsonStyle
{
    /// <summary>
    /// 不明确指定，使用框架默认设置
    /// </summary>
    None = 0,
    
    /// <summary>
    /// JSON序列化时，保留必要的 .NET 类型信息
    /// </summary>
    KeepType = 1,
    
    /// <summary>
    /// JSON序列化时，增加缩进
    /// </summary>
    Indented = 2,
    
    /// <summary>
    /// 采用小写开头的属性风格
    /// </summary>
    CamelCase = 4,

    /// <summary>
    /// 使用本地时区，日期格式：yyyy-MM-dd HH:mm:ss
    /// </summary>
    TimeFormat19 = 8,

    /// <summary>
    /// 属性名字全部小写
    /// </summary>
    NameToLower = 16,

    /// <summary>
    /// 保留NULL字段
    /// </summary>
    KeepNull = 32,

    /// <summary>
    /// 使用UTC时区
    /// </summary>
    UtcTime = 64,


}


