using System.ComponentModel;

namespace ClownFish.WebHost.Config;

/// <summary>
/// HTTP监听参数
/// </summary>
[Serializable]
public sealed class HttpListenerOption
{
    /// <summary>
    /// 协议，可选范围：http, https
    /// </summary>
    [XmlAttribute("protocol")]
    public string Protocol { get; set; }
    /// <summary>
    /// 需要监听的IP地址
    /// </summary>
    [XmlAttribute("ip")]
    [DefaultValue("*")]
    public string Ip { get; set; }
    /// <summary>
    /// 需要监听的TCP端口
    /// </summary>
    [XmlAttribute("port")]
    public int Port { get; set; }

    /// <summary>
    /// 重写ToString方法
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Protocol}://{Ip}:{Port}/";
    }

    /// <summary>
    /// 显式成URL格式
    /// </summary>
    /// <returns></returns>
    public string ToUrl()
    {
        if( this.Ip == "*" ) {
            //string host = System.Net.Dns.GetHostName();
            string host = "localhost";
            return $"{Protocol}://{host}:{Port}/";
        }
        else
            return $"{Protocol}://{Ip}:{Port}/";
    }
}
