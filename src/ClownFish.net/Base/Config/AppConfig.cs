using ClownFish.Base.Config.Models;

namespace ClownFish.Base;

/// <summary>
/// 用于读取 Appconfig 的工具类。
/// </summary>
public static class AppConfig
{
    private static string s_filename = null;
    private static bool s_inited = false;

    internal static bool Inited => s_inited;

#if NET9_0_OR_GREATER
    private static readonly Lock s_lock = new Lock();
#else
    private static readonly object s_lock = new object();
#endif

    private static AppConfigAccessor s_accessor;

    internal static AppConfigAccessor GetAccessor() => s_accessor;


#if NETCOREAPP    // 下面几个类型不参与裁剪，保留无参构造函数，确保可序列化
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppConfiguration))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppSetting))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ConnectionStringSetting))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DbConfig))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DatabaseType))]
    [UnconditionalSuppressMessage("AOT", "IL3050: Enum.GetValues")]
#endif
    internal static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {
                    InitConfig();
                }
            }
        }
    }

    private static void InitConfig()
    {
        string filePath = GetAppConfigFilePath();

        if( File.Exists(filePath) ) {
            Console2.Info("AppConfig filePath: " + filePath);
        }

        AppConfiguration config = AppConfiguration.LoadFromFile(filePath, false)
                                ?? AppConfiguration.LoadFromSysConfiguration()
                                ?? new AppConfiguration();

        s_accessor = new AppConfigAccessor(config);
        s_inited = true;
    }

    /// <summary>
    /// 根据一段 INI/XML 配置内容加载配置对象，
    /// 此方法不是线程安全的，必须在程序初始化时调用。
    /// </summary>
    /// <param name="text">配置内容</param>
    /// <param name="textType">文本类别，用于识别配置内容的格式，例如：ini, xml</param>
    public static void ReLoadFromString(string text, string textType)
    {
        if( text.IsNullOrEmpty() || textType.IsNullOrEmpty() )
            return;

        Console2.Info($"###### AppConfig 配置内容正在从 {textType} 文本中加载");

        AppConfiguration config = AppConfiguration.LoadFromString(text, textType)
                                ?? new AppConfiguration();

        s_accessor = new AppConfigAccessor(config);
        s_inited = true;
    }


    ///// <summary>
    ///// 获取一段配置文本的格式类别。
    ///// </summary>
    ///// <param name="text"></param>
    ///// <returns> -1: 格式未知,  0: 内容为空, 1: xml, 2: json, 3: ini</returns>
    //internal static int GetTextType(string text)
    //{
    //    if( text.IsNullOrEmpty() )
    //        return 0;

    //    using StringReader reader = new StringReader(text);

    //    while( true ) {
    //        string line = reader.ReadLine();
    //        if( line == null )
    //            break;

    //        line = line.Trim();
    //        if( line.IsNullOrEmpty() )
    //            continue;

    //        if( line[0] == '<' && line[line.Length - 1] == '>' )
    //            return 1;   // XML 格式

    //        if( line[0] == '{' )
    //            return 2;   // JSON 格式

    //        if( line[0] == '#' || line[0] == ';'  // 注释行
    //            || (line[0] == '[' && line[line.Length - 1] == ']') )
    //            return 3;   // INI 格式
    //    }

    //    return -1;  // 未知格式
    //}

    /// <summary>
    /// 设置 Appconfig 的名称，此方法仅在初始化之前调用有效。 【强烈建议】：如果需要调用这个方法，那么这个调用放在程序运行的【第一行】
    /// </summary>
    /// <param name="filenName"></param>
    public static void SetAppConfigFileName(string filenName)
    {
        s_filename = filenName;
    }


    internal static string GetAppConfigFilePath()
    {
        // 1, 优先使用 明确指定 的文件名
        if( s_filename.HasValue() ) {
            return PathUtils.GetFileAbsolutePath(s_filename);
        }

        // 2, 根据程序入口程序集来确定 配置文件的名称
        // 假如当前程序是 abc.exe
        // 那么默认的配置文件名称为：abc.config.ini  or  abc.appconfig

        string filePath3 = GetDefaultAppconfigFilePath(".config.ini");

        if( EnvArgs0.IsAot ) {
            // 如果采用 NativeAOT 方式发布，则仅支持 INI 配置文件，不再支持 XML 配置文件
            // 此处不管此文件是否存在，因为没有别的选择了~~
            return filePath3;
        }

        if( File.Exists(filePath3) )
            return filePath3;


        string filePath2 = GetDefaultAppconfigFilePath(".Appconfig");
        if( File.Exists(filePath2) )
            return filePath2;


        // 3, 使用 【统一名称】的配置文件，兼容老版本的配置文件名称
        return PathUtils.GetFileAbsolutePath("ClownFish.App.config");  // TODO:以后废弃这个名称
    }

    internal static string GetDefaultAppconfigFilePath(string extName)
    {
        string exeName = AsmHelper.GetExeName();
        string confName = exeName + extName;
        return PathUtils.GetFileAbsolutePath(confName);
    }


    internal static DebugReportBlock GetDebugReportBlock()
    {
        if( s_inited == false )
            Init();

        return s_accessor.GetConfObject().GetDebugReportBlock();
    }


    /// <summary>
    /// 获取一个与指定名称匹配的appSetting配置参数值。
    /// </summary>
    /// <param name="name">参数名称，不区分大小写</param>
    /// <returns></returns>
    public static string GetSetting(string name)
    {
        if( s_inited == false )
            Init();

        return s_accessor.GetSetting(name);
    }



    /// <summary>
    /// 获取一个与指定名称匹配的connectionString配置
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ConnectionStringSetting GetConnectionString(string name)
    {
        if( s_inited == false )
            Init();

        return s_accessor.GetConnectionString(name);
    }


    /// <summary>
    /// 获取一个数据库连接配置
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static DbConfig GetDbConfig(string name)
    {
        if( s_inited == false )
            Init();

        return s_accessor.GetDbConfig(name);
    }



    // 说明：提供 GetKeys 方法，而不是直接返回 AppConfiguration 对象是不希望在运行时配置参数被修改

    /// <summary>
    /// 获取所有的配置参数名称
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static string[] GetKeys(int kind)
    {
        AppConfiguration configuration = s_accessor.GetConfObject();

        if( kind == 1 ) {
            return configuration.AppSettings?.Select(x => x.Key)?.ToArray() ?? Empty.Array<string>();
        }
        else if( kind == 2 ) {
            return configuration.ConnectionStrings?.Select(x => x.Name)?.ToArray() ?? Empty.Array<string>();
        }
        else if( kind == 3 ) {
            return configuration.DbConfigs?.Select(x => x.Name)?.ToArray() ?? Empty.Array<string>();
        }
        else {
            return Empty.Array<string>();
        }
    }
}
