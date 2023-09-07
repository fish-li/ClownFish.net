using System.ComponentModel;

namespace ClownFish.WebHost.Config;

/// <summary>
/// 静态文件参数
/// </summary>
[Serializable]
public sealed class StaticFileOption
{
    /// <summary>
    /// 扩展名
    /// </summary>
    [XmlAttribute("ext")]
    public string Ext { get; set; }
    /// <summary>
    /// 需要缓存的秒数， 
    /// 小于 0  表示不缓存，
    /// 等于 0  表示取默认值（当前版本为 1年）
    /// 大于 0  表示缓存秒数
    /// </summary>
    [XmlAttribute("cache")]
    [DefaultValue(0)]
    public int Cache { get; set; }
    /// <summary>
    /// 扩展名对应的MimeType
    /// </summary>
    [XmlAttribute("mime")]
    public string Mine { get; set; }
}
