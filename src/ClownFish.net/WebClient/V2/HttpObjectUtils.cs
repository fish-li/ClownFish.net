#if NETCOREAPP

using System.Net.Http;
using System.Net.Sockets;
using MyHttpOption = ClownFish.WebClient.HttpOption;


namespace ClownFish.WebClient.V2;

internal static class HttpObjectUtils
{
    private static readonly string[] s_wellKnownContentHeaders = new string[10]
        {
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
        string[] array = s_wellKnownContentHeaders;
        foreach( string b in array ) {
            if( string.Equals(header, b, StringComparison.OrdinalIgnoreCase) ) {
                return true;
            }
        }
        return false;
    }


    public static HttpRequestMessage CreateRequestMessage(MyHttpOption httpOption)
    {
        Uri requestUri = httpOption.GetReuestUri();
        HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpOption.Method), requestUri);


        requestMessage.Headers.TransferEncodingChunked = false;
        requestMessage.Version = HttpVersion.Version11;

        // 构造请求体内容
        requestMessage.Content = CreateRequestMessageBody(httpOption);


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
            return CreateRequestMessageBody1(httpOption.Format, srcStream);
        }

        if( postData is byte[] bytes ) {
            return CreateRequestMessageBody2(httpOption.Format, bytes);
        }

        return CreateRequestMessageBody3(httpOption.Format, postData);
    }


    internal static HttpContent CreateRequestMessageBody1(SerializeFormat format, Stream srcStream)
    {
        if( srcStream.CanRead == false )
            throw new ArgumentException("指定的数据流不能读取。");

        if( srcStream.CanSeek )
            srcStream.Position = 0;

        HttpContent content = new StreamContent(srcStream);

        string contentType = RequestContentType.GetByFormat(format);
        if( contentType.IsNullOrEmpty() == false )
            content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentType, contentType);

        return content;
    }


    internal static HttpContent CreateRequestMessageBody2(SerializeFormat format, byte[] bytes)
    {
        HttpContent content = new ByteArrayContent(bytes);

        string contentType = RequestContentType.GetByFormat(format);
        if( contentType.IsNullOrEmpty() == false )
            content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentType, contentType);

        return content;
    }

    internal static HttpContent CreateRequestMessageBody3(SerializeFormat format, object postData)
    {
        MemoryStream ms = new MemoryStream();
        HttpContent content = new StreamContent(ms);

        var writer = new ClownFish.WebClient.RequestWriter();
        writer.Write(ms, postData, format);
        ms.Position = 0;

        //byte[] buffer = ms.ToArray();
        //HttpContent content = new ByteArrayContent(buffer);

        if( writer.ContentType.IsNullOrEmpty() == false ) {
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(writer.ContentType);
            content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentType, writer.ContentType);
        }

        return content;
    }

    public static void SetKeepAlive(this HttpRequestMessage requestMessage, bool keepAlive)
    {
        if( keepAlive ) {
            requestMessage.Headers.Connection.Add("Keep-Alive");
        }
        else {
            requestMessage.Headers.ConnectionClose = true;
        }
    }


    public static HttpClient CreateClient(MyHttpOption httpOption)
    {
        HttpMessageHandler clientHandler = httpOption.MessageHandler 
                                            ?? TryCreateUnixSocketHandler(httpOption) 
                                            ?? CreateClientHandler(httpOption);

        // 如果 MessageHandler 由外部指定，则由外部代码负责销毁
        // 如果是 UnixSocket，则调用结束后，随着HttpClient自动销毁
        // 其它，MessageHandler 由 ClownFish.net 创建的，随着HttpClient自动销毁

        bool disposeHandler = httpOption.MessageHandler == null;
        HttpClient client = new HttpClient(clientHandler, disposeHandler);


        if( httpOption.Timeout.HasValue ) {
            client.Timeout = httpOption.Timeout.Value > 0
                        ? TimeSpan.FromMilliseconds(httpOption.Timeout.Value)
                        : System.Threading.Timeout.InfiniteTimeSpan;
        }

        return client;
    }

    public static HttpClientHandler CreateClientHandler(MyHttpOption httpOption)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.AutomaticDecompression = DecompressionMethods.All;

        if( httpOption.Credentials != null )
            clientHandler.Credentials = httpOption.Credentials;

        if( httpOption.AllowAutoRedirect.HasValue )
            clientHandler.AllowAutoRedirect = httpOption.AllowAutoRedirect.Value;

        clientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => {
            return true;
        };

        //clientHandler.MaxAutomaticRedirections = 50;
        //clientHandler.MaxResponseHeadersLength = 128;
        //clientHandler.PreAuthenticate = false;
        clientHandler.MaxConnectionsPerServer = 1024;

        //if( httpOption.Cookie != null ) {
        //	clientHandler.CookieContainer = httpOption.Cookie;
        //}
        //else {
        clientHandler.UseCookies = false;
        //}
        //if( _proxy == null ) {
        //	clientHandler.UseProxy = false;
        //}
        //else if( _proxy != WebRequest.GetSystemWebProxy() ) {
        //	clientHandler.Proxy = _proxy;
        //}
        //clientHandler.ClientCertificates.AddRange(ClientCertificates);
        //clientHandler.SslProtocols = (SslProtocols)ServicePointManager.SecurityProtocol;
        //clientHandler.CheckCertificateRevocationList = ServicePointManager.CheckCertificateRevocationList;

        return clientHandler;
    }

    public static HttpMessageHandler TryCreateUnixSocketHandler(MyHttpOption httpOption)
    {
#if NET6_0_OR_GREATER
        if( httpOption.UnixSocketEndPoint.HasValue() )
            return UnixHelper.CreateSocketHandler(httpOption.UnixSocketEndPoint);
        else
            return null;
#else
        return null;
#endif
    }



}
#endif
