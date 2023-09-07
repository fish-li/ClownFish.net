namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class TSafeDictionaryTest
{
    [TestMethod]
    public void Test1_Basic()
    {
        TSafeDictionary<string, string> dict = new TSafeDictionary<string, string>(100, StringComparer.OrdinalIgnoreCase);

        dict.Set("key1", "111");
        Assert.AreEqual("111", dict.TryGet("KEY1"));

        dict["key1"] = "aaa";
        Assert.AreEqual("aaa", dict["KEY1"]);

        Assert.IsFalse(dict.TryAdd("Key1", "333"));  // 已经存在了，不允许再添加

        dict.AddValue("key2", "222");
        Assert.IsTrue(dict.TryGetValue("KeY2", out string value2));
        Assert.AreEqual("222", value2);

        ArgumentException lastException = null;
        try {
            dict.AddValue("key2", "333");  // 已经存在了，不允许再添加
        }
        catch( ArgumentException ex ) {
            lastException = ex;
        }
        Assert.IsNotNull(lastException);
        Assert.AreEqual("往集合中插入元素时发生了异常，当前Key=key2", lastException.Message);


        Assert.AreEqual(2, dict.Count);

        var dict2 = dict.Clone();
        Assert.AreEqual(2, dict2.Count);
        Assert.AreEqual("aaa", dict2["key1"]);
        Assert.AreEqual("222", dict2["key2"]);

        Assert.IsFalse(dict.TryRemove("key3", out string value3));
        Assert.IsTrue(dict.TryRemove("key2", out string value4));
        Assert.AreEqual("222", value4);

        Assert.AreEqual(1, dict.Count);

        dict.Clear();
        Assert.AreEqual(0, dict.Count);

        dict.Clear();
        Assert.AreEqual(0, dict.Count);


        Assert.IsTrue(dict.TryAdd("Key1", "333"));
        Assert.IsNull(dict.TryGet("key2"));
        Assert.IsFalse(dict.TryGetValue("key3", out string value5));
    }


    [TestMethod]
    public void Test2_Basic()
    {
        TSafeDictionary2<string, string> dict = new TSafeDictionary2<string, string>(100, StringComparer.OrdinalIgnoreCase);

        dict.Set("key1", "111");
        Assert.AreEqual("111", dict.TryGet("KEY1"));

        dict["key1"] = "aaa";
        Assert.AreEqual("aaa", dict["KEY1"]);

        Assert.IsFalse(dict.TryAdd("Key1", "333"));  // 已经存在了，不允许再添加

        dict.AddValue("key2", "222");
        Assert.IsTrue(dict.TryGetValue("KeY2", out string value2));
        Assert.AreEqual("222", value2);

        ArgumentException lastException = null;
        try {
            dict.AddValue("key2", "333");  // 已经存在了，不允许再添加
        }
        catch( ArgumentException ex ) {
            lastException = ex;
        }
        Assert.IsNotNull(lastException);
        Assert.AreEqual("往集合中插入元素时发生了异常，当前Key=key2", lastException.Message);


        Assert.AreEqual(2, dict.Count);

        var dict2 = dict.Clone();
        Assert.AreEqual(2, dict2.Count);
        Assert.AreEqual("aaa", dict2["key1"]);
        Assert.AreEqual("222", dict2["key2"]);

        Assert.IsFalse(dict.TryRemove("key3", out string value3));
        Assert.IsTrue(dict.TryRemove("key2", out string value4));
        Assert.AreEqual("222", value4);

        Assert.AreEqual(1, dict.Count);

        dict.Clear();
        Assert.AreEqual(0, dict.Count);

        dict.Clear();
        Assert.AreEqual(0, dict.Count);


        Assert.IsTrue(dict.TryAdd("Key1", "333"));
        Assert.IsNull(dict.TryGet("key2"));
        Assert.IsFalse(dict.TryGetValue("key3", out string value5));


        MyAssert.IsError<ArgumentNullException>(() => {
            dict.GetOrAdd(null, CreateFunc);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            dict.GetOrAdd("key", null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            dict.Set(null, "abc");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            dict.TryGet(null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            dict.AddValue(null, "xxx");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            dict.TryAdd(null, "xxx");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            dict.TryAdd(null, "xxx");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = dict.TryGetValue(null, out string xxx);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = dict.TryRemove(null, out string xxx);
        });

        (dict as IDisposable).Dispose();
    }


    [TestMethod]
    public void Test1_Concurrent()
    {
        int taskCount = 20;
        Random random = new Random();
        TSafeDictionary<string, string> dict = new TSafeDictionary<string, string>(100, StringComparer.OrdinalIgnoreCase);

        Task[] tasks = new Task[taskCount];

        for( int i = 0; i < taskCount; i++ )
            tasks[i] = Task.Run(() => {

                for( int k = 0; k < 200; k++ ) {

                    int flag = random.Next(1, 14);
                    switch( flag ) {
                        case 1:
                        case 2:
                        case 3: {
                                string value = dict.GetOrAdd("key1", CreateFunc);
                                Assert.AreEqual("key1_xx", value);
                            }
                            break;
                        case 4:
                        case 5:
                        case 6: {
                                string value = dict.GetOrAdd("key2", CreateFunc);
                                Assert.AreEqual("key2_xx", value);
                            }
                            break;

                        case 7:
                            dict["key3"] = "333";
                            break;

                        case 8: {
                                if( dict.TryGetValue("key3", out string value1) )
                                    Assert.AreEqual("333", value1);
                            }
                            break;

                        case 9:
                            dict.Set("key4", "444");
                            break;

                        case 10:
                            Assert.AreEqual("444", dict.TryGet("key4") ?? "444");
                            break;

                        case 11:
                            dict.Clone();
                            break;

                        case 12:
                            dict.Clear();
                            break;

                        case 13:
                            dict.TryAdd("key5", "555");
                            break;

                        case 14:
                            dict.TryRemove("key5", out string xx);
                            break;

                    }

                }
            });

        Task.WaitAll(tasks);
    }


    [TestMethod]
    public void Test2_Concurrent()
    {
        int taskCount = 20;
        Random random = new Random();
        TSafeDictionary2<string, string> dict = new TSafeDictionary2<string, string>(100, StringComparer.OrdinalIgnoreCase);

        Task[] tasks = new Task[taskCount];

        for( int i = 0; i < taskCount; i++ )
            tasks[i] = Task.Run(() => {

                for( int k = 0; k < 200; k++ ) {

                    int flag = random.Next(1, 14);
                    switch( flag ) {
                        case 1:
                        case 2:
                        case 3: {
                                string value = dict.GetOrAdd("key1", CreateFunc);
                                Assert.AreEqual("key1_xx", value);
                            }
                            break;
                        case 4:
                        case 5:
                        case 6: {
                                string value = dict.GetOrAdd("key2", CreateFunc);
                                Assert.AreEqual("key2_xx", value);
                            }
                            break;

                        case 7:
                            dict["key3"] = "333";
                            break;

                        case 8: {
                                if( dict.TryGetValue("key3", out string value1) )
                                    Assert.AreEqual("333", value1);
                            }
                            break;

                        case 9:
                            dict.Set("key4", "444");
                            break;

                        case 10:
                            Assert.AreEqual("444", dict.TryGet("key4") ?? "444");
                            break;

                        case 11:
                            dict.Clone();
                            break;

                        case 12:
                            dict.Clear();
                            break;

                        case 13:
                            dict.TryAdd("key5", "555");
                            break;

                        case 14:
                            dict.TryRemove("key5", out string xx);
                            break;

                    }

                }
            });

        Task.WaitAll(tasks);
    }


    private string CreateFunc(string x)
    {
        System.Threading.Thread.Sleep(3);
        return x + "_xx";
    }

}
