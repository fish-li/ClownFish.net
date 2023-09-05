namespace ClownFish.Http.Pipleline;

/// <summary>
/// NHttpModuleFactory
/// </summary>
public static class NHttpModuleFactory
{
    private static readonly List<Type> s_moduleTypeList = new List<Type>(32);
    private static volatile bool s_inited = false;


    /// <summary>
    /// 注册HTTP模块
    /// </summary>
    /// <param name="moduleType">NHttpModule的类型</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RegisterModule(Type moduleType)
    {
        if( moduleType == null )
            throw new ArgumentNullException(nameof(moduleType));

        if( moduleType.IsSubclassOf(typeof(NHttpModule)) == false )
            throw new ArgumentOutOfRangeException(nameof(moduleType), $"将要注册的插件 {moduleType.Name} 不符合基类要求。");

        if( moduleType.CanNew() == false )
            throw new ArgumentException($"将要注册的插件 {moduleType.Name} 没有公开无参的构造方法。");

        if( s_inited )
            throw new InvalidOperationException("RegisterModule方法只允许在程序初始化时调用。");

        // 允许运行时以配置方式取消某个模块
        if( ModuleIsEnable(moduleType) == false )
            return;

        if( s_moduleTypeList.Contains(moduleType) == false ) 
            s_moduleTypeList.Add(moduleType);
    }


    internal static bool ModuleIsEnable(Type moduleType)
    {
        string configName = moduleType.FullName.Replace('.', '_') + "_Enable";
        return LocalSettings.GetBool(configName, 1);
    }

    /// <summary>
    /// 注册HTTP模块
    /// </summary>
    /// <typeparam name="T">NHttpModule类型</typeparam>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RegisterModule<T>() where T : NHttpModule
    {
        RegisterModule(typeof(T));
    }


    internal static List<NHttpModule> CreateModuleList()
    {
        if( s_inited == false )
            s_inited = true;


        List<NHttpModule> list = new List<NHttpModule>(s_moduleTypeList.Count);

        foreach( var t in s_moduleTypeList ) {
            NHttpModule plugin = (NHttpModule)Activator.CreateInstance(t);
            list.Add(plugin);
        }

        return list.OrderBy(x => x.Order).ToList();
    }


    internal static List<Type> GetList()
    {
        return s_moduleTypeList;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Clear()  // 用于单元测试
    {
        s_moduleTypeList.Clear();
        s_inited = false;
    }
}
