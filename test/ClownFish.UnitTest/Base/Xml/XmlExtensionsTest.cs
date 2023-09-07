namespace ClownFish.UnitTest.Base.Xml;

[TestClass]
public class XmlExtensionsTest
{
    [TestMethod]
    public void Test_ToXml_FromXml()
    {
        Product2 p = Product2.CreateByRandomData();

        string xml = p.ToXml();
        Product2 p2 = xml.FromXml<Product2>();

        Assert.IsTrue(p.IsEqual(p2));
    }



    [TestMethod]
    public void Test_FromXml_ObjectType()
    {
        Product2 p = Product2.CreateByRandomData();

        string xml = p.ToXml();
        Product2 p2 = xml.FromXml(typeof(Product2)) as Product2;

        Assert.IsTrue(p.IsEqual(p2));
    }
}
