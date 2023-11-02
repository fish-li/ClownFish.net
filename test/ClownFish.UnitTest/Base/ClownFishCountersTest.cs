namespace ClownFish.UnitTest.Base;

[TestClass]
public class ClownFishCountersTest
{
    [TestMethod]
    public void Test1()
    {
        ClownFishCounters.ResetAll();
        Assert.AreEqual(0L, ClownFishCounters.Console2.Warnning.Get());
        Assert.AreEqual(0L, ClownFishCounters.Logging.WriteCount.Get());


        ClownFishCounters.Console2.Warnning.Increment();
        Assert.AreEqual(1L, ClownFishCounters.Console2.Warnning.Get());

        ClownFishCounters.Logging.WriteCount.Increment();
        Assert.AreEqual(1L, ClownFishCounters.Logging.WriteCount.Get());


        ClownFishCounters.ResetAll();
        Assert.AreEqual(0L, ClownFishCounters.Console2.Warnning.Get());
        Assert.AreEqual(0L, ClownFishCounters.Logging.WriteCount.Get());
    }

    [TestMethod]
    public void Test2()
    {
        List<NameInt64> list = ClownFishCounters.GetAllValues();
        Assert.IsTrue(list.Count > 0);
    }
}
