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
}
