namespace ClownFish.Http.Utils;

/// <summary>
/// 封装一些与HTTP操作相关的工具类
/// </summary>
public static class HttpUtils
{
    static HttpUtils()
    {
        InitContentTypeCache();
    }

    /// <summary>
    /// 根据一个请求的提交方法，判断是否包含请求体
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool RequestHasBody(string method)
    {
        if( string.IsNullOrEmpty(method) )
            throw new ArgumentNullException(nameof(method));

        //method = method.ToUpper();

        // 说明：DELETE 方法是【允许】有请求体的，
        // 可参考：https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/DELETE
        // 但是通常不会这样做，所以这里的判断规则是：DELETE 不允许有请求体

        if( method == "POST"        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST
            || method == "PUT"      // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PUT
            || method == "PATCH"    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PATCH
            )
            return true;
        else
            return false;
    }


    /// <summary>
    /// 判断是否可以写响应流
    /// </summary>
    /// <param name="method"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public static bool CanWriteResponseBody(string method, int statusCode)
    {
        // copy from: Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.CanWriteResponseBody

        if( statusCode == 204 || statusCode == 205 || statusCode == 304 || method == "HEAD" ) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 根据指定的contentType判断BODY是不是文本类型
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static bool RequestBodyIsText(string contentType)
    {
        if( contentType.IsNullOrEmpty() )
            return false;

        // MIME types are case-insensitive but are traditionally written in lowercase, 
        // with the exception of parameter values, whose case may or may not have specific meaning.

        // 虽然 MIME 类型是不区分大小写的，但是传统都习惯使用小写，因此下面的判断就直接使用小写
        // 例如：https://www.iana.org/assignments/media-types/media-types.xhtml

        if( contentType[0] == 't' ) {
            return contentType.StartsWith0("text/");
        }

        if( contentType[0] == 'a' ) {
            return contentType.StartsWith0(RequestContentType.Json)  // 可包含：application/json-seq
                || contentType.StartsWith0(RequestContentType.Xml)
                || contentType.StartsWith0(RequestContentType.Form)
                || contentType.StartsWith0(RequestContentType.JsonLines);
        }

        return false;
    }

    /// <summary>
    /// 根据指定的contentType判断BODY是不是文本类型
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static bool ResponseBodyIsText(string contentType)
    {
        if( contentType.IsNullOrEmpty() )
            return false;

        if( contentType[0] == 't' ) {
            return contentType.StartsWith0("text/");
        }

        if( contentType[0] == 'a' ) {
            return contentType.StartsWith0(ResponseContentType.Json)  // 可包含：application/json-seq
                || contentType.StartsWith0(ResponseContentType.Xml)
                //|| contentType.StartsWith0(RequestContentType.Form)  // Response根本不使用这个类型
                || contentType.StartsWith0(ResponseContentType.JsonLines)
                || contentType.StartsWith0("application/problem+json");

            //|| contentType.StartsWith0("application/problem+xml")
            // 其实还有更多，这里就不再一一列出 ~~~~
        }

        return false;
    }


    internal static string GetStatusReasonPhrase(int statusCode)
    {
        // copy from System.Net.HttpStatusDescription.Get()

        return statusCode switch {
            100 => "Continue",
            101 => "Switching Protocols",
            102 => "Processing",
            103 => "Early Hints",
            200 => "OK",
            201 => "Created",
            202 => "Accepted",
            203 => "Non-Authoritative Information",
            204 => "No Content",
            205 => "Reset Content",
            206 => "Partial Content",
            207 => "Multi-Status",
            208 => "Already Reported",
            226 => "IM Used",
            300 => "Multiple Choices",
            301 => "Moved Permanently",
            302 => "Found",
            303 => "See Other",
            304 => "Not Modified",
            305 => "Use Proxy",
            307 => "Temporary Redirect",
            308 => "Permanent Redirect",
            400 => "Bad Request",
            401 => "Unauthorized",
            402 => "Payment Required",
            403 => "Forbidden",
            404 => "Not Found",
            405 => "Method Not Allowed",
            406 => "Not Acceptable",
            407 => "Proxy Authentication Required",
            408 => "Request Timeout",
            409 => "Conflict",
            410 => "Gone",
            411 => "Length Required",
            412 => "Precondition Failed",
            413 => "Request Entity Too Large",
            414 => "Request-Uri Too Long",
            415 => "Unsupported Media Type",
            416 => "Requested Range Not Satisfiable",
            417 => "Expectation Failed",
            421 => "Misdirected Request",
            422 => "Unprocessable Entity",
            423 => "Locked",
            424 => "Failed Dependency",
            426 => "Upgrade Required",
            428 => "Precondition Required",
            429 => "Too Many Requests",
            431 => "Request Header Fields Too Large",
            451 => "Unavailable For Legal Reasons",
            500 => "Internal Server Error",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            504 => "Gateway Timeout",
            505 => "Http Version Not Supported",
            506 => "Variant Also Negotiates",
            507 => "Insufficient Storage",
            508 => "Loop Detected",
            510 => "Not Extended",
            511 => "Network Authentication Required",
            _ => statusCode.ToString(),
        };
    }


    private class ContentTypeInfo
    {
        public string MediaType { get; set; }
        public Encoding Encoding { get; set; }

        //public override string ToString()
        //{
        //    return $"MediaType={MediaType};     Encoding={Encoding?.WebName ?? "NULL"}";
        //}
    }

    private static readonly Dictionary<string, ContentTypeInfo> s_contentTypeCache = new Dictionary<string, ContentTypeInfo>(30, StringComparer.Ordinal);

    private static void InitContentTypeCache()
    {
        // 缓存一些常见的Content-Type对应的解析结果，避免每次解析时产生新对象
        // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Guides/MIME_types

        AddContentTypeCache(ResponseContentType.Text);
        AddContentTypeCache(ResponseContentType.TextUtf8);
        AddContentTypeCache(ResponseContentType.Json);
        AddContentTypeCache(ResponseContentType.JsonUtf8);
        AddContentTypeCache(ResponseContentType.JsonLines);
        AddContentTypeCache(ResponseContentType.Xml);
        AddContentTypeCache(ResponseContentType.XmlUtf8);
        AddContentTypeCache(ResponseContentType.Html);
        AddContentTypeCache(ResponseContentType.HtmlUtf8);
        AddContentTypeCache(ResponseContentType.OctetStream);
        AddContentTypeCache(RequestContentType.Form);
        AddContentTypeCache(RequestContentType.FormUtf8);
        AddContentTypeCache("application/problem+json");
        AddContentTypeCache("application/json-seq");
        AddContentTypeCache("text/csv");
        AddContentTypeCache("text/css");
        AddContentTypeCache("text/javascript");
        AddContentTypeCache("application/javascript");
        AddContentTypeCache("image/png");
        AddContentTypeCache("image/jpeg");
        AddContentTypeCache("image/gif");
        AddContentTypeCache("image/svg+xml");
        AddContentTypeCache("image/ico");
        AddContentTypeCache("font/woff");
        AddContentTypeCache("font/ttf");
        AddContentTypeCache("font/otf");
    }

    private static void AddContentTypeCache(string contentType)
    {
        if( ParseContentType(contentType, out string mediaType, out Encoding encoding) > 0 ) {

            ContentTypeInfo contentTypeInfo = new ContentTypeInfo {
                MediaType = mediaType,
                Encoding = encoding
            };
            s_contentTypeCache[contentType] = contentTypeInfo;
        }
    }

    /// <summary>
    /// 解析 Content-Type 标头
    /// </summary>
    /// <param name="contentType"></param>
    /// <param name="mediaType"></param>
    /// <param name="encoding"></param>
    /// <returns>解析出来多少个数据，0：contentType参数为空，1：只解析出 mediaType，2：已解析 mediaType 和 encoding </returns>
    public static int ParseContentType(string contentType, out string mediaType, out Encoding encoding)
    {
        // 参考链接：https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Reference/Headers/Content-Type
        // Content-Type 通常有以下2种用法：
        // 1, text/html; charset=utf-8
        // 2, multipart/form-data; boundary=something
        // 实际使用时，“用法1” 也可以不指定charset简写为：  text/html

        // 规范 https://httpwg.org/specs/rfc9110.html#field.content-type 列出几种用法：不区分大小写，空格可有可无，甚至charset值还允许用双引号
        /*    text/html;charset=utf-8
              Text/HTML;Charset="utf-8"
              text/html; charset="utf-8"
              text/html;charset=UTF-8
         */

        mediaType = null;
        encoding = null;

        if( contentType.IsNullOrEmpty() )
            return 0;


        ContentTypeInfo contentTypeInfo = s_contentTypeCache.TryGet(contentType);
        if( contentTypeInfo != null ) {
            mediaType = contentTypeInfo.MediaType;
            encoding = contentTypeInfo.Encoding;
            return encoding == null ? 1 : 2;
        }


        int p = contentType.IndexOf(';');   // 示例 Content-Type: application/json; charset=xxxxx
        if( p > 2 ) {

            mediaType = contentType.Substring(0, p);  // 这里不检查 media 的格式是否符合 type/subtype

            if( p == contentType.Length - 1 ) {
                return 1;      // 能到这里，参数contentType的格式就不规范了，这里先忽略这个错误～～～
            }

            string part2 = contentType.Substring(p + 1).Trim();

            int p2 = part2.IndexOf('=');
            if( p2 > 1 && p2 < part2.Length - 1 ) {
                string p2name = part2.Substring(0, p2);

                if( p2name.Is("charset") == false ) {
                    return 1;   // 后部分参数不是 charset，忽略～～～
                }

                string p2value = part2.Substring(p2 + 1);
                if( p2value.Length > 3 && p2value[0] == '"' && p2value[p2value.Length - 1] == '"' )
                    p2value = p2value.Substring(1, p2value.Length - 2);

                encoding = EncodingUtils.GetEncoding(p2value);
                return 2;
            }
            else {
                return 1;  // 后部分的格式未知，忽略～～～
            }
        }
        else {   // 示例 Content-Type: application/json
            mediaType = contentType;
            return 1;
        }
    }


    /// <summary>
    /// 解析 Content-Type 标头
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static string ParseMediaType(string contentType)
    {
        if( contentType.IsNullOrEmpty() )
            return string.Empty;


        ContentTypeInfo contentTypeInfo = s_contentTypeCache.TryGet(contentType);
        if( contentTypeInfo != null ) {
            return contentTypeInfo.MediaType;
        }

        int p = contentType.IndexOf(';');   // 示例 Content-Type: application/json; charset=xxxxx
        if( p > 2 ) {

            return contentType.Substring(0, p);
        }
        else {   // 示例 Content-Type: application/json
            return contentType;
        }
    }

}
