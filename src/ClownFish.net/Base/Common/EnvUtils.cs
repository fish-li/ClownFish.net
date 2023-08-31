namespace ClownFish.Base;

/// <summary>
/// 环境类别
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
    /// 当前环境标识名称
    /// </summary>
    public static readonly string EnvName;

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


    internal static readonly string ApplicationName;
    internal static readonly string HostName;
    internal static readonly string TempPath;


    static EnvUtils()
    {
        string env = GetEvnName();
        EnvName = env;
        CurEvnKind = GetEvnKind(env);


        TempPath = LocalSettings.GetSetting("APP_TEMPATH") ?? Path.Combine(AppContext.BaseDirectory, "temp");   // Path.GetTempPath();
        ApplicationName = GetApplicationName0();
        HostName = GetMachineName();        
    }

    private static string GetEvnName()
    {
        string env = EnvironmentVariables.Get("ASPNETCORE_ENVIRONMENT");

        if( env.IsNullOrEmpty() )
            env = EnvironmentVariables.Get("RUNTIME_ENVIRONMENT");

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


    internal static string GetMachineName()
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


    internal static void CheckApplicationName(string appName)
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

            if( c == '.' || c == '_' )
                continue;

            throw new ArgumentOutOfRangeException(nameof(appName), "应用名称不符合要求，名称允许范围：英文字母，数字，英文句号，下划线");
        }
    }



    public static string GetApplicationName()
    {
        return ClownFishBehavior.Instance.GetApplicationName();
    }

    public static string GetHostName()
    {
        return ClownFishBehavior.Instance.GetHostName();
    }

    public static string GetEnvName()
    {
        return ClownFishBehavior.Instance.GetEnvName();
    }

    public static string GetTempPath()
    {
        return ClownFishBehavior.Instance.GetTempPath();
    }

}
