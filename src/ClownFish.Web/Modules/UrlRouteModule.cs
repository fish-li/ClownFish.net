using System.Diagnostics.CodeAnalysis;
using ClownFish.Web.Attributes;

namespace ClownFish.Web.Modules;

public sealed class UrlRouteModule : NHttpModule
{
    private static readonly Dictionary<string, HandlerInfo> s_urlMapDict = new(100, StringComparer.OrdinalIgnoreCase);
    private static readonly List<HandlerInfo> s_regexRouteList = new(100);

    private class HandlerInfo
    {
        public UrlRouteAttribute Attribute { get; set; }
        public Regex RouteRegex { get; set; }

        public Type HandlerType { get; set; }
        public IAsyncNHttpHandler StaicInstance { get; set; }
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HandlerInfo))]
    public override void Init()
    {
        BuildRouteDict();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2075: handlerType.GetField")]
    [UnconditionalSuppressMessage("Trimming", "IL2065: handlerType.GetField")]
    private static void BuildRouteDict()
    {
        foreach( Assembly asm in AppPartUtils.GetApplicationPartAsmList() ) {

            Type[] types = (from x in asm.GetPublicTypes()
                            where x.IsClass && x.IsAbstract == false && x.IsCompatible(typeof(IAsyncNHttpHandler))
                            select x).ToArray();

            foreach( Type handlerType in types ) {

                IAsyncNHttpHandler singleInstance = null;
                bool flag = false;

                var attrs = handlerType.GetCustomAttributes<UrlRouteAttribute>();
                foreach( UrlRouteAttribute attr in attrs ) {
                    if( attr != null && attr.Route.HasValue() ) {

                        if( flag == false ) {
                            FieldInfo field = handlerType.GetField("Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            if( field != null && field.GetValue(null) is IAsyncNHttpHandler handler ) {
                                singleInstance = handler;
                            }
                            flag = true;
                        }

                        HandlerInfo handlerInfo = new HandlerInfo();
                        handlerInfo.HandlerType = handlerType;
                        handlerInfo.StaicInstance = singleInstance;

                        if( RegexUtils.HasRouteName(attr.Route) ) {
                            handlerInfo.RouteRegex = RegexUtils.CreateRouteRegex(attr.Route);

                            s_regexRouteList.Add(handlerInfo);
                        }
                        else {
                            s_urlMapDict[attr.Route] = handlerInfo;
                        }
                    }
                }
            }
        }

        Console2.Info($"{typeof(UrlRouteModule).FullName}: BuildRouteDict, found {s_urlMapDict.Count + s_regexRouteList.Count} handlers");
    }


    [UnconditionalSuppressMessage("Trimming", "IL2026: type.FastNew")]
    public override void MapRequestHandler(NHttpContext httpContext)
    {
        if( httpContext.PipelineContext.Action != null )
            return;

        string path = httpContext.Request.Path;

        HandlerInfo handlerInfo = s_urlMapDict.TryGetValue(path, out var value) ? value : null;
        if( handlerInfo == null ) {

            foreach( HandlerInfo item in s_regexRouteList ) {
                Match m = item.RouteRegex.Match(path);
                if( m.Success ) {
                    handlerInfo = item;
                    httpContext.Request.SetRouteResult(m);
                    break;
                }
            }
        }

        if( handlerInfo == null )
            return;

        if( handlerInfo.StaicInstance != null ) {
            httpContext.PipelineContext.SetHttpHandler(handlerInfo.StaicInstance);
            return;
        }

        if( handlerInfo.HandlerType != null ) {
            IAsyncNHttpHandler handler2 = (IAsyncNHttpHandler)handlerInfo.HandlerType.FastNew();
            httpContext.PipelineContext.SetHttpHandler(handler2);
            return;
        }
    }


}

