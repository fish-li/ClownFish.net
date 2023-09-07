namespace ClownFish.UnitTest.Base.Cache;

[TestClass]
public class AppCacheTest
{
    [TestMethod]
    public void Test_GetObject()
    {
        string key = Guid.NewGuid().ToString();

        Product2 p1 = AppCache.GetObject<Product2>(key);
        Assert.IsNull(p1);

        Product2 p2 = AppCache.GetObject<Product2>(key, ()=> {
            return new Product2 { ProductID = 3, ProductName = "Name5" };
        });

        Assert.IsNotNull(p2);
        Assert.AreEqual("Name5", p2.ProductName);

        Assert.IsTrue(AppCache.GetCount() > 0);  // 这个判断仅仅为了代码覆盖，没其他用途！
    }

    [TestMethod]
    public void Test_SetObject()
    {
        string key = Guid.NewGuid().ToString();

        Product2 p1 = AppCache.GetObject<Product2>(key);
        Assert.IsNull(p1);

        Product2 p2 = new Product2 { ProductID = 3, ProductName = "Name5" };
        AppCache.SetObject(key, p2, DateTime.Now.AddMinutes(1));

        p1 = AppCache.GetObject<Product2>(key);
        Assert.IsNotNull(p1);

        Assert.AreEqual(p1, p2);


        AppCache.RemoveObject(key);

        Product2 p3 = AppCache.GetObject<Product2>(key);
        Assert.IsNull(p3);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_GetObject_ArgsNull()
    {
        string key = null;
        Product2 value = AppCache.GetObject<Product2>(key);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_SetObject_ArgsNull()
    {
        string key = null;
        AppCache.SetObject(key, new Product2(), DateTime.Now.AddDays(1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_RemoveObject_ArgsNull()
    {
        string key = null;
        AppCache.RemoveObject(key);
    }

}
