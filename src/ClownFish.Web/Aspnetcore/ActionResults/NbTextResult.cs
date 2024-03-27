namespace ClownFish.Web.AspnetCore.ActionResults;

/// <summary>
/// 表示一个响应体内容为文本字符串的 ActionResult
/// </summary>
public sealed class NbTextResult : ActionResult
{
    /// <summary>
    /// 响应体内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// ContentType
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// StatusCode
    /// </summary>
    public int StatusCode { get; set; } = 200;


    /// <summary>
    /// 返回一个HTTP204的实例
    /// </summary>
    public static readonly NbTextResult Result204 = new NbTextResult { StatusCode = 204, Content = string.Empty };

    /// <summary>
    /// 创建一个表示 JSON 结果的 NbTextResult实例
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static NbTextResult CreateJsonResult(object data)
    {
        if( data == null )
            return Result204;

        return new NbTextResult { Content = data.ToJson(), ContentType = ResponseContentType.JsonUtf8 };
    }

    /// <summary>
    /// 创建一个表示 XML 结果的 NbTextResult实例
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static NbTextResult CreateXmlResult(object data)
    {
        if( data == null )
            return Result204;

        return new NbTextResult { Content = data.ToXml(), ContentType = ResponseContentType.XmlUtf8 };
    }

    /// <summary>
    /// 创建一个表示 TEXT 结果的 NbTextResult实例
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static NbTextResult CreateTextResult(string text)
    {
        if( text.IsNullOrEmpty() )
            return Result204;

        return new NbTextResult { Content = text, ContentType = ResponseContentType.TextUtf8 };
    }

    /// <summary>
    /// 创建一个表示 HTML 结果的 NbTextResult实例
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static NbTextResult CreateHtmlResult(string html)
    {
        if( html.IsNullOrEmpty() )
            return Result204;

        return new NbTextResult { Content = html, ContentType = ResponseContentType.HtmlUtf8 };
    }


    /// <summary>
    /// ExecuteResultAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        NHttpContext httpContextNetCore = HttpPipelineContext.Get2().HttpContext;
        await httpContextNetCore.HttpReplyAsync(this.StatusCode, this.Content, this.ContentType);
    }

    /// <summary>
    /// ExecuteResult
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void ExecuteResult(ActionContext context)
    {
        throw new NotImplementedException();
    }

}
