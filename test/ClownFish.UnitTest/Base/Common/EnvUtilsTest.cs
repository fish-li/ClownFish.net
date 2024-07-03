namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class EnvUtilsTest
{
    [TestMethod]
    public void Test_1()
    {
        Assert.IsTrue(EnvUtils.IsDevMode);
        Assert.IsFalse(EnvUtils.IsProdMode);
        Assert.IsFalse(EnvUtils.IsTestMode);        
    }

    [TestMethod]
    public void Test_2()
    {
        Assert.IsTrue(EnvUtils.IsDevMode);
        Assert.IsFalse(AsmHelper.IsSingleFileDeploy);

        Assert.AreEqual("ClownFish.UnitTest", EnvUtils.GetAppName());
        Assert.AreEqual("FishDev", EnvUtils.RunMode);
        Assert.AreEqual("ClownFish_TEST", EnvUtils.ClusterName);
        Assert.AreEqual("ClownFish_TEST", EnvUtils.GetClusterName());

        // 下面2个结果没有写断言
        Console.WriteLine(EnvUtils.GetHostName());
        Console.WriteLine(EnvUtils.GetTempPath());
    }

    [TestMethod]
    public void Test_EvnKind()
    {
        Assert.AreEqual(RunModeEnum.Prod, EnvUtils.GetRunMode(""));
        Assert.AreEqual(RunModeEnum.Prod, EnvUtils.GetRunMode("Prod"));
        Assert.AreEqual(RunModeEnum.Prod, EnvUtils.GetRunMode("Product"));
        Assert.AreEqual(RunModeEnum.Prod, EnvUtils.GetRunMode("production"));
        Assert.AreEqual(RunModeEnum.Prod, EnvUtils.GetRunMode("Product_2"));

        Assert.AreEqual(RunModeEnum.Test, EnvUtils.GetRunMode("Test"));
        Assert.AreEqual(RunModeEnum.Test, EnvUtils.GetRunMode("Test2"));

        Assert.AreEqual(RunModeEnum.Dev, EnvUtils.GetRunMode("dev"));
        Assert.AreEqual(RunModeEnum.Dev, EnvUtils.GetRunMode("xxx"));
        Assert.AreEqual(RunModeEnum.Dev, EnvUtils.GetRunMode("DEV"));
        Assert.AreEqual(RunModeEnum.Dev, EnvUtils.GetRunMode("Development"));
        Assert.AreEqual(RunModeEnum.Dev, EnvUtils.GetRunMode("FishDev"));
        Assert.AreEqual(RunModeEnum.Dev, EnvUtils.GetRunMode("xxxxxxxx"));
    }


    [TestMethod]
    public void Test_CheckApplicationName()
    {
        EnvUtils.CheckApplicationName("aa11_bb");
        EnvUtils.CheckApplicationName("aa11.bb");
        EnvUtils.CheckApplicationName("aa11-bb");

        MyAssert.IsError<ArgumentNullException>(() => {
            EnvUtils.CheckApplicationName("");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11/bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11 bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11+bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11~bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("中文汉字");
        });
    }
}
