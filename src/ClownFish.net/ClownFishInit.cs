using System.Runtime;

namespace ClownFish.Base;

/// <summary>
/// ClownFish初始化辅助工具类
/// </summary>
public static class ClownFishInit
{
    private static bool s_baseInited = false;
    private static bool s_dalInited = false;

    private static readonly CancellationTokenSource s_exitTokenSource = new CancellationTokenSource();
    /// <summary>
    /// 应用程序结束时通知对象
    /// </summary>
    public static CancellationToken AppExitToken => s_exitTokenSource.Token;

    /// <summary>
    /// 【此方法仅供框架内部使用】通知后台线程执行退出操作。
    /// </summary>
    public static void ApplicationEnd()
    {
        if( s_exitTokenSource.IsCancellationRequested )
            return;

        Console2.WriteSeparatedLine();

        // 通知所有后台线程，应用程序即将退出
        s_exitTokenSource.Cancel();

        Console2.WriteLine("Application End!");
    }

    /// <summary>
    /// 执行一些最基础的初始化，不包含 Data/Log 部分
    /// </summary>
    public static void InitBase()
    {
        if( s_baseInited == false ) {
            EnvironmentVariables.Init();
            AppConfig.Init();
            EnvUtils.Init();
            //SetDefaultCulture();
            SetThreadPool();
            ConfigMisc();
            StartGcCollect();

#if NETCOREAPP
            // support Encoding.GetEncoding("GB2312")
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
            s_baseInited = true;

            // 注意：下面这段代码【不要】移动到 AppConfig.Init() ，那样会造成死循环
            if( LocalSettings.GetBool("Show_ClownFish_App_Config") ) {
                string filePath = AppConfig.GetAppConfigFilePath();
                if( File.Exists(filePath) ) {
                    Console2.WriteLine($"----------------------- {Path.GetFileName(filePath)} ----------------------------");
                    Console2.WriteLine(File.ReadAllText(filePath, Encoding.UTF8));
                    Console2.WriteLine("-------------------------------------------------------------------------");
                }
                else {
                    Console2.Info($"[{filePath}] not found!");
                }
            }
        }
    }

    //private static void SetDefaultCulture()
    //{
    //    // donet 的基础镜像中并没有指定 区域语言 这个设置，例如： LANG=zh_CN.UTF-8 
    //    // 而是将线程的默认区域语言设置为：CultureInfo.InvariantCulture

    //    // 这样会给我们带来一些困扰：
    //    // 比如我们开发时，Windows环境中运行，默认就是 zh-CN
    //    // 但是在 linux-docker 中：CultureInfo.CurrentCulture is CultureInfo.InvariantCulture
    //    // 结果，汉字不是按拼音在排序~~~

    //    // 为了避免可能会产生的困扰，这里检查：如果没有设置区域语言时，强制修改为 zh-CN
    //    // 也就是说，不使用 “CultureInfo.InvariantCulture”，做到 生产环境 和 开发环境 使用相同的设置！


    //    if( CultureInfo.CurrentCulture == null || CultureInfo.CurrentCulture.Name.IsNullOrEmpty() ) {
    //        SetDefaultCulture0();
    //    }
    //}

    //private static void SetDefaultCulture0()
    //{
    //    string lang = EnvironmentVariables.Get("LANG").IfEmpty("zh-CN");
    //    CultureInfo defaultCulture = null;

    //    try {
    //        defaultCulture = new CultureInfo(lang);
    //    }
    //    catch( CultureNotFoundException ex ) {
    //        // 有些 linux 环境没有安装参数中指定的语言包，就会出现异常：
    //        // System.Globalization.CultureNotFoundException: Culture is not supported. (Parameter 'name')
    //        Console2.Warnning($"{ex.GetType().FullName}: Culture {lang} is not supported.");
    //        return;
    //    }

    //    Thread.CurrentThread.CurrentCulture = defaultCulture;
    //    CultureInfo.CurrentCulture = defaultCulture;
    //    CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
    //    Console2.Info("force set CurrentCulture => " + lang);
    //}


    private static void SetThreadPool()
    {
        // .net 默认值：
        // Min Worker Threads: {ProcessorCount}
        // Max Worker Threads: 32767
        //------------------ -
        // Min CompletionPort Threads: {ProcessorCount}
        // Max CompletionPort Threads: 1000

        int coreCount = System.Environment.ProcessorCount.Min(32);  // 最少32个线程

        int minWorker = LocalSettings.GetUInt("ThreadPool_MinWorker", coreCount);
        int maxWorker = LocalSettings.GetUInt("ThreadPool_MaxWorker", 2000);

        int minIOCP = LocalSettings.GetUInt("ThreadPool_MinIOCP", 256);
        int maxIOCP = LocalSettings.GetUInt("ThreadPool_MaxIOCP", 3000);

        // 下面2个调用不检查返回值，因为写单元测试太麻烦~~~
        ThreadPool.SetMaxThreads(maxWorker, maxIOCP);
        ThreadPool.SetMinThreads(minWorker, minIOCP);
    }


    private static void ConfigMisc()
    {
        if( LocalSettings.GetBool("ClownFish_LogError_ToConsole") ) {
            ClownFish.Log.LogHelper.OnError += LogHelperOnError;
        }

        if( LocalSettings.GetBool("ClownFish_ShowHttpClientEvent") ) {
            ClownFish.WebClient.HttpClientEvent.OnBeforeSendRequest += HttpClientEventOnBeforeSendRequest;
        }
    }

