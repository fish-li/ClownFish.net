namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class XDictionaryTest
{
    [TestMethod]
    public void Test1()
    {
        Hashtable hashtable = new Hashtable();
        XDictionary dict = new XDictionary(hashtable);

        RunTest(dict);
    }

    [TestMethod]
    public void Test2()
    {
        IDictionary<object,object> hashtable = new Dictionary<object, object>();
        XDictionary dict = new XDictionary(hashtable);

        RunTest(dict);
    }

    [TestMethod]
    public void Test3()
    {
        XDictionary dict = new XDictionary();

        RunTest(dict);
    }


    private void RunTest(XDictionary dict)
    {
        Assert.AreEqual(0, dict.Count);

        dict["key1"] = "value1";
        dict["key2"] = 123;
        Assert.AreEqual(2, dict.Count);

        Assert.IsTrue(dict.ContainsKey("key1"));
        Assert.IsTrue(dict.ContainsKey("key2"));

        Assert.AreEqual("value1", (string)dict["key1"]);
        Assert.AreEqual(123, (int)dict["key2"]);

        var keys = dict.Keys;
        Assert.AreEqual(2, keys.Count);

        MyAssert.IsError<NotImplementedException>(() => {
            var x = dict.Values;
        });

        Assert.AreEqual(false, dict.IsReadOnly);

        dict.Add(new KeyValuePair<object, object>("key3", Guid.Empty));
        dict.Add("key4", DateTime.MinValue);
        
        Assert.AreEqual(4, dict.Count);

        Assert.IsTrue(dict.Contains(new KeyValuePair<object, object>("key3", Guid.Empty)));
        Assert.IsTrue(dict.ContainsKey("key4"));

        MyAssert.IsError<NotImplementedException>(() => {
            KeyValuePair<object, object>[] xx = new KeyValuePair<object, object>[20];
            dict.CopyTo(xx, 0);
        });

        foreach(var kv in dict ) {
            Console.WriteLine($"key={kv.Key}, value={kv.Value}");
        }


        IEnumerator enumerator = (dict as IEnumerable).GetEnumerator();
        Assert.IsNotNull(enumerator);

        dict.Remove("key4");
        dict.Remove(new KeyValuePair<object, object>("key3", Guid.Empty));
        Assert.AreEqual(2, dict.Count);

        Assert.IsTrue(dict.TryGetValue("key1", out object value1));
        Assert.IsTrue(dict.TryGetValue("key2", out object value2));
        Assert.IsFalse(dict.TryGetValue("key3", out object value3));

        dict.Clear();
        Assert.AreEqual(0, dict.Count);
    }
}
