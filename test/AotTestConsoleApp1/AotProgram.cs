global using AotTestConsoleApp1.TestCase;
global using AotTestConsoleApp1.Utils;

using ClownFish.Hosting;

[assembly: ClownFish.Data.EntityAssembly]

namespace AotTestConsoleApp1;

internal class Program
{
    static async Task Main(string[] args)
    {
        Exception error = null;
        try {
            ConsoleAppStarter.Run(new AotTestAppStartup());

            ShowAsmList();
            Directory.CreateDirectory("temp");
            await TestDAL.Run();
            await TestCache.Run();
            await TestCompression.Run();
            await TestCryptography.Run();
            await TestConfig.Run();
            await TestFileIO.Run();
            await TestJwt.Run();
            await TestLog.Run();
            await TestHttpClient.Run();
            await TestMMQ.Run();
        }
        catch( Exception ex ) {
            error = ex;
        }

        await Task.Delay(3000);

        if( error != null ) {
            Console2.Error(error);

        }
        else {
            Console2.WriteLine("\r\n\r\n");
            Console2.WriteLine("--------------------- 所有测试用例执行成功 --------------------------");
        }
        Console.ReadLine();
    }

    private static void ShowAsmList()
    {
        Console2.WriteLine("======================================================================");
        Assembly[] asmList = AppDomain.CurrentDomain.GetAssemblies();
        foreach( Assembly asm in asmList.OrderBy(x => x.FullName) ) {
            Console2.WriteLine(asm.FullName);
        }
        Console2.WriteLine("======================================================================");
    }
}


public class AotTestAppStartup : ConsoleAppStartup
{
    public override bool AutoInitDAL => true;
    public override bool AutoInitLog => true;
    public override bool AutoInitTracing => true;

    public override bool WaitToEnd => false;


    public override void BeforeClownFishInit()
    {
        LoadDatabaseClientDlls();
        ClownFish.Log.LogHelper.RegisterFilter(TestLog.Filter);
    }

    private static bool LoadDatabaseClientDlls()
    {
        // 下面代码可以确保相关DLL能加载引用

#pragma warning disable CS0618 // 类型或成员已过时
        DbProviderFactory factory1 = System.Data.SqlClient.SqlClientFactory.Instance;
#pragma warning restore CS0618 // 类型或成员已过时
        //DbProviderFactory factory1 = Microsoft.Data.SqlClient.SqlClientFactory.Instance;

        DbProviderFactory factory2 = MySqlConnector.MySqlConnectorFactory.Instance;
        DbProviderFactory factory3 = Npgsql.NpgsqlFactory.Instance;
        //DbProviderFactory factory4 = System.Data.SQLite.SQLiteFactory.Instance;

        return factory1 != null && factory2 != null && factory3 != null;  // && factory4 != null;
    }

}
