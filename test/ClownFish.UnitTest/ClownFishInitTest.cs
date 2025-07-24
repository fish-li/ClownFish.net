using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base;
[TestClass]public class ClownFishInitTest
{
    //[TestMethod]
    //public void Test_SetDefaultCulture()
    //{
    //    typeof(BaseInitUtils).InvokeMethod("SetDefaultCulture0", null);
    //}


    [TestMethod]
    public void Test_ConfigMisc()
    {
        EnvironmentVariables.Set("ClownFish_LogError_ToConsole", "1");
        EnvironmentVariables.Set("ClownFish_ShowHttpClientEvent", "1");
        typeof(BaseInitUtils).InvokeMethod("ConfigMisc", null);


        Exception ex = ExceptionHelper.CreateException();
        ExceptionEventArgs e = new ExceptionEventArgs(ex);
        typeof(BaseInitUtils).InvokeMethod("LogHelperOnError", new object[] { null, e });



        BeforeSendEventArgs e2 = new BeforeSendEventArgs {
            HttpOption = new HttpOption {
                Method = "POST",
                Url = "http://www.abc.com/aa/bb.aspx"
            }
        };
        typeof(BaseInitUtils).InvokeMethod("HttpClientEventOnBeforeSendRequest", new object[] { null, e2 });
    }


    [TestMethod]
    public void Test_ShowClownFishAppConfig()
    {
        EnvironmentVariables.Set("Show_ClownFish_App_Config", "1");
        BaseInitUtils.ShowClownFishAppConfig();
    }




}
