global using System.Data.SqlClient;
global using ClownFish.UnitTest._Common;
global using ClownFish.Http.MockTest;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using ClownFish.UnitTest.Base;
using ClownFish.UnitTest.Data.Events;
using ClownFish.UnitTest.Data.Models;
using ClownFish.UnitTest.Data.MultiDB;
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


        ClownFishInit.InitBase();
        ThreadPool.SetMinThreads(100, 1000);

        // 设置重试次数
        ClownFish.Base.Retry.Default.Count = 2;
        ClownFish.Base.Retry.Default.WaitMillisecond = 100;

        InitClownFishData();

        ClownFish.Log.LogHelper.RegisterFilter(LogHelperTest.Filter);
        ClownFishInit.InitLog("ClownFish.Log.config");

        DbContextEventTest.Init(context);
        ClownFish.Log.Logging.DbLogger.Init();
        ClownFish.Log.Logging.HttpClientLogger.Init();

#if NETCOREAPP
        // support Encoding.GetEncoding("GB2312")
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
    }

    private static void InitClownFishData()
    {
        LoadDatabaseClientDlls();

        

        //string dllName = "ClownFish.UnitTest.EntityProxy.dll";
        //string dllOutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", dllName);

        ClownFish.Data.Initializer.Instance
                        //.RegisterSqlServerProvider()
                        //.RegisterMySqlProvider()
                        //.RegisterPostgreSqlProvider()
                        //.RegisterSQLiteProvider()
                        .RegisterClientProvider(XxxBaseClientProvider.Name, XxxBaseClientProvider.Instance)
                        .SetListInitLength(60)
                        .InitConnection()
                        .AddDataFieldTypeHandler(typeof(System.Drawing.Point), new PointDataFieldTypeHandler())
                        .AddDataFieldTypeHandler(typeof(EncSaveString), new EncSaveStringDataFieldTypeHandler())
                        //.LoadXmlCommandFromDirectory()
                        .LoadXmlCommandFromText(string.Empty);
        //.CompileAllEntityProxy(dllOutPath);

        ClownFishInit.InitDAL();

#if NETCOREAPP
        ClownFish.Data.Initializer.Instance.RegisterDamengProvider();
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


    [AssemblyCleanup()]
    public static void AssemblyCleanup()
    {
        ClownFishInit.ApplicationEnd();

        // 等待 HttpWriter的操作
        System.Threading.Thread.Sleep(2000);
    }


}
