using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base;
[TestClass]public class ClownFishInitTest
{
    [TestMethod]
    public void Test_SetDefaultCulture()
    {
        typeof(ClownFishInit).InvokeMethod("SetDefaultCulture0", null);
    }


    [TestMethod]
    public void Test_ConfigMisc()
    {
        EnvironmentVariables.Set("ClownFish_LogError_ToConsole", "1");
        EnvironmentVariables.Set("ClownFish_ShowHttpClientEvent", "1");
        typeof(ClownFishInit).InvokeMethod("ConfigMisc", null);



        Exception ex = ExceptionHelper.CreateException();
        ExceptionEventArgs e = new ExceptionEventArgs(ex);
        typeof(ClownFishInit).InvokeMethod("LogHelperOnError", new object[] { null, e });



        BeforeSendEventArgs e2 = new BeforeSendEventArgs {
            HttpOption = new HttpOption {
                Method = "POST",
                Url = "http://www.abc.com/aa/bb.aspx"
            }
        };
        typeof(ClownFishInit).InvokeMethod("HttpClientEventOnBeforeSendRequest", new object[] { null, e2 });
    }


    [TestMethod]
    public void Test_InitLog()
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
        LogConfiguration config = LogConfiguration.LoadFromFile("ClownFish.Log.config", true);
        ClownFishInit.InitLog(config);
        ClownFishInit.InitLog(config);


        EnvironmentVariables.Set("ClownFish_Log_WritersMap", "");
        EnvironmentVariables.Set("Show_ClownFish_Log_Config", "1");
        typeof(LogConfig).SetFieldValue("s_inited", false);
        ClownFishInit.InitLog("ClownFish.Log.config");
        ClownFishInit.InitLog("ClownFish.Log.config");
    }

    [TestMethod]
    public void Test_AutoRegisterMySqlClient()
    {
        int mysqlFlag = LocalSettings.GetInt("MySqlClientProviderSupport", 0);

        EnvironmentVariables.Set("MySqlClientProviderSupport", "0");

        // 强制走另外的分支
        typeof(ClownFishInit).GetMethod("AutoRegisterMySqlClient", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

        // 恢复正常配置
        EnvironmentVariables.Set("MySqlClientProviderSupport", mysqlFlag.ToString());
        typeof(ClownFishInit).GetMethod("AutoRegisterMySqlClient", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
    }
}
