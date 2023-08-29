namespace PerformanceTest.Http;

public static class TestHttpClient
{
    private class ThreadParam
    {
        public int Index;
        public string Result;
        public string Time;
        internal ManualResetEvent ManualEvent;
    }

    private static readonly List<Exception> s_exceptions = new List<Exception>();

    [MenuMethod(Title = "并发测试 HttpClient - 100 线程 * 1000次")]
    public static void Test1()
    {
        Test(100);
    }

    [MenuMethod(Title = "并发测试 HttpClient -  10 线程 * 1000次")]
    public static void Test2()
    {
        Test(10);
    }


    [MenuMethod(Title = "并发测试 HttpClient -   4 线程 * 1000次")]
    public static void Test3()
    {
        Test(4);
    }

    private static void Test(int threadCount)
    {
        ThreadParam[] results = new ThreadParam[threadCount];

        ManualResetEvent manualEvent = new ManualResetEvent(false);

        DateTime start = DateTime.Now;
        Thread[] threads = new Thread[threadCount];

        for( int i = 0; i < threadCount; i++ ) {
            threads[i] = new Thread(CallConfigService);
            results[i] = new ThreadParam { Index = i, ManualEvent = manualEvent };

            threads[i].Start(results[i]);
        }

        manualEvent.Set();

        for( int i = 0; i < threadCount; i++ ) {
            threads[i].Join();
        }

        var result = new {
            Time = DateTime.Now - start,
            results = results.Take(5).ToList().Union(results.SkipWhile(x => x.Index < threadCount - 5).ToList())
        };

        Console.WriteLine(result.ToJson(JsonStyle.Indented));

        if( s_exceptions.Count > 0 ) {
            Console.WriteLine("===========================================");
            Console.WriteLine("异常次数：" + s_exceptions.Count);
            Console.WriteLine("===========================================");
            Console.WriteLine(s_exceptions[0].ToString());
        }
    }





    private static void CallConfigService(object args)
    {
        string name = "Kibana.Url";
        string configServiceUrl = LocalSettings.GetSetting("ConfigServiceUrl", true).TrimEnd('/');

        HttpOption httpOption = new HttpOption {
            Url = configServiceUrl + "/v20/api/moon/setting/get",
            Data = new { name },
            //Timeout = 5000
        };

        ThreadParam threadParam = (ThreadParam)args;
        threadParam.ManualEvent.WaitOne();

        DateTime start = DateTime.Now;
        try {
            for( int i = 0; i < 1000; i++ )
                threadParam.Result = httpOption.GetResult();  // send request
        }
        catch( Exception ex ) {
            lock( s_exceptions )
                s_exceptions.Add(ex);
        }
        threadParam.Time = (DateTime.Now - start).ToString();
    }
}
