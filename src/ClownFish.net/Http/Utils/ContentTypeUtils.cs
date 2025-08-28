namespace ClownFish.Http.Utils;
/// <summary>
/// 
/// </summary>
public static class ContentTypeUtils
{
    /// <summary>
    /// 根据 Content-Type 请求头字符串，转换成SerializeFormat枚举
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static SerializeFormat GetFormat(string contentType)
    {
        string mediaType = HttpUtils.ParseMediaType(contentType);

        if( string.IsNullOrEmpty(mediaType) )
            return SerializeFormat.None;

        // MIME types are case-insensitive but are traditionally written in lowercase, 
        // with the exception of parameter values, whose case may or may not have specific meaning.

        // 虽然 MIME 类型是不区分大小写的，但是传统都习惯使用小写，因此下面的判断就直接使用小写
        // 例如：https://www.iana.org/assignments/media-types/media-types.xhtml

        if( mediaType[0] == 'a' ) {

            if( mediaType == RequestContentType.Json )
                return SerializeFormat.Json;

            if( mediaType == RequestContentType.JsonLines )
                return SerializeFormat.JsonLines;

            if( mediaType == RequestContentType.Xml )
                return SerializeFormat.Xml;

            if( mediaType == RequestContentType.Form )
                return SerializeFormat.Form;

            if( mediaType == RequestContentType.Binary )
                return SerializeFormat.Binary;

            return SerializeFormat.Unknown;
        }

        if( mediaType[0] == 'm' ) {

            if( mediaType == RequestContentType.Multipart )
                return SerializeFormat.Multipart;

            return SerializeFormat.Unknown;
        }

        if( mediaType[0] == 't' ) {

            if( mediaType == RequestContentType.Text )
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
        return format switch {
            SerializeFormat.Text => ResponseContentType.TextUtf8,
            SerializeFormat.Json => ResponseContentType.JsonUtf8,
            SerializeFormat.Json2 => ResponseContentType.JsonUtf8,
            SerializeFormat.JsonLines => RequestContentType.JsonLines,
            SerializeFormat.Xml => ResponseContentType.XmlUtf8,
            SerializeFormat.Form => RequestContentType.FormUtf8,
            SerializeFormat.Multipart => RequestContentType.Multipart,
            SerializeFormat.Binary => RequestContentType.Binary,
            _ => string.Empty
        };
    }

}
