using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base.Config.Models;

namespace ClownFish.UnitTest.Base.Config;

[TestClass]
public class AppConfigTest2
{
    // https://learn.microsoft.com/zh-cn/dotnet/core/testing/unit-testing-mstest-writing-tests-lifecycle#cleanup-phase

    [TestCleanup]
    public void TestCleanup()
    {
        typeof(AppConfig).SetFieldValue("s_inited", false);
        AppConfig.SetAppConfigFileName(null);
        AppConfig.Init();
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
    public void Test_SetAppConfigFileName()
    {
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
}
