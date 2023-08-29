namespace PerformanceTest.Http;

// 部署测试站点
// 1, 将 \test\TestWebSite1 部署到IIS中，端口号：7000 ， 使用 .NET4.0 应用程序池
// 2, 将 \test\TestWebSite1 部署到另一台机器，设置同上，并且修改本机的 hosts 文件，增加一个域名映射  192.168.xx.xx    TestServer


public static class TestHttpClient2
{
    private static HttpOption NewHttpOption_LocalGet()
    {
        return new HttpOption {
            Url = "http://localhost:7000/test.txt",
        };
    }

    private static HttpOption NewHttpOption_LocalPost()
    {
        return new HttpOption {
            Method = "POST",
            Url = "http://localhost:7000/test1.aspx",
            Data = new {
                id = 2,
                name = "aaaaaaaaaaa",
                time = DateTime.Now,
                num = DateTime.Now.Ticks,
            },
            Format = ClownFish.Base.Http.SerializeFormat.Text,
        };
    }

    private static HttpOption NewHttpOption_RemoteGet()
    {
        return new HttpOption {
            Url = "http://TestServer:7000/test.txt",
        };
    }

    private static HttpOption NewHttpOption_RemotePost()
    {
        return new HttpOption {
            Method = "POST",
            Url = "http://TestServer:7000/test1.aspx",
            Data = new {
                id = 2,
                name = "aaaaaaaaaaa",
                time = DateTime.Now,
                num = DateTime.Now.Ticks,
            },
            Format = ClownFish.Base.Http.SerializeFormat.Text
        };
    }

#if NETFRAMEWORK
		[MenuMethod(Title = "性能测试 HttpClient V1 - Local GET")]
		public static void Test_HttpClient1_Local_GET()
		{
			Test_HttpClient(NewHttpOption_LocalGet(), SendRequest1);
		}


		[MenuMethod(Title = "性能测试 HttpClient V1 - Local POST")]
		public static void Test_HttpClient1_Local_POST()
		{
			Test_HttpClient(NewHttpOption_LocalPost(), SendRequest1);
		}
#endif

    [MenuMethod(Title = "性能测试 HttpClient V2 - Local GET")]
    public static void Test_HttpClient2_Local_GET()
    {
        Test_HttpClient(NewHttpOption_LocalGet(), SendRequest2);
    }


    [MenuMethod(Title = "性能测试 HttpClient V2 - Local POST")]
    public static void Test_HttpClient2_Local_POST()
    {
        Test_HttpClient(NewHttpOption_LocalPost(), SendRequest2);
    }



    // =================================================================================
#if NETFRAMEWORK
		[MenuMethod(Title = "性能测试 HttpClient V1 - Remote GET")]
		public static void Test_HttpClient1_Remote_GET()
		{
			Test_HttpClient(NewHttpOption_RemoteGet(), SendRequest1);
		}


		[MenuMethod(Title = "性能测试 HttpClient V1 - Remote POST")]
		public static void Test_HttpClient1_Remote_POST()
		{
			Test_HttpClient(NewHttpOption_RemotePost(), SendRequest1);
		}
#endif

    [MenuMethod(Title = "性能测试 HttpClient V2 - Remote GET")]
    public static void Test_HttpClient2_Remote_GET()
    {
        Test_HttpClient(NewHttpOption_RemoteGet(), SendRequest2);
    }


    [MenuMethod(Title = "性能测试 HttpClient V2 - Remote POST")]
    public static void Test_HttpClient2_Remote_POST()
    {
        Test_HttpClient(NewHttpOption_RemotePost(), SendRequest2);
    }






    private static void Test_HttpClient(HttpOption option, Func<HttpOption, string> action)
    {
        // 先发一次，预热
        action(option);

        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < 10000; i++ ) {
            action(option);
        }
        watch.Stop();

        Console.WriteLine("===================================");
        Console.WriteLine(watch.Elapsed);
    }

#if NETFRAMEWORK
		private static string SendRequest1(HttpOption option)
		{
			var httpClient = new ClownFish.Base.WebClient.V1.HttpClient(option);
			return httpClient.Send<string>();
		}
#endif

    private static string SendRequest2(HttpOption option)
    {
        var httpClient = new ClownFish.Base.WebClient.V2.HttpClient2(option);
        return httpClient.Send<string>();
    }

}
