using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers;

[TestClass]
public class WriterFactoryTest
{
    [TestMethod]
    public void Test_IsSupport()
    {
        Assert.IsTrue(WriterFactory.IsSupport(typeof(OprLog)));
        Assert.IsTrue(WriterFactory.IsSupport(typeof(InvokeLog)));
        Assert.IsTrue(WriterFactory.IsSupport(typeof(XMessage)));

        Assert.IsFalse(WriterFactory.IsSupport(typeof(string)));
        Assert.IsFalse(WriterFactory.IsSupport(typeof(NameValue)));
    }


    [TestMethod]
    public void Test_GetWriters()
    {
        Assert.AreEqual(3, WriterFactory.GetWriters(typeof(OprLog)).Length);     // Xml,Json,txt
        Assert.AreEqual(5, WriterFactory.GetWriters(typeof(InvokeLog)).Length);  // Xml,Json,Json2,http,txt
        Assert.AreEqual(2, WriterFactory.GetWriters(typeof(XMessage)).Length);   // mem,NULL


        Assert.IsNull(WriterFactory.GetWriters(typeof(string)));
        Assert.IsNull(WriterFactory.GetWriters(typeof(NameValue)));
    }
}
