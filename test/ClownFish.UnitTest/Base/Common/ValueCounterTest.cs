namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class ValueCounterTest
{
    [TestMethod]
    public void Test()
    {
        ValueCounter counter = new ValueCounter("abc");
        Assert.AreEqual("abc", counter.Label);
        Assert.AreEqual(0L, counter.Get());

        long count1 = counter.Increment();
        Assert.AreEqual(1L, count1);

        long count2 = counter.Increment();
        Assert.AreEqual(2L, count2);

        Assert.AreEqual(2L, counter.Get());

        Assert.AreEqual("2", counter.AsString());
        Assert.AreEqual("abc=2", counter.ToString());

        counter.Add(3);
        Assert.AreEqual(5L, counter.Get());

        Assert.AreEqual("5", counter.AsString());
        Assert.AreEqual("abc=5", counter.ToString());

        counter.Set(7L);
        Assert.AreEqual(7L, counter.Get());

        long value = counter;
        Assert.AreEqual(7L, value);

        counter.Reset();
        Assert.AreEqual(0L, counter.Get());


        DateTime now = DateTime.Now;
        counter.Set(now);
        DateTime time2 = counter.GetAsDateTime();
        Assert.AreEqual(now, time2);


        counter.Set(2L);
        long count3 = counter.Decrement();
        Assert.AreEqual(1L, count3);

        long count4 = counter.Decrement();
        Assert.AreEqual(0L, count4);

        Assert.AreEqual(0L, counter.Get());
    }
}
