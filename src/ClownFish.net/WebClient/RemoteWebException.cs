namespace ClownFish.WebClient;

/// <summary>
/// 表示在HTTP调用时发生的远程服务端异常。例如：HTTP 404, HTTP500 之类的异常。
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

        return StatusCodeUtils.GetStatusCodeForRemoteWebException(statusCode);
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


    /// <summary>
    /// 异常的简单描述
    /// </summary>
    public override string Message {
        get {
            return (_message ?? base.Message)
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
            using( ResponseReader reader = new ResponseReader(response) ) {  // 读写之后释放 Response
                this.Result = reader.Read<HttpResult<string>>();
            }
        }
        catch( Exception exx ) {
            Console2.Warnning(exx);
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

                // 尝试从网页头<title>中获取消息描述
                string contentType = this.Result.Headers[HttpHeaders.Response.ContentType];

                if( contentType != null && contentType.StartsWith("text/html", StringComparison.Ordinal) ) {
                    _message = GetHtmlTitle(this.Result.Result);
                }
            }
        }
    }


    /// <summary>
    /// 尝试从一段HTML代码中读取文档标题部分
    /// </summary>
    /// <param name="text">HTML代码</param>
    /// <returns>文档标题</returns>
    private static string GetHtmlTitle(string text)
    {
        if( string.IsNullOrEmpty(text) )
            return null;

        int p1 = text.IndexOfIgnoreCase("<title>");
        int p2 = text.IndexOfIgnoreCase("</title>");

        if( p2 > p1 && p1 > 0 ) {
            p1 += "<title>".Length;
            return text.Substring(p1, p2 - p1);
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
