namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class MyTimeZoneTest
{
    [TestMethod]
    public void Test()
    {
        Assert.AreEqual("Asia/Shanghai", MyTimeZone.CurrentTZ);

        TimeZoneInfo z1 = MyTimeZone.Get("Asia/Shanghai");
        TimeZoneInfo z2 = MyTimeZone.Get("China Standard Time");

        Assert.AreEqual(z1.Id, z2.Id);
        Assert.AreEqual(z1.StandardName, z2.StandardName);
        Assert.AreEqual(z1.BaseUtcOffset.Ticks, z2.BaseUtcOffset.Ticks);
    }

#if NETCOREAPP
    [TestMethod]
    public void Test2()
    {
        TimeZoneInfo z1 = MyTimeZone.GetTzForWin("Asia/Shanghai");
        TimeZoneInfo z2 = MyTimeZone.GetTzForWin("China Standard Time");
        TimeZoneInfo z3 = MyTimeZone.GetTzForLinux("Asia/Shanghai");
        TimeZoneInfo z4 = MyTimeZone.GetTzForLinux("China Standard Time");


        Assert.AreEqual(z1.Id, z2.Id);
        Assert.AreEqual(z3.Id, z4.Id);

        Assert.AreEqual(z1.StandardName, z2.StandardName);
        Assert.AreEqual(z1.BaseUtcOffset.Ticks, z2.BaseUtcOffset.Ticks);

        Assert.AreEqual(z1.StandardName, z3.StandardName);
        Assert.AreEqual(z1.BaseUtcOffset.Ticks, z3.BaseUtcOffset.Ticks);
        
        Assert.AreEqual(z1.StandardName, z4.StandardName);
        Assert.AreEqual(z1.BaseUtcOffset.Ticks, z4.BaseUtcOffset.Ticks);
    }
#endif

}
