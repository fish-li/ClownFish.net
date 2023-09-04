#if NET6_0_OR_GREATER
namespace ClownFish.Web.Aspnetcore;

/// <summary>
/// 启动asp.netcore的工具类
/// </summary>
public static class AspnetCoreStarter
{
    internal static WebApplication WebApplication { get; private set; }

    /// <summary>
    /// 创建WebApplication实例
    /// </summary>
    /// <param name="startup"></param>
    /// <returns></returns>
    public static WebApplication CreateWebApp(WebApplicationStartup startup = null)
    {
        if( startup == null )
            startup = new WebApplicationStartup();

        // 供 ClownFish.Tracing 判断
        MemoryConfig.AddSetting(ConstNames.AspnetCoreStarterName, typeof(AspnetCoreStarter).FullName);

        WebApplicationBuilder appBuilder = WebApplication.CreateBuilder();

        // 给 Ioc 容器注册组件
        startup.ConfigureServices(appBuilder.Services);

        // 创建 WebApplication 实例，并生成IServiceProvider
        WebApplication app = appBuilder.Build();

        // 配置ASP.NET管道
        app.UseMiddleware<SimpleSpacerModule>();   // 这个太重要，必须固定下来!
        startup.ConfigureWeb(app);

        startup.RegisterHttpModules();

        WebApplication = app;        
        return app;
    }

    /// <summary>
    /// 启动asp.netcore的监听，接受HTTP请求
    /// </summary>
    public static void Run()
    {
        InitHttpApplication();

        if( LocalSettings.GetBool("ClownFish_Aspnet_ShowHttpModules") ) {
            Console2.WriteSeparatedLine();
            foreach( Type t in NHttpModuleFactory.GetList() ) {
                Console2.WriteLine("NHttpModule: " + t.FullName);
            }
        }


        // 为什么要在这里修改 Console2.InfoEnabled 的设置？
        // 因为：如果直接在 Console2的静态构造方法中就读取 LocalSettings，会导致这个开关一直关闭，
        // 那么程序在启动时的调用就会被忽略，一些重要的初始化消息就看不到了~~~
        // 所以这里放在这里在关闭开关，可以确保初始过程中的消息能被输出。

        if( LocalSettings.GetInt("ClownFish_Console2_Info_Enabled", 1) == 0 ) {
            Console2.Info("##### 由于设置了 ClownFish_Console2_Info_Enabled=0，Console2.Info() 方法的调用即将被禁用！");
            Console2.InfoEnabled = false;
        }


        Console2.WriteSeparatedLine();
        Console2.WriteLine("EnvironmentName: " + EnvUtils.EnvName);
        Console2.WriteLine("ApplicationName: " + EnvUtils.ApplicationName);
        Console2.WriteLine("ClownFish.Web Ver: " + FileVersionInfo.GetVersionInfo(typeof(AspnetCoreStarter).Assembly.Location).FileVersion);
        Console2.WriteLine("FrameworkDescription: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        Console2.WriteLine("Now listening on: " + EnvironmentVariables.Get("ASPNETCORE_URLS") ?? "http://0.0.0.0:80");


        Console2.WriteLine("Application started. Press Ctrl+C to shut down.");
        Console2.WriteSeparatedLine();

        // 进入ASP.NET CORE的启动过程
        // 注意：执行下面这行代码后，主线程会被阻塞，直到 Ctrl+C

        WebApplication.Run();

        // 注意：这后面的代码将不会立即执行！
    }

    private static void InitHttpApplication()
    {
        // 加载HTTP模块
        LoadModules();

        // 启动 HTTP管线
        NHttpApplication.Start(WorkMode.AspnetCore);
    }


    private static void LoadModules()
    {
        // 先注册框架内部的Http模块
        if( LoggingOptions.HttpActionEnableLog ) {
            NHttpModuleFactory.RegisterModule<OprLogModule>();
        }

        NHttpModuleFactory.RegisterModule<ExceptionModule>();


        // 搜索当前应用中的Http模块并注册
        Type[] types = (from x in Assembly.GetEntryAssembly().GetPublicTypes()
                        where x.IsClass && x.IsAbstract == false && x.IsSubclassOf(typeof(NHttpModule))
                        select x).ToArray();

        foreach( Type t in types ) {
            NHttpModuleFactory.RegisterModule(t);
        }
    }

}


#endif