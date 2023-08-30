namespace ClownFish.Base;

/// <summary>
/// Assembly工具类
/// </summary>
public static class AsmHelper
{
    private static readonly object s_lock = new object();
    private static bool s_inited = false;


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

    internal static Assembly GetEntryAssembly()
    {
        return EntryAssembly ?? Assembly.GetEntryAssembly();
    }


    /// <summary>
    /// 加载所有DLL文件。
    /// 在启动时，如果不显式加载，就有可能在反射时取不到没有访问到的类型
    /// </summary>
    private static void LoadAllAssemblies()
    {
        string[] files = RetryDirectory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", System.IO.SearchOption.TopDirectoryOnly);

        foreach( string file in files ) {
            try {
                Assembly.LoadFrom(file);
            }
            catch {
                // 忽略加载错误
            }
        }
    }


    private static Assembly[] GetAssemblies()
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
        Assembly[] assemblies = GetAssemblies();


        // 过滤一些反射中几乎用不到的程序集
        List<Assembly> list = new List<Assembly>(128);

        foreach( Assembly assembly in assemblies ) {

            if( assembly.IsDynamic )    // 动态程序集基本上是不需要分析的
                continue;

            if( assembly.Location.IsNullOrEmpty() )
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
        List<Assembly> list = new List<Assembly>(128);

        var assemblies = GetLoadAssemblies(true);
        foreach( Assembly assembly in assemblies ) {

            if( assembly.GetAttributes<T>().Length == 0 )
                continue;

            list.Add(assembly);
        }

        return list;
    }




}
