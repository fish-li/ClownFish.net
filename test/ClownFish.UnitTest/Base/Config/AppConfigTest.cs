using ClownFish.Base.Config.Models;

namespace ClownFish.UnitTest.Base.Config;

[TestClass]
public class AppConfigTest
{
    // https://learn.microsoft.com/zh-cn/dotnet/core/testing/unit-testing-mstest-writing-tests-attributes?view=vs-2022#class-level

    [ClassCleanup]
    public static void ClassCleanup() // Starting with MSTest 3.8, it can be ClassCleanup(TestContext testContext)
    {
        FieldInfo field = typeof(AppConfig).GetField("s_inited", BindingFlags.Static | BindingFlags.NonPublic);
        field.SetValue(null, false);
        AppConfig.Init();
    }


    [TestMethod]
    public void Test_GetSetting()
    {
        string value1 = LocalSettings.GetSetting("key1");
        Assert.AreEqual("abcd", value1);


        string value111 = LocalSettings.GetSetting("key11111111", "11111111111111111");
        Assert.AreEqual("11111111111111111", value111);


        int value2 = LocalSettings.GetUInt("key2");
        Assert.AreEqual(1234, value2);


        int value222 = LocalSettings.GetUInt("key222222222", 222222222);
        Assert.AreEqual(222222222, value222);

        Assert.IsNull(LocalSettings.GetSetting("xxx_url"));
        Assert.AreEqual("http://www.abc.com", AppConfig.GetSetting("xxx_url"));
    }


    //[TestMethod]
    //public void Test_Compatibility()
    //{
    //    Assert.AreEqual("123456", LocalSettings.GetSetting("aa.bb.cc"));
    //    Assert.AreEqual("123456", LocalSettings.GetSetting("aa_bb_cc"));
    //    Assert.IsNull(LocalSettings.GetSetting("aa_bb.cc"));

    //    var conn1 = AppConfig.GetConnectionString("sqlserver.2");
    //    var conn2 = AppConfig.GetConnectionString("sqlserver_2");
    //    Assert.IsNotNull(conn1);
    //    Assert.IsNotNull(conn2);
    //    Assert.AreEqual(conn1.ToString(), conn2.ToString());

    //    var db1 = AppConfig.GetDbConfig("dm.2");
    //    var db2 = AppConfig.GetDbConfig("dm_2");
    //    Assert.IsNotNull(db1);
    //    Assert.IsNotNull(db2);
    //    Assert.AreEqual(db1.ToJson(), db2.ToJson());

    //    Assert.AreEqual("a5431626-00df-44bd-bb35-f8f108f9ccfa", LocalSettings.GetSetting("Environment.Key"));
    //    Assert.AreEqual("a5431626-00df-44bd-bb35-f8f108f9ccfa", LocalSettings.GetSetting("Environment_Key"));

    //    Assert.AreEqual("2", LocalSettings.GetSetting("ClownFish.CacheDictionary.ExpirationScanFrequency"));
    //    Assert.AreEqual("2", LocalSettings.GetSetting("ClownFish_CacheDictionary_ExpirationScanFrequency"));
    //}

    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetSetting_ArgumentNullException1()
    {
        var x = AppConfig.GetSetting(string.Empty);
    }
    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetSetting_ArgumentNullException2()
    {
        var x = AppConfig.GetSetting(null);
    }


    [TestMethod]
    public void Test_GetConnectionStrings()
    {
        string keys = string.Join(",", (from x in AppConfig.GetAccessor().GetConfObject().ConnectionStrings select x.Name).ToArray());
        string[] values = (from x in AppConfig.GetAccessor().GetConfObject().ConnectionStrings select x.ConnectionString).ToArray();


        Assert.IsTrue(keys.Contains("sqlserver"));
        Assert.IsTrue(keys.Contains("mysql"));
    }

    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetConnectionStrings_ArgumentNullException1()
    {
        var x = AppConfig.GetConnectionString(string.Empty);
    }
    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetConnectionStrings_ArgumentNullException2()
    {
        var x = AppConfig.GetConnectionString(null);
    }

    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_AppConfigObject_ctor_ArgumentNullException1()
    {
        var x = new AppConfigAccessor(null);
    }



