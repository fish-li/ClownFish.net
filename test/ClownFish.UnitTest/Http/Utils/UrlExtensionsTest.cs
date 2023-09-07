namespace ClownFish.UnitTest.Http.Utils;

[TestClass]
public class UrlExtensionsTest
{
    [TestMethod]
    public void Test_UrlEncode()
    {
        string s = @"已评估过类似方案，上海和融25版本做过类似改动，但需求有差异。";

        string enc = s.UrlEncode();
        string s2 = enc.UrlDecode();

        Assert.AreEqual(s, s2);
        Assert.IsTrue(enc.IndexOf("已评估") < 0);

        Assert.AreEqual(string.Empty, string.Empty.UrlEncode());
        Assert.IsNull(UrlExtensions.UrlEncode(null));

        Assert.AreEqual(string.Empty, string.Empty.UrlDecode());
        Assert.IsNull(UrlExtensions.UrlDecode(null));
    }

    [TestMethod]
    public void Test_HtmlEncode()
    {
        string s = @"<b>已评估过类似方案，上海和融25版本做过类似改动，但需求有差异。</b>";

        string enc = s.HtmlEncode();
        string s2 = enc.HtmlDecode();

        Assert.AreEqual(s, s2);
        Assert.IsTrue(enc.IndexOf("<b>") < 0);

        Assert.AreEqual(string.Empty, string.Empty.HtmlEncode());
        Assert.IsNull(UrlExtensions.HtmlEncode(null));

        Assert.AreEqual(string.Empty, string.Empty.HtmlDecode());
        Assert.IsNull(UrlExtensions.HtmlDecode(null));
    }

    [TestMethod]
    public void Test_AddQueryStringArgs()
    {
        string root = "http://abc.com/xx.aspx";
        Assert.AreEqual("http://abc.com/xx.aspx?x1=2", root.AddUrlQueryArgs("x1", "2"));
        Assert.AreEqual("http://abc.com/xx.aspx?x1=", root.AddUrlQueryArgs("x1", null));

        List<NameValue> list = new List<NameValue> {
            new NameValue{Name = "x1", Value = "2"},
            new NameValue{Name = "x2", Value = "3"}
        };
        Assert.AreEqual("http://abc.com/xx.aspx?x1=2&x2=3", root.AddUrlQueryArgs(list));

        Assert.AreEqual(root, root.AddUrlQueryArgs(new List<NameValue>(0)));
        Assert.AreEqual(root, root.AddUrlQueryArgs((List<NameValue>)null));


        string root2 = "http://abc.com/xx.aspx?aa=2";
        Assert.AreEqual("http://abc.com/xx.aspx?aa=2&x2=3", root2.AddUrlQueryArgs("x2", "3"));
        Assert.AreEqual("http://abc.com/xx.aspx?aa=2&x1=2&x2=3", root2.AddUrlQueryArgs(list));
    }

    [TestMethod]
    public void Test_AddQueryStringArgs2()
    {
        string root = "http://abc.com/xx.aspx";
        Assert.AreEqual(root, root.AddUrlQueryArgs(null, "2"));

        MyAssert.IsError<ArgumentNullException>(() => {
            string url = null;
            Assert.IsNull(url.AddUrlQueryArgs("x2", "2"));
        });
    }


    [TestMethod]
    public void Test_EnsureUrlRoot()
    {
        string s1 = "http://InfluxHost:8086/";
        string s2 = "http://InfluxHost:8086";
        string s3 = "InfluxHost:8086/";
        string s4 = "InfluxHost:8086";
        Assert.AreEqual("http://InfluxHost:8086", s1.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost:8086", s2.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost:8086", s3.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost:8086", s4.EnsureUrlRoot());



        string s5 = "http://InfluxHost/";
        string s6 = "http://InfluxHost";
        string s7 = "InfluxHost/";
        string s8 = "InfluxHost";
        Assert.AreEqual("http://InfluxHost", s5.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost", s6.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost", s7.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost", s8.EnsureUrlRoot());
    }

    [TestMethod]
    public void Test_EnsureUrlRoot2()
    {
        string s1 = "https://InfluxHost:8086/";
        string s2 = "https://InfluxHost:8086";
        string s3 = "InfluxHost:8086/";
        string s4 = "InfluxHost:8086";
        Assert.AreEqual("https://InfluxHost:8086", s1.EnsureUrlRoot());
        Assert.AreEqual("https://InfluxHost:8086", s2.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost:8086", s3.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost:8086", s4.EnsureUrlRoot());



        string s5 = "https://InfluxHost/";
        string s6 = "https://InfluxHost";
        string s7 = "InfluxHost/";
        string s8 = "InfluxHost";
        Assert.AreEqual("https://InfluxHost", s5.EnsureUrlRoot());
        Assert.AreEqual("https://InfluxHost", s6.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost", s7.EnsureUrlRoot());
        Assert.AreEqual("http://InfluxHost", s8.EnsureUrlRoot());
    }

    [TestMethod]
    public void Test_GetUrlRoot()
    {
        Uri u1 = new Uri("http://www.abc.com:14752/aaa/bb");
        Assert.AreEqual("http://www.abc.com:14752", u1.GetUrlRoot());

        Uri u2 = new Uri("https://www.abc.com:14752/aaa/bb");
        Assert.AreEqual("https://www.abc.com:14752", u2.GetUrlRoot());

        Uri u3 = new Uri("https://www.abc.com/");
        Assert.AreEqual("https://www.abc.com", u3.GetUrlRoot());

        Uri u4 = new Uri("http://configservice");
        Assert.AreEqual("http://configservice", u4.GetUrlRoot());


        MyAssert.IsError<ArgumentNullException>(() => {
            var x = UrlExtensions.GetUrlRoot(null);
        });
    }

}
