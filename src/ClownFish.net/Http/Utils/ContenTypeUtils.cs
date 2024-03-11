namespace ClownFish.Http.Utils;
/// <summary>
/// 
/// </summary>
public static class ContenTypeUtils
{
    /// <summary>
    /// 根据 Content-Type 请求头字符串，转换成SerializeFormat枚举
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static SerializeFormat GetFormat(string contentType)
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
                return ResponseContentType.TextUtf8;   // 固定采用 utf-8

            case SerializeFormat.Json:
            case SerializeFormat.Json2:
                return ResponseContentType.JsonUtf8;   // 固定采用 utf-8

            case SerializeFormat.Xml:
                return ResponseContentType.XmlUtf8;   // 固定采用 utf-8

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
