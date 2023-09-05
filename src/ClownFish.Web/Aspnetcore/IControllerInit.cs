namespace ClownFish.Web.Aspnetcore;

public interface IControllerInit
{
    void Init(NHttpContext httpContext);
}
