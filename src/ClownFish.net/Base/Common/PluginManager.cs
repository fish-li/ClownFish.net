namespace ClownFish.Base;

/// <summary>
/// 插件管理工具类
/// </summary>
/// <typeparam name="T">包含插件的对象类型</typeparam>
/// <typeparam name="P">插件基类</typeparam>
internal static class PluginManager<T, P>
{
    private static readonly List<Type> s_pluginList = new List<Type>();
    private static volatile bool s_inited = false;

    /// <summary>
    /// 注册已实现插件基类的类型
    /// </summary>
    /// <typeparam name="P2"></typeparam>
    public static void RegisterPlugin<P2>()
    {
        RegisterPlugin(typeof(P2));
    }

    /// <summary>
    /// 注册已实现插件基类的类型
    /// </summary>
    /// <param name="pluginType"></param>
    public static void RegisterPlugin(Type pluginType)
    {
        if( pluginType == null )
            throw new ArgumentNullException(nameof(pluginType));

        if( pluginType.IsSubclassOf(typeof(P)) == false )
            throw new ArgumentOutOfRangeException(nameof(pluginType), $"将要注册的插件 {pluginType.Name} 不符合基类要求。");

        if( pluginType.CanNew() == false )
            throw new ArgumentException($"将要注册的插件 {pluginType.Name} 没有公开无参的构造方法。");

        if( s_inited )
            throw new InvalidOperationException("RegisterPlugin方法只允许在程序初始化时调用。");

        s_pluginList.Add(pluginType);
    }


    /// <summary>
    /// 创建插件对象列表
    /// </summary>
    /// <returns></returns>
    public static List<P> CreatePlugins()
    {
        if( s_inited == false )
            s_inited = true;

        List<P> list = new List<P>(s_pluginList.Count);

        foreach( var t in s_pluginList ) {
            P plugin = (P)Activator.CreateInstance(t);
            list.Add(plugin);
        }

        return list;
    }
}
