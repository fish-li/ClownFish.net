using System.Runtime;

namespace ClownFish.Base;
internal static class BaseInitUtils
{
    private static bool s_baseInited = false;


#if NETCOREAPP    // 下面几个类型不参与裁剪，保留无参构造函数，确保可序列化
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NameInt64))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NameTime))]    
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NameValue))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IUserInfo))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IValidate))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(EndClientUserInfo))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WebUserInfo))]
#endif
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            s_baseInited = true;

            // 注意：下面这段代码【不要】移动到 AppConfig.Init() ，那样会造成死循环
            ShowClownFishAppConfig();
        }
    }

    internal static void ShowClownFishAppConfig()
    {
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

        int coreCount = Environment.ProcessorCount.Min(32);  // 最少32个线程

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
        // 下面2个事件订阅，和 Base 模块确实没有关系，它们太零散了，单独为它们搞2个方法也太麻烦了，所以就勉强放在这里吧~~~

        if( LocalSettings.GetBool("ClownFish_LogError_ToConsole") ) {
            LogHelper.OnError += LogHelperOnError;
        }

        if( LocalSettings.GetBool("ClownFish_ShowHttpClientEvent") ) {
            HttpClientEvent.OnBeforeSendRequest += HttpClientEventOnBeforeSendRequest;
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
            await Task2.Delay(waitMs, ClownFishInit.AppExitToken);

            if( ClownFishInit.AppExitToken.IsCancellationRequested )
                return;

#if NET46_OR_GREATER || NETCOREAPP
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
#endif
            GC.Collect();
        }
    }


}
