namespace ClownFish.WebApi.Controllers;

internal sealed class Http404Handler : IHttpHandler
{
    public static readonly Http404Handler Instance = new Http404Handler();

    public void ProcessRequest(NHttpContext httpContext)
    {
        httpContext.Response.StatusCode = 404;
        httpContext.Response.WriteAll("Not Found".GetBytes());
    }

}
