namespace ClownFish.WebApi.Controllers;

internal sealed class OptionsHandler : IHttpHandler
{
    public static readonly OptionsHandler Instance = new OptionsHandler();

    public void ProcessRequest(NHttpContext httpContext)
    {
    }

}
