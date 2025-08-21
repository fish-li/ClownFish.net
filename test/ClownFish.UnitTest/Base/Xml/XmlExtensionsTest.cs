namespace ClownFish.UnitTest.Base.Xml;

[TestClass]
public class XmlExtensionsTest
{
    [TestMethod]
    public void Test_ToXml_FromXml()
    {
        Product3 p = Product3.CreateByRandomData();

        string xml = p.ToXml();
        Product3 p2 = xml.FromXml<Product3>();

        Assert.IsTrue(p.IsEqual(p2));
    }



    [TestMethod]
    public void Test_FromXml_ObjectType()
    {
        Product3 p = Product3.CreateByRandomData();

        string xml = p.ToXml();
        Product3 p2 = xml.FromXml(typeof(Product3)) as Product3;

        Assert.IsTrue(p.IsEqual(p2));
    }
}
