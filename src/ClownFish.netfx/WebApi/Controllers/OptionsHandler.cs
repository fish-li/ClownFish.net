
namespace ClownFish.WebApi.Controllers;

internal sealed class OptionsHandler : IAsyncNHttpHandler
{
    public static readonly OptionsHandler Instance = new OptionsHandler();

    public void ProcessRequest(NHttpContext httpContext)
    {
    }

    public Task ProcessRequestAsync(NHttpContext httpContext)
    {
        return Task2.CompletedTask;
    }
}
