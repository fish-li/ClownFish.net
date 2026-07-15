namespace ClownFish.WebClient;

/// <summary>
/// 表示在HTTP调用时发生的异常，包括来自客户端和服务端的异常，
/// 通常客户端出现的异常有：HTTP调用超时，网络不通，或者URL地址无效，等等。
/// 服务端的异常包含有：HTTP 404, HTTP500,502,503 之类的异常。
/// 
/// 这个异常类型解决了二个问题：
/// 1、WebException异常消息Message太笼统，没有任何价值，不利于排查问题。
/// 2、Response属性的内容编码不确定（ASP.NET 采用UTF-8，IIS采用GB2312），直接获取异常页面内容时容易出现乱码问题。
/// </summary>
public sealed class RemoteWebException : System.Exception, ILoggingObject, IToAllText, IToString2, IErrorCode, IHttpResultString
{
    private string _message;


    /// <summary>
    /// 发生异常时的调用网址
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// 服务端返回的状态码
    /// </summary>
    public int StatusCode {
        get {
            return this.Result?.StatusCode ?? 0;
        }
    }

    int IErrorCode.GetErrorCode()
    {
        int statusCode = this.StatusCode;
        if( statusCode == 0 )
            statusCode = 500;

        return StatusCodeUtils.GetStatusCodeForRemoteWebException(statusCode);   // 例如 401 => 701
    }

    /// <summary>
    /// 服务端的返回结果（有可能为NULL）
    /// </summary>
    public HttpResult<string> Result { get; private set; }


    HttpResult<string> IHttpResultString.Response => this.Result;


    /// <summary>
    /// 服务端返回的响应内容（有可能为NULL）
    /// </summary>
    public string ResponseText {
        get => this.Result?.Result ?? string.Empty;
    }

    /* 一些典型的异常调用堆栈，
     * 可以发现直接取 innerException.Message 得到的结果如果是 "An error occurred while sending the request."
     * 那就没什么意义了，所以要取 GetBaseException().Message
     * 
---> System.Net.Http.HttpRequestException: An error occurred while sending the request.
---> System.Net.Http.HttpIOException: The response ended prematurely. (ResponseEnded)
     * 
---> System.Net.Http.HttpRequestException: An error occurred while sending the request.
 ---> System.IO.IOException: Unable to read data from the transport connection: Connection timed out.
 ---> System.Net.Sockets.SocketException (110): Connection timed out


 ---> System.Net.Http.HttpRequestException: An error occurred while sending the request.
 ---> System.IO.IOException: Unable to read data from the transport connection: Connection reset by peer.
 ---> System.Net.Sockets.SocketException (104): Connection reset by peer


 ---> System.Net.Http.HttpRequestException: Connection timed out (xxxxxxxxx.com:443)
 ---> System.Net.Sockets.SocketException (110): Connection timed out

 ---> System.Net.Http.HttpRequestException: Name or service not known (xxxxxxxxx.com:443)
 ---> System.Net.Sockets.SocketException (0xFFFDFFFF): Name or service not known
     */

    /// <summary>
    /// 异常的简单描述
    /// </summary>
    public override string Message {
        get {
            return (_message ?? this.GetBaseException().Message)
                    + $"\r\n[StatusCode={this.StatusCode}]"
                    + (string.IsNullOrEmpty(Url) ? string.Empty : ("\r\n=)本次调用的目标地址：" + this.Url));
        }
    }


    internal string ServerMessage => _message;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="url"></param>
    public RemoteWebException(Exception ex, string url) : base(ex?.Message, ex)
    {
        if( ex == null )
            throw new ArgumentNullException(nameof(ex));


        this.Url = url;

        if( ex is WebException wex ) {
            ReadResponse(wex.Response as HttpWebResponse);
        }
    }


    private void ReadResponse(HttpWebResponse response)
    {
        if( response == null )
            return;

        try {
            using( ResponseReader reader = new ResponseReader(response) ) {
                this.Result = reader.Read<HttpResult<string>>();
            }
        }
        catch( Exception exx ) {
            Console2.Warnning(exx);
        }
        finally {
            response.Dispose();
        }


        if( this.Result != null ) {

            try {
                // 获取一个有价值的异常消息描述，WebException的Message完全没有意义！
                // 先尝试从响应头上获取异常消息，需要对方框架支持
                _message = this.Result.Headers[HttpHeaders.XResponse.ErrorMessage].UrlDecode();
            }
            catch {
                _message = null;
            }

            if( _message.IsNullOrEmpty() ) {

                string contentType = this.Result.Headers[HttpHeaders.Response.ContentType];
                if( contentType != null ) {

                    if( contentType.StartsWith0("text/plain") ) {  // 服务端输出就是简单的错误描述信息，直接使用即可
                        _message = this.Result.Result;
                    }
                    else if( contentType.StartsWith0("text/html") ) {  // 尝试从网页头<title>中获取消息描述
                        _message = GetHtmlTitle(this.Result.Result);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 尝试从一段HTML代码中读取文档标题部分
    /// </summary>
    /// <param name="html">HTML代码</param>
    /// <returns>文档标题</returns>
    internal static string GetHtmlTitle(string html)
    {
        if( string.IsNullOrEmpty(html) )
            return null;

        int p1 = html.IndexOfIgnoreCase("<title>");
        int p2 = html.IndexOfIgnoreCase("</title>");

        if( p2 > p1 && p1 > 0 ) {
            p1 += "<title>".Length;
            return html.Substring(p1, p2 - p1);
        }

        return null;
    }

    /// <summary>
    /// 获取当前对象的日志展示文本
    /// </summary>
    /// <returns></returns>
    public string ToLoggingText()
    {
        string exAll = this.ToString();

        string response = this.Result?.ToAllText(true);
        if( response.IsNullOrEmpty() == false )
            return exAll + "\r\n-------------------------Response-------------------------\r\n" + response.SubstringN(LoggingLimit.HttpBodyMaxLen);
        else
            return exAll;
    }

    /// <summary>
    /// 将一个对象的所有信息全部转成文本形式输出
    /// </summary>
    /// <returns></returns>
    public string ToAllText()
    {
        string exAll = this.ToString();

        string response = this.Result?.ToAllText(true);
        if( response.IsNullOrEmpty() == false )
            return exAll + "\r\n-------------------------Response-------------------------\r\n" + response;
        else
            return exAll;
    }


    /// <summary>
    /// ToString()的增强版本，结果包含服务端的响应内容
    /// </summary>
    /// <returns></returns>
    public string ToString2()
    {
        return this.ToLoggingText();
    }

}
