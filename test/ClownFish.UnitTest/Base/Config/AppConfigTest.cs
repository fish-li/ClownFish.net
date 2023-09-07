using ClownFish.Base.Config.Models;

namespace ClownFish.UnitTest.Base.Config;

[TestClass]
public class AppConfigTest
{
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
    }


    [TestMethod]
    public void Test_Compatibility()
    {
        Assert.AreEqual("123456", LocalSettings.GetSetting("aa.bb.cc"));
        Assert.AreEqual("123456", LocalSettings.GetSetting("aa_bb_cc"));
        Assert.IsNull(LocalSettings.GetSetting("aa_bb.cc"));

        var conn1 = AppConfig.GetConnectionString("sqlserver.2");
        var conn2 = AppConfig.GetConnectionString("sqlserver_2");
        Assert.IsNotNull(conn1);
        Assert.IsNotNull(conn2);
        Assert.AreEqual(conn1.ToString(), conn2.ToString());

        var db1 = AppConfig.GetDbConfig("dm.2");
        var db2 = AppConfig.GetDbConfig("dm_2");
        Assert.IsNotNull(db1);
        Assert.IsNotNull(db2);
        Assert.AreEqual(db1.ToJson(), db2.ToJson());

        Assert.AreEqual("a5431626-00df-44bd-bb35-f8f108f9ccfa", LocalSettings.GetSetting("Nebula.Environment.Key"));
        Assert.AreEqual("a5431626-00df-44bd-bb35-f8f108f9ccfa", LocalSettings.GetSetting("Nebula_Environment_Key"));

        Assert.AreEqual("2", LocalSettings.GetSetting("ClownFish.CacheDictionary.ExpirationScanFrequency"));
        Assert.AreEqual("2", LocalSettings.GetSetting("ClownFish_CacheDictionary_ExpirationScanFrequency"));
    }

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
        string keys = string.Join(",", (from x in AppConfig.GetConfigObject().GetConfiguration().ConnectionStrings select x.Name).ToArray());
        string[] values = (from x in AppConfig.GetConfigObject().GetConfiguration().ConnectionStrings select x.ConnectionString).ToArray();


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
        var x = new AppConfigObject(null);
    }



    [TestMethod]
    public void Test_LoadFromXml()
    {
        typeof(AppConfig).InvokeMember("s_configuration",
                                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });


        string filePath = ConfigHelper.GetFileAbsolutePath(AppConfig.ClownFishAppconfig);
        string xml = File.ReadAllText(filePath, Encoding.UTF8);

        AppConfig.ReLoadFromXml(xml);
        AppConfiguration config1 = AppConfig.GetConfigObject().GetConfiguration();

        Assert.IsNotNull(config1);
        Assert.AreEqual("abcd", config1.AppSettings.First(x => x.Key == "key1").Value);
        Assert.AreEqual("1234", config1.AppSettings.First(x => x.Key == "key2").Value);


        DebugReportBlock block = AppConfig.GetDebugReportBlock();
        string text = block.ToString2();
        Assert.IsTrue(text.Contains("key1 = abcd"));
        Assert.IsTrue(text.Contains("key2 = 1234"));


        MyAssert.IsError<ArgumentNullException>(() => {
            AppConfig.ReLoadFromXml(null);
        });
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


}
