namespace ClownFish.Web.Handlers;
public sealed class Http302Handler : IAsyncNHttpHandler
{
    private readonly string _url;

    public Http302Handler(string targetUrl)
    {
        _url = targetUrl.IfEmpty("/");
    }

    public Task ProcessRequestAsync(NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        response.StatusCode = 302;
        response.SetHeader("Location", _url);

        return Task.CompletedTask;
    }
}
