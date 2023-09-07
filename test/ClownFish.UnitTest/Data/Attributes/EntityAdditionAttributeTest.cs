using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.Attributes;

[TestClass]
public class EntityAdditionAttributeTest
{
    [TestMethod]
    public void Test()
    {
        EntityAdditionAttribute a = new EntityAdditionAttribute {
            ProxyType = typeof(Customer),
        };

        Assert.AreEqual(typeof(Customer), a.ProxyType);
    }
}
