namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class HttpHeaderCollectionTest
{
    [TestMethod]
    public void Test()
    {
        HttpHeaderCollection h = new HttpHeaderCollection();
        h.Add("a", "11");
        h.Add("b", "22");
        h.Add("b", "bb");
        h.Add("x", string.Empty);

        Assert.AreEqual(4, h.Count);
        Assert.AreEqual("11", h["a"]);
        Assert.AreEqual("22", h["b"]);
        Assert.AreEqual("", h["x"]);

        MyAssert.IsError<ArgumentNullException>(() => {
            h.Add(null, "xx");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            h.Add("x2", null);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            h.Remove(null);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            h.ContainsName(null);
        });

        Assert.IsTrue(h.ContainsName("b"));
        h.Remove("b");
        Assert.AreEqual(2, h.Count);
        Assert.IsFalse(h.ContainsName("b"));
    }

    [TestMethod]
    public void Test_CreateFrom_Dictionary()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            Dictionary<string, string> x1 = null;
            HttpHeaderCollection x2 = x1;
        });


        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict["a"] = "11";
        dict["b"] = "22";

        HttpHeaderCollection h = dict;
        Assert.AreEqual(2, h.Count);
        Assert.AreEqual("11", h["a"]);
        Assert.AreEqual("22", h["b"]);
        Assert.IsNull(h["xx"]);

        h["xx"] = "xxxxxx";
        Assert.AreEqual(3, h.Count);
    }

    [TestMethod]
    public void Test_CreateFrom_object()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = HttpHeaderCollection.Create(null);
        });
        MyAssert.IsError<ArgumentException>(()=> {
            _ = HttpHeaderCollection.Create(111);
        });
        MyAssert.IsError<ArgumentException>(() => {
            _ = HttpHeaderCollection.Create("xx=xxxxx");
        });

        HttpHeaderCollection h1 = new HttpHeaderCollection();
        h1["xx"] = "xxxxx";

        HttpHeaderCollection h2 = HttpHeaderCollection.Create(h1);
        Assert.IsTrue(object.ReferenceEquals(h1, h2));
    }

    [TestMethod]
    public void Test_CreateFrom_Dictionary2()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict["a"] = "11";
        dict["b"] = "22";
        HttpHeaderCollection h = HttpHeaderCollection.Create(dict);
        Assert.AreEqual(2, h.Count);
        Assert.AreEqual("11", h["a"]);
        Assert.AreEqual("22", h["b"]);
    }

    [TestMethod]
    public void Test_CreateFrom_NameValueCollection()
    {
        NameValueCollection nvc = new NameValueCollection();
        nvc["a"] = "11";
        nvc["b"] = "22";
        nvc.Add("b", "33");
        HttpHeaderCollection h = HttpHeaderCollection.Create(nvc);
        Assert.AreEqual(3, h.Count);
        Assert.AreEqual("11", h["a"]);
        Assert.AreEqual("22", h["b"]);

        MyAssert.IsError<ArgumentNullException>(() => {
            HttpHeaderCollection h2 = null;
            NameValueCollection nv2 = h2;
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            NameValueCollection nv3 = null;
            HttpHeaderCollection h3 = nv3;
        });

        HttpHeaderCollection h4 = nvc;
    }

    [TestMethod]
    public void Test_CreateFrom_object2()
    {
        var data = new {
            a_1 = "11",
            b = 22,
            c = (string)null
        };
        HttpHeaderCollection h = HttpHeaderCollection.Create(data);
        Assert.AreEqual(3, h.Count);
        Assert.AreEqual("11", h["a-1"]);
        Assert.AreEqual("22", h["b"]);
        Assert.AreEqual("", h["c"]);
    }



}
