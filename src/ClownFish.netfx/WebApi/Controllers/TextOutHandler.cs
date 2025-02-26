namespace ClownFish.WebApi.Controllers;

internal class TextOutHandler : IAsyncNHttpHandler
{
    private readonly string _html;
    private readonly string _contentType;

    public TextOutHandler(string html, string contentType = null)
    {
        _html = html;
        _contentType = contentType;
    }

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        httpContext.Response.ContentType = _contentType ?? ResponseContentType.HtmlUtf8;
        await httpContext.Response.WriteAllAsync(_html.GetBytes());
    }
}
