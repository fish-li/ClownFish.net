using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class OsUtilsTest
{

    private static readonly string s_text1 = @"
PRETTY_NAME=""Ubuntu 22.04.3 LTS""
NAME=""Ubuntu""
VERSION_ID=""22.04""
VERSION=""22.04.3 LTS (Jammy Jellyfish)""
VERSION_CODENAME=jammy
ID=ubuntu
ID_LIKE=debian
HOME_URL=""https://www.ubuntu.com/""
SUPPORT_URL=""https://help.ubuntu.com/""
BUG_REPORT_URL=""https://bugs.launchpad.net/ubuntu/""
PRIVACY_POLICY_URL=""https://www.ubuntu.com/legal/terms-and-policies/privacy-policy""
UBUNTU_CODENAME=jammy
".Trim();

    private static readonly string s_text2 = @"
PRETTY_NAME=""Debian GNU/Linux 12 (bookworm)""
NAME=""Debian GNU/Linux""
VERSION_ID=""12""
VERSION=""12 (bookworm)""
VERSION_CODENAME=bookworm
ID=debian
HOME_URL=""https://www.debian.org/""
SUPPORT_URL=""https://www.debian.org/support""
BUG_REPORT_URL=""https://bugs.debian.org/""
".Trim();



    [TestMethod]
    public void Test_GetOsName()
    {
        string name = OsUtils.GetOsName();
        Console.WriteLine(name);  // Microsoft Windows NT 10.0.19045.0
    }

    [TestMethod]
    public void Test_GetLinuxName()
    {
        string name = (string)typeof(OsUtils).InvokeMethod("GetLinuxName");

        if( RuntimeInformation.IsOSPlatform(OSPlatform.Linux) == false ) {
            Assert.AreEqual("NULL", name);
        }
    }

    [TestMethod]
    public void Test_GetLinuxName0()
    {
        string name1 = (string)typeof(OsUtils).InvokeMethod("GetLinuxName0", s_text1);
        Assert.AreEqual("Ubuntu 22.04.3 LTS", name1);

        string name2 = (string)typeof(OsUtils).InvokeMethod("GetLinuxName0", s_text2);
        Assert.AreEqual("Debian GNU/Linux 12 (bookworm)", name2);
    }
}
