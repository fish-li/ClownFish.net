using System.ComponentModel;

namespace ClownFish.Data.Xml;

/// <summary>
/// XmlCommand的命令参数。
/// </summary>
[Serializable]
public sealed class XmlCmdParameter
{
    /// <summary>
    /// 参数名称
    /// </summary>
    [XmlAttribute]
    public string Name { get; set; }

    /// <summary>
    /// 参数的数据类型
    /// </summary>
    [XmlAttribute]
    public DbType Type { get; set; }

    /// <summary>
    /// 参数值的长度。
    /// </summary>
    [DefaultValue(0)]
    [XmlAttribute]
    public int Size { get; set; }


    /// <summary>
    /// 参数的输入输出方向
    /// </summary>
    [DefaultValue(ParameterDirection.Input)]
    [XmlAttribute]
    public ParameterDirection Direction = ParameterDirection.Input;
}
