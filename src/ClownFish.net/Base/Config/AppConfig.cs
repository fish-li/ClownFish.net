using ClownFish.Base.Config.Models;

namespace ClownFish.Base;

/// <summary>
/// 用于读取 ClownFish.App.config 的工具类。
/// </summary>
public static class AppConfig
{
    internal static readonly string ClownFishAppconfig = "ClownFish.App.config";

    private static bool s_inited = false;
    private static readonly object s_lock = new object();

    private static AppConfigObject s_configuration;

    internal static AppConfigObject GetConfigObject() => s_configuration;


    internal static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {
                    string filePath = ConfigHelper.GetFileAbsolutePath(ClownFishAppconfig);
                    InitConfig(filePath);
                }
            }
        }
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

}
