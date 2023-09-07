using ClownFish.Base.Config.Models;

namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class AppConfigurationTest
{
#if NETCOREAPP
    [TestMethod]
    public void Test_LoadFromConfigManager()
    {
        AppConfiguration config = AppConfiguration.LoadFromSysConfiguration();

        // 因为当前测试项目是基于 .net core 3.1 所以结果一定是 null
        Assert.IsNull(config);

    }
#endif


    [TestMethod]
    public void Test_ctor()
    {
        AppConfiguration config = new AppConfiguration();
        config.AppSettings = new[] {
                new AppSetting { Key = "key1", Value = "123" },
                new AppSetting { Key = "key2", Value = null },
                new AppSetting { Key = null, Value = "234" },   // 无效数据
            };
        config.ConnectionStrings = new[] {
                new ConnectionStringSetting{Name = "x1", ConnectionString="xxxx1"},
                new ConnectionStringSetting{Name = "x2", ConnectionString=""},       // 无效数据
                new ConnectionStringSetting{Name = "", ConnectionString="xxxx333"}   // 无效数据
            };

        config.DbConfigs = new[] {
                new XmlDbConfig { Name = "s1", Server="localhost1" },
                new XmlDbConfig { Name = "s2", Server="" },          // 无效数据
                new XmlDbConfig { Name = "", Server="localhost3" },  // 无效数据
            };


        AppConfigObject appConfig = new AppConfigObject(config);

        Dictionary<string, string> settings = (Dictionary<string, string>)appConfig.GetFieldValue("_settings");
        Dictionary<string, ConnectionStringSetting> conns = (Dictionary<string, ConnectionStringSetting>)appConfig.GetFieldValue("_conns");
        Dictionary<string, DbConfig> dbConfigs = (Dictionary<string, DbConfig>)appConfig.GetFieldValue("_dbConfigs");

        Assert.AreEqual(2, settings.Count);
        Assert.AreEqual(1, conns.Count);
        Assert.AreEqual(1, dbConfigs.Count);

        Assert.AreEqual("123", appConfig.GetSetting("key1"));
        Assert.AreEqual("", appConfig.GetSetting("key2"));
        Assert.IsNull(appConfig.GetSetting("key3"));

        Assert.IsNotNull(appConfig.GetConnectionString("x1"));
        Assert.IsNull(appConfig.GetConnectionString("x2"));

        Assert.IsNotNull(appConfig.GetDbConfig("s1"));
        Assert.IsNull(appConfig.GetDbConfig("s2"));
    }

    [TestMethod]
    public void Test_CorrectData()
    {
        AppConfiguration conf = new AppConfiguration();
        Assert.IsNull(conf.AppSettings);
        Assert.IsNull(conf.ConnectionStrings);
        Assert.IsNull(conf.DbConfigs);

        conf.CorrectData();
        Assert.IsNotNull(conf.AppSettings);
        Assert.IsNotNull(conf.ConnectionStrings);
        Assert.IsNotNull(conf.DbConfigs);
    }


    [TestMethod]
    public void Test_LoadFromFile()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = AppConfiguration.LoadFromFile("");
        });


        MyAssert.IsError<FileNotFoundException>(() => {
            _ = AppConfiguration.LoadFromFile("abc.xml", true);
        });

        Assert.IsNull(AppConfiguration.LoadFromFile("abc.xml", false));

        Assert.IsNotNull(AppConfiguration.LoadFromFile("ClownFish.App.config", true));
    }

}
