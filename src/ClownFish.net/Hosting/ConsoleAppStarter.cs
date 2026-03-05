using System.Runtime;

namespace ClownFish.Hosting;

/// <summary>
/// 
/// </summary>
public static class ConsoleAppStarter
{
    //internal static IHost AppHost { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startup"></param>
    public static void Run(ConsoleAppStartup startup = null)
    {
        Console2.BeginListen();

        if( startup == null )
            startup = new ConsoleAppStartup();

        startup.BeforeClownFishInit();

        ClownFishInit.InitBase();
        TypeHelper.Init();

        EnvUtils.ShowSysEnvInfo();

        startup.ConfigDAL0();
        startup.ConfigLog0();
        startup.ConfigTracing0();
        startup.AfterClownFishInit();

        //CreateAppHost(startup);

        Console2.WriteLine("----------------------- Application Initializer ----------------------------");
        ApplicationInitializer.Execute();
        startup.AppInit();

        DebugReport.WriteAllToFile();

        startup.BeforeRun();
        BeforeRun();

#if NET6_0_OR_GREATER
        if( startup.WaitToEnd ) {

            // #################### 注意：执行下面这行代码后，主线程会被阻塞，直到 Ctrl+C  ####################
            //AppHost.Run();
            using( ConsoleEndWaiter waiter = new ConsoleEndWaiter() ) {
                waiter.Wait();
            }
            // #################### 注意：这后面的代码将不会立即执行！  ########################################

            ClownFishInit.ApplicationEnd();
            startup.AppEnd();
        }
#endif
    }


    internal static void InitTracing()
    {
        if( LoggingOptions.TracingEnabled == false ) {
            Console2.Info("########### 由于 LoggingOptions.TracingEnabled == false ，ClownFish.Tracing 性能监控将不会启用！");
            return;
        }

        DbLogger.Init();

#if NETCOREAPP
        EFLogger.Init();
        HttpClientLogger2.Init();
#else
        HttpClientLogger.Init();
#endif
    }


    ///// <summary>
    ///// 创建WebApplication实例
    ///// </summary>
    ///// <param name="startup"></param>
    ///// <returns></returns>
    //internal static IHost CreateAppHost(ConsoleAppStartup startup = null)
    //{
    //    if( startup == null )
    //        startup = new ConsoleAppStartup();

    //    //IHostBuilder appBuilder = Host.CreateDefaultBuilder();
    //    HostApplicationBuilder appBuilder = Host.CreateApplicationBuilder();

    //    // 给 Ioc 容器注册组件
    //    //appBuilder.ConfigureServices(startup.ConfigureServices);
    //    startup.ConfigureServices(appBuilder.Services);

    //    // 创建 WebApplication 实例，并生成IServiceProvider
    //    startup.BeforeApplicationBuild(appBuilder);
    //    IHost host = appBuilder.Build();
    //    startup.AfterApplicationBuild(host);


    //    var applicationLifetime = host.Services.GetService<IHostApplicationLifetime>();
    //    applicationLifetime.ApplicationStopping.Register(ClownFishInit.ApplicationEnd);

    //    AppHost = host;
    //    return host;
    //}


    internal static void BeforeRun()
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
        Console2.WriteLine("Application started. Press Ctrl+C to shut down.");
        Console2.WriteSeparatedLine();

        Console2.EndListen("_app_startup.log");
    }

}

