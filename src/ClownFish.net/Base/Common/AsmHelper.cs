namespace ClownFish.Base;

/// <summary>
/// Assembly工具类
/// </summary>
public static class AsmHelper
{
    private static readonly object s_lock = new object();
    private static bool s_inited = false;


    /// <summary>
    /// 当前程序是否以“单文件部署”方式运行
    /// </summary>
    public static bool IsSingleFileDeploy => EnvArgs0.IsSingleFileDeploy;

    // 参考：https://learn.microsoft.com/zh-cn/dotnet/core/deploying/single-file/overview?tabs=cli#api-incompatibility
    // 由于.NET并没有提供一种专用的方法来判断【单文件部署】，所以这里使用 Assembly.Location 来判断。


    // 在单元测试环境下，Assembly.GetEntryAssembly() 的结果不是我们期望的，所以可以直接修改下面这个变量
    internal static Assembly EntryAssembly { get; private set; }

    /// <summary>
    /// 在单元测试环境下，Assembly.GetEntryAssembly() 的结果不是我们期望的，所以可以调用当前方法。
    /// 然后需要获取时，调用 GetEntryAssembly()
    /// </summary>
    /// <param name="entryAssembly"></param>
    public static void SetEntryAssembly(Assembly entryAssembly)
    {
        EntryAssembly = entryAssembly;
    }

    /// <summary>
    /// 获取当前进程的入口程序集
    /// </summary>
    /// <returns></returns>
    public static Assembly GetEntryAssembly()
    {
        return EntryAssembly ?? Assembly.GetEntryAssembly();
    }

    /// <summary>
    /// 获取当前进程的入口程序集路径
    /// </summary>
    /// <returns></returns>
    public static string GetExeFilePath()
    {
        // 参考：https://learn.microsoft.com/zh-cn/dotnet/core/deploying/single-file/overview?tabs=cli#api-incompatibility

        if( AsmHelper.IsSingleFileDeploy ) {
            return Environment.GetCommandLineArgs()[0];
        }
        else {
            return GetEntryAssembly().Location;
        }
    }

    /// <summary>
    /// 获取某个类型所在程序集文件的文件版本号
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetFileVersion(Type type)
    {
        // 参考：https://learn.microsoft.com/zh-cn/dotnet/core/deploying/single-file/overview?tabs=cli#api-incompatibility

        if( AsmHelper.IsSingleFileDeploy ) {
            return string.Empty;
        }
        else {
            if( type == null )
                throw new ArgumentNullException(nameof(type));

            return FileVersionInfo.GetVersionInfo(type.Assembly.Location).FileVersion;
        }
    }


    /// <summary>
    /// 加载所有DLL文件。
    /// 在启动时，如果不显式加载，就有可能在反射时取不到没有访问到的类型
    /// </summary>
    private static void LoadAllAssemblies()
    {
        if( AsmHelper.IsSingleFileDeploy )
            return;

        string[] files = RetryDirectory.GetFiles(AppContext.BaseDirectory, "*.dll", System.IO.SearchOption.TopDirectoryOnly);

        foreach( string file in files ) {
            try {
                Assembly.LoadFrom(file);
            }
            catch {
                // 忽略加载错误
            }
        }
    }


    internal static Assembly[] GetCurrentDomainAssemblies()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {
                    LoadAllAssemblies();
                    s_inited = true;
                }
            }
        }

        return System.AppDomain.CurrentDomain.GetAssemblies();
    }

    /// <summary>
    /// 获取当前程序加载的所有程序集
    /// </summary>
    /// <param name="ignoreSystemAssembly">是否忽略系统（微软提供的）程序集，通常反射时不需要分析它们。</param>
    /// <returns></returns>
    public static Assembly[] GetLoadAssemblies(bool ignoreSystemAssembly = false)
    {
        Assembly[] assemblies = GetCurrentDomainAssemblies();

        // 过滤一些反射中几乎用不到的程序集
        List<Assembly> list = new List<Assembly>(assemblies.Length);

        foreach( Assembly assembly in assemblies ) {

            if( assembly.IsDynamic )    // 动态程序集基本上是不需要分析的
                continue;

            if( AsmHelper.IsSingleFileDeploy == false 
                && assembly.Location.IsNullOrEmpty() )  // 程序运行过程中，通用CodeDom这类方法生成的程序集
                continue;

            if( ignoreSystemAssembly ) {

                if( assembly.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) )
                    continue;

                if( assembly.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) )
                    continue;
            }

            list.Add(assembly);
        }
        return list.ToArray();
    }


    /// <summary>
    /// 获取带个指定修饰属性的程序集列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<Assembly> GetAssemblyList<T>() where T : Attribute
    {
        Assembly[] assemblies = GetLoadAssemblies(true);

        return assemblies.Where(x => x.GetAttributes<T>().Length > 0).ToList();
    }


    /// <summary>
    /// 处理所有程序集的XML文件
    /// </summary>
    /// <param name="xmlFileAction"></param>
    public static void ForeachXmlFiles(Action<string> xmlFileAction)
    {
        //string binPath = Path.GetDirectoryName(AsmHelper.GetEntryAssembly().Location);
        string binPath = AppContext.BaseDirectory;
        string[] files = Directory.GetFiles(binPath, "*.xml", SearchOption.TopDirectoryOnly);

        foreach( var file in files ) {
            string dllPath = file.Substring(0, file.Length - 4) + ".dll";
            if( File.Exists(dllPath) ) {
                try {
                    xmlFileAction(file);
                }
                catch( Exception ex ) {
                    Console2.Warnning($"加载XML文件 [{file}] 失败：" + ex.Message);
                }
            }
        }
    }

}
