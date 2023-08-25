using ClownFish.Base.Http;

namespace ClownFish.Base;

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


    /// <summary>
    /// 根据 Content-Type 请求头字符串，转换成SerializeFormat枚举
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    internal static SerializeFormat GetFormat(string contentType)
    {
        if( string.IsNullOrEmpty(contentType) )
            return SerializeFormat.None;

        // MIME types are case-insensitive but are traditionally written in lowercase, 
        // with the exception of parameter values, whose case may or may not have specific meaning.

        // 虽然 MIME 类型是不区分大小写的，但是传统都习惯使用小写，因此下面的判断就直接使用小写
        // 例如：https://www.iana.org/assignments/media-types/media-types.xhtml

        if( contentType[0] == 'a' ) {

            if( contentType.StartsWith(RequestContentType.Json, StringComparison.Ordinal) )
                return SerializeFormat.Json;

            if( contentType.StartsWith(RequestContentType.Xml, StringComparison.Ordinal) )
                return SerializeFormat.Xml;

            if( contentType.StartsWith(RequestContentType.Form, StringComparison.Ordinal) )
                return SerializeFormat.Form;

            if( contentType.StartsWith(RequestContentType.Binary, StringComparison.Ordinal) )
                return SerializeFormat.Binary;

            return SerializeFormat.Unknown;
        }

        if( contentType[0] == 'm' ) {

            if( contentType.StartsWith(RequestContentType.Multipart, StringComparison.Ordinal) )
                return SerializeFormat.Multipart;

            return SerializeFormat.Unknown;
        }

        if( contentType[0] == 't' ) {

            if( contentType.StartsWith(RequestContentType.Text, StringComparison.Ordinal) )
                return SerializeFormat.Text;

            return SerializeFormat.Unknown;
        }

        return SerializeFormat.Unknown;
    }


    /// <summary>
    /// 根据SerializeFormat枚举转换成 Content-Type 请求头字符串，
    /// 对于无效的枚举，返回空字符串“”
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    internal static string GetByFormat(SerializeFormat format)
    {
        switch( format ) {
            case SerializeFormat.Text:
                return RequestContentType.Text;

            case SerializeFormat.Json:
            case SerializeFormat.Json2:
                return RequestContentType.Json;

            case SerializeFormat.Xml:
                return RequestContentType.Xml;

            case SerializeFormat.Form:
                return RequestContentType.Form;

            case SerializeFormat.Multipart:
                return RequestContentType.Multipart;

            case SerializeFormat.Binary:
                return RequestContentType.Binary;

            default:
                return string.Empty;
        }
    }



}
