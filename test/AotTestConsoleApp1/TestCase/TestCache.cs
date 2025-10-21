using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AotTestConsoleApp1.TestCase;
internal class TestCache
{
    private static readonly CacheDictionary<NameValue> s_cache = new CacheDictionary<NameValue>();

    public static async Task Run()
    {
        await Test_CacheDictionary();
        await Test_AppCache();
    }

    private static async Task Test_CacheDictionary()
    {
        string key1 = Guid.NewGuid().ToString();
        NameValue nv = new NameValue { Name = "abc", Value = "111" };
        s_cache.Set(key1, nv);

        await Task.Delay(1);

        NameValue nv2 = s_cache.Get(key1);
        Assert.IsNotNull(nv2);
        Assert.AreEqual("abc", nv2.Name);
        Assert.AreEqual("111", nv2.Value);


        string key2 = Guid.NewGuid().ToString();
        NameValue nv3 = new NameValue { Name = "qaz", Value = "222" };
        s_cache.Set(key2, nv3, DateTime.Now.AddDays(-1));

        await Task.Delay(1);

        NameValue nv4 = s_cache.Get(key2);
        Assert.IsNull(nv4);
    }

    private static async Task Test_AppCache()
    {
        string key1 = Guid.NewGuid().ToString();
        NameValue nv = new NameValue { Name = "abc", Value = "111" };
        AppCache.SetObject(key1, nv, DateTime.Now.AddDays(1));

        await Task.Delay(1);
        NameValue nv2 = AppCache.GetObject<NameValue>(key1);
        Assert.IsNotNull(nv2);
        Assert.AreEqual("abc", nv2.Name);
        Assert.AreEqual("111", nv2.Value);

        AppCache.RemoveObject(key1);
        await Task.Delay(1);

        NameValue nv4 = s_cache.Get(key1);
        Assert.IsNull(nv4);
    }
}
