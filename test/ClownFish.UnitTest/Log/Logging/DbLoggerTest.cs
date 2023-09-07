namespace ClownFish.UnitTest.Log.Logging;
[TestClass]
public class DbLoggerTest
{
    [TestMethod]
    public void Test_GetTransFlag()
    {
        Assert.AreEqual("", DbLogger.GetTransFlag(null));
        Assert.AreEqual("_NOLOCK", DbLogger.GetTransFlag(IsolationLevel.ReadUncommitted));
        Assert.AreEqual("_TRANS", DbLogger.GetTransFlag(IsolationLevel.ReadCommitted));
        Assert.AreEqual("_TRANS", DbLogger.GetTransFlag(IsolationLevel.RepeatableRead));
        Assert.AreEqual("_TRANS", DbLogger.GetTransFlag(IsolationLevel.Serializable));
        Assert.AreEqual("_TRANS", DbLogger.GetTransFlag(IsolationLevel.Snapshot));
        Assert.AreEqual("_TRANS", DbLogger.GetTransFlag(IsolationLevel.Unspecified));
        Assert.AreEqual("_TRANS", DbLogger.GetTransFlag(IsolationLevel.Chaos));
    }
}
