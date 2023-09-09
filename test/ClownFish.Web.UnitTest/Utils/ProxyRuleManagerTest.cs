using ClownFish.Web.Utils;

namespace ClownFish.Web.UnitTest.Utils;

[TestClass]
public class ProxyRuleManagerTest
{
    private readonly ProxyRuleManager _ruleManager1 = new ProxyRuleManager();
    private readonly ProxyRuleManager _ruleManager2 = new ProxyRuleManager();
    public ProxyRuleManagerTest()
    {
        _ruleManager1.Init("Test_ProxyMapRule.xml");
        _ruleManager2.Init("Test_ProxyMapRule2.xml");
    }

    [TestMethod]
    public void Test1()
    {
        ProxyMapRule rule = _ruleManager2.Rules;

        Assert.IsTrue(rule.Rules.HasValue());

        var r1 = rule.Rules.First(x => x.Src == "/new/api/test/add");
        Assert.IsNull(r1.SrcRegex);

        var r2 = rule.Rules.First(x => x.Src == "/new/api/test/ShowRequest");
        Assert.IsNull(r2.SrcRegex);
    }


    [TestMethod]
    public void Test2()
    {
        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < 10_0000; i++ ) {
            Test3();
        }

        watch.Stop();
        Console.WriteLine(watch.Elapsed.ToString());
    }

    [TestMethod]
    public void Test3()
    {
        // datainsapp
        Assert.AreEqual("http://datainsapp/v20/datains/aa/bb.aspx", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://datains.test.com/v20/datains/aa/bb.aspx"));
        Assert.AreEqual("http://datainsapp/v20/datains/aa/bb.aspx?a=2&b=3", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://datains.test.com/v20/datains/aa/bb.aspx?a=2&b=3"));
        Assert.AreEqual("http://ui-datains/11.js", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://datains.test.com/11.js"));
        Assert.AreEqual("http://ui-datains/11.js?a=2&b=3", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://datains.test.com/11.js?a=2&b=3"));
        Assert.AreEqual("http://ui-datains/js/123.js", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://datains.test.com/js/123.js"));
        Assert.AreEqual("http://ui-datains/11.css", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://xxxxx.datains.test.com/11.css"));
        Assert.AreEqual("http://ui-datains/css/123.css", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://xxxxx.datains.test.com/css/123.css"));

        // localhost
        Assert.AreEqual("http://localhost:20040/test/2", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://www.fish-tucao.com/site/test/2"));
        Assert.AreEqual("http://localhost:20020/service1/test/2", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://www.fish-tucao.com/service1/test/2"));
        Assert.AreEqual("http://localhost:20030/service2/test/2", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://www.fish-tucao.com/service2/test/2"));

        Assert.AreEqual("http://localhost:20040/test/2?a=2&b=3", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://www.fish-tucao.com/site/test/2?a=2&b=3"));
        Assert.AreEqual("http://localhost:20020/service1/test/2?a=2&b=3", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://www.fish-tucao.com/service1/test/2?a=2&b=3"));
        Assert.AreEqual("http://localhost:20030/service2/test/2?a=2&b=3", _ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://www.fish-tucao.com/service2/test/2?a=2&b=3"));

        // 没有规则匹配的地址
        Assert.IsNull(_ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://xxxxx.abc.com/v20/datains/aa/bb.aspx"));
        Assert.IsNull(_ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://xxxxx.abc.com/v20/datains/aa/bb.aspx"));
        Assert.IsNull(_ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://xxxxx.abc.com/css/123.css"));
        Assert.IsNull(_ruleManager1.GetProxyDestUrl((ProxyTestRequest)"http://xxxxx.abc.com/css/123.css"));
    }



    [TestMethod]
    public void Test4()
    {
        // 一对一URL映射
        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/test/Add.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/add"));
        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/test/ShowRequest.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/ShowRequest"));
        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/auth/login.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/login"));
        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/Customer/findByTel.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/findByTel"));

        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/test/Add.aspx?a=2&b=3", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/add?a=2&b=3"));
        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/test/ShowRequest.aspx?a=2&b=3", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/ShowRequest?a=2&b=3"));
        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/auth/login.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/login"));
        Assert.AreEqual("http://demo_app/v20/api/WebSiteApp/Customer/findByTel.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.fish.com/new/api/test/findByTel"));

        // 特殊规则匹配 *
        Assert.AreEqual("http://localhost:50000/site2/page/123.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/site2/page/123.aspx"));
        Assert.AreEqual("http://localhost:50000/123.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/123.aspx"));
        Assert.AreEqual("http://localhost:50000/", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/"));

        Assert.AreEqual("http://localhost:50000/site2/page/123.aspx?a=2&b=3", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/site2/page/123.aspx?a=2&b=3"));
        Assert.AreEqual("http://localhost:50000/123.aspx?a=2&b=3", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/123.aspx?a=2&b=3"));
        Assert.AreEqual("http://localhost:50000/?a=2&b=3", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/?a=2&b=3"));

        // Path前缀匹配规则
        Assert.AreEqual("http://localhost:30000/v30/page/123.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/v30/page/123.aspx"));
        Assert.AreEqual("http://localhost:40000/page/123.aspx", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/v40/page/123.aspx"));

        Assert.AreEqual("http://localhost:30000/v30/page/123.aspx?a=2&b=3", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/v30/page/123.aspx?a=2&b=3"));
        Assert.AreEqual("http://localhost:40000/page/123.aspx?a=2&b=3", _ruleManager2.GetProxyDestUrl((ProxyTestRequest)"http://www.abc.com/v40/page/123.aspx?a=2&b=3"));
    }
}
