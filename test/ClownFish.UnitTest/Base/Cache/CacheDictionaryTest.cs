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
        CacheDictionary<string> dict = new CacheDictionary<string>();

        string key = "key1";
        string value = "aaa";

        dict.Set(key, value, DateTime.Now.AddMilliseconds(10));
        Assert.AreEqual(value, dict.Get(key));

        System.Threading.Thread.Sleep(30);

        // 触发主动清理
        dict.SetFieldValue("_lastScanTime", DateTime.MinValue.Ticks);
        dict.CheckForExpiredItems();
        System.Threading.Thread.Sleep(100);

        Assert.AreEqual(0, dict.GetCount());
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
        Assert.IsTrue((bool)cache1.GetFieldValue("_autoExpiredClean"));

        CacheDictionary<string> cache2 = new CacheDictionary<string>(true);
        Assert.IsTrue((bool)cache2.GetFieldValue("_autoExpiredClean"));

        CacheDictionary<string> cache3 = new CacheDictionary<string>(false);
        Assert.IsFalse((bool)cache3.GetFieldValue("_autoExpiredClean"));

        CacheDictionary<string> cache4 = new CacheDictionary<string>(100, false, true);
        Assert.IsTrue((bool)cache4.GetFieldValue("_autoExpiredClean"));

        CacheDictionary<string> cache5 = new CacheDictionary<string>(100, false, false);
        Assert.IsFalse((bool)cache5.GetFieldValue("_autoExpiredClean"));
    }

    [TestMethod]
    public void Test_set_null()
    {
        var dict = new CacheDictionary<string>();
        dict.Set("aa", null);
        Assert.AreEqual(0, dict.GetCount());
    }

}
