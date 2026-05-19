//using System.Diagnostics.CodeAnalysis;
//using ClownFish.Web.Aspnetcore.ActionResults;
//using ClownFish.Web.Attributes;
//using Microsoft.AspNetCore.Mvc.Routing;

//namespace ClownFish.Web.Modules;

//// SlimWebApiModule 主要用于AOT模式下提供基本的 WebApi 功能支持，
//// 为了简化实现，必须满足以下要求：
//// 1. Controller 类型必须标记 [WebApi]，
//// 2. Action方法签名必须是：public Task/Task<xx> ActionName(NHttpContext httpContext)


//internal sealed class SlimWebApiModule : NHttpModule
//{
//    private class WebApiActionInfo : IWebApiActionInfo
//    {
//        public Type ControllerType { get; set; }
//        public object Controller { get; set; }  // 缓存字典中不赋值
//        public MethodInfo MethodInfo { get; set; }
//        public string[] HttpMethods { get; set; }
//        public UrlRouteAttribute Attribute { get; set; }
//        public Regex RouteRegex { get; set; }
//    }

//    private static readonly Dictionary<string, WebApiActionInfo> s_urlMapDict = new(300, StringComparer.OrdinalIgnoreCase);
//    private static readonly List<WebApiActionInfo> s_regexRouteList = new(100);

//    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WebApiActionInfo))]
//    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ApiActionHandler))]
//    public override void Init()
//    {
//        BuildRouteDict();
//    }

//    [UnconditionalSuppressMessage("Trimming", "IL2075: controllerType.GetMethods")]
//    private static void BuildRouteDict()
//    {
//        foreach( Assembly asm in AppPartUtils.GetApplicationPartAsmList() ) {

//            Type[] types = (from x in asm.GetPublicTypes()
//                            where x.IsClass && x.IsAbstract == false && x.GetCustomAttribute<WebApiAttribute>() != null
//                            select x).ToArray();

//            foreach( Type controllerType in types ) {

//                MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
//                foreach( MethodInfo method in methods ) {

//                    // 检查方法签名是否符合要求: Task/Task<xx> ActionName(NHttpContext httpContext)

//                    if( CheckActionMethodReturnType(method) == false )
//                        continue;

//                    if( CheckActionMethodParameter(method) == false )
//                        continue;

//                    var attrs = method.GetCustomAttributes<UrlRouteAttribute>();
//                    foreach( UrlRouteAttribute attr in attrs ) {
//                        if( attr != null && attr.Route.HasValue() ) {

//                            WebApiActionInfo actionInfo = new WebApiActionInfo {
//                                ControllerType = controllerType,
//                                MethodInfo = method,
//                                Attribute = attr
//                            };

//                            var httpMethods = method.GetCustomAttributes<HttpMethodAttribute>();
//                            actionInfo.HttpMethods = (from x in httpMethods
//                                                      let m = x.HttpMethods.First()
//                                                      select m
//                                                      ).ToArray();

//                            if( RegexUtils.HasRouteName(attr.Route) ) {
//                                actionInfo.RouteRegex = RegexUtils.CreateRouteRegex(attr.Route);
//                                s_regexRouteList.Add(actionInfo);
//                            }
//                            else {
//                                s_urlMapDict[attr.Route] = actionInfo;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        Console2.Info($"{typeof(SlimWebApiModule).FullName}: BuildRouteDict, found {s_urlMapDict.Count + s_regexRouteList.Count} actions");
//    }


//    private static bool CheckActionMethodReturnType(MethodInfo method)
//    {
//        if( method.IsTaskMethod() )
//            return true;

//        Type returnType = method.ReturnType;

//        if( returnType.IsCompatible(typeof(IWebApiResult))
//            || returnType.IsSimpleValueType()
//            || returnType == typeof(string)
//            || returnType == typeof(object)
//            || returnType == typeof(void) )
//            return true;

//        return false;
//    }

//    private static bool CheckActionMethodParameter(MethodInfo method)
//    {
//        var ps = method.GetParameters();
//        if( ps.Length == 0 )
//            return true;

//        return ps.Length == 1 && ps[0].ParameterType == typeof(NHttpContext);
//    }

//    [UnconditionalSuppressMessage("Trimming", "IL2075: controllerType.GetMethod")]
//    public override void MapRequestHandler(NHttpContext httpContext)
//    {
//        if( httpContext.PipelineContext.Action != null )
//            return;

//        WebApiActionInfo actionInfo = GetAction(httpContext);
//        if( actionInfo == null )
//            return;

//        bool allowed = OnlyTestEnvAttribute.CurrentIsAllow(actionInfo.ControllerType);
//        if( allowed == false ) {
//            return;
//        }

