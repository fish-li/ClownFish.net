using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class TestHttpHeader
{
    [TestMethod]
    public void Test1()
    {
        HttpResponseMessage response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.OK;
        response.Headers.Add("Server", "Docker/24.0.6 (linux)");

        Assert.AreEqual("Docker/24.0.6 (linux)", response.Headers.Server.ToString());

        var values = response.Headers.GetValues("Server").ToArray();
        Assert.AreEqual(2, values.Length);        // 不应该被拆开
        Assert.AreEqual("Docker/24.0.6", values[0]);
        Assert.AreEqual("(linux)", values[1]);

        // 参考文档：
        // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Server
        // https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/User-Agent
        // Server 头的格式应该是： <product> / <product-version> <comment>
        // 因此，上面的结果是不对的！虽然直接访问 Server 属性的结果看起来是对的。
    }

    [TestMethod]
    public void Test2()
    {
        HttpResponseMessage response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.OK;
        response.Headers.Add("Server", "Docker/24.0.6 (linux)");
        response.Headers.Add("Server", "2b");

        Assert.AreEqual("Docker/24.0.6 (linux) 2b", response.Headers.Server.ToString());

        var values = response.Headers.GetValues("Server").ToArray();
        Assert.AreEqual(3, values.Length);        
        Assert.AreEqual("Docker/24.0.6", values[0]);
        Assert.AreEqual("(linux)", values[1]);
        Assert.AreEqual("2b", values[2]);

        // 遇到有多个 Server 头的场景，无论用哪种方式访问，结果都不对
    }


    [TestMethod]
    public void Test3()
    {
        HttpRequestMessage request = new HttpRequestMessage();
        request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36 OPR/38.0.2220.41");

        var values = request.Headers.GetValues("User-Agent").ToArray();

        Console.WriteLine(request.Headers.UserAgent);
        foreach( var value in values ) {
            Console.WriteLine(value);
        }

        Assert.AreEqual(7, values.Length);
        Assert.AreEqual("Mozilla/5.0", values[0]);
        Assert.AreEqual("(X11; Linux x86_64)", values[1]);
        Assert.AreEqual("AppleWebKit/537.36", values[2]);
        Assert.AreEqual("(KHTML, like Gecko)", values[3]);
        Assert.AreEqual("Chrome/51.0.2704.106", values[4]);
        Assert.AreEqual("Safari/537.36", values[5]);
        Assert.AreEqual("OPR/38.0.2220.41", values[6]);
    }
}
