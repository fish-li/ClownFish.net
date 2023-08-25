namespace ClownFish.Base;

/// <summary>
/// 定义一些框架中经常使用的请求头名称
/// </summary>
public static class HttpHeaders
{
    #region 标准请求头

    /// <summary>
    /// 标准请求头
    /// </summary>
    public static class Request
    {
        /// <summary>
        /// "Content-Type"
        /// </summary>
        public static readonly string ContentType = "Content-Type";

        /// <summary>
        /// "Content-Length"
        /// </summary>
        public static readonly string ContentLength = "Content-Length";

        /// <summary>
        /// "User-Agent"
        /// </summary>
        public static readonly string UserAgent = "User-Agent";

        /// <summary>
        /// "Content-Encoding"
        /// </summary>
        public static readonly string ContentEncoding = "Content-Encoding";

        /// <summary>
        /// "Content-Charset"
        /// </summary>
        public static readonly string ContentCharset = "Content-Charset";

        /// <summary>
        /// "Referer"
        /// </summary>
        public static readonly string Referer = "Referer";
    }


    /// <summary>
    /// 标准呼应头
    /// </summary>
    public static class Response
    {
        /// <summary>
        /// "Content-Type"
        /// </summary>
        public static readonly string ContentType = "Content-Type";

        /// <summary>
        /// "Content-Encoding"
        /// </summary>
        public static readonly string ContentEncoding = "Content-Encoding";
    }

    #endregion

    /// <summary>
    /// 自定义请求头
    /// </summary>
    public static class XRequest
    {
        /// <summary>
        /// "x-datatype"
        /// </summary>
        public static readonly string DataType = "x-datatype";

        /// <summary>
        /// "x-nebula-DEBUG"
        /// </summary>
        public static readonly string Debug = "x-nebula-DEBUG";

        /// <summary>
        /// "x-client-app"
        /// </summary>
        public static readonly string ClientApp = "x-client-app";


        /// <summary>
        /// "xnb-rootid"
        /// </summary>
        internal static readonly string RootId = "xnb-rootid";

        /// <summary>
        /// "xnb-parentid"
        /// </summary>
        internal static readonly string ParentId = "xnb-parentid";
    }


    /// <summary>
    /// 自定义响应头
    /// </summary>
    public static class XResponse
    {
        /// <summary>
        /// "x-RequestId"
        /// </summary>
        public static readonly string RequestId = "x-RequestId";

        /// <summary>
        /// "x-HostName"
        /// </summary>
        public static readonly string HostName = "x-HostName";

        /// <summary>
        /// "x-ErrorCode"
        /// </summary>
        public static readonly string ErrorCode = "x-ErrorCode";

        /// <summary>
        /// "x-ErrorMessage"
        /// </summary>
        public static readonly string ErrorMessage = "x-ErrorMessage";

        /// <summary>
        /// "x-ExceptionType"
        /// </summary>
        public static readonly string ExceptionType = "x-ExceptionType";

    }

}
