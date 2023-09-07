namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class HttpExtensionsTest
{
    [TestMethod]
    public void Test_WebHeaderCollection_AddWithoutValidate()
    {
        WebHeaderCollection collection = new WebHeaderCollection();

        collection.InternalAdd("a1", "111");
        Assert.AreEqual("111", collection["a1"]);
    }
}
