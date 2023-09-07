namespace ClownFish.WebApi.Routing;

internal static class RoutingManager
{
    private static readonly object s_lock = new object();
    private static bool s_inited = false;

    /// <summary>
    /// 直接可以做URL映射查找的字典表
    /// </summary>
    private static readonly Dictionary<string, List<RoutingObject>> s_urlDict = new Dictionary<string, List<RoutingObject>>(1024, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 需要做正则比较的路由规则表
    /// </summary>
    private static readonly List<RoutingObject> s_regexList = new List<RoutingObject>(1024);



    public static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {
                    LoadRoutes();
                    s_inited = true;
                }
            }
        }
    }


    private static void LoadRoutes()
    {
        List<Assembly> assemblies = AsmHelper.GetAssemblyList<ControllerAssemblyAttribute>();

        foreach( Assembly asm in assemblies ) {

            foreach( Type t in asm.GetPublicTypes() ) {

                RouteAttribute a1 = t.GetMyAttribute<RouteAttribute>();
                if( a1 != null )
                    LoadActions(t, a1);
            }
        }
    }


    private static void LoadActions(Type t, RouteAttribute a1)
    {
        // 只查找公开的实例方法
        var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        foreach( MethodInfo m in methods ) {

            if( m.GetMyAttribute<NonActionAttribute>() != null )
                continue;

            RouteAttribute a2 = m.GetMyAttribute<RouteAttribute>();
            if( a2 == null || a2.Url.IsNullOrEmpty() )
                continue;


            RoutingObject routing = RouteHelper.CreateRoutingObject(t, m, a1, a2);


            if( routing.UrlRegex != null ) {
                s_regexList.Add(routing);
            }
            else {
                List<RoutingObject> list = s_urlDict.TryGet(routing.Url);
                if( list == null ) {
                    list = new List<RoutingObject>();
                    list.Add(routing);
                    s_urlDict[routing.Url] = list;
                }
                else {
                    list.Add(routing);
                }
            }
        }
    }



    public static RoutingObject FindAction(NHttpContext httpContext)
    {
        string method = httpContext.Request.HttpMethod;
        string path = httpContext.Request.Path;

        List<RoutingObject> list = s_urlDict.TryGet(path);
        if( list != null ) {

            foreach( var x in list ) {

                if( x.IsMatchMethod(method) )
                    return x;
            }

            // 只要是进入URL字典匹配，如果失败就直接返回，不再按查找正则表达式列表
            return null;
        }


        // 遍历所有正则表达式，寻找合适的匹配
        foreach( RoutingObject x in s_regexList ) {
            if( IsMatchRequest(method, path, x) )
                return x;
        }

        return null;
    }


    private static bool IsMatchRequest(string method, string path, RoutingObject route)
    {
        Match match = route.UrlRegex.Match(path);
        if( match.Success == false )
            return false;

        return route.IsMatchMethod(method);
    }





}
