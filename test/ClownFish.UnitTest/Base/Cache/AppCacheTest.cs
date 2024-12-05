using static Mysqlx.Expect.Open.Types.Condition.Types;

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

        Product2 p2 = AppCache.GetObject<Product2>(key, () => {
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
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            string key = null;
            Product2 value = AppCache.GetObject<Product2>(key);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            string key = null;
            AppCache.SetObject(key, new Product2(), DateTime.Now.AddDays(1));
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            string key = null;
            AppCache.RemoveObject(key);
        });
    }


    [TestMethod]
    public void Test_GetObject_cacheMs()
    {
        string key = Guid.NewGuid().ToString();

        Product2 LoadData()
        {
            return new Product2 { ProductID = 3, ProductName = "Name5" };
        }

        Product2 p1 = AppCache.GetObject<Product2>(key, LoadData, 20);
        Assert.IsNotNull(p1);

        Thread.Sleep(50);

        Product2 p2 = AppCache.GetObject<Product2>(key);
        Assert.IsNull(p2);
    }


    [TestMethod]
    public void Test_GetObject_lock()
    {
        string key = Guid.NewGuid().ToString();
        ValueCounter loadCounter = new ValueCounter();
        ValueCounter errorCounter = new ValueCounter();

        Product2 LoadData()
        {
            loadCounter.Increment();
            Thread.Sleep(100);
            return new Product2 { ProductID = 3, ProductName = "Name" + loadCounter.Get() };
        }

        void ThreadAction(object xx)
        {
            Product2 p = AppCache.GetObject<Product2>(key, LoadData) as Product2;

            try {
                Assert.IsNotNull(p);
                Assert.AreEqual("Name1", p.ProductName);
            }
            catch {
                errorCounter.Increment();
            }
        }

        Thread[] threads = new Thread[20];

        for( int i = 0; i < threads.Length; i++ ) {
            Thread thread = new Thread(ThreadAction);
            thread.IsBackground = true;
            threads[i] = thread;
        }

        for( int i = 0; i < threads.Length; i++ ) {
            threads[i].Start();
        }

        for( int i = 0; i < threads.Length; i++ ) {
            threads[i].Join();
        }

        Assert.AreEqual(1, loadCounter.Get());
        Assert.AreEqual(0, errorCounter.Get());
    }

}
