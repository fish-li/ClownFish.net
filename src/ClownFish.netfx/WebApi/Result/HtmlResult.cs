namespace ClownFish.WebApi.Result;

/// <summary>
/// 表示一个以HTML形式返回的结果
/// </summary>
public sealed class HtmlResult : IActionResult
{
    /// <summary>
    /// 将要输出的HTML代码
    /// </summary>
    public string Html { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="html">将要输出的HTML代码</param>
    public HtmlResult(string html)
    {
        if( html == null )
            throw new ArgumentNullException("html");

        this.Html = html;
    }


    void IActionResult.Ouput(NHttpContext context)
    {
        context.Response.ContentType = ResponseContentType.HtmlUtf8;
        context.Response.WriteAll(Html.GetBytes());
    }
}
