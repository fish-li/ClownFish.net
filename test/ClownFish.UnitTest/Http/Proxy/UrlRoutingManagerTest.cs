namespace ClownFish.UnitTest.Http.Proxy;

[TestClass]
public class UrlRoutingManagerTest
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
                },new ProxyItem {
                    Src = "/aaa/bb/ccc.aspx",
                    Dest = "http://www.fish-test.com/show-request2.aspx",
                },
                new ProxyItem {
                    Src = "/user/*",
                    Dest = "http://www.fish-test.com",
                },
                new ProxyItem {
                    Src = "*",
                    Dest = "http://www.fish-abc.com",
                }
            }
        };
    }


    [TestMethod]
    public void Test_RegisterRules()
    {
        ProxyConfig config = GetProxyConfig();
        UrlRoutingManager.RegisterRules(config);

        UrlRoutingManager.RegisterRules(null);

        UrlRoutingManager.RegisterRules(new ProxyConfig());

        MyAssert.IsError<InvalidOperationException>(()=> {
            UrlRoutingManager.RegisterRules(config);
        });
    }


    [TestMethod]
    public void Test_GetDestUrl_1()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockHttpRequest request = MockHttpRequest.FromRequestText(requestText);

        Assert.IsNull(UrlRoutingManager.GetDestUrl(request));

        //----------------------------------------------

        UrlRoutingManager.RegisterRules(GetProxyConfig());

        Assert.AreEqual("http://www.fish-test.com/show-request2.aspx?tenantId=my57972739adc90", UrlRoutingManager.GetDestUrl(request));
    }


    [TestMethod]
    public void Test_GetDestUrl_2()
    {
        string requestText = @"
GET http://www.abc.com:14752/user/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockHttpRequest request = MockHttpRequest.FromRequestText(requestText);

        UrlRoutingManager.RegisterRules(GetProxyConfig());

        Assert.AreEqual("http://www.fish-test.com/user/bb/ccc.aspx?tenantId=my57972739adc90", UrlRoutingManager.GetDestUrl(request));
    }


    [TestMethod]
    public void Test_GetDestUrl_3()
    {
        string requestText = @"
GET http://www.abc.com:14752/xxx/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockHttpRequest request = MockHttpRequest.FromRequestText(requestText);

        UrlRoutingManager.RegisterRules(GetProxyConfig());

        Assert.AreEqual("http://www.fish-abc.com/xxx/bb/ccc.aspx?tenantId=my57972739adc90", UrlRoutingManager.GetDestUrl(request));
    }

    [TestMethod]
    public void Test_GetDestUrl_4()
    {
        string requestText = @"
GET http://www.abc.com:14752/xxx/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockHttpRequest request = MockHttpRequest.FromRequestText(requestText);

        ProxyConfig config = GetProxyConfig();
        config.Items.RemoveAt(3);  // 删除那条【万能匹配】规则
        UrlRoutingManager.RegisterRules(config);

        Assert.IsNull(UrlRoutingManager.GetDestUrl(request));
    }

}
