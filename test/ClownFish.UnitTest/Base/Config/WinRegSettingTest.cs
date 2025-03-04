using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ClownFish.UnitTest.Base.Config;

[TestClass]
public class WinRegSettingTest
{
    private static readonly string s_regPath0 = @"SOFTWARE\Fish-li\Test\ClownFish.UnitTest";
    private static readonly string s_regPath = @"HKEY_CURRENT_USER\SOFTWARE\Fish-li\Test\ClownFish.UnitTest";

    static WinRegSettingTest()
    {
#if NETFRAMEWORK
        Registry.CurrentUser.DeleteSubKey(s_regPath0, false);
        ClownFishInit.SetRegPath(s_regPath);
#else
        if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) {
            Registry.CurrentUser.DeleteSubKey(s_regPath0, false);
            ClownFishInit.SetRegPath(s_regPath);
        }
#endif
    }

    [TestMethod]
    public void Test1()
    {
        Assert.IsNull(WinRegSetting.GetSetting("key1"));

#if NETFRAMEWORK
        Registry.SetValue(s_regPath, "key1", "");
        Assert.AreEqual("", WinRegSetting.GetSetting("key1"));

        Registry.SetValue(s_regPath, "key1", "123a");
        Assert.AreEqual("123a", WinRegSetting.GetSetting("key1"));
#else
        if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) {
            Registry.SetValue(s_regPath, "key1", "");
            Assert.AreEqual("", WinRegSetting.GetSetting("key1"));

            Registry.SetValue(s_regPath, "key1", "123a");
            Assert.AreEqual("123a", WinRegSetting.GetSetting("key1"));
        }
#endif
    }
}

