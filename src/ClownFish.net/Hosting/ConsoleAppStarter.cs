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

        startup.BeforeFrameworkInit();

        ClownFishInit.InitBase();
        TypeHelper.Init();

        ShowSysEnvInfo();

        if( startup.AutoInitDAL )
            ClownFishInit.InitDAL();

        if( startup.AutoInitTracing )
            CheckLogConfig();

        // 监控必须使用日志组件
        if( startup.AutoInitLog || startup.AutoInitTracing )
            ClownFishInit.InitLogAsDefault();


        //CreateAppHost(startup);

        // 开启性能监控
        // 放在这里调用，可以监控 ApplicationInit 的执行过程（需要配合 CodeSnippetContext 来实现）
        // 但是这样做也有一个【隐患】：如果在那里 开启后台线程（3种方式），【默认】会导致 OprLogScope 传递到那些后台线程
        if( startup.AutoInitTracing )
            InitTracing();

        startup.AfterFrameworkInit();

        Console2.WriteLine("----------------------- Application Initializer ----------------------------");
        ApplicationInitializer.Execute();
        startup.AppInit();

        WriteDebugReport();

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

    private static void CheckLogConfig()
    {
        string writesMap = Settings.GetSetting("ClownFish_Log_WritersMap");
        if( writesMap.IsNullOrEmpty() ) {
            Console2.Info("force set: ClownFish_Log_WritersMap => OprLog=http");
            MemoryConfig.AddSetting("ClownFish_Log_WritersMap", "OprLog=http");
        }
    }

    private static void InitTracing()
    {
        if( LoggingOptions.TracingEnabled == false )
            return;

        DbLogger.Init();

#if NETCOREAPP
        EFLogger.Init();
        HttpClientLogger2.Init();
#else
        HttpClientLogger.Init();
#endif
    }


    internal static void ShowSysEnvInfo()
    {
        Console2.WriteSeparatedLine();
        Console2.WriteLine("ApplicationName          : " + EnvUtils.GetAppName());
        Console2.WriteLine("AppRuntimeId             : " + EnvUtils.AppRuntimeId);
        Console2.WriteLine("ProcessId                : " + GetProcessId().ToString());
        Console2.WriteLine("EntryAssembly            : " + AsmHelper.GetExeFilePath());
        Console2.WriteLine("AppStartTime             : " + EnvUtils.AppStartTime.ToTime23String());
        Console2.WriteLine("IsInDocker               : " + EnvUtils.IsInDocker.ToString2());
        Console2.WriteLine("IsSingleFileDeploy       : " + AsmHelper.IsSingleFileDeploy.ToString2());
        Console2.WriteLine("CLUSTER_ENVIRONMENT      : " + EnvUtils.GetClusterName());
        Console2.WriteLine("RUNTIME_ENVIRONMENT      : " + EnvUtils.GetRunEnv());
        Console2.WriteLine("HostName                 : " + EnvUtils.GetHostName());
        Console2.WriteLine("OS Name                  : " + OsUtils.GetOsName());
        Console2.WriteLine("OSArchitecture           : " + GetOSArchitecture());
        Console2.WriteLine("ProcessorCount           : " + Environment.ProcessorCount.ToString());
        Console2.WriteLine("TimeZone                 : " + MyTimeZone.CurrentTZ);
        Console2.WriteLine("CurrentCulture           : " + System.Globalization.CultureInfo.CurrentCulture?.Name);
        Console2.WriteLine("GC Mode                  : " + (GCSettings.IsServerGC ? "Server" : "WorkStation"));
        Console2.WriteLine("Framework  Info          : " + GetFrameworkInfo());
        Console2.WriteLine("ClownFishVer             : " + AsmHelper.GetFileVersion(typeof(ClownFishInit)).IfEmpty(ConstValues.CurrentVersion));
        Console2.WriteLine("BaseDirectory            : " + AppContext.BaseDirectory);
        Console2.WriteLine("CurrentDirectory         : " + Environment.CurrentDirectory);
        Console2.WriteLine("TempPath                 : " + EnvUtils.GetTempPath());
        Console2.WriteSeparatedLine();
    }


    private static string GetFrameworkInfo()
    {
#if NETCOREAPP
        return System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
#else
        return System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion();
#endif
    }

    private static string GetOSArchitecture()
    {
#if NETCOREAPP
        return System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString();
#else
        return Environment.Is64BitOperatingSystem ? "X64" : "X86";
#endif
    }

    private static int GetProcessId()
    {
#if NET6_0_OR_GREATER
        return Environment.ProcessId;
#else
        return Process.GetCurrentProcess().Id;
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

