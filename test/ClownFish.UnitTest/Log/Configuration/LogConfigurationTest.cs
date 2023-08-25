using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Log.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Configuration;
[TestClass]
public class LogConfigurationTest
{
    [TestMethod]
    public void Test_LoadFromFile()
    {
        LogConfiguration conf = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);
        Assert.IsNotNull(conf);

        LogConfiguration conf2 = LogConfiguration.LoadFromFile("ClownFish.Log22222.config", false);
        Assert.IsNull(conf2);

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = LogConfiguration.LoadFromFile("", false);
        });

        MyAssert.IsError<FileNotFoundException>(() => {
            _ = LogConfiguration.LoadFromFile("ClownFish.Log22222.config", true);
        });
    }


    [TestMethod]
    public void Test_LoadFromXml()
    {
        string xml = RetryFile.ReadAllText("ClownFish.Log.config");
        LogConfiguration conf = LogConfiguration.LoadFromXml(xml);
        Assert.IsNotNull(conf);

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = LogConfiguration.LoadFromXml("");
        });
    }


    [TestMethod]
    public void Test_MegerConfig()
    {
        LogConfiguration conf1 = new LogConfiguration();
        LogConfiguration conf2 = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);
        LogConfiguration conf3 = LogConfiguration.MegerConfig(conf1, conf2);

        Assert.IsNotNull(conf3.Performance);
    }

    [TestMethod]
    public void Test_OverrideWriters()
    {
        LogConfiguration conf = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);

        Assert.AreEqual("ClownFish.Log.Logging.OprLog, ClownFish.net", conf.Types[0].DataType);
        Assert.AreEqual("ClownFish.Log.Logging.InvokeLog, ClownFish.net", conf.Types[1].DataType);

        Assert.AreEqual("Xml,Json", conf.Types[0].Writers);
        Assert.AreEqual("Xml,Json,Json2,http", conf.Types[1].Writers);

        conf.OverrideWriters("InvokeLog=Rabbit;OprLog=es,Json;*=es");

        Assert.AreEqual("es,Json", conf.Types[0].Writers);
        Assert.AreEqual("Rabbit", conf.Types[1].Writers);
    }

    [TestMethod]
    public void Test_OverrideWriters2()
    {
        LogConfiguration conf = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);

        Assert.AreEqual("ClownFish.Log.Logging.OprLog, ClownFish.net", conf.Types[0].DataType);
        Assert.AreEqual("ClownFish.Log.Logging.InvokeLog, ClownFish.net", conf.Types[1].DataType);

        Assert.AreEqual("Xml,Json", conf.Types[0].Writers);
        Assert.AreEqual("Xml,Json,Json2,http", conf.Types[1].Writers);

        conf.OverrideWriters("InvokeLog=xml");   // 只修改一个

        Assert.AreEqual("Xml,Json", conf.Types[0].Writers);
        Assert.AreEqual("xml", conf.Types[1].Writers);
    }

    [TestMethod]
    public void Test_OverrideWriters3()
    {
        LogConfiguration conf = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);

        Assert.AreEqual("ClownFish.Log.Logging.OprLog, ClownFish.net", conf.Types[0].DataType);
        Assert.AreEqual("ClownFish.Log.Logging.InvokeLog, ClownFish.net", conf.Types[1].DataType);

        Assert.AreEqual("Xml,Json", conf.Types[0].Writers);
        Assert.AreEqual("Xml,Json,Json2,http", conf.Types[1].Writers);

        conf.OverrideWriters("OprLog=xml");   // 只修改一个

        Assert.AreEqual("xml", conf.Types[0].Writers);
        Assert.AreEqual("Xml,Json,Json2,http", conf.Types[1].Writers);
    }


    [TestMethod]
    public void Test_OverrideWriters4()
    {
        LogConfiguration conf = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);

        conf.OverrideWriters("*=http");   // 修改所有

        Assert.AreEqual("http", conf.Types[0].Writers);
        Assert.AreEqual("http", conf.Types[1].Writers);
    }

    [TestMethod]
    public void Test_OverrideWriters5()
    {
        LogConfiguration conf = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);

        Assert.AreEqual("ClownFish.Log.Logging.OprLog, ClownFish.net", conf.Types[0].DataType);
        Assert.AreEqual("ClownFish.Log.Logging.InvokeLog, ClownFish.net", conf.Types[1].DataType);

        Assert.AreEqual("Xml,Json", conf.Types[0].Writers);
        Assert.AreEqual("Xml,Json,Json2,http", conf.Types[1].Writers);

        conf.OverrideWriters("xxxxxxxxx=xml");   // 修改不存在的名称

        Assert.AreEqual("Xml,Json", conf.Types[0].Writers);
        Assert.AreEqual("Xml,Json,Json2,http", conf.Types[1].Writers);
    }



    [TestMethod]
    public void Test_TryUpdateFromLocalSetting()
    {
        LogConfiguration conf = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);

        EnvironmentVariables.Set("ClownFish_Log_Performance_HttpExecute", "1001");
        EnvironmentVariables.Set("ClownFish_Log_Performance_HandleMessage", "2001");
        EnvironmentVariables.Set("ClownFish_Log_TimerPeriod", "101");

        conf.TryUpdateFromLocalSetting();

        Assert.AreEqual(1001, conf.Performance.HttpExecute);
        Assert.AreEqual(2001, conf.Performance.HandleMessage);
        Assert.AreEqual(101, conf.TimerPeriod);
    }
}
