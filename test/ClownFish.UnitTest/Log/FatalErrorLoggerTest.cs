namespace ClownFish.UnitTest.Log;

[TestClass]
public class FatalErrorLoggerTest
{
    private static readonly ValueCounter s_errorCount = new ValueCounter("ErrorCount");

    private static void OnErrorEvent(object sender, ExceptionEventArgs e)
    {
        s_errorCount.Increment();
    }

    [TestMethod]
    public void Test_RaiseErrorEvent()
    {
        long fatalCount1 = ClownFishCounters.Logging.FatalErrorCount.Get();
        long errorCount1 = s_errorCount.Get();

        FatalErrorLogger.LogException(ExceptionHelper.CreateException("9b35fa1b7c4a4413a7e37e724e4652b1"));

        long fatalCount2 = ClownFishCounters.Logging.FatalErrorCount.Get();
        long errorCount2 = s_errorCount.Get();

        Assert.AreEqual(1, fatalCount2 - fatalCount1);
        Assert.AreEqual(0, errorCount1);
        Assert.AreEqual(errorCount2, errorCount1);

        FatalErrorLogger.OnExceptionRecived += OnErrorEvent;
        FatalErrorLogger.LogException(ExceptionHelper.CreateException("71e6bd09a78c4f34ba7ade0bca3b867c"));
        FatalErrorLogger.OnExceptionRecived -= OnErrorEvent;

        long fatalCount3 = ClownFishCounters.Logging.FatalErrorCount.Get();
        long errorCount3 = s_errorCount.Get();

        Assert.AreEqual(1, errorCount3 - errorCount2);
        Assert.AreEqual(1, fatalCount3 - fatalCount2);
    }

}
