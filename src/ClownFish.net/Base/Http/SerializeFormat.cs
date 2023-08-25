namespace ClownFish.Base.Http;

/// <summary>
/// 指示数据用HTTP协议传输时使用的序列化方式
/// </summary>
public enum SerializeFormat
{
    /// <summary>
    /// 默认值，没有指定。注意：有些场景下不指定将会出现异常。
    /// </summary>
    None,
    /// <summary>
    /// 直接调用 ToString() 方法
    /// 匹配标头："text/plain"
    /// </summary>
    Text,
    /// <summary>
    /// 采用 JSON.NET 序列化为 JSON 字符串
    /// 匹配标头："application/json"
    /// </summary>
    Json,
    /// <summary>
    /// 采用 JSON.NET 序列化为 JSON 字符串，并尽量输出类型信息，可用于服务端之间或者客户端是C#的反序列化。
    /// 匹配标头："application/json"
    /// </summary>
    Json2,
    /// <summary>
    /// 序列化成 XML 字符串
    /// 匹配标头："application/xml"
    /// </summary>
    Xml,
    /// <summary>
    /// 采用 "application/x-www-form-urlencoded" 方式序列化
    /// </summary>
    Form,
    /// <summary>
    /// 采用 "multipart/form-data" 方式序列化
    /// </summary>
    Multipart,
    /// <summary>
    /// 采用二进制数据传输
    /// </summary>
    Binary,
    /// <summary>
    /// 用于服务端自动识别客户端的期望结果类型，由 Request.Headers["X-Result-Format"]来决定。
    /// </summary>
    Auto,
    /// <summary>
    /// 未知的请求头（不是标准的请求头）
    /// </summary>
    Unknown
}



