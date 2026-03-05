using System.Runtime;

namespace ClownFish.Base;


/// <summary>
/// 获取运行环境信息的工具类
/// </summary>
public static class EnvUtils
{
    /// <summary>
    /// 应用程序运行时产生的动态ID
    /// </summary>
    public static readonly string AppRuntimeId = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 应用程序的启动时间
    /// </summary>
    public static readonly DateTime AppStartTime = DateTime.Now;

    /// <summary>
    /// 判断当前进程是不是运行在 docker 容器中
    /// </summary>
    public static bool IsInDocker => EnvArgs0.IsInDocker;

    /// <summary>
    /// 判断当前进程是不是部署在 Kubernetes 集群中
    /// </summary>
    public static bool IsInK8s => EnvArgs0.IsInK8s;

    /// <summary>
    /// 获取当前POD所在的 K8S 命名空间。如果当前进程没有部署在K8S集群中，则返回 null
    /// </summary>
    public static string K8sNamespace => EnvArgs0.K8sNamespace;


    internal static readonly string HostName;
    internal static readonly string TempPath;

    internal static readonly string RunEnv;

    /// <summary>
    /// 当前进程是否以 DEV 方式运行
    /// </summary>
    public static readonly bool IsDevEnv;

    /// <summary>
    /// 当前进程是否以 TEST 方式运行
    /// </summary>
    public static readonly bool IsTestEnv;

    /// <summary>
    /// 当前进程是否以 PROD 方式运行
    /// </summary>
    public static readonly bool IsProdEnv;



    internal static string ApplicationName { get; private set; }

    /// <summary>
    /// 当前进程所在的(集群)部署环境名称。
    /// </summary>
    internal static string ClusterName { get; private set; }


    // RunEnv, ClusterName 的说明
    // RunEnv 取值于  微软定义的 RUNTIME_ENVIRONMENT, ASPNETCORE_ENVIRONMENT
    // 用于控制进程的运行行为，例如：if( app.Environment.IsDevelopment() ) xxxxxxxxxxx;

    // 而 ClusterName 是指 集群名称，它由多个进程构成的【部署环境】，它不用来控制程序的行为，仅仅只是一个名称。
    // PROD, TEST, DEV 这些看起来也称为环境名称，但是它们通常用于指示程序的“运行模式”，而无法表达“部署集群”这个概念。
    // 如果线上有多个生产集群，如果都用 PROD 这个名称就无法区分了，
    // 而且线上有时候为了方便排查问题，是希望某个进程以 DEV/DEBUG 模式运行的，
    // 这种这种场景下（DEBUG模式），用1个 “ENVIRONMENT-名称” 就无法描述，必须使用2个名称！ 

    // 引入 ClusterName，它还有一个好处：便于统一日志中记录当前的部署环境，
    // 因为日志【通常】需要区分来源（集群名称），而不关心进程使用哪种 “运行模式” 
    // 因此，OprLog.EnvName 会使用 “集群名称”


    static EnvUtils()
    {
        RunEnv = GetRunEnvName();

        RunEnvEnum flag = GetRunEnvEnum(RunEnv);
        IsDevEnv = flag == RunEnvEnum.Dev;
        IsTestEnv = flag == RunEnvEnum.Test;
        IsProdEnv = flag == RunEnvEnum.Prod;

        HostName = GetMachineName();
        TempPath = LocalSettings.GetSetting("APP_TEMPATH") ?? EvalAppTempPath();

        Directory.CreateDirectory(TempPath);

        ReLoad();
    }

    /// <summary>
    /// Init
    /// </summary>
    public static void Init()
    {
        // 调用这个方法是为了触发 cctor
    }

    // 注意：这个类不使用配置服务，因为它可能会很“早”被调用，甚至在初始化 配置服务客户端 之前，
    //       所以，它仅访问 “本地配置”，如果在集群中运行，可再调用 ReLoad 方法。

    /// <summary>
    /// ClownFish/Nebula 内部使用
    /// </summary>
    public static void ReLoad()
    {
        // 真实使用时，部署条件会比较复杂，不能直接依赖于 进程自身的环境变量 参数来决定，
        // 所以，这里提供一个方法，允许特殊场景下修改以下参数值，然后刷新它们。

        ApplicationName = GetApplicationName0();

        // 微服务部署时，集群名称由配置服务统一指定
        ClusterName = LocalSettings.GetSetting("CLUSTER_ENVIRONMENT") ?? "cluster1";
    }

    private static string GetRunEnvName()
    {
        string env = LocalSettings.GetSetting("RUNTIME_ENVIRONMENT") ?? LocalSettings.GetSetting("ASPNETCORE_ENVIRONMENT");

        // 如果不明确指定，就认为是【生产环境】
        if( env.IsNullOrEmpty() )
            env = "PROD";

        return env;
    }

    internal static RunEnvEnum GetRunEnvEnum(string env)
    {
        if( env.IsNullOrEmpty() || env.StartsWithIgnoreCase("PROD") )
            return RunEnvEnum.Prod;

        if( env.StartsWithIgnoreCase("TEST") )
            return RunEnvEnum.Test;

        // 【生产】和【测试】之外的所有环境都认为是【开发】环境
        return RunEnvEnum.Dev;
    }


