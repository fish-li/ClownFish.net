global using System.Data.SqlClient;
global using ClownFish.Http.MockTest;
global using ClownFish.UnitTest._Common;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Hosting;
using ClownFish.UnitTest.Data.Events;
using ClownFish.UnitTest.Data.Models;
using ClownFish.UnitTest.Data.MultiDB;
using ClownFish.UnitTest.Data.PostgreSQL;
using ClownFish.UnitTest.Log;

namespace ClownFish.UnitTest;

[TestClass]
public class Initializer
{
    [AssemblyInitialize]
    public static void InitRuntime(TestContext context)
    {
        AsmHelper.SetEntryAssembly(typeof(Initializer).Assembly);
        EnvironmentVariables.Set("MySqlClientProviderSupport", "3");
        EnvironmentVariables.Set("RUNTIME_ENVIRONMENT", "FishDev");
        EnvironmentVariables.Set("ClownFish_Console2_Trace_Enabled", "1");
        EnvironmentVariables.Set("x1.y1.z1", "123");
        EnvironmentVariables.Set("ClownFish_DebugReport_HideEnvNames", "api-key;xx-SecretKey;test_hide");
        EnvironmentVariables.Set("xxx_url", "");   // 它将屏蔽 App.config 中的同名配置项

        System.Environment.CurrentDirectory = Path.GetDirectoryName(typeof(Initializer).Assembly.Location);

        ConsoleAppStarter.Run(new UnitTestAppStartup());
    }

    

    [AssemblyCleanup()]
    public static void AssemblyCleanup()
    {
        ClownFishInit.ApplicationEnd();

        // 等待 HttpWriter的操作
        System.Threading.Thread.Sleep(2000);
    }


}


public class UnitTestAppStartup : ConsoleAppStartup
{
#if NET6_0_OR_GREATER
    public override bool WaitToEnd => false;
#endif

    public override void BeforeClownFishInit()
    {
        ThreadPool.SetMinThreads(100, 1000);

        // 设置重试次数
        ClownFish.Base.Retry.Default.Count = 2;
        ClownFish.Base.Retry.Default.WaitMillisecond = 100;


#if NETCOREAPP
        // support Encoding.GetEncoding("GB2312")
        //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
    }


    public override void ConfigTracing()
    {
        DbContextEventTest.Init();
        ClownFish.Log.Logging.DbLogger.Init();
        ClownFish.Log.Logging.HttpClientLogger.Init();

#if NETCOREAPP
        ClownFish.Log.Logging.HttpClientLogger2.Init();
#endif
    }

    public override void ConfigLog()
    {
        ClownFish.Log.LogHelper.RegisterFilter(LogHelperTest.Filter);
        ClownFishInit.InitLog("ClownFish.Log.config");
    }

    public override void ConfigDAL()
    {
        LoadDatabaseClientDlls();


        //string dllName = "ClownFish.UnitTest.EntityProxy.dll";
        //string dllOutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", dllName);

        ClownFish.Data.Initializer.Instance
                        //.RegisterSqlServerProvider()
                        //.RegisterMySqlProvider()
                        //.RegisterPostgreSqlProvider()
                        //.RegisterSQLiteProvider()
                        .RegisterClientProvider(XxxBaseClientProvider.ProviderName, XxxBaseClientProvider.Instance)
                        .SetListInitLength(60)
                        .InitConnection()
                        .AddDataFieldTypeHandler(typeof(System.Drawing.Point), new PointDataFieldTypeHandler())
                        .AddDataFieldTypeHandler(typeof(EncSaveString), new EncSaveStringDataFieldTypeHandler())
                        //.LoadXmlCommandFromDirectory()
                        .LoadXmlCommandFromText(string.Empty);
                        //.CompileAllEntityProxy(dllOutPath);

        ClownFishInit.InitDAL();

#if NET8_0_OR_GREATER
        ClownFish.Data.Initializer.Instance.RegisterDamengProvider();

        KingbaseESClientProvider.RegisterProvider();
#endif

        // 用于输出所有执行的SQL语句及命令参数（实现项目中不需要这个步骤）
        ClownFishDataEventSubscriber.SubscribeEvent();
    }

    private static bool LoadDatabaseClientDlls()
    {
        // 下面代码可以确保相关DLL能在编译后复制到BIN目录

        DbProviderFactory factory1 = MySql.Data.MySqlClient.MySqlClientFactory.Instance;
        DbProviderFactory factory2 = MySqlConnector.MySqlConnectorFactory.Instance;
        DbProviderFactory factory3 = Npgsql.NpgsqlFactory.Instance;
        DbProviderFactory factory4 = System.Data.SQLite.SQLiteFactory.Instance;

        return factory1 != null && factory2 != null && factory3 != null && factory4 != null;
    }
}
