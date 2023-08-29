namespace PerformanceTest.Cache;

public static class TestData
{
    public static OprLog CreateTestMessage()
    {
        return new OprLog {
            OprId = "bc1da6d7-4a41-4756-8c1c-c5cc267fd760",
            StartTime = new DateTime(2010, 4, 18, 17, 22, 15),
            HostName = "FISHWIN81DEV2",
            AppName = "ClownFish.Venus",

            AppKind = 100,
            RetryCount = 0,

            Duration = 1530,
            Url = "http://LinuxTest:8208/v20/api/venus/heartbeat/test.aspx",
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36 Edg/86.0.622.56",

            Module = "演示代码",
            Controller = "客户管理",
            Action = "根据多个ID号查询客户",                
            
            TenantId = "my596c871f21722",
            UserId = "user111111111111",
            UserName = "username222222",
            UserRole = "userrole333333",

            Status = 200,
            IsSlow = 1,

            CtxData = "cccccccccccccccccccccccccc",
            Addition = "aaaaaaaaaaaaaaaaaaaaaaaaaaa",
            OprDetails = "ddddddddddddddddddddddddddddd",

            Text1 = "text111111111",
            Text2 = "text222222222",
            Text3 = "text333333333"

        };
    }


    internal static readonly string XmlText = CreateTestMessage().ToXml();

}
