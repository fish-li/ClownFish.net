using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Debug;
[TestClass]
public class SecurityLogUtilsTest
{
    [TestMethod]
    public void Test_HideConnectionStringPwd()
    {
        Assert.AreEqual("server=xxx;password=********;name=abc", SecurityLogUtils.HideConnectionStringPwd("server=xxx;password=123;name=abc"));
        Assert.AreEqual("server=xxx;pwd=********;name=abc", SecurityLogUtils.HideConnectionStringPwd("server=xxx;pwd=123;name=abc"));
        Assert.AreEqual("", SecurityLogUtils.HideConnectionStringPwd(""));
    }


    [TestMethod]
    public void Test_GetEnvironmentVariableLine()
    {
        Assert.AreEqual("Name: aa", SecurityLogUtils.GetEnvironmentVariableLine("Name", "aa", 0));
        Assert.AreEqual("Name: aa", SecurityLogUtils.GetEnvironmentVariableLine("Name", "aa", 2));
        Assert.AreEqual("Name   : aa", SecurityLogUtils.GetEnvironmentVariableLine("Name", "aa", -7));
        Assert.AreEqual("   Name: aa", SecurityLogUtils.GetEnvironmentVariableLine("Name", "aa", 7));

        Assert.AreEqual("db_ConnectionString: server=xxx;password=********;name=abc", SecurityLogUtils.GetEnvironmentVariableLine("db_ConnectionString", "server=xxx;password=123;name=abc"));
        Assert.AreEqual("db_ConnectionString: server=xxx;pwd=********;name=abc", SecurityLogUtils.GetEnvironmentVariableLine("db_ConnectionString", "server=xxx;pwd=123;name=abc"));

        Assert.AreEqual("xx_Password: ********", SecurityLogUtils.GetEnvironmentVariableLine("xx_Password", "aa", 0));
        Assert.AreEqual("xx_Key: ********", SecurityLogUtils.GetEnvironmentVariableLine("xx_Key", "aa", 0));
        Assert.AreEqual("test_hide: ********", SecurityLogUtils.GetEnvironmentVariableLine("test_hide", "aa", 0));

        Assert.AreEqual("test_hide2: aa", SecurityLogUtils.GetEnvironmentVariableLine("test_hide2", "aa", 0));


    }
}
