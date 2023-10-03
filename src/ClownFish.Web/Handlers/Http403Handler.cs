namespace ClownFish.Web.Handlers;

public sealed class Http403Handler : IAsyncNHttpHandler
{
    public static readonly Http403Handler Instance = new Http403Handler();

    internal static readonly string LoginUrl = LocalSettings.GetSetting("UserLoginUrl", "/").UrlEncode();
    internal static readonly string HtmlContent = typeof(Http404Handler).Assembly.ReadResAsText("ClownFish.Web.files.http403-not-login.html");

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        response.StatusCode = 403;
        response.ContentType = ResponseContentType.HtmlUtf8;

        string html = HtmlContent.Replace("##returnUrl##", LoginUrl);
        
        await response.WriteAllAsync(html.GetBytes());
    }
}
