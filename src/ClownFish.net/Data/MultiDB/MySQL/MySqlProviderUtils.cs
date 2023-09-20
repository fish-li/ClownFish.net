namespace ClownFish.Data.MultiDB.MySQL;

/// <summary>
/// MySQL专属工具类
/// </summary>
public static class MySqlProviderUtils
{
    internal static int CurrentProviderFlag { get; private set; }

    /// <summary>
    /// 注册 MySQL 客户端提供者
    /// </summary>
    /// <param name="flag">MySql.Data = 1 / MySqlConnector = 2 / all = 3 / auto = 0</param>
    /// <returns></returns>
    public static void RegisterProvider(int flag)
    {
        if( flag == 0 ) {
            
            // 读取本地配置参数，决定使用哪个客户端
            flag = LocalSettings.GetInt("MySqlClientProviderSupport", 0);

            // 如果没有配置，就根据项目引用的 DLL 来判断
            if( flag == 0 ) {
                if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MySqlConnector.dll")) )
                    flag = 2;
                else if( File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MySql.Data.dll")) )
                    flag = 1;
                else
                    throw new FileNotFoundException("没有找到MySQL客户端类库 MySqlConnector.dll or MySql.Data.dll ！");
            }
        }

        switch( flag ) {
            case 1: {
                    DbClientFactory.RegisterProvider(DatabaseClients.MySqlClient, MySqlDataClientProvider.Instance);
                    break;
                }

            case 2: {
                    DbClientFactory.RegisterProvider(DatabaseClients.MySqlClient, MySqlConnectorClientProvider.Instance);
                    break;
                }

            case 3: {
                    DbClientFactory.RegisterProvider(DatabaseClients.MySqlClient, MySqlConnectorClientProvider.Instance);
                    // 注册下面2个用于对比测试
                    DbClientFactory.RegisterProvider("MySql.Data", MySqlDataClientProvider.Instance);
                    DbClientFactory.RegisterProvider("MySqlConnector", MySqlConnectorClientProvider.Instance);
                    break;
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(flag), "参数flag的取值超出有效范围(1~3)，当前值：" + flag.ToString());
        }

        CurrentProviderFlag = flag;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentProviderName()
    {
        if( CurrentProviderFlag == 1 || CurrentProviderFlag  == 2 ) {
            var provider = DbClientFactory.GetDbProviderFactory(DatabaseClients.MySqlClient);
            return provider.GetType().Namespace;
        }

        if( CurrentProviderFlag == 3 ) {
            return "MySqlConnector; MySql.Data";
        }

        return "UnKnow";
    }




}
