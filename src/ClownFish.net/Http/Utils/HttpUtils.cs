namespace ClownFish.Http.Utils;

/// <summary>
/// 封装一些与HTTP操作相关的工具类
/// </summary>
public static class HttpUtils
{

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

        if( contentType.StartsWith0("text/")
            || contentType.StartsWith0(RequestContentType.Json)
            || contentType.StartsWith0(RequestContentType.Xml)
            || contentType.StartsWith0(RequestContentType.Form)
            )
            return true;

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

        if( contentType.StartsWith0("text/")
            || contentType.StartsWith0(RequestContentType.Json)
            || contentType.StartsWith0(RequestContentType.Xml)

            // 下面是response才会用到的，并且可能会遇到的，            
            || contentType.StartsWith0("application/problem+json")
            //|| contentType.StartsWith0("application/api-problem+json")
            //|| contentType.StartsWith0("application/problem+xml")
            //|| contentType.StartsWith0("application/json-seq")
            //|| contentType.StartsWith0("application/x-ndjson")
            // 其实还有更多，这里就不再一一列出 ~~~~
            )
            return true;

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
}
