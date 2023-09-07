using System.Net.Sockets;

namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class HttpRetryTest
{
    [TestMethod]
    public void Test()
    {
        Retry retry = HttpRetry.Create();
        Assert.AreEqual(Retry.Default.Count, retry.Count);
        Assert.AreEqual(Retry.Default.WaitMillisecond, retry.Milliseconds);

    }

    [TestMethod]
    public void Test_NeedRetry()
    {
        RemoteWebException ex1 = RemoteWebExceptionTest.CreateRemoteWebException(null);
        Assert.IsFalse(HttpRetry.NeedRetry(ex1));

        RemoteWebException ex2 = RemoteWebExceptionTest.CreateRemoteWebException(null, 502);
        Assert.IsTrue(HttpRetry.NeedRetry(ex2));

        RemoteWebException ex3 = RemoteWebExceptionTest.CreateRemoteWebException(null, 503);
        Assert.IsTrue(HttpRetry.NeedRetry(ex3));

        SocketException socketException = new SocketException(600);
        RemoteWebException ex4 = RemoteWebExceptionTest.CreateRemoteWebException(socketException);
        Assert.IsTrue(HttpRetry.NeedRetry(ex4));

        ApplicationException ex5 = new ApplicationException("xxx", socketException);
        RemoteWebException ex6 = RemoteWebExceptionTest.CreateRemoteWebException(ex5);
        Assert.IsTrue(HttpRetry.NeedRetry(ex6));
    }
}
