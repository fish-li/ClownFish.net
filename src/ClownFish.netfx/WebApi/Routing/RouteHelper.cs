namespace ClownFish.WebApi.Routing;

internal static partial class RouteHelper
{

    public static RoutingObject CreateRoutingObject(Type t, MethodInfo m, RouteAttribute a1, RouteAttribute a2)
    {
        RoutingObject routing = new RoutingObject {
            Url = a2.Url.StartsWith("/", StringComparison.Ordinal) ? a2.Url : a1.Url + a2.Url,
            ControllerType = t.GetTypeInfo(),
            MethodInfo = m,
            Methods = m.GetMyAttributes<HttpMethodAttribute>()
        };

        if( routing.Url.IndexOfIgnoreCase("[controller]") > 0 ) {

            string name = t.Name;
            if( name.EndsWithIgnoreCase("Controller") ) {

                name = name.Substring(0, name.Length - 10);
                routing.Url = routing.Url.Replace("[controller]", name);
            }
        }

        if( routing.Url.IndexOf('{') >= 0 )
            routing.UrlRegex = RegexUtils.CreateRouteRegex(routing.Url);

        return routing;
    }


}
