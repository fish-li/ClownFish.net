using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.Utils;

[TestClass]
public class SerializerTest
{

    [TestMethod]
    public void Test_代理对象序列化_XML()
    {
        using( DbContext db = DbContext.Create() ) {
            Product p = db.Entity.BeginEdit<Product>();
            p.ProductID = 2;
            p.CategoryID = 3;
            p.ProductName = "汉字";


            string xml = p.ToXml2();
            Console.WriteLine(xml);
            Product p3 = xml.FromXml<Product>();

            Assert.AreEqual(p.ProductID, p3.ProductID);
            Assert.AreEqual(p.CategoryID, p3.CategoryID);
            Assert.AreEqual(p.ProductName, p3.ProductName);
            Assert.AreEqual(p.Quantity, p3.Quantity);

            string xml3 = p3.ToXml2();
            Assert.AreEqual(xml, xml3);
        }
    }


    [TestMethod]
    public void Test_代理对象序列化_JSON()
    {
        using( DbContext db = DbContext.Create() ) {
            Product p = db.Entity.BeginEdit<Product>();
            p.ProductID = 2;
            p.CategoryID = 3;
            p.ProductName = "汉字";


            string json = p.ToJson(JsonStyle.KeepType);
            Console.WriteLine(json);
            Product p3 = json.FromJson<Product>();

            Assert.AreEqual(p.ProductID, p3.ProductID);
            Assert.AreEqual(p.CategoryID, p3.CategoryID);
            Assert.AreEqual(p.ProductName, p3.ProductName);
            Assert.AreEqual(p.Quantity, p3.Quantity);


            string json3 = p3.ToJson(JsonStyle.KeepType);
            Assert.AreEqual(json, json3);
        }
    }
}
