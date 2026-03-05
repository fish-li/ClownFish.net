namespace ClownFish.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class UrlRouteAttribute : Attribute
{
    public UrlRouteAttribute(string route)
    {
        this.Route = route;
    }

    internal string Route { get; private set; }
}