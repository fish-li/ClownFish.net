namespace ClownFish.Base;

// ### 注意：这个类只依赖于 .NET API，不能依赖 LocalSettings/Settings，避免产生循环依赖

internal static class EnvArgs0
{
    /// <summary>
    /// 当前程序是否以“单文件部署”方式运行
    /// </summary>

    public static readonly bool IsSingleFileDeploy = IsRunningAsSingleFile();
    //public static readonly bool IsSingleFileDeploy = typeof(EnvArgs0).Assembly.Location.IsNullOrEmpty();

#if NETCOREAPP
    [UnconditionalSuppressMessage("SingleFileAnalyzer", "IL3000: Assembly.Location always returns an empty string for assemblies embedded in a single-file app")]
#endif
    private static bool IsRunningAsSingleFile()
    {
#if NETCOREAPP
        // Assembly.Location 在单文件部署时总是返回空字符串
        // 推荐的判断方式是：AppContext.GetData("IsSingleFile")，但需要 .NET 5+
        object isSingleFile = AppContext.GetData("IsSingleFile");
        if (isSingleFile is bool b)
            return b;

        // 兼容旧逻辑
        return typeof(EnvArgs0).Assembly.Location.IsNullOrEmpty();
#else
        return typeof(EnvArgs0).Assembly.Location.IsNullOrEmpty();
#endif
    }

    /// <summary>
    /// 当前程序是否以“NativeAOT”方式运行。需要2个条件：1，单文件部署，2，已设置AOT标记（ClownFish不能自行判断）
    /// </summary>
    public static readonly bool IsAot = IsSetAotFlag();

    // NativeAOT有以下限制：https://learn.microsoft.com/zh-cn/dotnet/core/deploying/native-aot/?tabs=windows%2Cnet8#limitations-of-native-aot-deployment


    /// <summary>
    /// 判断当前进程是不是运行在 docker 容器中
    /// </summary>
    public static readonly bool IsInDocker = EnvironmentVariables.Get("DOTNET_RUNNING_IN_CONTAINER").TryToBool();

    /// <summary>
    /// 判断当前进程是不是部署在 Kubernetes 集群中
    /// </summary>
    public static readonly bool IsInK8s;

    /// <summary>
    /// 获取当前POD所在的 K8S 命名空间。如果当前进程没有部署在K8S集群中，则返回 null
    /// </summary>
    public static readonly string K8sNamespace;

    static EnvArgs0()
    {
        string k8sNamespace = GetCurrentK8sNamespace();
        IsInK8s = IsInDocker && k8sNamespace.HasValue();
        K8sNamespace = IsInK8s ? k8sNamespace : null;
    }

    private static string GetCurrentK8sNamespace()
    {
        string filePath = "/var/run/secrets/kubernetes.io/serviceaccount/namespace";
        if( File.Exists(filePath) ) {
            return File.ReadAllText(filePath);
        }
        else {
            return null;
        }
    }



    private  static bool IsSetAotFlag()
    {
#if NETCOREAPP
        if( EnvironmentVariables.Get("ClownFish_RUNNING_IsAot").TryToBool() )
            return true;

        if( AppContext.TryGetSwitch("ClownFish_RUNNING_IsAot", out bool isEnabled) && isEnabled )
            return true;
#endif

        return false;
    }


}
