namespace ClownFish.UnitTest.Http.Utils;

[TestClass]
public class RequestContentTypeTest
{
    [TestMethod]
    public void Test_GetFormat()
    {
        Assert.AreEqual(SerializeFormat.None, ContentTypeUtils.GetFormat(null));
        Assert.AreEqual(SerializeFormat.None, ContentTypeUtils.GetFormat(string.Empty));

        Assert.AreEqual(SerializeFormat.Json, ContentTypeUtils.GetFormat("application/json"));
        Assert.AreEqual(SerializeFormat.Json, ContentTypeUtils.GetFormat("application/json; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.JsonLines, ContentTypeUtils.GetFormat("application/x-ndjson"));
        Assert.AreEqual(SerializeFormat.JsonLines, ContentTypeUtils.GetFormat("application/x-ndjson; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Xml, ContentTypeUtils.GetFormat("application/xml"));
        Assert.AreEqual(SerializeFormat.Xml, ContentTypeUtils.GetFormat("application/xml; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Form, ContentTypeUtils.GetFormat("application/x-www-form-urlencoded"));
        Assert.AreEqual(SerializeFormat.Form, ContentTypeUtils.GetFormat("application/x-www-form-urlencoded; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Multipart, ContentTypeUtils.GetFormat("multipart/form-data; boundary=xxxxx"));

        Assert.AreEqual(SerializeFormat.Binary, ContentTypeUtils.GetFormat("application/octet-stream"));

        Assert.AreEqual(SerializeFormat.Text, ContentTypeUtils.GetFormat("text/plain"));
        Assert.AreEqual(SerializeFormat.Text, ContentTypeUtils.GetFormat("text/plain; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Unknown, ContentTypeUtils.GetFormat("application/xx"));
        Assert.AreEqual(SerializeFormat.Unknown, ContentTypeUtils.GetFormat("multipart/xx"));
        Assert.AreEqual(SerializeFormat.Unknown, ContentTypeUtils.GetFormat("text/xx"));
        Assert.AreEqual(SerializeFormat.Unknown, ContentTypeUtils.GetFormat("xx/xx"));
    }

    [TestMethod]
    public void Test_GetByFormat()
    {
        Assert.AreEqual("text/plain; charset=utf-8", ContentTypeUtils.GetByFormat(SerializeFormat.Text));
        Assert.AreEqual("application/json; charset=utf-8", ContentTypeUtils.GetByFormat(SerializeFormat.Json));
        Assert.AreEqual("application/json; charset=utf-8", ContentTypeUtils.GetByFormat(SerializeFormat.Json2));
        Assert.AreEqual("application/xml; charset=utf-8", ContentTypeUtils.GetByFormat(SerializeFormat.Xml));
        Assert.AreEqual("application/x-www-form-urlencoded; charset=utf-8", ContentTypeUtils.GetByFormat(SerializeFormat.Form));
        Assert.AreEqual("multipart/form-data", ContentTypeUtils.GetByFormat(SerializeFormat.Multipart));
        Assert.AreEqual("application/octet-stream", ContentTypeUtils.GetByFormat(SerializeFormat.Binary));
        Assert.AreEqual("application/x-ndjson", ContentTypeUtils.GetByFormat(SerializeFormat.JsonLines));
        Assert.AreEqual(string.Empty, ContentTypeUtils.GetByFormat(SerializeFormat.None));
        Assert.AreEqual(string.Empty, ContentTypeUtils.GetByFormat(SerializeFormat.Auto));
        Assert.AreEqual(string.Empty, ContentTypeUtils.GetByFormat(SerializeFormat.Unknown));
    }
}
