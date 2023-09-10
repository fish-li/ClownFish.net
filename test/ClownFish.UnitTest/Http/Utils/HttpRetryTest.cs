namespace ClownFish.UnitTest.Http.Utils;

[TestClass]
public class HttpRetryTest
{
    [TestMethod]
    public void TestCreateRetry()
    {
        HttpOption httpOption = new HttpOption {
            // 一个错误的URL（端口），它会触发重试策略
            Url = "http://localhost:18205/xxxxx/debug/heartbeat.aspx",
            Timeout = 100
        };

        // 下面这种写法也是可以的，只是当站点不可用时，要等待很长时间
        //Retry retry = HttpRetry.Create();

        Retry retry = Retry.Create(Retry.Default.Count, 10);

        try {
            string text = httpOption.GetResult(retry);
            Console.WriteLine(text);
        }
        catch(Exception ex) {
            Console.WriteLine(ex.ToString());
        }

        // 实际执行次数（所有重试全部失败） = 第一次执行（1） + 重试次数
        Assert.AreEqual(Retry.Default.Count + 1, retry.ExecuteCount);
    }
}
