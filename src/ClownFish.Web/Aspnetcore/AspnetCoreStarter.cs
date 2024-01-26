﻿namespace ClownFish.Web.Aspnetcore;

/// <summary>
/// 启动asp.netcore的工具类
/// </summary>
public static class AspnetCoreStarter
{
    internal static WebApplication WebApplication { get; private set; }

    public static void Run(WebApplicationStartup startup = null)
    {
        Console2.BeginListen();

        if( startup == null )
            startup = new WebApplicationStartup();

        ClownFishInit.InitBase();
        ConfigClownFish();

        startup.BeforeFrameworkInit();

        if( startup.AutoInitDAL )
            ClownFishInit.InitDAL();

        if( startup.AutoInitTracing )
            TracingUtils.CheckLogConfig();

        // 监控必须使用日志组件
        if( startup.AutoInitLog || startup.AutoInitTracing )
            ClownFishInit.InitLogAsDefault();

        if( startup.AutoInitAuth )
            ClownFishWebInit.InitAuth();

        TypeHelper.Init();

        CreateWebApp(startup);

        // 初始化经典风格的ASP.NET管道
        InitNHttpApplication();

        // 开启性能监控
        // 放在这里调用，可以监控 ApplicationInit 的执行过程（需要配合 CodeSnippetContext 来实现）
        // 但是这样做也有一个【隐患】：如果在那里 开启后台线程（3种方式），【默认】会导致 OprLogScope 传递到那些后台线程
        if( startup.AutoInitTracing )
            TracingUtils.Init();

        Console2.WriteLine("----------------------- Application Initializer ----------------------------");
        ApplicationInitializer.Execute();
        startup.AppInit();

        WriteDebugReport();
        RunAspnetcore();

        ClownFishInit.ApplicationEnd();
    }

    private static void ConfigClownFish()
    {
        ClownFish.Base.ExceptionExtensions.GetErrorCodeCallbackFunc = GetErrorCode;

        string tempPath = EnvUtils.GetTempPath();
        RetryDirectory.CreateDirectory(tempPath);
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
    internal static WebApplication CreateWebApp(WebApplicationStartup startup = null)
    {
        if( startup == null )
            startup = new WebApplicationStartup();

        WebApplicationBuilder appBuilder = WebApplication.CreateBuilder();

        // 给 Ioc 容器注册组件
        startup.ConfigureServices(appBuilder.Services);

        // 创建 WebApplication 实例，并生成IServiceProvider
        startup.BeforeApplicationBuild(appBuilder);
        WebApplication app = appBuilder.Build();
        startup.AfterApplicationBuild(app);

        // 配置ASP.NET管道
        app.UseMiddleware<FirstModule>();   // 这个太重要，必须固定下来!
        startup.ConfigureWeb(app);

        WebApplication = app;
        return app;
    }

    /// <summary>
    /// 启动asp.netcore的监听，接受HTTP请求
    /// </summary>
    internal static void RunAspnetcore()
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
        Console2.WriteLine("ApplicationName : " + EnvUtils.GetAppName());
        Console2.WriteLine("AppRuntimeId    : " + EnvUtils.AppRuntimeId);
        Console2.WriteLine("AppStartTime    : " + EnvUtils.AppStartTime.ToTimeString());
        Console2.WriteLine("EntryAssembly   : " + Assembly.GetEntryAssembly().Location);
        Console2.WriteLine("EnvironmentName : " + EnvUtils.GetRuntimeEnvName() + "/" + EnvUtils.GetClusterName());
        Console2.WriteLine("ApplicationPath : " + AppContext.BaseDirectory);
        Console2.WriteLine("CurrentDirectory: " + Environment.CurrentDirectory);        
        Console2.WriteLine("TempPath        : " + EnvUtils.GetTempPath());
        Console2.WriteLine("HostName        : " + EnvUtils.GetHostName());
        Console2.WriteLine("TimeZone        : " + MyTimeZone.CurrentTZ);
        Console2.WriteLine("CurrentCulture  : " + System.Globalization.CultureInfo.CurrentCulture?.Name);
        Console2.WriteLine("ClownFishWebVer : " + FileVersionInfo.GetVersionInfo(typeof(AspnetCoreStarter).Assembly.Location).FileVersion);
        Console2.WriteLine("Framework  Info : " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        Console2.WriteLine("OS Name         : " + OsUtils.GetOsName());

        Console2.WriteSeparatedLine();
        Console2.WriteLine("Listening  Urls : " + EnvironmentVariables.Get("ASPNETCORE_URLS") ?? "");
        Console2.WriteLine("Application started. Press Ctrl+C to shut down.");
        Console2.WriteSeparatedLine();

        Console2.EndListen("_app_startup.log");

        // 进入ASP.NET CORE的启动过程
        // 注意：执行下面这行代码后，主线程会被阻塞，直到 Ctrl+C

        WebApplication.Run();

        // 注意：这后面的代码将不会立即执行！
    }

    internal static void InitNHttpApplication()
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
        if( LoggingOptions.HttpActionEnableLog ) {
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

        if( LocalSettings.GetBool("ClownFish_ExecHttpUiModule_Enable") ) {
            NHttpModuleFactory.RegisterModule<ExecHttpUiModule>();
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




    private static void WriteDebugReport()
    {
        if( LocalSettings.GetBool("CreateDebugReport_AtAppStartup", 1) == false )
            return;


        // 获取所有的诊断信息，并写入到临时文件中
        string text = DebugReport.GetReport("ALL");
        string filePath = Path.Combine(EnvUtils.GetTempPath(), "DebugReport.txt");
        RetryFile.WriteAllText(filePath, text);
    }
}
