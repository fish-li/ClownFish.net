namespace ClownFish.WebClient;

/// <summary>
/// 指示数据用HTTP协议传输时使用的序列化方式
/// </summary>
public enum SerializeFormat
{
    /// <summary>
    /// 默认值，不指定 Content-Type
    /// </summary>
    None,
    /// <summary>
    /// 调用 ToString() 方法做数据的序列化。
    /// 并设置请求头：Content-Type: text/plain
    /// </summary>
    Text,
    /// <summary>
    /// 将提交数据采用 JSON.NET 序列化为 JSON 字符串
    /// 并设置请求头：Content-Type: application/json
    /// </summary>
    Json,
    /// <summary>
    /// 将提交数据采用 JSON.NET 序列化为 JSON 字符串，并尽量输出类型信息，可用于服务端之间或者客户端是C#的反序列化。
    /// 并设置请求头：Content-Type: application/json
    /// </summary>
    Json2,
    /// <summary>
    /// 将提交数据序列化成 XML 字符串
    /// 并设置请求头：Content-Type: application/xml
    /// </summary>
    Xml,
    /// <summary>
    /// 将提交数据采用“表单”方式序列化，
    /// 并设置请求头：Content-Type: application/x-www-form-urlencoded
    /// </summary>
    Form,
    /// <summary>
    /// 将提交数据采用“表单”方式序列化，可支持上传文件，
    /// 并设置请求头：Content-Type: multipart/form-data
    /// </summary>
    Multipart,
    /// <summary>
    /// 指示提交数据是二进制数据或者是流对象，发起请求时不做序列化处理，
    /// 并设置请求头：Content-Type: application/octet-stream
    /// </summary>
    Binary,
    /// <summary>
    /// 些标志暂未实现。
    /// </summary>
    Auto,
    /// <summary>
    /// 未知的数据格式，不指定 Content-Type
    /// </summary>
    Unknown
}



