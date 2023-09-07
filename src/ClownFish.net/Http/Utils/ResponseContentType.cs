namespace ClownFish.Http.Utils;

/// <summary>
/// 常用的响应内容类型
/// </summary>
public static class ResponseContentType
{
    /// <summary>
    /// 表示以普通文本形式响应
    /// </summary>
    public static readonly string Text = "text/plain";

    /// <summary>
    /// 表示以普通文本形式响应
    /// </summary>
    public static readonly string TextUtf8 = "text/plain; charset=utf-8";


    /// <summary>
    /// 表示以JSON形式响应
    /// </summary>
    public static readonly string Json = "application/json";

    /// <summary>
    /// 表示以JSON形式响应
    /// </summary>
    public static readonly string JsonUtf8 = "application/json; charset=utf-8";

    /// <summary>
    /// 表示以XML形式响应
    /// </summary>
    public static readonly string Xml = "application/xml";

    /// <summary>
    /// 表示以XML形式响应
    /// </summary>
    public static readonly string XmlUtf8 = "application/xml; charset=utf-8";


    /// <summary>
    /// 表示以HTML形式响应
    /// </summary>
    public static readonly string Html = "text/html";

    /// <summary>
    /// 表示以HTML形式响应
    /// </summary>
    public static readonly string HtmlUtf8 = "text/html; charset=utf-8";


    /// <summary>
    /// 表示以二进制形式响应
    /// </summary>
    public static readonly string OctetStream = "application/octet-stream";


}
