using System.Diagnostics.CodeAnalysis;
using ClownFish.Web.Aspnetcore.ActionResults;
using ClownFish.Web.Attributes;

namespace ClownFish.Web.Modules;

// ApiModule 主要用于AOT模式下提供基本的 WebApi 功能支持，
// 为了简化实现，必须满足以下要求：
// 1. Controller 类型必须标记 [WebApiController]，
// 2. Action方法签名必须是：public Task/Task<xx> ActionName(NHttpContext httpContext)

public sealed class SlimWebApiModule : NHttpModule
{
    private class ApiActionInfo
    {
        public Type ControllerType { get; set; }
        public MethodInfo Method { get; set; }
        public UrlRouteAttribute Attribute { get; set; }
        public Regex RouteRegex { get; set; }
    }

    private static readonly Dictionary<string, ApiActionInfo> s_urlMapDict = new(300, StringComparer.OrdinalIgnoreCase);
    private static readonly List<ApiActionInfo> s_regexRouteList = new(100);

    public override void Init()
    {
        BuildRouteDict();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2075: controllerType.GetMethods")]
    private static void BuildRouteDict()
    {
        foreach( Assembly asm in AppPartUtils.GetApplicationPartAsmList() ) {

            Type[] types = (from x in asm.GetPublicTypes()
                            where x.IsClass && x.IsAbstract == false && x.GetCustomAttribute<WebApiAttribute>() != null
                            select x).ToArray();

            foreach( Type controllerType in types ) {

                MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach( MethodInfo method in methods ) {

                    // 检查方法签名是否符合要求: Task/Task<xx> ActionName(NHttpContext httpContext)

                    if( method.IsTaskMethod() == false )
                        continue;

                    if( method.GetParameters().Length != 1 || method.GetParameters()[0].ParameterType != typeof(NHttpContext) )
                        continue;

                    var attrs = method.GetCustomAttributes<UrlRouteAttribute>();
                    foreach( UrlRouteAttribute attr in attrs ) {
                        if( attr != null && attr.Route.HasValue() ) {

                            ApiActionInfo actionInfo = new ApiActionInfo {
                                ControllerType = controllerType,
                                Method = method,
                                Attribute = attr
                            };

                            if( RegexUtils.HasRouteName(attr.Route) ) {
                                actionInfo.RouteRegex = RegexUtils.CreateRouteRegex(attr.Route);

                                s_regexRouteList.Add(actionInfo);
                            }
                            else {
                                s_urlMapDict[attr.Route] = actionInfo;
                            }
                        }
                    }
                }
            }
        }

        Console2.Info($"ClownFish.Web SlimWebApiModule: BuildRouteDict, found {s_urlMapDict.Count + s_regexRouteList.Count} actions");
    }


    public override void MapRequestHandler(NHttpContext httpContext)
    {
        if( httpContext.PipelineContext.Action != null )
            return;

        string path = httpContext.Request.Path;

        ApiActionInfo actionInfo = s_urlMapDict.TryGetValue(path, out var value) ? value : null;
        if( actionInfo == null ) {

            foreach( ApiActionInfo item in s_regexRouteList ) {
                Match m = item.RouteRegex.Match(path);
                if( m.Success ) {
                    actionInfo = item;
                    httpContext.Request.SetRouteResult(m);
                    break;
                }
            }
        }

        if( actionInfo != null ) {
            ApiActionHandler handler = new ApiActionHandler(actionInfo.ControllerType, actionInfo.Method);
            httpContext.PipelineContext.SetHttpHandler(handler);
        }

    }
}


internal class ApiActionHandler : IAsyncNHttpHandler
{
    private readonly Type _controllerType;
    private readonly MethodInfo _method;

    public ApiActionHandler(Type controllerType, MethodInfo method)
    {
        _controllerType = controllerType;
        _method = method;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026: CallMethod")]
    [UnconditionalSuppressMessage("Trimming", "IL2077: CallMethod")]
    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        object instance = Activator.CreateInstance(_controllerType);

        IDisposable disposable = (instance as IDisposable) ?? NullDisposable.Instance;

        using( disposable ) {
            object result = await ReflectionUtils.CallMethod(instance, _method, new object[] { httpContext });

            await OutputResultAsync(httpContext, result);
        }
    }

    private async Task OutputResultAsync(NHttpContext httpContext, object result)
    {
        if( result == null )
            return;

        if( result is string str ) {
            await httpContext.HttpReplyAsync(str);
            return;
        }

        if( result.GetType().IsSimpleValueType() ) {
            await httpContext.HttpReplyAsync(result.ToString());
        }

        if( result is IOutActionResult actionResult ) {
            await actionResult.OutResultAsync(httpContext);
            return;
        }

        await httpContext.HttpJsonReplyAsync(result);
    }

    private sealed class NullDisposable : IDisposable
    {
        internal static readonly NullDisposable Instance = new NullDisposable();
        public void Dispose()
        {
            // 什么都不做
        }
    }
}

