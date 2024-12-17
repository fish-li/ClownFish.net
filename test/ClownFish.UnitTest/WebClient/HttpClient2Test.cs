using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


#if NETCOREAPP
using ClownFish.WebClient.V2;
using MySqlX.XDevAPI.Common;

namespace ClownFish.UnitTest.WebClient;
[TestClass]
public class HttpClient2Test
{
    [TestMethod]
    public void Test_CreateClientHandler_1()
    {
        SocketsHttpHandler clientHandler1 = new SocketsHttpHandler();

        HttpOption httpOption = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            MessageHandler = clientHandler1,
        };

        HttpMessageHandler clientHandler2 = HttpClient2.CreateClientHandler(httpOption, out bool disposeHandler);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsTrue(object.ReferenceEquals(clientHandler1, clientHandler2));
        Assert.IsFalse(disposeHandler);
    }

    /*
    [TestMethod]
    public void Test_CreateClientHandler_2()
    {
        string requestRaw = @"
GET http://localhost/v1.40/containers/tucao/stats?stream=false HTTP/1.1
--unix-socket: /var/run/docker.sock".Trim();

        HttpOption httpOption = HttpOption.FromRawText(requestRaw);

        HttpMessageHandler clientHandler2 = HttpClient2.CreateClientHandler(httpOption, out bool disposeHandler);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsTrue(clientHandler2 is System.Net.Http.SocketsHttpHandler);
        Assert.IsNotNull((clientHandler2 as System.Net.Http.SocketsHttpHandler).ConnectCallback);
    }*/

    [TestMethod]
    public void Test_CreateClientHandler_3()   // HttpMessageHandler 重用
    {
        HttpOption httpOption1 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
        };

        HttpMessageHandler clientHandler1 = HttpClient2.CreateClientHandler(httpOption1, out bool disposeHandler1);

        Assert.IsTrue(clientHandler1 != null);
        Assert.IsFalse(disposeHandler1);


        HttpOption httpOption2 = new HttpOption() {
            Url = "http://www.fish-test.com/aaaaaaaaaaa.aspx",
        };

        HttpMessageHandler clientHandler2 = HttpClient2.CreateClientHandler(httpOption2, out bool disposeHandler2);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsFalse(disposeHandler2);
        Assert.IsTrue(object.ReferenceEquals(clientHandler1, clientHandler2));
    }

    [TestMethod]
    public void Test_CreateClientHandler_4()   // HttpMessageHandler 【不】重用
    {
        HttpOption httpOption1 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
        };

        HttpMessageHandler clientHandler1 = HttpClient2.CreateClientHandler(httpOption1, out bool disposeHandler1);

        Assert.IsTrue(clientHandler1 != null);
        Assert.IsFalse(disposeHandler1);


        HttpOption httpOption2 = new HttpOption() {
            Url = "http://www.fish-test.com:333/aaaaaaaaaaa.aspx",   // 站点“端口号”不同
        };

        HttpMessageHandler clientHandler2 = HttpClient2.CreateClientHandler(httpOption2, out bool disposeHandler2);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsFalse(disposeHandler2);

        Assert.IsFalse(object.ReferenceEquals(clientHandler1, clientHandler2));
    }

    [TestMethod]
    public void Test_CreateClientHandler_5()   // HttpMessageHandler 【不】重用
    {
        HttpOption httpOption1 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
        };

        HttpMessageHandler clientHandler1 = HttpClient2.CreateClientHandler(httpOption1, out bool disposeHandler1);


        Assert.IsTrue(clientHandler1 != null);
        Assert.IsFalse(disposeHandler1);


        HttpOption httpOption2 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            AllowAutoRedirect = true,   // 默认值，不影响重用性
        };

        HttpMessageHandler clientHandler2 = HttpClient2.CreateClientHandler(httpOption2, out bool disposeHandler2);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsFalse(disposeHandler2);

        Assert.IsTrue(object.ReferenceEquals(clientHandler1, clientHandler2));

        HttpOption httpOption3 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            AllowAutoRedirect = false,   // 它导致不重用
        };

        HttpMessageHandler clientHandler3 = HttpClient2.CreateClientHandler(httpOption3, out bool disposeHandler3);

        Assert.IsTrue(clientHandler3 != null);
        Assert.IsFalse(disposeHandler3);

        Assert.IsFalse(object.ReferenceEquals(clientHandler1, clientHandler3));
    }


    [TestMethod]
    public void Test_CreateClientHandler_6()   // HttpMessageHandler 【不】重用
    {
        HttpOption httpOption1 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            Credentials = new NetworkCredential("fishli", "abc")
        };

        HttpMessageHandler clientHandler1 = HttpClient2.CreateClientHandler(httpOption1, out bool disposeHandler1);


        Assert.IsTrue(clientHandler1 != null);
        Assert.IsFalse(disposeHandler1);


        HttpOption httpOption2 = new HttpOption() {
            Url = "http://www.fish-test.com/aaaaaaaaaaa.aspx",
            Credentials = new NetworkCredential("fishli", "abc")
        };

        HttpMessageHandler clientHandler2 = HttpClient2.CreateClientHandler(httpOption2, out bool disposeHandler2);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsFalse(disposeHandler2);
        Assert.IsTrue(object.ReferenceEquals(clientHandler1, clientHandler2));


        HttpOption httpOption3 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            Credentials = new NetworkCredential("user1", "abc"),   // 它导致不重用
        };

        HttpMessageHandler clientHandler3 = HttpClient2.CreateClientHandler(httpOption3, out bool disposeHandler3);

        Assert.IsTrue(clientHandler3 != null);
        Assert.IsFalse(disposeHandler3);

        Assert.IsFalse(object.ReferenceEquals(clientHandler1, clientHandler3));
    }


    private class MyTestCredentials : ICredentials
    {
        public MyTestCredentials(string username, string password)
        {
            // 忽略
        }
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            return new NetworkCredential("fishli", "abc");
        }
    }


    [TestMethod]
    public void Test_CreateClientHandler_7()   // HttpMessageHandler 【不】重用
    {
        HttpOption httpOption1 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            Credentials = new MyTestCredentials("fishli", "abc")
        };

        HttpMessageHandler clientHandler1 = HttpClient2.CreateClientHandler(httpOption1, out bool disposeHandler1);

        Assert.IsTrue(clientHandler1 != null);
        Assert.IsTrue(disposeHandler1);


        HttpOption httpOption2 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            Credentials = new MyTestCredentials("fishli", "abc")
        };

        HttpMessageHandler clientHandler2 = HttpClient2.CreateClientHandler(httpOption2, out bool disposeHandler2);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsTrue(disposeHandler2);
        Assert.IsFalse(object.ReferenceEquals(clientHandler1, clientHandler2));
    }



    [TestMethod]
    public void Test_CreateClientHandler_8()   // Proxy 行为
    {
        HttpOption httpOption1 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
        };

        SocketsHttpHandler clientHandler1 = (SocketsHttpHandler)HttpClient2.CreateClientHandler(httpOption1, out bool disposeHandler1);

        Assert.IsTrue(clientHandler1 != null);
        Assert.IsFalse(disposeHandler1);

        Assert.AreEqual(DecompressionMethods.GZip | DecompressionMethods.Brotli, clientHandler1.AutomaticDecompression);
        Assert.IsTrue(clientHandler1.AllowAutoRedirect);

        HttpOption httpOption2 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            IsProxyRequest = true
        };

        SocketsHttpHandler clientHandler2 = (SocketsHttpHandler)HttpClient2.CreateClientHandler(httpOption2, out bool disposeHandler2);

        Assert.IsTrue(clientHandler2 != null);
        Assert.IsFalse(disposeHandler2);
        Assert.IsFalse(object.ReferenceEquals(clientHandler1, clientHandler2));

        Assert.AreEqual(DecompressionMethods.None, clientHandler2.AutomaticDecompression);
        Assert.IsFalse(clientHandler2.AllowAutoRedirect);
    }



    [TestMethod]
    public void Test_CreateHttpClient_1()
    {
        HttpOption httpOption1 = new HttpOption() {
            Url = "http://www.fish-test.com/test1.aspx",
            Timeout = 22_000
        };

        HttpClient httpClient = HttpClient2.CreateHttpClient(httpOption1);
        Assert.IsNotNull(httpClient);

        Assert.AreEqual(22_000, (int)httpClient.Timeout.TotalMilliseconds);
    }
}

#endif
