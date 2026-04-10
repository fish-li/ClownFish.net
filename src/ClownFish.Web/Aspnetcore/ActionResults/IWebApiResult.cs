namespace ClownFish.Web.Aspnetcore.ActionResults;

public interface IWebApiResult
{
    Task OutResultAsync(NHttpContext httpContext);
}
