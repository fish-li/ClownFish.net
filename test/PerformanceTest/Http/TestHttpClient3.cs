namespace PerformanceTest.Http;

// 部署测试站点
// 将 \test\TestWebSite1 部署到IIS中，端口号：7000 ， 使用 .NET4.0 应用程序池

public static class TestHttpClient3
{
    [MenuMethod(Title = "50 线程--发起HTTP请求")]
    public static void Test()
    {
        int threadCount = 50;
        Thread[] threads = new Thread[threadCount];

        for( int i = 0; i < threadCount; i++ ) {
            threads[i] = new Thread(ThreadAction);
            threads[i].Start();
        }

        for( int i = 0; i < threadCount; i++ ) {
            threads[i].Join();
        }
    }

    private static void ThreadAction()
    {
        for( int i = 0; i < 100000; i++ ) {

            HttpOption httpOption = NewHttpOption_LocalPost();

            try {
                SendRequest2(httpOption);
                Console2.Info("OK");
            }
            catch( Exception ex ) {
                Console2.Error(ex.Message);
            }
        }
    }


    private static HttpOption NewHttpOption_LocalPost()
    {
        return new HttpOption {
            Method = "POST",
            Url = "http://localhost:7000/test-slow-req.aspx",
            Data = new {
                id = 2,
                name = "aaaaaaaaaaa",
                time = DateTime.Now,
                num = DateTime.Now.Ticks,
            },
            Format = ClownFish.Base.Http.SerializeFormat.Json,
            Timeout = 2000
        };
    }

    private static string SendRequest2(HttpOption option)
    {
        var httpClient = new ClownFish.Base.WebClient.V2.HttpClient2(option);
        return httpClient.Send<string>();
    }

}
