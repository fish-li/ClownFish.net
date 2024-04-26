namespace ClownFish.UnitTest.Base.Cache;

[TestClass]
public class CacheDictionaryTest
{
    [TestMethod]
    public void Test_BasicGetSet()
    {
        CacheDictionary<string> dict = new CacheDictionary<string>();

        string key = "key1";
        string value = "aaa";

        dict.Set(key, value);
        Assert.AreEqual(value, dict.Get(key));

        dict.Set(key, value, new DateTime(2000, 1, 1));
        Assert.IsNull(dict.Get(key));

        //-------------------------------------------
        dict.Set(key, value);
        Assert.AreEqual(value, dict.Get(key));

        dict.Remove(key);
        Assert.IsNull(dict.Get(key));

        //----------------------------------------------
        dict.Set(key, value);
        Assert.AreEqual(value, dict.Get(key));
        //----------------------------------------------
        var dump = dict.DumpData();
        Assert.AreEqual(1, dump.Count);
        Assert.AreEqual("key1", dump.Keys.First());

        //----------------------------------------------
        dict.Clear();
        Assert.IsNull(dict.Get(key));
    }

    [TestMethod]
    public void Test_CheckForExpiredItems()
    {
        int deleteCount = 0;

        Action<string> cleanCallback = name => { 
            Console.WriteLine("CacheDictionary clean key: " + name);
            deleteCount++;
        };

        CacheDictionary<string> dict = new CacheDictionary<string>(10, false, 1, cleanCallback);
        Assert.IsTrue(dict.IsAutoExpiredClean);

        string key = "key1";
        string value = "aaa";

        dict.Set(key, value, DateTime.Now.AddMilliseconds(100));
        Assert.AreEqual(value, dict.Get(key));

        Thread.Sleep(1000 /* expirationScanFrequency */ + 100 /* item expiration */ + 50);

        dict.Set("key2", "bbb", DateTime.Now.AddMilliseconds(100));   // 触发 dict.CheckExpiredItems()

        // 等待过期清理的后台线程执行
        System.Threading.Thread.Sleep(500);

        Assert.AreEqual(1, dict.GetCount());
        Assert.AreEqual(1, deleteCount);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ArgumentNullException()
    {
        CacheDictionary<string> dict = new CacheDictionary<string>();

        string key = null;
        string value = "aaa";            
        dict.Set(key, value);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_ArgumentOutOfRangeException()
    {
        CacheDictionary<string> dict = new CacheDictionary<string>();

        string key = new string('x', 257);
        string value = "aaa";
        dict.Set(key, value);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ArgumentNullException1()
    {
        CacheDictionary<string> dict = new CacheDictionary<string>();

        var str = dict.Get(null);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_ArgumentNullException2()
    {
        CacheDictionary<string> dict = new CacheDictionary<string>();

        dict.Remove(null);
    }

    [TestMethod]
    public void Test_ctor()
    {
        CacheDictionary<string> cache1 = new CacheDictionary<string>();
        Assert.IsTrue(cache1.IsAutoExpiredClean);

        CacheDictionary<string> cache2 = new CacheDictionary<string>(true);
        Assert.IsTrue(cache2.IsAutoExpiredClean);

        CacheDictionary<string> cache3 = new CacheDictionary<string>(false);
        Assert.IsFalse(cache3.IsAutoExpiredClean);

        CacheDictionary<string> cache4 = new CacheDictionary<string>(100, false, true);
        Assert.IsTrue(cache4.IsAutoExpiredClean);

        CacheDictionary<string> cache5 = new CacheDictionary<string>(100, false, false);
        Assert.IsFalse(cache5.IsAutoExpiredClean);
    }

    [TestMethod]
    public void Test_set_null()
    {
        var dict = new CacheDictionary<string>();
        dict.Set("aa", null);
        Assert.AreEqual(0, dict.GetCount());
    }

}
