namespace ClownFish.Data.MultiDB.MsSQL;

internal static class MsSqlProviderUtils
{
    internal static string CurrentProviderName { get; private set; } = "System.Data.SqlClient";

    /// <summary>
    /// 注册 SQLSERVER 客户端提供者
    /// </summary>
    /// <param name="flag">0 = auto / 1 = System.Data.SqlClient / 2 = Microsoft.Data.SqlClient </param>
    public static void RegisterProvider(int flag = 0)
    {
        // 在 .net framework 环境下，System.Data.SqlClient 会自动注册，所以不需要做什么
#if NETCOREAPP

        // 在 .net core/5/... 下面有2个选择: System.Data.SqlClient 和 Microsoft.Data.SqlClient

        if( flag == 0 ) {
            string[] asmList = AsmHelper.GetCurrentDomainAssemblies().Select(x => x.GetName().Name).OrderBy(x => x).ToArray();

            if( asmList.Contains("Microsoft.Data.SqlClient") ) {
                DbClientFactory.RegisterProvider(DatabaseClients.SqlClient2, MsSqlClientProvider2.Instance);
                CurrentProviderName = "Microsoft.Data.SqlClient";
            }

            if( asmList.Contains("System.Data.SqlClient") ) {
                DbClientFactory.RegisterProvider(DatabaseClients.SqlClient, MsSqlClientProvider.Instance);
                CurrentProviderName = "System.Data.SqlClient";
            }
        }

        if( flag == 1 ) {
            DbClientFactory.RegisterProvider(DatabaseClients.SqlClient, MsSqlClientProvider.Instance);
            CurrentProviderName = "System.Data.SqlClient";
        }

        if( flag == 2 ) {
            DbClientFactory.RegisterProvider(DatabaseClients.SqlClient2, MsSqlClientProvider2.Instance);
            CurrentProviderName = "Microsoft.Data.SqlClient";
        }
#endif
    }
}


// 2个SqlClient的已知问题

// 1，System.Data.SqlClient 和 <InvariantGlobalization>true</InvariantGlobalization> 不能一起使用
//    会出现异常： Unhandled exception. System.InvalidOperationException: Internal connection fatal error.

// 2，Microsoft.Data.SqlClient 和 <InvariantGlobalization>true</InvariantGlobalization> 不能一起使用
//    会出现异常： Unhandled exception. System.Globalization.CultureNotFoundException: Only the invariant culture is supported in globalization-invariant mode

// 3，Microsoft.Data.SqlClient 会强制使用SSL并校验证书，
//    会出现异常：Unhandled exception. Microsoft.Data.SqlClient.SqlException (0x80131904): A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - 证书链是由不受信任的颁发机构颁发的。)
//    此问题可解决，请 “搜索网络”

