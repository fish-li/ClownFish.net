namespace ClownFish.WebHost.Config;

/// <summary>
/// key/value 配置项
/// </summary>
[Serializable]
public sealed class AppSetting
{
    /// <summary>
    /// key
    /// </summary>
    [XmlAttribute("key")]
    public string Key { get; set; }

    /// <summary>
    /// value
    /// </summary>
    [XmlAttribute("value")]
    public string Value { get; set; }
}