//        bool isLogin = LoginActionAttribute.CurrentIsLogin(actionInfo.MethodInfo);
//        // 登录请求一定不允许记录请求体，不管有没有 [LogRequestBody] 标记！
//        if( isLogin ) {
//            httpContext.LogRequestBody = false;
//        }
//        else {
//            // 非登录请求，并且【明确】要求记录请体
//            if( actionInfo.GetActionAttribute<LogRequestBodyAttribute>() != null ) {
//                httpContext.LogRequestBody = true;
//            }
//        }

//        ApiActionHandler handler = new ApiActionHandler(actionInfo);
//        //httpContext.PipelineContext.SetHttpHandler(handler);

//        Type controllerType = handler.GetType();
//        MethodInfo method = controllerType.GetMethod(nameof(ApiActionHandler.ProcessRequestAsync),
//                                                    BindingFlags.Instance | BindingFlags.Public, null,
//                                                    s_call_args_types1, null);

//        ActionDescription actionInfo2 = new ActionDescription(handler, method, controllerType);
//        httpContext.PipelineContext.SetAction(actionInfo2, isLogin);
//    }

//    private static readonly Type[] s_call_args_types1 = new Type[] { typeof(NHttpContext) };

//    private static WebApiActionInfo GetAction(NHttpContext httpContext)
//    {
//        string path = httpContext.Request.Path;

//        WebApiActionInfo actionInfo = s_urlMapDict.TryGetValue(path, out var value) ? value : null;
//        if( actionInfo == null ) {

//            foreach( WebApiActionInfo item in s_regexRouteList ) {
//                Match m = item.RouteRegex.Match(path);
//                if( m.Success ) {
//                    actionInfo = item;
//                    httpContext.Request.SetRouteResult(m);
//                    break;
//                }
//            }
//        }

//        return actionInfo;
//    }



//    private class ApiActionHandler : IAsyncNHttpHandler, IGetActionInfo
//    {
//        private readonly WebApiActionInfo _actionInfo;

//        public ApiActionHandler(WebApiActionInfo actionInfo)
//        {
//            _actionInfo = actionInfo;
//        }

//        IWebApiActionInfo IGetActionInfo.GetActionInfo() => _actionInfo;


//        [UnconditionalSuppressMessage("Trimming", "IL2026: CallMethod")]
//        [UnconditionalSuppressMessage("Trimming", "IL2077: CallMethod")]
//        [UnconditionalSuppressMessage("Trimming", "IL2072: Activator.CreateInstance")]
//        public async Task ProcessRequestAsync(NHttpContext httpContext)
//        {
//            if( CheckHttpMethod(httpContext, _actionInfo) == false ) {
//                await httpContext.HttpReplyAsync(405, "HttpMethod与Action申明的调用方法不匹配！");
//                return;
//            }

//            if( AuthorizeModule.AuthorizeCheck(httpContext, _actionInfo) == false ) {
//                await httpContext.HttpReplyAsync(403, "Unauthorized");
//                return;
//            }

//            object instance = Activator.CreateInstance(_actionInfo.ControllerType);

//            if( instance is IDisposable disposable ) {
//                httpContext.RegisterForDispose(disposable);
//            }

//            if( instance is IControllerInit handler ) {
//                handler.Init(httpContext);
//            }

//            OprLogScope scope = httpContext.PipelineContext.OprLogScope;
//            if( scope.CanLog ) {
//                OprLog oprLog = scope.OprLog;
//                oprLog.Module = _actionInfo.ControllerType.Namespace;
//                oprLog.Controller = _actionInfo.ControllerType.Name;
//                oprLog.Action = _actionInfo.MethodInfo.Name;
//            }


//            object[] args = _actionInfo.MethodInfo.GetParameters().Length == 0 ? Array.Empty<object>() : new object[] { httpContext };
//            object result = await ReflectionUtils.CallMethod(instance, _actionInfo.MethodInfo, args);

//            await OutputResultAsync(httpContext, result);
//        }

//        private bool CheckHttpMethod(NHttpContext httpContext, WebApiActionInfo actionInfo)
//        {
//            string[] methods = actionInfo.HttpMethods;
//            if( methods.IsNullOrEmpty() )
//                return true;

//            string current = httpContext.Request.HttpMethod;
//            return methods.Contains(current);
//        }

//        private async Task OutputResultAsync(NHttpContext httpContext, object result)
//        {
//            if( result == null )
//                return;

//            if( result is string str ) {
//                await httpContext.HttpReplyAsync(str);
//                return;
//            }

//            if( result.GetType().IsSimpleValueType() ) {
//                await httpContext.HttpReplyAsync(result.ToString());
//                return;
//            }

//            if( result is IWebApiResult actionResult ) {
//                await actionResult.OutResultAsync(httpContext);
//                return;
//            }

//            await httpContext.HttpJsonReplyAsync(result);
//        }
//    }

//}