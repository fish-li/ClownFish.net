using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Net;

namespace ClownFish.UnitTest.Base.Net;
[TestClass]
public class NetworkUtilsTest
{
    [TestMethod]
    public void Test_GetLocalIp()
    {
        string ip = NetworkUtils.GetLocalIp();
        Console.WriteLine(ip);

    }


    [TestMethod]
    public void Test_IsLanIP()
    {
        Assert.IsFalse(NetworkUtils.IsLanIP("191.168.1.1"));
        Assert.IsFalse(NetworkUtils.IsLanIP("192.167.0.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("192.168.0.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("192.168.1.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("192.168.200.1"));
        Assert.IsFalse(NetworkUtils.IsLanIP("192.169.0.1"));

        Assert.IsFalse(NetworkUtils.IsLanIP("172.15.3.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("172.16.3.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("172.31.8.117"));
        Assert.IsTrue(NetworkUtils.IsLanIP("172.31.3.1"));
        Assert.IsFalse(NetworkUtils.IsLanIP("172.32.3.1"));

        Assert.IsFalse(NetworkUtils.IsLanIP("9.1.1.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("10.1.1.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("10.5.10.159"));
        Assert.IsTrue(NetworkUtils.IsLanIP("10.5.11.229"));
        Assert.IsTrue(NetworkUtils.IsLanIP("10.255.1.1"));
        Assert.IsFalse(NetworkUtils.IsLanIP("11.1.1.1"));

        Assert.IsTrue(NetworkUtils.IsLanIP("127.0.0.1"));
        Assert.IsFalse(NetworkUtils.IsLanIP("aa.1.1.1"));
    }


    [TestMethod]
    public void Test_IsLanIP2()
    {
        Assert.IsTrue(NetworkUtils.IsLanIP("100.64.0.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("100.65.0.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("100.127.222.255"));

        Assert.IsTrue(NetworkUtils.IsLanIP("127.1.1.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("127.222.111.255"));
        
        Assert.IsTrue(NetworkUtils.IsLanIP("169.254.0.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("169.254.222.255"));

        Assert.IsTrue(NetworkUtils.IsLanIP("192.0.2.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("192.0.2.222"));

        Assert.IsTrue(NetworkUtils.IsLanIP("198.51.100.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("198.51.100.222"));

        Assert.IsTrue(NetworkUtils.IsLanIP("203.0.113.1"));
        Assert.IsTrue(NetworkUtils.IsLanIP("203.0.113.233"));
    }
}
