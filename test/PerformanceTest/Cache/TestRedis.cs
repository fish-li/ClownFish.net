//using StackExchange.Redis;

//namespace PerformanceTest.Cache;

//public static class TestRedis
//{
//    [MenuMethod(Title = "Redis-Write--单线程")]
//    public static void Test1()
//    {
//        int count = 10000;
//        TimeSpan time1 = TestStringSetGet(count);
//        Console.WriteLine($"Redis String, Set/Get {count} 次调用, 耗时: {time1}");
//    }


//    [MenuMethod(Title = "Redis-Write--多线程")]
//    public static void Test2()
//    {
//        int count = 1000;
//        int threadCount = 10;
//        Console.WriteLine("线程数量： " + threadCount);
//        TimeSpan time1 = ThreadUtils.TestMultiThread(threadCount, () => TestStringSetGet(count));
//        Console.WriteLine($"Redis String, Set/Get {count * threadCount} 条消息, 耗时: {time1}");
//    }


//    private static TimeSpan TestStringSetGet(int count)
//    {
//        IDatabase db = Redis.GetDatabase();
//        Stopwatch stopwatch = Stopwatch.StartNew();

//        for( int i = 0; i < count; i++ ) {
//            string key = "xml_" + Guid.NewGuid().ToString("N");
//            db.StringSet(key, TestData.XmlText, TimeSpan.FromMinutes(2));
//            db.StringGet(key);
//        }
//        stopwatch.Stop();
//        return stopwatch.Elapsed;
//    }

//}
