using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using System.Data.Common;
using ClownFish.UnitTest.Data.Events;
using System.Threading;
using ClownFish.UnitTest.Data.MultiDB;


// 这个项目只有一个用途：测试ClownFish.net是否支持 Microsoft.Data.SqlClient
// 而且为了和老项目保持一不致，选择了一个老版本

namespace ClownFish.UnitTest
{
    [TestClass]
    public class Initializer
    {
        [AssemblyInitialize]
        public static void InitRuntime(TestContext context)
        {
            AsmHelper.EntryAssembly = typeof(Initializer).Assembly;
            EnvironmentVariables.Set("RUNTIME_ENVIRONMENT", "FishDev");
            EnvironmentVariables.Set("ClownFish_Console2_Trace_Enabled", "1");
            EnvironmentVariables.Set("x1.y1.z1", "123");

            ClownFishInit.InitBase();
            ThreadPool.SetMinThreads(100, 1000);
            

            // 设置重试次数
            ClownFish.Base.Retry.Default.Count = 2;
            ClownFish.Base.Retry.Default.WaitMillisecond = 100;

            InitClownFishData();

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

            string dllName = "ClownFish.UnitTest.EntityProxy.dll";
            string dllOutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", dllName);

            ClownFish.Data.Initializer.Instance
                            .RegisterSqlServerProvider(2)
                            .SetListInitLength(60)
                            .InitConnection()
                            .LoadXmlCommandFromDirectory()
                            .LoadXmlCommandFromText(string.Empty)
                            .CompileAllEntityProxy(dllOutPath);


            // 用于输出所有执行的SQL语句及命令参数（实现项目中不需要这个步骤）
            ClownFishDataEventSubscriber.SubscribeEvent();
        }

        private static bool LoadDatabaseClientDlls()
        {
            // 下面代码可以确保相关DLL能在编译后复制到BIN目录

            DbProviderFactory factory1 = Microsoft.Data.SqlClient.SqlClientFactory.Instance;

            return factory1 != null;
        }
        

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            // 等待 HttpWriter的操作
            System.Threading.Thread.Sleep(2000);
        }


    }
}
