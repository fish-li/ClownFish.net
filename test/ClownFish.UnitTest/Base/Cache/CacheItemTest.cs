﻿namespace ClownFish.UnitTest.Base.Cache;

[TestClass]
public class CacheItemTest
{
    [TestMethod]
    public void Test1()
    {
        CacheItem<string> x = new CacheItem<string>("abc", DateTime.Now.AddDays(1));
        Assert.AreEqual("abc", x.Value);

        x.Set(null, DateTime.Now.AddDays(1));
        Assert.IsNull(x.Value);
    }


    [TestMethod]
    public void TestSmallTextGetSet()
    {
        CacheItem<string> item = new CacheItem<string>(null, DateTime.MinValue);

        string text = "aaaaaaaaaaaaaaaaaaaaaaaaa";

        item.Set(text, DateTime.Now.AddDays(1));
        Assert.AreEqual(text, item.Get());

        object itemValue = item.GetType().InvokeMember("_value",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, item, null);
        Assert.IsNotNull(itemValue);

        object itemWeakObject = item.GetType().InvokeMember("_weakObject",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, item, null);
        Assert.IsNull(itemWeakObject);


        item.Set(text, new DateTime(2000, 1, 1));
        Assert.IsNull(item.Get());
    }


    [TestMethod]
    public void TestLargeTextGetSet()
    {
        CacheItem<string> item = new CacheItem<string>(null, DateTime.MinValue);

        string text = new string('a', 2048);

        item.Set(text, DateTime.Now.AddDays(1));
        Assert.AreEqual(text, item.Get());

        object itemValue = item.GetType().InvokeMember("_value",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, item, null);
        Assert.IsNull(itemValue);

        object itemWeakObject = item.GetType().InvokeMember("_weakObject",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, item, null);
        Assert.IsNotNull(itemWeakObject);


        item.Set(text, new DateTime(2000, 1, 1));
        Assert.IsNull(item.Get());
    }

    [TestMethod]
    public void TestNotOverdue()
    {
        CacheItem<string> item = new CacheItem<string>(null, DateTime.MinValue);

        string text = new string('a', 2048);

        item.Set(text, DateTime.MaxValue);      // 永不过期
        Assert.AreEqual(text, item.Get());

        object itemValue = item.GetType().InvokeMember("_value",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, item, null);
        Assert.IsNotNull(itemValue);

        object itemWeakObject = item.GetType().InvokeMember("_weakObject",
                    BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, item, null);
        Assert.IsNull(itemWeakObject);
    }

    [TestMethod]
    public void Test_Dispose()
    {
        int count1 = TestData.Count;

        using(CacheItem<TestData> item = new CacheItem<TestData>(new TestData(), DateTime.Now.AddDays(1), false) ) {
            Assert.IsNull(item.GetFieldValue("_weakObject"));
            Assert.IsNotNull(item.GetFieldValue("_value"));
        }

        int count2 = TestData.Count;
        Assert.AreEqual(count1+1, count2);

        using( CacheItem<TestData> item = new CacheItem<TestData>(new TestData(), DateTime.Now.AddDays(1), true) ) {
            Assert.IsNull(item.GetFieldValue("_value"));
            Assert.IsNotNull(item.GetFieldValue("_weakObject"));
        }

        int count3 = TestData.Count;
        Assert.AreEqual(count2 + 1, count3);
    }



    private class TestData : IDisposable
    {
        public static int Count = 0;

        public void Dispose()
        {
            Count++;
        }
    }


    [TestMethod]
    public void TestDisposableData()
    {
        CacheItem<TestData> item = new CacheItem<TestData>(null, DateTime.MinValue);

        TestData data = new TestData();
        TestData.Count = 0;

        item.Set(data, DateTime.Now.AddDays(-1));
        Assert.IsNull(item.Get());

        Assert.AreEqual(1, TestData.Count);

        item.Dispose();
    }

}
