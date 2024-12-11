namespace ClownFish.UnitTest.Http.Utils;

[TestClass]
public class RequestContentTypeTest
{
    [TestMethod]
    public void Test_GetFormat()
    {
        Assert.AreEqual(SerializeFormat.None, ContenTypeUtils.GetFormat(null));
        Assert.AreEqual(SerializeFormat.None, ContenTypeUtils.GetFormat(string.Empty));

        Assert.AreEqual(SerializeFormat.Json, ContenTypeUtils.GetFormat("application/json"));
        Assert.AreEqual(SerializeFormat.Json, ContenTypeUtils.GetFormat("application/json; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Xml, ContenTypeUtils.GetFormat("application/xml"));
        Assert.AreEqual(SerializeFormat.Xml, ContenTypeUtils.GetFormat("application/xml; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Form, ContenTypeUtils.GetFormat("application/x-www-form-urlencoded"));
        Assert.AreEqual(SerializeFormat.Form, ContenTypeUtils.GetFormat("application/x-www-form-urlencoded; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Multipart, ContenTypeUtils.GetFormat("multipart/form-data; boundary=xxxxx"));

        Assert.AreEqual(SerializeFormat.Binary, ContenTypeUtils.GetFormat("application/octet-stream"));

        Assert.AreEqual(SerializeFormat.Text, ContenTypeUtils.GetFormat("text/plain"));
        Assert.AreEqual(SerializeFormat.Text, ContenTypeUtils.GetFormat("text/plain; charset=utf-8"));

        Assert.AreEqual(SerializeFormat.Unknown, ContenTypeUtils.GetFormat("application/xx"));
        Assert.AreEqual(SerializeFormat.Unknown, ContenTypeUtils.GetFormat("multipart/xx"));
        Assert.AreEqual(SerializeFormat.Unknown, ContenTypeUtils.GetFormat("text/xx"));
        Assert.AreEqual(SerializeFormat.Unknown, ContenTypeUtils.GetFormat("xx/xx"));
    }

    [TestMethod]
    public void Test_GetByFormat()
    {
        Assert.AreEqual("text/plain; charset=utf-8", ContenTypeUtils.GetByFormat(SerializeFormat.Text));
        Assert.AreEqual("application/json; charset=utf-8", ContenTypeUtils.GetByFormat(SerializeFormat.Json));
        Assert.AreEqual("application/json; charset=utf-8", ContenTypeUtils.GetByFormat(SerializeFormat.Json2));
        Assert.AreEqual("application/xml; charset=utf-8", ContenTypeUtils.GetByFormat(SerializeFormat.Xml));
        Assert.AreEqual("application/x-www-form-urlencoded; charset=utf-8", ContenTypeUtils.GetByFormat(SerializeFormat.Form));
        Assert.AreEqual("multipart/form-data", ContenTypeUtils.GetByFormat(SerializeFormat.Multipart));
        Assert.AreEqual("application/octet-stream", ContenTypeUtils.GetByFormat(SerializeFormat.Binary));
        Assert.AreEqual(string.Empty, ContenTypeUtils.GetByFormat(SerializeFormat.None));
        Assert.AreEqual(string.Empty, ContenTypeUtils.GetByFormat(SerializeFormat.Auto));
        Assert.AreEqual(string.Empty, ContenTypeUtils.GetByFormat(SerializeFormat.Unknown));
    }
}
