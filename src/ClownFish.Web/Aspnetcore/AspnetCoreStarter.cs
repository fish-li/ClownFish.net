using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;

namespace ClownFish.Web.Aspnetcore;

/// <summary>
/// 启动asp.netcore的工具类
/// </summary>
public static class AspnetCoreStarter
{
    internal static WebApplication WebAppInstance { get; private set; }

    public static void Run(WebApplicationStartup startup = null)
    {
        Console2.BeginListen();

        if( startup == null )
            startup = new WebApplicationStartup();

        startup.BeforeClownFishInit();

        ClownFishInit.InitBase();
        ConfigClownFish();
        TypeHelper.Init();

        EnvUtils.ShowSysEnvInfo();

        startup.ConfigDAL0();
        startup.ConfigLog0();
        startup.ConfigAuth0();
        startup.ConfigTracing0();
        startup.AfterClownFishInit();


        startup.BeforeAspnetInit();
        CreateWebApp(startup);
        startup.AfterAspnetInit();

        
        startup.BeforeClownFishWebInit();
        InitNHttpApplication();   // 初始化经典风格的ASP.NET管道
        startup.AfterClownFishWebInit();


        Console2.WriteLine("----------------------- Application Initializer ----------------------------");
        ApplicationInitializer.Execute();
        startup.AppInit();

        DebugReport.WriteAllToFile();

        startup.BeforeRun();
        RunAspnetcore(WebAppInstance);

        ClownFishInit.ApplicationEnd();
        startup.AppEnd();
    }

    private static void ConfigClownFish()
    {
        ClownFish.Base.ExceptionExtensions.GetErrorCodeCallbackFunc = GetErrorCode;
    }

    private static int? GetErrorCode(Exception ex)
    {
        if( ex is Microsoft.AspNetCore.Http.BadHttpRequestException bex )
            return bex.StatusCode;

        return null;
    }


