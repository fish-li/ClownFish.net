using System.Globalization;
using System.Runtime.InteropServices;

namespace ClownFish.Base;

/// <summary>
/// ClownFish初始化辅助工具类
/// </summary>
public static class ClownFishInit
{
    private static bool s_baseInited = false;
    private static bool s_dalInited = false;

    /// <summary>
    /// 执行一些最基础的初始化，不包含 Data/Log 部分
    /// </summary>
    public static void InitBase()
    {
        if( s_baseInited == false ) {
            EnvironmentVariables.Init();
            AppConfig.Init();
            SetDefaultCulture();
            SetThreadPool();
            ConfigMisc();
            s_baseInited = true;
        }
    }

    private static void SetDefaultCulture()
    {
        // donet 的基础镜像中并没有指定 区域语言 这个设置，例如： LANG=zh_CN.UTF-8 
        // 而是将线程的默认区域语言设置为：CultureInfo.InvariantCulture

        // 这样会给我们带来一些困扰：
        // 比如我们开发时，Windows环境中运行，默认就是 zh-CN
        // 但是在 linux-docker 中：CultureInfo.CurrentCulture is CultureInfo.InvariantCulture
        // 结果，汉字不是按拼音在排序~~~

        // 为了避免可能会产生的困扰，这里检查：如果没有设置区域语言时，强制修改为 zh-CN
        // 也就是说，不使用 “CultureInfo.InvariantCulture”，做到 生产环境 和 开发环境 使用相同的设置！


        if( CultureInfo.CurrentCulture == null || CultureInfo.CurrentCulture.Name.IsNullOrEmpty() ) {

            string lang = EnvironmentVariables.Get("LANG").IfEmpty("zh-CN");
            CultureInfo defaultCulture = new CultureInfo(lang);

            Thread.CurrentThread.CurrentCulture = defaultCulture;
            CultureInfo.CurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            Console2.Info("force set CurrentCulture => " + lang);
        }
    }

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

        if( ThreadPool.SetMaxThreads(maxWorker, maxIOCP) == false )
            Console2.Warnning($"SetMaxThreads({maxWorker}, {maxIOCP}) failed.");

        if( ThreadPool.SetMinThreads(minWorker, minIOCP) == false )
            Console2.Warnning($"SetMinThreads({minWorker}, {minIOCP}) failed.");
    }


    private static void ConfigMisc()
    {
        if( LocalSettings.GetBool("ClownFish_LogError_ToConsole") ) {
            ClownFish.Log.LogHelper.OnError += LogHelperOnError;
        }

        if( LocalSettings.GetBool("ClownFish_ShowHttpClientEvent") ) {
            ClownFish.Base.WebClient.HttpClientEvent.OnBeforeSendRequest += HttpClientEventOnBeforeSendRequest;
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
        Console2.Info($"{e.HttpOption.Method} {e.HttpOption.Url}");
    }


    /// <summary>
    /// 初始化 ClownFish.Data
    /// </summary>
    public static void InitDAL()
    {
        if( s_dalInited == false ) {
            AutoRegisterDbProviders();

            ClownFish.Data.Initializer.Instance.LoadXmlCommandFromDirectory();

            string exePath = AsmHelper.GetEntryAssembly().Location;
            string newName = Path.GetFileNameWithoutExtension(exePath) + ".EntityProxy.dll";
            string dllOutPath = Path.Combine(ClownFishBehavior.Instance.GetTempPath(), newName);

            ClownFish.Data.Initializer.Instance.CompileAllEntityProxy(dllOutPath);
            s_dalInited = true;
        }
    }


    private static void AutoRegisterDbProviders()
    {
#if NETFRAMEWORK
        ClownFish.Data.Initializer.Instance.RegisterSqlServerProvider(1);
#endif
#if NETCOREAPP
        if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Microsoft.Data.SqlClient.dll")) ) {
            ClownFish.Data.Initializer.Instance.RegisterSqlServerProvider(2);
        }
        if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Data.SqlClient.dll")) ) {
            ClownFish.Data.Initializer.Instance.RegisterSqlServerProvider(1);
        }
#endif



        int mysqlFlag = LocalSettings.GetInt("MySqlClientProviderSupport", 0);
        if( mysqlFlag > 0 ) {
            ClownFish.Data.Initializer.Instance.RegisterMySqlProvider(mysqlFlag);
        }
        else {
            if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MySql.Data.dll")) ) {
                ClownFish.Data.Initializer.Instance.RegisterMySqlProvider(1);
            }

            if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MySqlConnector.dll")) ) {
                ClownFish.Data.Initializer.Instance.RegisterMySqlProvider(2);
            }

            // 如果没有找到 任何一个DLL，就忽略！
        }


        if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Npgsql.dll")) ) {
            ClownFish.Data.Initializer.Instance.RegisterPostgreSqlProvider();
        }

        if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DmProvider.dll")) ) {
            ClownFish.Data.Initializer.Instance.RegisterDamengProvider();
        }

        if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Data.SQLite.dll")) ) {
            ClownFish.Data.Initializer.Instance.RegisterSQLiteProvider();
        }
    }

    /// <summary>
    /// 初始化 ClownFish.Log
    /// </summary>
    /// <param name="config"></param>
    public static void InitLog(LogConfiguration config)
    {
        if( config == null )
            throw new ArgumentNullException(nameof(config));

        // 尝试本地参数中更新日志配置
        config.TryUpdateFromLocalSetting();

        // 允许重新指定写入器类型，例如：开发时写到XML文件，生产环境部署时统一写到ES
        string logWriterNames = Settings.GetSetting("Nebula_Log_WritersMap") ?? Settings.GetSetting("ClownFish_Log_WritersMap");
        if( logWriterNames.HasValue() ) {
            Console2.Info("OverrideWriters: " + logWriterNames);
            config.OverrideWriters(logWriterNames);
        }

        if( LocalSettings.GetBool("Show_ClownFish_Log_Config") ) {
            string configXml = XmlHelper.XmlSerialize(config, Encoding.UTF8);
            Console2.WriteLine("======================= ClownFish_Log_Config ============================");
            Console2.WriteLine(configXml);
            Console2.WriteLine("===================================================");
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

        LogConfiguration config = LogConfiguration.LoadFromFile(filePath, true);
        ClownFish.Log.LogConfig.Init(config);
    }

}
