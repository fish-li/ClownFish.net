namespace ClownFish.Data.MultiDB.MsSQL;

internal static class MsSqlProviderUtils
{
    internal static string SqlServerDefaultProviderName { get; private set; } = "System.Data.SqlClient";

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
                SqlServerDefaultProviderName = "Microsoft.Data.SqlClient";
            }

            if( asmList.Contains("System.Data.SqlClient") ) {
                DbClientFactory.RegisterProvider(DatabaseClients.SqlClient, MsSqlClientProvider.Instance);
                SqlServerDefaultProviderName = "System.Data.SqlClient";
            }
        }

        if( flag == 1 ) {
            DbClientFactory.RegisterProvider(DatabaseClients.SqlClient, MsSqlClientProvider.Instance);
        }

        if( flag == 2 ) {
            DbClientFactory.RegisterProvider(DatabaseClients.SqlClient2, MsSqlClientProvider2.Instance);
            SqlServerDefaultProviderName = "Microsoft.Data.SqlClient";
        }
#endif
    }
}
