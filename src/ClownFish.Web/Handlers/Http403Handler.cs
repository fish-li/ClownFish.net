namespace ClownFish.Web.Handlers;

public sealed class Http403Handler : IAsyncNHttpHandler
{
    public static readonly Http403Handler Instance = new Http403Handler();

    internal static readonly byte[] HtmlContentBytes = typeof(Http403Handler).Assembly.ReadResAsText("ClownFish.Web.files.http403-not-login.html").GetBytes();

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
