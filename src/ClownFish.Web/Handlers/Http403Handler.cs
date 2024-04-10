namespace ClownFish.Web.Handlers;

/// <summary>
/// 显示 HTTP 403 的错误页面，带有登录跳转功能，用于 Website 项目
/// </summary>
public sealed class Http403Handler : IAsyncNHttpHandler
{
    public static readonly Http403Handler Instance = new Http403Handler();

    internal static readonly byte[] HtmlContentBytes;

    static Http403Handler()
    {
        string html = typeof(Http403Handler).Assembly.ReadResAsText("ClownFish.Web.files.http403-not-login.html");

        string loginUrl = ClownFishWebOptions.LoginPageUrl;
        if( loginUrl.HasValue() ) {
            html = html.Replace("/x/login.phtml", loginUrl);
        }

        HtmlContentBytes = Encoding.UTF8.GetBytes(html);
    }

    // 像 Kibana 这类程序喜欢用前端路由，产生的URL例如：http://linuxtest:8208/app/discover#/?_g=(time:(from:now-30m,to:now))&xxxxxxx
    // 此时，在服务端只能取到：http://linuxtest:8208/app/discover
    // 所以没法在服务端生成回跳链接，所以最终采用在JS在页面中获取当前URL并修改链接的方法来解决

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        response.StatusCode = 403;
        response.ContentType = ResponseContentType.HtmlUtf8;

        //string html = HtmlContent.Replace("##returnUrl##", _url.UrlEncode());
        
        await response.WriteAllAsync(HtmlContentBytes);
    }
}


/// <summary>
/// 生成 HTTP 403 的错误响应（非HTML页面）。用于 Service 项目
/// </summary>
public sealed class Http403MsgHandler : IAsyncNHttpHandler
{
    public static readonly Http403MsgHandler Instance = new Http403MsgHandler();

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        await httpContext.HttpReplyAsync(403, "HTTP403禁止访问，登录后才能访问此页面！");
    }
}