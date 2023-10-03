namespace ClownFish.Web.Handlers;

public sealed class Http404Handler : IAsyncNHttpHandler
{
    public static readonly Http404Handler Instance = new Http404Handler();

    internal static readonly byte[] HtmlContent = typeof(Http404Handler).Assembly.ReadResAsText("ClownFish.Web.files.http404-url-error.html").GetBytes();


    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        response.StatusCode = 404;
        response.ContentType = ResponseContentType.HtmlUtf8;
        await response.WriteAllAsync(HtmlContent);
    }
}
