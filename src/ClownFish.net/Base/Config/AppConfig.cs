using ClownFish.Base.Config.Models;

namespace ClownFish.Base;

/// <summary>
/// 用于读取 ClownFish.App.config 的工具类。
/// </summary>
public static class AppConfig
{
    internal static readonly string ClownFishAppconfig = "ClownFish.App.config";
    private static string s_filename = null;

    private static bool s_inited = false;
    private static readonly object s_lock = new object();

    private static AppConfigObject s_configuration;

    internal static AppConfigObject GetConfigObject() => s_configuration;

    internal static AppConfiguration GetAppConfiguration() => s_configuration.GetConfiguration();


    internal static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    string filePath = GetAppConfigFilePath();
                    Console2.Info("AppConfig filePath: " + filePath);

                    InitConfig(filePath);
                }
            }
        }
    }

    /// <summary>
    /// 设置 App.config 的名称。 【强烈建议】：这个方法的调用做为程序运行的 【第一行】代码。
    /// App.config 的查找过程：
    /// 1，优先查找当前方法指定的名称，
    /// 2，根据 程序入口程序集去查找，
    /// 3，使用【统一名称】的配置文件名称：ClownFish.App.config
    /// </summary>
    /// <param name="filenName"></param>
    public static void SetAppConfigFileName(string filenName)
    {
        if( s_inited )
            throw new InvalidOperationException("此时调用此方法无效（时机过晚），因为 AppConfig 已初始化完成！");

        s_filename = filenName;
    }


    internal static string GetAppConfigFilePath()
    {
        // 1, 优先使用 明确指定 的文件名
        if( s_filename.HasValue() ) {
            return ConfigHelper.GetFileAbsolutePath(s_filename);
        }

        // 2, 根据程序入口程序集来确定 配置文件的名称
        // 假如当前程序是 abc.exe or abc.dll
        // 那么默认的配置文件名称为：abc.appconfig
        string filePath2 = GetDefaultAppconfigFilePath();
        if( File.Exists(filePath2) )
            return filePath2;

        // 3, 使用 【统一名称】的配置文件
        return ConfigHelper.GetFileAbsolutePath(ClownFishAppconfig);
    }

    internal static string GetDefaultAppconfigFilePath()
    {
        string asmName = Path.GetFileNameWithoutExtension(AsmHelper.GetExeFilePath());
        string confName = asmName + ".Appconfig";
        return ConfigHelper.GetFileAbsolutePath(confName);
    }

    internal static void InitConfig(string filePath)
    {
        AppConfiguration config = AppConfiguration.LoadFromFile(filePath, false)
                                ?? AppConfiguration.LoadFromSysConfiguration()
                                ?? new AppConfiguration();

        s_configuration = new AppConfigObject(config);
        s_inited = true;
    }


    /// <summary>
    /// 根据一段XML配置内容加载配置对象，
    /// 此方法不是线程安全的，必须在程序初始化时调用。
    /// </summary>
    /// <param name="xml"></param>
    public static void ReLoadFromXml(string xml)
    {
        Console2.Info("###### AppConfig 配置内容正在从XML文本中加载");

        AppConfiguration config = AppConfiguration.LoadFromXml(xml);
        s_configuration = new AppConfigObject(config);
        s_inited = true;
    }


    internal static DebugReportBlock GetDebugReportBlock()
    {
        if( s_inited == false )
            Init();

        return s_configuration.GetConfiguration().GetDebugReportBlock();
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

        return s_configuration.GetSetting(name);
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

        return s_configuration.GetConnectionString(name);
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

        return s_configuration.GetDbConfig(name);
    }

    // 说明：提供 GetKeys 方法，而不是直接返回 AppConfiguration 对象是不希望在运行时配置参数被修改

    /// <summary>
    /// 获取所有的配置参数名称
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static string[] GetKeys(int kind)
    {
        AppConfiguration configuration = GetAppConfiguration();

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
