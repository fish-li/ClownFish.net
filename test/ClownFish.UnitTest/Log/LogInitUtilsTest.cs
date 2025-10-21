using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Log;
[TestClass]
public class LogInitUtilsTest
{
    [TestMethod]
    public void Test_0_InitLog()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ClownFishInit.InitLog((string)null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ClownFishInit.InitLog((LogConfiguration)null);
        });

        EnvironmentVariables.Set("ClownFish_Log_WritersMap", "*=NULL");
        EnvironmentVariables.Set("Show_ClownFish_Log_Config", "1");

        typeof(LogConfig).SetFieldValue("s_inited", false);
        ClownFishInit.InitLogAsDefault();
        ClownFishInit.InitLogAsDefault();


        typeof(LogConfig).SetFieldValue("s_inited", false);
        LogConfiguration config = LogConfig.LoadFromFile("ClownFish.Log.config", true);
        ClownFishInit.InitLog(config);
        ClownFishInit.InitLog(config);


        EnvironmentVariables.Set("ClownFish_Log_WritersMap", "");
        EnvironmentVariables.Set("Show_ClownFish_Log_Config", "1");
        typeof(LogConfig).SetFieldValue("s_inited", false);
        ClownFishInit.InitLog("ClownFish.Log.config");
        ClownFishInit.InitLog("ClownFish.Log.config");

        EnvironmentVariables.Set("Show_ClownFish_Log_Config", "0");
    }

    [TestMethod]
    public void Test_LoadFromClownFishAssembly()
    {
        LogConfiguration config = LogInitUtils.LoadFromClownFishAssembly();
        Assert.IsNotNull(config);

        Assert.AreEqual(500, config.TimerPeriod);
        Assert.AreEqual(100, config.Performance.HttpExecute);
        Assert.AreEqual(200, config.Performance.HandleMessage);
    }

    [TestMethod]
    public void Test_LoadFromConfigService()
    {
        Console.WriteLine("LogConfigFileName: " + ConfigFile.LogConfigFileName);

        LogConfiguration config = LogInitUtils.LoadFromConfigService();
        Assert.IsNotNull(config);

        Assert.AreEqual(123, config.TimerPeriod);
        Assert.AreEqual(456, config.Performance.HttpExecute);
        Assert.AreEqual(789, config.Performance.HandleMessage);
    }

    [TestMethod]
    public void Test_LoadFromLocalFile()
    {
        LogConfiguration config = LogInitUtils.LoadFromLocalFile();
        Assert.IsNotNull(config);

        Assert.AreEqual(100, config.TimerPeriod);
        Assert.AreEqual(1000, config.Performance.HttpExecute);
        Assert.AreEqual(2000, config.Performance.HandleMessage);
    }

    [TestMethod]
    public void Test_InitLogAsDefault()
    {
        LogConfiguration config = LogInitUtils.InitLogAsDefault();
        Assert.IsNotNull(config);

        Assert.AreEqual(123, config.TimerPeriod);
        Assert.AreEqual(456, config.Performance.HttpExecute);
        Assert.AreEqual(789, config.Performance.HandleMessage);
    }

    [TestMethod]
    public void Test_InitLogAsDefault2()
    {
        LogConfiguration config1 = new LogConfiguration {
            TimerPeriod = 100,
            Performance = new PerformanceConfig {
                HttpExecute = 100,
                HandleMessage = 100
            }
        };

        LogConfiguration config2 = new LogConfiguration {
            TimerPeriod = 11,
            Performance = new PerformanceConfig {
                HttpExecute = 22,
                HandleMessage = 33
            }
        };

        LogConfiguration config = LogInitUtils.InitLogAsDefault(config1, config2);
        Assert.IsNotNull(config);

        Assert.AreEqual(11, config.TimerPeriod);
        Assert.AreEqual(22, config.Performance.HttpExecute);
        Assert.AreEqual(33, config.Performance.HandleMessage);
    }

    

}
