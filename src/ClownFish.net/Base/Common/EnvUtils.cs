namespace ClownFish.Base;

/// <summary>
/// 进程的运行模式类别
/// </summary>
public enum EvnKind
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
    public static readonly bool IsInDocker = EnvironmentVariables.Get("DOTNET_RUNNING_IN_CONTAINER").TryToBool();

    /// <summary>
    /// 当前环境类别
    /// </summary>
    internal static readonly EvnKind CurEvnKind;

    /// <summary>
    /// 当前环境是否为【开发】环境
    /// </summary>
    public static bool IsDevEnv => CurEvnKind == EvnKind.Dev;

    /// <summary>
    /// 当前环境是否为【测试】环境
    /// </summary>
    public static bool IsTestEnv => CurEvnKind == EvnKind.Test;

    /// <summary>
    /// 当前环境是否为【生产】环境
    /// </summary>
    public static bool IsProdEnv => CurEvnKind == EvnKind.Prod;


    /// <summary>
    /// 进程运行环境标识名称。
    /// 可以把它理解为：当前进程的运行模式：DEV/TEST/PROD
    /// </summary>
    internal static readonly string EnvName;

    internal static readonly string HostName;
    internal static readonly string TempPath;

    internal static string ApplicationName { get; private set; }

    /// <summary>
    /// 当前进程所在的(集群)部署环境名称。
    /// </summary>
    internal static string ClusterName { get; private set; }


    // EnvName, ClusterName 的说明
    // EnvName 等同于/取值于  微软定义的 RUNTIME_ENVIRONMENT, ASPNETCORE_ENVIRONMENT
    // 用于控制进程的运行时行为，例如：if( app.Environment.IsDevelopment() ) xxxxxxxxxxx;
    // 因此，变量 CurEvnKind 由 EnvName 来决定。

    // 而 ClusterName 是指 集群名称，它由多个进程构成的部署环境，它不用来控制程序的行为，仅仅只是一个名称。
    // PROD, TEST, DEV 这些看起来也称为环境名称，但是它们更像是“类别”，现在被用于控制运行时行为，无法标识“集群”这个概念。
    // 如果线上有多个生产集群，如果都用 PROD 这个名称就无法区分了，
    // 而且线上有时候为了方便排查问题，是希望某个进程以 DEV/DEBUG 模式运行的，
    // 这种这种场景下（DEBUG模式），用1个 “名称” 就无法实现，必须使用2个名称！ 

    // 单独出来一个 ClusterName，它还有一个好处：便于统一日志中记录当前的部署环境，
    // 因为日志【通常】需要区分来源（集群名称），而不关心进程使用哪种 “运行模式” 
    // 因此，OprLog.EnvName 会使用 “集群名称”


    static EnvUtils()
    {
        EnvName = GetEvnName();
        CurEvnKind = GetEvnKind(EnvName);
        HostName = GetMachineName();
        TempPath = LocalSettings.GetSetting("APP_TEMPATH") ?? EvalAppTempPath();

        if( Directory.Exists(TempPath) == false )
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
    /// 框架内部使用
    /// </summary>
    public static void ReLoad()
    {
        // 真实使用时，部署条件会比较复杂，不能直接依赖于 进程自身的环境变量 参数来决定，
        // 所以，这里提供一个方法，允许特殊场景下修改以下参数值，然后刷新它们。

        ApplicationName = GetApplicationName0();
        ClusterName = LocalSettings.GetSetting("CLUSTER_ENVIRONMENT") ?? "ClownFish.TEST";
    }

    private static string GetEvnName()
    {
        string env = EnvironmentVariables.Get("ASPNETCORE_ENVIRONMENT") ?? EnvironmentVariables.Get("RUNTIME_ENVIRONMENT");

        // 如果不明确指定，就认为是【生产环境】
        if( env.IsNullOrEmpty() )
            env = "PROD";

        return env;
    }

    internal static EvnKind GetEvnKind(string env)
    {
        if( env.IsNullOrEmpty() || env.Is("PROD") || env.StartsWithIgnoreCase("Product") )
            return EvnKind.Prod;

        if( env.Is("TEST") || env.StartsWithIgnoreCase("Test") )
            return EvnKind.Test;

        // 【生产】和【测试】之外的所有环境都认为是【开发】环境
        return EvnKind.Dev;
    }


    private static string EvalAppTempPath()
    {
        if( IsInDocker )
            // linux 内置的临时目录 /tmp, /var/tmp 有自动清理机制，所以不使用它们
            return "/temp";
        else
            return Path.Combine(Path.GetTempPath(), "ClownFishApp", Path.GetFileNameWithoutExtension(AsmHelper.GetEntryAssembly().Location));
    }

    private static string GetMachineName()
    {
        try {
            return Environment.MachineName;
        }
        catch { /* 这里出异常，只能忽略了  */
            return "#######";
        }
    }


    private static string GetApplicationName0()
    {
        string appName = LocalSettings.GetSetting("Application_Name") ?? Path.GetFileNameWithoutExtension(AsmHelper.GetEntryAssembly().Location);

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
    public static string GetTempPath() => EnvUtils.TempPath;

    /// <summary>
    /// 获取当前应用程序的名称
    /// </summary>
    /// <returns></returns>
    public static string GetAppName() => EnvUtils.ApplicationName;

    /// <summary>
    /// 获取当前进程所在的(集群)部署环境名称。
    /// 为了能让日志取值统一，所以这里使用【集群名称】
    /// </summary>
    /// <returns></returns>
    public static string GetClusterName() => EnvUtils.ClusterName;

    /// <summary>
    /// 获取 "微软定义的DOTNET环境变量" 的 “环境名称”。
    /// 可以把它理解为：当前进程的运行模式：DEV/TEST/PROD
    /// </summary>
    /// <returns></returns>
    public static string GetRuntimeEnvName() => EnvUtils.EnvName;

    /// <summary>
    /// 获取当前进程所在的机器名称
    /// </summary>
    /// <returns></returns>
    public static string GetHostName() => EnvUtils.HostName;

}


