namespace ClownFish.UnitTest.Http.Utils;

[TestClass]
public class UrlsTest
{
    [TestMethod]
    public void Test_GetWebSiteRoot()
    {
        string url = "http://www.abc.com/test/callback.aspx";
        Assert.AreEqual("http://www.abc.com", Urls.GetWebSiteRoot(url));

        Assert.IsNull(Urls.GetWebSiteRoot("/test/callback.aspx"));
    }

    [TestMethod]
    public void Test_Combine()
    {
        string expected = "http://www.abc.com/aa/bb/cc.aspx";
        Assert.AreEqual(expected, Urls.Combine("http://www.abc.com/", "/aa/bb/cc.aspx"));
        Assert.AreEqual(expected, Urls.Combine("http://www.abc.com/", "aa/bb/cc.aspx"));

        Assert.AreEqual(expected, Urls.Combine("http://www.abc.com", "/aa/bb/cc.aspx"));
        Assert.AreEqual(expected, Urls.Combine("http://www.abc.com", "aa/bb/cc.aspx"));

        MyAssert.IsError<ArgumentNullException>(()=> {
            _ = Urls.Combine(null, "/test/callback.aspx");
        });

        Assert.AreEqual("http://www.abc.com", Urls.Combine("http://www.abc.com", null));
        Assert.AreEqual("http://www.abc.com/", Urls.Combine("http://www.abc.com/", null));
    }

    [TestMethod]
    public void Test_AddQuery()
    {
        string expected = "http://www.abc.com/aa/bb/cc.aspx?xx=2&yy=abc";
        Assert.AreEqual(expected, UrlExtensions.AddUrlQueryArgs("http://www.abc.com/aa/bb/cc.aspx", "xx", "2").AddUrlQueryArgs("yy", "abc"));
        Assert.AreEqual(expected, UrlExtensions.AddUrlQueryArgs("http://www.abc.com/aa/bb/cc.aspx?xx=2", "yy", "abc"));

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = UrlExtensions.AddUrlQueryArgs(null, "yy", "abc");
        });
        Assert.AreEqual("aa", UrlExtensions.AddUrlQueryArgs("aa", null, "xx"));
    }

    [TestMethod]
    public void Test_RemoveHost()
    {
        Assert.IsNull(Urls.RemoveHost(null));
        Assert.AreEqual("", Urls.RemoveHost(""));

        Assert.AreEqual("/", Urls.RemoveHost("http://wwww.abc.com/"));
        Assert.AreEqual("/", Urls.RemoveHost("https://wwww.abc.com/"));

        Assert.AreEqual("/aa", Urls.RemoveHost("http://wwww.abc.com/aa"));
        Assert.AreEqual("/aa", Urls.RemoveHost("https://wwww.abc.com/aa"));

        Assert.AreEqual("/aa/", Urls.RemoveHost("http://wwww.abc.com/aa/"));
        Assert.AreEqual("/aa/", Urls.RemoveHost("https://wwww.abc.com/aa/"));

        Assert.AreEqual("/aa/bb.aspx", Urls.RemoveHost("http://wwww.abc.com/aa/bb.aspx"));
        Assert.AreEqual("/aa/bb.aspx", Urls.RemoveHost("https://wwww.abc.com/aa/bb.aspx"));


        Assert.AreEqual("/aa/bb.aspx", Urls.RemoveHost("/aa/bb.aspx"));
        Assert.AreEqual("/aa/", Urls.RemoveHost("/aa/"));
        Assert.AreEqual("/", Urls.RemoveHost("/"));


        // 这个有点特殊，也不知道合不合适~~~~~~~~~~
        Assert.AreEqual("/", Urls.RemoveHost("https://wwww.abc.com"));

        // 不正确的地址
        Assert.AreEqual("httpx://wwww.abc.com/aa/bb.aspx", Urls.RemoveHost("httpx://wwww.abc.com/aa/bb.aspx"));
    }
}
