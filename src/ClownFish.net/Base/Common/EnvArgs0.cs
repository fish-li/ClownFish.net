namespace ClownFish.Base;

// ### 注意：这个类只依赖于 .NET API，不能依赖 LocalSettings/Settings，避免产生循环依赖

internal static class EnvArgs0
{
    /// <summary>
    /// 当前程序是否以“单文件部署”方式运行
    /// </summary>
    public static readonly bool IsSingleFileDeploy = typeof(EnvArgs0).Assembly.Location.IsNullOrEmpty();

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

}
