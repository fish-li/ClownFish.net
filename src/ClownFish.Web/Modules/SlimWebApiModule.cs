using System.Diagnostics.CodeAnalysis;
using ClownFish.Web.Aspnetcore.ActionResults;
using ClownFish.Web.Attributes;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ClownFish.Web.Modules;

// SlimWebApiModule 主要用于AOT模式下提供基本的 WebApi 功能支持，
// 为了简化实现，必须满足以下要求：
// 1. Controller 类型必须标记 [WebApi]，
// 2. Action方法签名必须是：public Task/Task<xx> ActionName(NHttpContext httpContext)




public sealed class SlimWebApiModule : NHttpModule
{
    private class ApiActionInfo : IBaseActionInfo
    {
        public Type ControllerType { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public string[] HttpMethods { get; set; }
        public UrlRouteAttribute Attribute { get; set; }
        public Regex RouteRegex { get; set; }
    }

    private static readonly Dictionary<string, ApiActionInfo> s_urlMapDict = new(300, StringComparer.OrdinalIgnoreCase);
    private static readonly List<ApiActionInfo> s_regexRouteList = new(100);

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ApiActionInfo))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ApiActionHandler))]
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

                    if( CheckActionMethodReturnType(method) == false )
                        continue;

                    if( CheckActionMethodParameter(method) == false )
                        continue;

                    var attrs = method.GetCustomAttributes<UrlRouteAttribute>();
                    foreach( UrlRouteAttribute attr in attrs ) {
                        if( attr != null && attr.Route.HasValue() ) {

                            ApiActionInfo actionInfo = new ApiActionInfo {
                                ControllerType = controllerType,
                                MethodInfo = method,
                                Attribute = attr
                            };

                            var httpMethods = method.GetCustomAttributes<HttpMethodAttribute>();
                            actionInfo.HttpMethods = (from x in httpMethods
                                                      let m = x.HttpMethods.First()
                                                      select m
                                                      ).ToArray();

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


    private static bool CheckActionMethodReturnType(MethodInfo method)
    {
        if( method.IsTaskMethod() )
            return true;

        Type returnType = method.ReturnType;

        if( returnType.IsCompatible(typeof(IWebApiResult))
            || returnType.IsSimpleValueType()
            || returnType == typeof(string)
            || returnType == typeof(object)
            || returnType == typeof(void) )
            return true;

        return false;
    }

    private static bool CheckActionMethodParameter(MethodInfo method)
    {
        var ps = method.GetParameters();
        if( ps.Length == 0 )
            return true;

        return ps.Length == 1 && ps[0].ParameterType == typeof(NHttpContext);
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
            ApiActionHandler handler = new ApiActionHandler(actionInfo);
            httpContext.PipelineContext.SetHttpHandler(handler);
        }
    }



    private class ApiActionHandler : IAsyncNHttpHandler
    {
        private readonly ApiActionInfo _actionInfo;

        public ApiActionHandler(ApiActionInfo actionInfo)
        {
            _actionInfo = actionInfo;
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026: CallMethod")]
        [UnconditionalSuppressMessage("Trimming", "IL2077: CallMethod")]
        [UnconditionalSuppressMessage("Trimming", "IL2072: Activator.CreateInstance")]
        public async Task ProcessRequestAsync(NHttpContext httpContext)
        {
            if( CheckHttpMethod(httpContext, _actionInfo) == false ) {
                await httpContext.HttpReplyAsync(405, "HttpMethod与Action申明的调用方法不匹配！");
                return;
            }

            if( AuthorizeModule.AuthorizeCheck(httpContext, _actionInfo) == false ) {
                return;
            }

            object instance = Activator.CreateInstance(_actionInfo.ControllerType);

            if( instance is IControllerInit handler ) {
                handler.Init(httpContext);
            }

            OprLogScope scope = httpContext.PipelineContext.OprLogScope;
            if( scope.CanLog ) {
                OprLog oprLog = scope.OprLog;
                oprLog.Module = _actionInfo.ControllerType.Namespace;
                oprLog.Controller = _actionInfo.ControllerType.Name;
                oprLog.Action = _actionInfo.MethodInfo.Name;
            }

            IDisposable disposable = (instance as IDisposable) ?? NullDisposable.Instance;

            using( disposable ) {
                object[] args = _actionInfo.MethodInfo.GetParameters().Length == 0 ? Array.Empty<object>() : new object[] { httpContext };
                object result = await ReflectionUtils.CallMethod(instance, _actionInfo.MethodInfo, args);

                await OutputResultAsync(httpContext, result);
            }
        }

        private bool CheckHttpMethod(NHttpContext httpContext, ApiActionInfo actionInfo)
        {
            string[] methods = actionInfo.HttpMethods;
            if( methods.IsNullOrEmpty() )
                return true;

            string current = httpContext.Request.HttpMethod;
            return methods.Contains(current);
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
                return;
            }

            if( result is IWebApiResult actionResult ) {
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

}