namespace ClownFish.Http.Utils;

/// <summary>
/// 常用的请求内容类型
/// </summary>
public static class RequestContentType
{
    /// <summary>
    /// 指示请求体是一个简单的无规则文本
    /// </summary>
    public static readonly string Text = "text/plain";

    /// <summary>
    /// 指示请求体是一个FORM表单数据格式
    /// </summary>
    public static readonly string Form = "application/x-www-form-urlencoded";

    /// <summary>
    /// 指示请求体是一个FORM表单且包含上传文件
    /// </summary>
    public static readonly string Multipart = "multipart/form-data";

    /// <summary>
    /// 指示请求体是一个JSON
    /// </summary>
    public static readonly string Json = "application/json";

    /// <summary>
    /// 指示请求体是一个XML
    /// </summary>
    public static readonly string Xml = "application/xml";

    /// <summary>
    /// 指示请求体是一个二进制数据
    /// </summary>
    public static readonly string Binary = "application/octet-stream";

}
