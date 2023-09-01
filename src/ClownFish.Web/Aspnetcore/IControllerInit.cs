namespace ClownFish.Web.Aspnetcore;

public interface IControllerInit
{
    void ControllerInit(NHttpContext httpContext);
}
