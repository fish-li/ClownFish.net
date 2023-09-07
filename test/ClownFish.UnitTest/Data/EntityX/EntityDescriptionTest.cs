using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.EntityX;

[TestClass]
public class EntityDescriptionTest
{
    [TestMethod]
    public void Test1()
    {
        EntityDescription desc = EntityDescriptionCache.Get(typeof(OrderDetailX1));
        Assert.IsNotNull(desc);

        MyAssert.IsError<NotSupportedException>(()=> {
            var c1 = desc.GetPrimaryKey();
        });


        var c2 = desc.GetIdentity();
        Assert.IsNull(c2);
    }


    [TestMethod]
    public void Test2()
    {
        EntityDescription desc = EntityDescriptionCache.Get(typeof(OrderDetailX2));
        Assert.IsNotNull(desc);

        MyAssert.IsError<NotSupportedException>(() => {
            var c1 = desc.GetIdentity();
        });

        var c2 = desc.GetPrimaryKey();
        Assert.IsNull(c2);

        string text1 = desc.GetInsertColumns(true).Select(x => x.DbName).Merge(",");
        string text2 = desc.GetInsertColumns(false).Select(x => x.DbName).Merge(",");

        Console.WriteLine(text1);
        Console.WriteLine(text2);

        Assert.AreEqual("UnitPrice,Quantity,X2", text1);
        Assert.AreEqual("OrderID,ProductID,UnitPrice,Quantity,X2", text2);
    }
}
