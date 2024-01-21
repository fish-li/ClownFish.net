using ClownFish.Http.Proxy;

namespace ClownFish.UnitTest.Http.Proxy;

internal class MyTestHttpProxyModule : HttpProxyModule
{
    protected override string GetDestUrl(NHttpRequest request)
    {
        return UrlRoutingManager.GetDestUrl(request);
    }
}


[TestClass]
public class HttpTest
{
    [TestCleanup]
    public void Cleanup()
    {
        UrlRoutingManager.ClearRules();
    }

    private static ProxyConfig GetProxyConfig()
    {
        return new ProxyConfig {
            Items = new List<ProxyItem> {
                new ProxyItem {
                    Src = "/aaa/bb/ccc.aspx",
                    Dest = "",
                },
                new ProxyItem {
                    Src = "/error/bb/ccc.aspx",
                    Dest = "http://www.fish-test.com/throw-error.aspx",
                },
                new ProxyItem {
                    Src = "/notfound/bb/ccc.aspx",
                    Dest = "http://www.fish-test.com/xxxxxxxx.aspx", 
                },
                new ProxyItem {
                    Src = "/xxx/bb/ccc.aspx",
                    Dest = "http://xxxhost/xxxxxxxx.aspx", 
                },
                new ProxyItem {
                    Src = "/aaa/bb/ccc.aspx",
                    Dest = "http://www.fish-test.com/show-request2.aspx",
                },
                new ProxyItem {
                    Src = "user/*",
                    Dest = "http://www.fish-test.com",
                }
            }
        };
    }


    [TestMethod]
    public async Task Test_POST()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Connection: keep-alive
Referer: http://www.abc.com:14752/aaa/bb/text.aspx

8a8cf217f2fa4ef38e247f6628c75205
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<MyTestHttpProxyModule>();
            UrlRoutingManager.RegisterRules(GetProxyConfig());

            await mock.ProcessRequest();

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;

            //----------------------------------
            string responseText = response.GetResponseAsText();
            string[] lines = responseText.ToLines();

            // 确认当前响应内容来自于转发
            Assert.IsTrue(lines.Contains("HttpMethod: POST"));
            Assert.IsTrue(lines.Contains("Url: http://www.fish-test.com/show-request2.aspx?tenantId=my57972739adc90"));

            // 检查请求内容
            Assert.IsTrue(lines.Contains("8a8cf217f2fa4ef38e247f6628c75205"));

            // 检查请求头
            Assert.IsTrue(lines.Contains("Content-Type = text/plain"));
            Assert.IsTrue(lines.Contains("Cookie = c1=1111111; c2=22222222"));
            Assert.IsTrue(lines.Contains("x-client-app = HttpTest1"));
            Assert.IsTrue(lines.Contains("X-CfProxy-OrgUrl = http://www.abc.com:14752/aaa/bb/ccc.aspx"));
            Assert.IsTrue(lines.Contains("Referer = http://www.fish-test.com/aaa/bb/text.aspx"));

            //Console.WriteLine("OK");
            //File.WriteAllText("./temp/__test_Proxy_responseText.txt", responseText, Encoding.UTF8);
        }
    }


    [TestMethod]
    public async Task Test_GET()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Connection: keep-alive
Referer: http://www.abc.com:14752/aaa/bb/text.aspx
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<MyTestHttpProxyModule>();
            UrlRoutingManager.RegisterRules(GetProxyConfig());

            await mock.ProcessRequest();

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;

            //----------------------------------
            string responseText = response.GetResponseAsText();
            string[] lines = responseText.ToLines();

            // 确认当前响应内容来自于转发
            Assert.IsTrue(lines.Contains("HttpMethod: GET"));
            Assert.IsTrue(lines.Contains("Url: http://www.fish-test.com/show-request2.aspx?tenantId=my57972739adc90"));


            // 检查请求头
            Assert.IsTrue(lines.Contains("Content-Type = text/plain"));
            Assert.IsTrue(lines.Contains("Cookie = c1=1111111; c2=22222222"));
            Assert.IsTrue(lines.Contains("x-client-app = HttpTest1"));
            Assert.IsTrue(lines.Contains("X-CfProxy-OrgUrl = http://www.abc.com:14752/aaa/bb/ccc.aspx"));
            Assert.IsTrue(lines.Contains("Referer = http://www.fish-test.com/aaa/bb/text.aspx"));

            //Console.WriteLine("OK");
            //File.WriteAllText("./temp/__test_Proxy_responseText.txt", responseText, Encoding.UTF8);
        }
    }


    [TestMethod]
    public async Task Test_Server500()
    {
        string requestText = @"
GET http://www.abc.com:14752/error/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Connection: keep-alive
Referer: http://www.abc.com:14752/aaa/bb/text.aspx
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<MyTestHttpProxyModule>();
            UrlRoutingManager.RegisterRules(GetProxyConfig());

            await mock.ProcessRequest();

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;

            //----------------------------------
            string responseText = response.GetResponseAsText();
            string[] lines = responseText.ToLines();

            Assert.AreEqual(500, response.StatusCode);

            // 确认当前响应内容来自于转发
            Assert.IsTrue(lines.Contains("<title>test throw error!</title>"));

            //Console.WriteLine("OK");
            //File.WriteAllText("./temp/__test_Proxy_responseText.txt", responseText, Encoding.UTF8);
        }
    }

    [TestMethod]
    public async Task Test_Server404()
    {
        string requestText = @"
GET http://www.abc.com:14752/notfound/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Connection: keep-alive
Referer: http://www.abc.com:14752/aaa/bb/text.aspx
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<MyTestHttpProxyModule>();
            UrlRoutingManager.RegisterRules(GetProxyConfig());

            await mock.ProcessRequest();

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;

            //----------------------------------
            string responseText = response.GetResponseAsText();
            string[] lines = responseText.ToLines();

            Assert.AreEqual(404, response.StatusCode);

            // 确认当前响应内容来自于转发
            //Assert.IsTrue(lines.Contains("<title>test throw error!</title>"));

            //Console.WriteLine("OK");
            //File.WriteAllText("./temp/__test_Proxy_responseText.txt", responseText, Encoding.UTF8);
        }
    }

    [TestMethod]
    public async Task Test_ClientError()
    {
        string requestText = @"
GET http://www.abc.com:14752/xxx/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Referer: http://www.abc.com:14752/aaa/bb/text.aspx
Connection: keep-alive
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            NHttpModuleFactory.RegisterModule<MyTestHttpProxyModule>();
            UrlRoutingManager.RegisterRules(GetProxyConfig());

            await mock.ProcessRequest();

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;

            //----------------------------------
            string responseText = response.GetResponseAsText();
            string[] lines = responseText.ToLines();

            Assert.AreEqual(500, response.StatusCode);

            Assert.AreEqual("1", response.OutHeaders["X-HttpProxyHandler-error"]);

#if NETCOREAPP
            //Assert.IsTrue(lines.Contains("System.Net.Http.HttpRequestException: 不知道这样的主机。 (xxxhost:80)"));                
            Assert.IsNotNull(lines.FirstOrDefault(x=>x.StartsWith("---> System.Net.Sockets.SocketException (11001):")));
#else
            //Assert.IsTrue(lines.Contains("System.Net.WebException: 未能解析此远程名称: 'xxxhost'"));
#endif
            //Console.WriteLine("OK");
            //File.WriteAllText("./temp/__test_Proxy_responseText.txt", responseText, Encoding.UTF8);
        }
    }

}
