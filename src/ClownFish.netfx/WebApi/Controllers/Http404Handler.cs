namespace ClownFish.WebApi.Controllers;

internal sealed class Http404Handler : IHttpHandler
{
    public static readonly Http404Handler Instance = new Http404Handler();

    private static readonly byte[] s_data = "Not Found".GetBytes();

    public void ProcessRequest(NHttpContext httpContext)
    {
        httpContext.Response.StatusCode = 404;
        httpContext.Response.WriteAll(s_data);
    }

}