    [TestMethod]
    public void Test_LoadFromXml()
    {
        string filePath = PathUtils.GetFileAbsolutePath("ClownFish.Appconfig.xml");
        string xml = File.ReadAllText(filePath, Encoding.UTF8);

        AppConfig.ReLoadFromString(xml, "xml");
        AppConfiguration config1 = AppConfig.GetAccessor().GetConfObject();

        Assert.IsNotNull(config1);
        Assert.AreEqual("00abcd", config1.AppSettings.First(x => x.Key == "key1").Value);
        Assert.AreEqual("001234", config1.AppSettings.First(x => x.Key == "key2").Value);


        DebugReportBlock block = AppConfig.GetDebugReportBlock();
        string text = block.ToString2();
        Assert.IsTrue(text.Contains("key1=00abcd"));
        Assert.IsTrue(text.Contains("key2=001234"));

        // 无效参数，忽略调用
        AppConfig.ReLoadFromString(null, "ini");
    }




    [TestMethod]
    public void Test_GetConnectionString()
    {
        ConnectionStringSetting settings = AppConfig.GetConnectionString("sqlserver");
        Assert.AreEqual("System.Data.SqlClient", settings.ProviderName);
        Assert.IsTrue(settings.ConnectionString.IndexOf(@"database=MyNorthwind") > 0);


        MyAssert.IsError<ArgumentNullException>(() => {
            _ = AppConfig.GetConnectionString("");
        });
    }

    [TestMethod]
    public void Test_GetDbConfig()
    {
        DbConfig dbConfig = AppConfig.GetDbConfig("m1");
        Assert.AreEqual(DatabaseType.MySQL, dbConfig.DbType);


        MyAssert.IsError<ArgumentNullException>(() => {
            _ = AppConfig.GetDbConfig("");
        });
    }

    [TestMethod]
    public void Test_GetKeys()
    {
        string[] settingsNames = AppConfig.GetKeys(1);
        string[] connNames = AppConfig.GetKeys(2);
        string[] dbNames = AppConfig.GetKeys(3);
        string[] names = AppConfig.GetKeys(4);

        Assert.AreEqual(0, names.Length);
        Assert.IsTrue(settingsNames.Contains("ConfigServiceUrl"));
        Assert.IsTrue(settingsNames.Contains("key_test_setting"));

        Assert.IsTrue(connNames.Contains("mysql2"));
        Assert.IsTrue(connNames.Contains("sqlserver2"));

        Assert.IsTrue(dbNames.Contains("pg1"));
        Assert.IsTrue(dbNames.Contains("dm1"));
    }

    [TestMethod]
    public void Test_init()
    {
        FieldInfo field = typeof(AppConfig).GetField("s_inited", BindingFlags.Static | BindingFlags.NonPublic);

        field.SetValue(null, false);
        DebugReportBlock block = AppConfig.GetDebugReportBlock();
        Assert.IsNotNull(block);

        field.SetValue(null, false);
        string s1 = AppConfig.GetSetting("key_test_setting");
        Assert.AreEqual("123456789", s1);

        field.SetValue(null, false);
        ConnectionStringSetting connection = AppConfig.GetConnectionString("mysql2");
        Assert.IsNotNull(connection);
        Assert.AreEqual("MySql.Data.MySqlClient", connection.ProviderName);

        field.SetValue(null, false);
        DbConfig config = AppConfig.GetDbConfig("pg1");
        Assert.IsNotNull(config);
        Assert.AreEqual("PgSqlHost", config.Server);

        bool value2 = (bool)field.GetValue(null);
        Assert.IsTrue(value2);
    }

    [TestMethod]
    public void Test_SetAppConfigFileName()
    {
        typeof(AppConfig).SetFieldValue("s_inited", false);

        string path1 = AppConfig.GetAppConfigFilePath();
        Console.WriteLine(path1);
        Assert.IsTrue(path1.EndsWith1("ClownFish.UnitTest.config.ini"));

        AppConfig.SetAppConfigFileName("111.conf");
        string path2 = AppConfig.GetAppConfigFilePath();
        Console.WriteLine(path2);
        Assert.IsTrue(path2.EndsWith1("111.conf"));

        AppConfig.SetAppConfigFileName(null);
        string path3 = AppConfig.GetAppConfigFilePath();
        Assert.AreEqual(path1, path3);
    }

    [TestMethod]
    public void Test_GetDefaultAppconfigFilePath()
    {
        string path1 = AppConfig.GetDefaultAppconfigFilePath(".appconfig");
        Console.WriteLine(path1);
        Assert.IsTrue(path1.EndsWith1("ClownFish.UnitTest.appconfig"));
    }

}
