namespace PerformanceTest.Cache;

public static class TestCacheDictionary
{
    private static readonly CacheDictionary<string> s_cache = new CacheDictionary<string>();

    [MenuMethod(Title = "CacheDictionary-Write--单线程")]
    public static void Test1()
    {
        int count = 10000;
        TimeSpan time1 = TestStringSetGet(count);
        Console.WriteLine($"CacheDictionary<String>, Set/Get {count} 次调用, 耗时: {time1}");
    }


    [MenuMethod(Title = "CacheDictionary-Write--多线程")]
    public static void Test2()
    {
        int count = 1000;
        int threadCount = 10;
        Console.WriteLine("线程数量： " + threadCount);
        TimeSpan time1 = ThreadUtils0.TestMultiThread(threadCount, () => TestStringSetGet(count));
        Console.WriteLine($"CacheDictionary<String>, Set/Get {count * threadCount} 条消息, 耗时: {time1}");
    }


    private static TimeSpan TestStringSetGet(int count)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        for( int i = 0; i < count; i++ ) {
            string key = "xml_" + Guid.NewGuid().ToString("N");
            s_cache.Set(key, TestData.XmlText, DateTime.Now.AddMinutes(2));
            var x = s_cache.Get(key);
        }
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}
