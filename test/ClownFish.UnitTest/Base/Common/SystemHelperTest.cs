namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class SystemHelperTest
{
    [TestMethod]
    public void Test()
    {
        // 这几个方法能调用就行了，不用检查结果
        Console.WriteLine(SystemHelper.GetComputerName());
        Assert.AreEqual("Windows", SystemHelper.GetOsName());

        var x = SystemHelper.GetCurrentNetworkInfo();
        Console.WriteLine(x.GetMac());
        Console.WriteLine(x.GetIPv4());
        

    }
}
