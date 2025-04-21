namespace ClownFish.Data.MultiDB.MySQL;

/// <summary>
/// MySQL专属工具类
/// </summary>
internal static class MySqlProviderUtils
{
    internal static int CurrentProviderFlag { get; private set; }

    /// <summary>
    /// 注册 MySQL 客户端提供者
    /// </summary>
    /// <param name="flag">0 = auto / 1 = MySql.Data / 2 = MySqlConnector / 3 = both</param>
    /// <returns></returns>
    public static void RegisterProvider(int flag = 0)
    {
        if( flag == 0 ) {
            
            // 读取本地配置参数，决定使用哪个客户端
            flag = LocalSettings.GetInt("MySqlClientProviderSupport", 0);

            // 如果没有配置，就根据项目引用的 DLL 来判断
            if( flag == 0 ) {
                string[] asmList = AsmHelper.GetCurrentDomainAssemblies().Select(x => x.GetName().Name).ToArray();

                if( asmList.Contains("MySqlConnector") )
                    flag = 2;
                else if( asmList.Contains("MySql.Data") )
                    flag = 1;
                else
                    //throw new FileNotFoundException("没有找到MySQL客户端类库 MySqlConnector.dll or MySql.Data.dll ！");
                    flag = -1;
            }
        }

        switch( flag ) {
            case -1:
                break;  // 当前程序不使用 mysql 数据库，所以不用注册提供者

            case 1: {
                    DbClientFactory.RegisterProvider(DatabaseClients.MySqlClient, MySqlDataClientProvider.Instance);
                    break;
                }

            case 2: {
                    DbClientFactory.RegisterProvider(DatabaseClients.MySqlClient, MySqlConnectorClientProvider.Instance);
                    break;
                }

            case 3: {
                    DbClientFactory.RegisterProvider("MySql.Data", MySqlDataClientProvider.Instance);
                    DbClientFactory.RegisterProvider("MySqlConnector", MySqlConnectorClientProvider.Instance);

                    DbClientFactory.RegisterProvider(DatabaseClients.MySqlClient, MySqlConnectorClientProvider.Instance);  // 放在“后面”注册
                    break;
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(flag), "参数flag的取值超出有效范围(1~3)，当前值：" + flag.ToString());
        }

        CurrentProviderFlag = flag;
    }


    //public static string GetCurrentProviderName()
    //{
    //    if( CurrentProviderFlag == 1 || CurrentProviderFlag  == 2 ) {
    //        var provider = DbClientFactory.GetDbProviderFactory(DatabaseClients.MySqlClient);
    //        return provider.GetType().Namespace;
    //    }

    //    if( CurrentProviderFlag == 3 ) {
    //        return "MySqlConnector; MySql.Data";
    //    }

    //    if( CurrentProviderFlag == -1 )
    //        return "NotUse";

    //    return "UnKnow";
    //}




}
