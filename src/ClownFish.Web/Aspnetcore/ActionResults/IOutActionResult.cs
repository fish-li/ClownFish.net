namespace ClownFish.Web.Aspnetcore.ActionResults;

public interface IOutActionResult
{
    Task OutResultAsync(NHttpContext httpContext);
}