    /// <summary>
    /// 创建WebApplication实例
    /// </summary>
    /// <param name="startup"></param>
    /// <returns></returns>
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NHttpContext))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpPipelineContext))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NHttpApplication))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NHttpRequest))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NHttpResponse))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WebApplicationStartup))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ActionDescription))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Http302Handler))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Http403Handler))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Http404Handler))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HttpXxxHandler))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(StaticFileHandler))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NHttpModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(OprLogModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TransferModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ExceptionModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AuthenticateModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AuthorizeModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UrlRouteModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SlimWebApiModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ExecHttpUiModule))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WebStaticFileModule))]
    internal static WebApplication CreateWebApp(WebApplicationStartup startup = null)
    {
        if( startup == null )
            startup = new WebApplicationStartup();

        DateTime start = DateTime.Now;
        WebApplicationBuilder appBuilder = startup.CreateWebApplicationBuilder();

        if( ClownFishWebOptions.ShutdownTimeoutSeconds > 0 ) {
            appBuilder.Host.ConfigureHostOptions(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(ClownFishWebOptions.ShutdownTimeoutSeconds));
        }

        // 给 Ioc 容器注册组件
        startup.ConfigureServices(appBuilder.Services);

        // 创建 WebApplication 实例，并生成IServiceProvider
        startup.BeforeApplicationBuild(appBuilder);
        WebApplication app = appBuilder.Build();
        startup.AfterApplicationBuild(app);

        // 配置ASP.NET管道
        app.UseMiddleware<FirstModule>();   // 这个太重要，必须固定下来放在第一位!
        startup.ConfigureWeb(app);

        Console2.Info($"ASP.NET WebApplication init OK, execute time: {(DateTime.Now - start)}");

        WebAppInstance = app;
        app.Lifetime.ApplicationStopping.Register(OnAspnetApplicationStopping);
        return app;
    }


    private static void OnAspnetApplicationStopping()
    {
        Console2.WriteLine("--");
        Console2.Info("##### ASP.NET WebApplication stopping ...");
        ClownFishInit.ApplicationEnd();
    }


    /// <summary>
    /// 启动asp.netcore的监听，接受HTTP请求
    /// </summary>
    public static void RunAspnetcore(WebApplication app)
    {
        // 为什么要在这里修改 Console2.InfoEnabled 的设置？
        // 因为：如果直接在 Console2的静态构造方法中就读取 LocalSettings，会导致这个开关一直关闭，
        // 那么程序在启动时的调用就会被忽略，一些重要的初始化消息就看不到了~~~
        // 所以这里放在这里在关闭开关，可以确保初始过程中的消息能被输出。

        if( LocalSettings.GetInt("ClownFish_Console2_Info_Enabled", 1) == 0 ) {
            Console2.Info("##### 由于设置了 ClownFish_Console2_Info_Enabled=0，Console2.Info() 方法的调用即将被禁用！");
            Console2.InfoEnabled = false;
        }


        

        Console2.WriteSeparatedLine();
        Console2.WriteLine("Listening  Urls : " + GetListeningUrls(app));
        Console2.WriteLine("Application started. Press Ctrl+C to shut down.");
        Console2.WriteSeparatedLine();

        Console2.EndListen("_app_startup.log");

        // 进入ASP.NET CORE的启动过程
        // 注意：执行下面这行代码后，主线程会被阻塞，直到 Ctrl+C

        app.Run();

        // 注意：这后面的代码将不会立即执行！
    }

    public static string GetListeningUrls(WebApplication app)
    {
        // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/servers/kestrel/endpoints
        // 由于 .NET 并没有提供一种可以获取监听URL的方法，所以这里的实现也不保证适用于所有场景

        // https://andrewlock.net/8-ways-to-set-the-urls-for-an-aspnetcore-app/
        // 目前总共有 8 种方式可以指定监听地址，非常乱…………

        string url1 = GetKestrelUrl(app);
        if( url1.HasValue() )
            return url1;


        var url4 = app.Urls;
        if( url4 != null && url4.Count > 0 )
            return string.Join(",", url4);


        string url3 = app.Configuration["urls"];
        if( url3.HasValue() )
            return url3;


        string url2 = EnvironmentVariables.Get("ASPNETCORE_URLS");
        if( url2.HasValue() )
            return url2;

        return "";

        string GetKestrelUrl(WebApplication app)
        {
            string url1 = app.Configuration["Kestrel:Endpoints:Http:Url"];
            string url2 = app.Configuration["Kestrel:Endpoints:Https:Url"];

            if( url1.HasValue() && url2.HasValue() )
                return url1 + ";" + url2;

            if( url1.HasValue() )
                return url1;
            if( url2.HasValue() )
                return url2;

            return null;
        }
    }

    public static void InitNHttpApplication()
    {
        // 加载HTTP模块
        LoadModules();

        // 启动 HTTP管线
        NHttpApplication.Start();

        NHttpApplication.Instance.ShowModules(1);
    }

    private static void LoadModules()
    {
        // 先注册框架内部的Http模块
        if( LoggingOptions.HttpActionEnableLog && LogConfig.IsInited ) {
            NHttpModuleFactory.RegisterModule<OprLogModule>();
        }

        if( TransferModule.IsEnable() ) {
            NHttpModuleFactory.RegisterModule<TransferModule>();
        }

        NHttpModuleFactory.RegisterModule<ExceptionModule>();

        if( AuthenticationManager.Inited ) {
            NHttpModuleFactory.RegisterModule<AuthenticateModule>();
            NHttpModuleFactory.RegisterModule<AuthorizeModule>();
        }

        if( LocalSettings.GetBool("ClownFish_UrlRouteModule_Enable") ) {
            NHttpModuleFactory.RegisterModule<UrlRouteModule>();
        }

        if( LocalSettings.GetBool("ClownFish_SlimWebApiModule_Enable") ) {
            NHttpModuleFactory.RegisterModule<SlimWebApiModule>();
        }

        if( LocalSettings.GetBool("ClownFish_ExecHttpUiModule_Enable") ) {
            NHttpModuleFactory.RegisterModule<ExecHttpUiModule>();
        }

        if( LocalSettings.GetBool("ClownFish_WebStaticFileModule_Enable") ) {
            NHttpModuleFactory.RegisterModule<WebStaticFileModule>();
        }

        // 搜索当前应用中的Http模块并注册
        foreach( Assembly asm in AppPartUtils.GetApplicationPartAsmList() ) {
            Type[] types = (from x in asm.GetPublicTypes()
                            where x.IsClass && x.IsAbstract == false && x.IsSubclassOf(typeof(NHttpModule))
                            select x).ToArray();

            foreach( Type t in types ) {
                NHttpModuleFactory.RegisterModule(t);
            }
        }
    }

}
