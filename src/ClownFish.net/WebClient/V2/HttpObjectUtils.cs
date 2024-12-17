#if NETCOREAPP

using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using ClownFish.Http.Utils;
using MyHttpOption = ClownFish.WebClient.HttpOption;


namespace ClownFish.WebClient.V2;

internal static class HttpObjectUtils
{
    private static readonly HashSet<string> s_wellKnownContentHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "Content-Disposition",
            "Content-Encoding",
            "Content-Language",
            "Content-Length",
            "Content-Location",
            "Content-MD5",
            "Content-Range",
            "Content-Type",
            "Expires",
            "Last-Modified"
     };

    public static bool IsWellKnownContentHeader(string header)
    {
        return s_wellKnownContentHeaders.Contains(header);
    }


    public static HttpRequestMessage CreateRequestMessage(MyHttpOption httpOption)
    {
        Uri requestUri = httpOption.GetReuestUri();
        HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpOption.Method), requestUri);


        requestMessage.Headers.TransferEncodingChunked = false;
        requestMessage.Version = HttpVersion.Version11;

        // 构造请求体内容
        requestMessage.Content = CreateRequestMessageBody(httpOption);

        if( httpOption.Id.HasValue() ) {
            requestMessage.SetOptionValue(LoggingKeys.HttpOptionId, httpOption.Id);
        }

        // 设置请求头
        foreach( var item in httpOption.Headers ) {
            if( IsWellKnownContentHeader(item.Name) ) {
                requestMessage.Content.Headers.TryAddWithoutValidation(item.Name, item.Value);
            }
            else {
                requestMessage.Headers.TryAddWithoutValidation(item.Name, item.Value);
            }
        }

        if( requestMessage.Headers.Contains(HttpHeaders.Request.UserAgent) == false ) {
            if( string.IsNullOrEmpty(httpOption.UserAgent) == false )
                requestMessage.Headers.TryAddWithoutValidation(HttpHeaders.Request.UserAgent, httpOption.UserAgent);
            else
                requestMessage.Headers.TryAddWithoutValidation(HttpHeaders.Request.UserAgent, ConstValues.HttpClientUserAgent);
        }

        return requestMessage;
    }

    internal static HttpContent CreateRequestMessageBody(MyHttpOption httpOption)
    {
        object postData = httpOption.GetPostData();
        if( postData == null )
            return new ByteArrayContent(Array.Empty<byte>());


        if( postData is Stream srcStream ) {
            return CreateRequestMessageBody1(httpOption.Format, srcStream);   // StreamContent
        }

        if( postData is byte[] bytes ) {
            return CreateRequestMessageBody2(httpOption.Format, bytes);     // ByteArrayContent
        }

        return CreateRequestMessageBody3(httpOption.Format, postData, httpOption.AutoGzipUpload);      // StreamContent
    }


    internal static HttpContent CreateRequestMessageBody1(SerializeFormat format, Stream srcStream)
    {
        if( srcStream.CanRead == false )
            throw new ArgumentException("指定的数据流不能读取。");

        if( srcStream.CanSeek )
            srcStream.Position = 0;

        HttpContent content = new StreamContent(srcStream);

        string contentType = ContenTypeUtils.GetByFormat(format);
        if( contentType.IsNullOrEmpty() == false )
            content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentType, contentType);

        return content;
    }


    internal static HttpContent CreateRequestMessageBody2(SerializeFormat format, byte[] bytes)
    {
        HttpContent content = new ByteArrayContent(bytes);

        string contentType = ContenTypeUtils.GetByFormat(format);
        if( contentType.IsNullOrEmpty() == false )
            content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentType, contentType);

        return content;
    }

    internal static HttpContent CreateRequestMessageBody3(SerializeFormat format, object postData, bool autoGzip = false)
    {
        MemoryStream ms = new MemoryStream();
        HttpContent content = new StreamContent(ms);

        var writer = new ClownFish.WebClient.RequestWriter();
        writer.Write(ms, postData, format, autoGzip);
        ms.Position = 0;

        //byte[] buffer = ms.ToArray();
        //HttpContent content = new ByteArrayContent(buffer);

        if( writer.ContentType.IsNullOrEmpty() == false ) {
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(writer.ContentType);
            content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentType, writer.ContentType);
        }

        if( writer.IsGzip ) {
            content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentEncoding, "gzip");
        }

        return content;
    }

    //public static void SetKeepAlive(this HttpRequestMessage requestMessage, bool keepAlive)
    //{
    //    if( keepAlive ) {
    //        requestMessage.Headers.Connection.Add("Keep-Alive");
    //    }
    //    else {
    //        requestMessage.Headers.ConnectionClose = true;
    //    }
    //}

}
#endif
