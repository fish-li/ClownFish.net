using System.Diagnostics.CodeAnalysis;
using ClownFish.Web.Aspnetcore.ActionResults;
using ClownFish.Web.Attributes;

namespace ClownFish.Web.Aspnetcore;

// 主要用于AOT模式下提供基本的 WebApi 功能支持，
// 为了简化实现，必须满足以下要求：
// 1. Controller 类型必须标记 [WebApi]，
// 2. Action方法签名必须是：public Task/Task<xx> ActionName(NHttpContext httpContext)

internal class WebApiActionInfo : IWebApiActionInfo
{
    public Type ControllerType { get; set; }
    public object Controller { get; set; }  // 缓存字典中不赋值
    public MethodInfo MethodInfo { get; set; }
    public string[] HttpMethods { get; set; }
    public UrlRouteAttribute Attribute { get; set; }
    public Regex RouteRegex { get; set; }
    public bool IsLoginAction { get; set; }
    public bool IsLogRequestBody { get; set; }
    public bool AllowExecute { get; set; }
}

internal static class MiniWebApiUtil
{
    private static readonly Dictionary<string, WebApiActionInfo> s_urlMapDict = new(300, StringComparer.OrdinalIgnoreCase);
    private static readonly List<WebApiActionInfo> s_regexRouteList = new(100);

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WebApiActionInfo))]
    public static void Init()
    {
        BuildRouteDict();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2075: controllerType.GetMethods")]
    private static void BuildRouteDict()
    {
        HashSet<string> allRoute = new HashSet<string>(128, StringComparer.OrdinalIgnoreCase);

        foreach( Assembly asm in AppPartUtils.GetApplicationPartAsmList() ) {

            Type[] types = (from x in asm.GetPublicTypes()
                            where x.IsClass && x.IsAbstract == false && x.GetCustomAttribute<WebApiAttribute>() != null
                            select x).ToArray();

            foreach( Type controllerType in types ) {

                MethodInfo[] methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach( MethodInfo method in methods ) {

                    var attrs = method.GetCustomAttributes<UrlRouteAttribute>();
                    foreach( UrlRouteAttribute attr in attrs ) {
                        if( attr != null && attr.Route.HasValue() ) {

                            if( allRoute.Add(attr.Route) == false )
                                // 不支持多个 http-Method 共用一个URL，不太喜欢这样做法！
                                throw new InvalidCodeException($"重复的路由URL: [{attr.Route}]");

                            WebApiActionInfo actionInfo = new WebApiActionInfo {
                                ControllerType = controllerType,
                                MethodInfo = method,
                                Attribute = attr
                            };

                            actionInfo.IsLoginAction = LoginActionAttribute.CurrentIsLogin(method);
                            actionInfo.IsLogRequestBody = actionInfo.GetActionAttribute<LogRequestBodyAttribute>() != null;
                            actionInfo.AllowExecute = OnlyTestEnvAttribute.CurrentIsAllow(controllerType);

                            var httpMethods = method.GetCustomAttributes<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>();
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

        Console2.Info($"{typeof(MiniWebApiUtil).FullName}: BuildRouteDict, found {s_urlMapDict.Count + s_regexRouteList.Count} actions");
    }


    public static WebApiActionInfo FindAction(NHttpContext httpContext)
    {
        string path = httpContext.Request.Path;

        WebApiActionInfo actionInfo = s_urlMapDict.TryGetValue(path, out var value) ? value : null;
        if( actionInfo == null ) {

            foreach( WebApiActionInfo item in s_regexRouteList ) {
                Match m = item.RouteRegex.Match(path);
                if( m.Success ) {
                    actionInfo = item;
                    httpContext.Request.SetRouteResult(m);
                    break;
                }
            }
        }

        return actionInfo;
    }


    [UnconditionalSuppressMessage("Trimming", "IL2072: Activator.CreateInstance")]
    public static void SetHttpContextAction(NHttpContext httpContext, WebApiActionInfo actionInfo)
    {
        object instance = Activator.CreateInstance(actionInfo.ControllerType);

        if( instance is IDisposable disposable ) {
            httpContext.RegisterForDispose(disposable);
        }

        WebApiActionInfo action2 = new WebApiActionInfo {  // 不修改缓存中的实例，避免线程安全问题
            ControllerType = actionInfo.ControllerType,
            MethodInfo = actionInfo.MethodInfo,
            HttpMethods = actionInfo.HttpMethods,
            Attribute = actionInfo.Attribute,
            RouteRegex = actionInfo.RouteRegex,
            IsLoginAction = actionInfo.IsLoginAction,
            IsLogRequestBody = actionInfo.IsLogRequestBody,
            AllowExecute = actionInfo.AllowExecute,
            Controller = instance
        };

        // 登录请求一定不允许记录请求体，不管有没有 [LogRequestBody] 标记！
        if( actionInfo.IsLoginAction ) {
            httpContext.LogRequestBody = false;
        }
        else {
            // 非登录请求，并且【明确】要求记录请体
            if( actionInfo.IsLogRequestBody ) {
                httpContext.LogRequestBody = true;
            }
        }

        httpContext.PipelineContext.SetAction(action2, action2.IsLoginAction);
    }


    [UnconditionalSuppressMessage("Trimming", "IL2026: CallMethod")]
    [UnconditionalSuppressMessage("Trimming", "IL2077: CallMethod")]
    [UnconditionalSuppressMessage("Trimming", "IL2072: Activator.CreateInstance")]
    public static async Task ExecuteActionAsync(NHttpContext httpContext)
    {
        WebApiActionInfo actionInfo = (WebApiActionInfo)httpContext.PipelineContext.Action;

        if( actionInfo.AllowExecute == false ) {
            await httpContext.HttpReplyAsync(405, "当前Action不允许在此环境中执行！");
            return;
        }

        if( CheckHttpMethod(httpContext, actionInfo) == false ) {
            await httpContext.HttpReplyAsync(405, "HttpMethod与Action申明的调用方法不匹配！");
            return;
        }


        object[] args = GetCallArgs(actionInfo, httpContext, out string argsError);

        if( argsError.HasValue() ) {
            await httpContext.HttpReplyAsync(400, argsError);
            return;
        }

        object result = null;
        httpContext.BeginExecuteTime = DateTime.Now;
        httpContext.LogFxEvent(new NameTime("UserCode begin", httpContext.BeginExecuteTime));

        try {
            if( actionInfo.Controller is IControllerInit handler ) {
                handler.Init(httpContext);
            }

            result = await ReflectionUtils.CallMethod(actionInfo.Controller, actionInfo.MethodInfo, args);
        }
        finally {
            httpContext.EndExecuteTime = DateTime.Now;
            httpContext.LogFxEvent(new NameTime("UserCode end", httpContext.EndExecuteTime));
        }

        await OutputResultAsync(httpContext, result);
    }



    private static bool CheckHttpMethod(NHttpContext httpContext, WebApiActionInfo actionInfo)
    {
        string[] methods = actionInfo.HttpMethods;
        if( methods.IsNullOrEmpty() )
            return true;

        string current = httpContext.Request.HttpMethod;
        return methods.Contains(current);
    }


    private static object[] GetCallArgs(WebApiActionInfo actionInfo, NHttpContext httpContext, out string error)
    {
        error = null;

        var ps = actionInfo.MethodInfo.GetParameters();
        if( ps.Length == 0 )
            return Empty.Array<object>();

        if( ps.Length == 1 && ps[0].ParameterType == typeof(NHttpContext) )
            return new object[] { httpContext };


        List<object> list = new List<object>();

        foreach( var p in ps ) {
            
            if( GetOneCallArgs(p, httpContext, list, out string err2) == false ) {
                error = err2;
                return null;     // ############################### 只要发现一个错误，就结束整个方法
            }
        }

        return list.ToArray();
    }


    private static bool GetOneCallArgs(ParameterInfo p, NHttpContext httpContext, List<object> list, out string error)
    {
        error = null;

        if( p.ParameterType == typeof(NHttpContext) ) {
            list.Add(httpContext);
            return true;
        }

        if( p.ParameterType == typeof(NHttpRequest) ) {
            list.Add(httpContext.Request);
            return true;
        }

        
        if( p.ParameterType == typeof(string) ) {

            // 一个特殊的名称，**仅用于** 读取“请求体”         // 查询字符串也要使用这个参数名 ？？不支持！
            if( p.Name == "requestBody" ) {
                string body = httpContext.Request.GetBodyText();
                list.Add(body);
                return true;
            }
            else {
                string text = httpContext.Request.GetValue(p.Name);
                if( text.IsNullOrEmpty() ) {
                    RequiredAttribute attr = p.GetCustomAttribute<RequiredAttribute>();   // 字符串【必填】
                    if( attr != null ) {
                        error = $"没有为Action参数指定调用值，Parameter-Name={p.Name}";
                        return false;     // ############################### 自动获取参数值失败
                    }
                }
                list.Add(text);
                return true;
            }
        }


        if( p.ParameterType.IsNullableType() ) {  // 支持 int? 这种可空类型参数
            Type paramType = p.ParameterType.GetRealType();

            string text = httpContext.Request.GetValue(p.Name);  // 允许 text is null
            object value = StringConverter.ChangeType(text, paramType);
            list.Add(value);
            return true;
        }


        if( p.ParameterType.IsSimpleValueType() ) {
            string text = httpContext.Request.GetValue(p.Name);

            // **空字符串** 直接转值类型的结果很不靠谱，典型场景：DateTime
            // 就算参数类型是 int 也不行，因为这种情况可将参数类型申明为 int?  ，这才是理想的目标类型
            if( text.IsNullOrEmpty() ) {
                error = $"没有为Action参数指定调用值，Parameter-Name={p.Name}";
                return false;     // ############################### 自动获取参数值失败
            }

            object value = StringConverter.ChangeType(text, p.ParameterType);
            list.Add(value);
            return true;
        }

        // 剩下的类型，应该是一些复杂类型的参数，为了简化实现，暂时不支持自动绑定参数
        // 实际应用时可访问 Request 对象 ，然后自行反序列化或者其它的转换处理
        error = $"不支持为Action参数准备调用数据(数据类型不支持)，Parameter-Name={p.Name}, Parameter-Type={p.ParameterType.FullName}";
        return false;
    }


    private static async Task OutputResultAsync(NHttpContext httpContext, object result)
    {
        if( result == null )
            return;

        if( result is string str ) {
            await httpContext.HttpReplyAsync(str);
            return;
        }

        if( result is DataTable table ) {
            DataTableResult result2 = new DataTableResult(table, "xml");
            await result2.OutResultAsync(httpContext);
            return;
        }

        if( result is IWebApiResult actionResult ) {
            await actionResult.OutResultAsync(httpContext);
            return;
        }

        if( result.GetType().IsSimpleValueType() ) {
            await httpContext.HttpReplyAsync(result.ToString());
            return;
        }

        await httpContext.HttpJsonReplyAsync(result);
    }


}