    private static void LogHelperOnError(object sender, ExceptionEventArgs e)
    {
        try {
            Console2.Warnning(e.Exception);
        }
        catch {
            // 这里吃掉异常
        }
    }
    private static void HttpClientEventOnBeforeSendRequest(object sender, BeforeSendEventArgs e)
    {
        Console2.Info($"HttpClient send: {e.HttpOption.Method} {e.HttpOption.Url}");
    }

    private static void StartGcCollect()
    {
        // 多数情况下，机器资源都是有限的，尽量减少内存占用对于公司的成本支出来说是有利的，
        // 然而 GC 的回收时机难以控制，所以为了更好的降低进程的内存占用，周期性的触发GC回收就有意义了

        if( ClownFishOptions.GCCollectPeriodSec > 1 ) {
            ThreadUtils.RunAsync(nameof(StartGcCollect), StartGcCollectTask);
        }
    }
    private static async Task StartGcCollectTask()
    {
        int waitMs = ClownFishOptions.GCCollectPeriodSec * 1000;

        while( true ) {
            await Task2.Delay(waitMs, AppExitToken);

            if( AppExitToken.IsCancellationRequested )
                return;

#if NET46_OR_GREATER || NETCOREAPP
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
#endif
            GC.Collect();
        }
    }


    /// <summary>
    /// 初始化 ClownFish.Data
    /// </summary>
    public static void InitDAL()
    {
        if( s_dalInited == false ) {
            AutoRegisterDbProviders();

            ClownFish.Data.Initializer.Instance.LoadXmlCommandFromDirectory();

            // 【单文件部署】场景下，不允许在运行时生成代理程序集，因为Assembly相关的API有限制
            if( AsmHelper.IsSingleFileDeploy == false ) {
                string exePath = AsmHelper.GetExeFilePath();
                string newName = Path.GetFileNameWithoutExtension(exePath) + ".EntityProxy.dll";
                string dllOutPath = Path.Combine(EnvUtils.GetTempPath(), newName);
                ClownFish.Data.Initializer.Instance.CompileAllEntityProxy(dllOutPath);
            }

            s_dalInited = true;
        }
    }


    private static void AutoRegisterDbProviders()
    {
        ClownFish.Data.Initializer.Instance.RegisterSqlServerProvider();

        ClownFish.Data.Initializer.Instance.RegisterMySqlProvider();

        AutoRegisterOthersSqlClient();
    }

    private static void AutoRegisterOthersSqlClient()
    {
        string[] asmList = AsmHelper.GetCurrentDomainAssemblies().Select(x => x.GetName().Name).OrderBy(x => x).ToArray();

        if( asmList.Contains("Npgsql") ) {
            ClownFish.Data.Initializer.Instance.RegisterPostgreSqlProvider();
        }

        if( asmList.Contains("DmProvider") ) {
            ClownFish.Data.Initializer.Instance.RegisterDamengProvider();
        }

        if( asmList.Contains("System.Data.SQLite") ) {
            ClownFish.Data.Initializer.Instance.RegisterSQLiteProvider();
        }
    }

    /// <summary>
    /// 按照默认方式初始化日志组件
    /// </summary>
    public static void InitLogAsDefault()
    {
        if( ClownFish.Log.LogConfig.IsInited )
            return;


        // 从程序集中加载默认配置文件
        string xml = typeof(LogHelper).Assembly.ReadResAsText("ClownFish.ClownFish.Log.config");
        LogConfiguration config = XmlHelper.XmlDeserialize<LogConfiguration>(xml);

        ClownFishInit.InitLog(config);
    }



    /// <summary>
    /// 初始化 ClownFish.Log
    /// </summary>
    /// <param name="config"></param>
    public static void InitLog(LogConfiguration config)
    {
        if( config == null )
            throw new ArgumentNullException(nameof(config));

        if( ClownFish.Log.LogConfig.IsInited )
            return;

        // 允许重新指定写入器类型，例如：开发时写到XML文件，生产环境部署时统一写到ES
        string logWriterNames = Settings.GetSetting("ClownFish_Log_WritersMap");
        if( logWriterNames.HasValue() ) {
            Console2.Info("ClownFish_Log_WritersMap: " + logWriterNames);
            config.OverrideWriters(logWriterNames);
        }

        // 尝试本地参数中更新日志配置
        config.TryUpdateFromLocalSetting();

        if( LocalSettings.GetBool("Show_ClownFish_Log_Config") ) {
            // 由于 Log_Config 的内容会做【合并】，所以这里显示【最终生效】的配置对象
            string configXml = XmlHelper.XmlSerialize(config, Encoding.UTF8);
            Console2.WriteLine("----------------------- ClownFish.Log.config ----------------------------");
            Console2.WriteLine(configXml);
            Console2.WriteLine("-------------------------------------------------------------------------");
        }

        ClownFish.Log.LogConfig.Init(config);
    }


    /// <summary>
    /// 初始化 ClownFish.Log
    /// </summary>
    /// <param name="filePath">ClownFish.Log.config的完整路径</param>
    public static void InitLog(string filePath)
    {
        if( filePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filePath));

        if( ClownFish.Log.LogConfig.IsInited )
            return;

        LogConfiguration config = LogConfiguration.LoadFromFile(filePath, true);
        ClownFishInit.InitLog(config);
    }

}