    private static string EvalAppTempPath()
    {
        if( IsInDocker )
            // linux 内置的临时目录 /tmp, /var/tmp 有自动清理机制，所以不使用它们
            return "/temp";
        else
            return Path.Combine(Path.GetTempPath(), "ClownFishApp", AsmHelper.GetExeName());
    }

    private static string GetMachineName()
    {
        // 有些机器名命名混乱，可以指定 HOST_NAME 配置参数 来代替
        string value = LocalSettings.GetSetting("HOST_NAME");
        if( value.HasValue() )
            return value;

        try {
            return Environment.MachineName;
        }
        catch { /* 这里出异常，只能忽略了  */
            return "#######";
        }
    }


    private static string GetApplicationName0()
    {
        string appName = LocalSettings.GetSetting("Application_Name") ?? AsmHelper.GetExeName();

        // 检查应用名称是否符合要求，如果不符合要求，则抛出异常
        // 虽然不建议在静态构造方法中抛出异常，但是现在确实想不到更好的方法~~~
        CheckApplicationName(appName);
        return appName;
    }


    /// <summary>
    /// 检查应用名称是否合法
    /// </summary>
    /// <param name="appName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void CheckApplicationName(string appName)
    {
        if( appName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(appName));

        foreach( char c in appName ) {
            if( c >= 'a' && c <= 'z' )
                continue;

            if( c >= 'A' && c <= 'Z' )
                continue;

            if( c >= '0' && c <= '9' )
                continue;

            if( c == '.' || c == '_' || c == '-' )
                continue;

            throw new ArgumentOutOfRangeException(nameof(appName), "应用名称不符合要求，名称允许范围：英文字母，数字，英文句号，下划线");
        }
    }



    /// <summary>
    /// 获取进程能使用的临时目录
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTempPath() => EnvUtils.TempPath;

    /// <summary>
    /// 获取当前应用程序的名称
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetAppName() => EnvUtils.ApplicationName;

    /// <summary>
    /// 获取当前进程所在的【集群名称】
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetClusterName() => EnvUtils.ClusterName;

    /// <summary>
    /// 当前进程的运行模式：DEV/TEST/PROD
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRunEnv() => EnvUtils.RunEnv;

    /// <summary>
    /// 获取当前进程所在的机器名称
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetHostName() => EnvUtils.HostName;


    /// <summary>
    /// 显示系统环境信息
    /// </summary>
    public static void ShowSysEnvInfo()
    {
        Console2.WriteSeparatedLine();
        ShowSysEnvInfoItem("ApplicationName", EnvUtils.GetAppName());
        ShowSysEnvInfoItem("AppRuntimeId", EnvUtils.AppRuntimeId);
        ShowSysEnvInfoItem("ProcessId", GetProcessId().ToString());
        ShowSysEnvInfoItem("AppStartTime", EnvUtils.AppStartTime.ToTime23String());

        ShowSysEnvInfoItem("IsNativeAOT", EnvArgs0.IsAot.ToString2());
        ShowSysEnvInfoItem("IsSingleFileDeploy", EnvArgs0.IsSingleFileDeploy.ToString2());
        ShowSysEnvInfoItem("IsInDocker", EnvUtils.IsInDocker.ToString2());
        ShowSysEnvInfoItem("K8S Namespace", EnvUtils.K8sNamespace);

        ShowSysEnvInfoItem("CLUSTER_ENVIRONMENT", EnvUtils.GetClusterName());
        ShowSysEnvInfoItem("RUNTIME_ENVIRONMENT", EnvUtils.GetRunEnv());
        ShowSysEnvInfoItem("HostName", EnvUtils.GetHostName());
        ShowSysEnvInfoItem("OS Name", OsUtils.GetOsName());
        ShowSysEnvInfoItem("OSArchitecture", GetOSArchitecture());
        ShowSysEnvInfoItem("ProcessorCount", Environment.ProcessorCount.ToString());
        ShowSysEnvInfoItem("TimeZone", MyTimeZone.CurrentTZ);
        ShowSysEnvInfoItem("CurrentCulture", System.Globalization.CultureInfo.CurrentCulture?.Name);
        ShowSysEnvInfoItem("GC Mode", (GCSettings.IsServerGC ? "Server" : "WorkStation"));
        ShowSysEnvInfoItem("Framework Info", GetFrameworkInfo());

        ShowSysEnvInfoItem("EntryAssembly", AsmHelper.GetExeFilePath());
        ShowSysEnvInfoItem("BaseDirectory", AppContext.BaseDirectory);
        ShowSysEnvInfoItem("CurrentDirectory", Environment.CurrentDirectory);
        ShowSysEnvInfoItem("TempPath", EnvUtils.GetTempPath());

        ShowSysEnvInfoItem("ClownFish-Ver", ConstValues.CurrentVersion);
        Console2.WriteSeparatedLine();
    }

    private static void ShowSysEnvInfoItem(string name, string value)
    {
        Console2.WriteLine(name.PadRight(30) + ": " + value);
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


}



/// <summary>
/// 进程的运行模式枚举
/// </summary>
internal enum RunEnvEnum
{
    /// <summary>
    /// 开发环境
    /// </summary>
    Dev,
    /// <summary>
    /// 测试环境
    /// </summary>
    Test,
    /// <summary>
    /// 生产环境
    /// </summary>
    Prod
}


