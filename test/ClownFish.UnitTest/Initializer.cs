﻿global using System.Data.SqlClient;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;
using System.Data.Common;
using ClownFish.UnitTest.Data.Events;
using System.Threading;
using ClownFish.UnitTest.Log;
using ClownFish.UnitTest.Data.MultiDB;
using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest
{
    [TestClass]
    public class Initializer
    {
        [AssemblyInitialize]
        public static void InitRuntime(TestContext context)
        {
            AsmHelper.EntryAssembly = typeof(Initializer).Assembly;
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

            ClownFishInit.InitDAL();

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
            // 等待 HttpWriter的操作
            System.Threading.Thread.Sleep(2000);
        }


    }
}
