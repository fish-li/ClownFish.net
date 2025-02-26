
namespace ClownFish.WebApi.Controllers;

internal sealed class Http404Handler : IAsyncNHttpHandler
{
    public static readonly Http404Handler Instance = new Http404Handler();

    private static readonly byte[] s_data = "Not Found".GetBytes();

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        httpContext.Response.StatusCode = 404;
        await httpContext.Response.WriteAllAsync(s_data);
    }
}
