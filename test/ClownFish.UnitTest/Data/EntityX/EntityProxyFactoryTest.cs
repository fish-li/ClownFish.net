using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.EntityX;

[TestClass]
public class EntityProxyFactoryTest : BaseTest
{
    [TestMethod]
    public void Test_GetEntityTypes()
    {
        string names = EntityProxyFactory.GetEntityTypes().Select(x => x.Name).Merge("\n");

        Console.WriteLine(names);

        Assert.IsTrue(names.Contains("Product"));
        Assert.IsTrue(names.Contains("Customer"));
        Assert.IsTrue(names.Contains("Category"));
        Assert.IsTrue(names.Contains("ModelX"));

        Assert.IsFalse(names.Contains("OrderDetailX1"));
    }


    [TestMethod]
    public void Test_Register()
    {
        using( DbContext dbContext = DbContext.Create() ) {
            Product product = dbContext.Entity.CreateProxy<Product>();
            Type t1 = product.GetType();

            Assert.AreEqual(typeof(Product), t1.BaseType);


            // 覆盖初始化时的注册，结果其实是一样的
            EntityProxyFactory.Register(t1);

            Product product2 = dbContext.Entity.CreateProxy<Product>();
            Type t2 = product2.GetType();

            Assert.AreEqual(typeof(Product), t2.BaseType);
            Assert.AreEqual(t1, t2);
        }
    }


    [TestMethod]
    public void Test_Register_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            EntityProxyFactory.Register(null);
        });

        MyAssert.IsError<ArgumentException>(() => {
            EntityProxyFactory.Register(typeof(OrderDetailX1));
        });

        MyAssert.IsError<ArgumentException>(() => {
            EntityProxyFactory.Register(typeof(Product));
        });

    }





}
