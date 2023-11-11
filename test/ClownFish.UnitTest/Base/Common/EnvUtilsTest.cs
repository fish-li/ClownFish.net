namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class EnvUtilsTest
{
    [TestMethod]
    public void Test_1()
    {
        Assert.AreEqual(EvnKind.Dev, EnvUtils.CurEvnKind);
        Assert.IsTrue(EnvUtils.IsDevEnv);
        Assert.IsFalse(EnvUtils.IsProdEnv);
        Assert.IsFalse(EnvUtils.IsTestEnv);
    }

    [TestMethod]
    public void Test_2()
    {
        Assert.IsTrue(EnvUtils.IsDevEnv);

        Assert.AreEqual("ClownFish.UnitTest", EnvUtils.GetAppName());
        Assert.AreEqual("FishDev", EnvUtils.EnvName);
        Assert.AreEqual("ClownFish.TEST", EnvUtils.ClusterName);
        Assert.AreEqual("ClownFish.TEST", EnvUtils.GetClusterName());

        // 下面2个结果没有写断言
        Console.WriteLine(EnvUtils.GetHostName());
        Console.WriteLine(EnvUtils.GetTempPath());
    }

    [TestMethod]
    public void Test_EvnKind()
    {
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind(""));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("Prod"));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("Product"));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("production"));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("Product_2"));

        Assert.AreEqual(EvnKind.Test, EnvUtils.GetEvnKind("Test"));
        Assert.AreEqual(EvnKind.Test, EnvUtils.GetEvnKind("Test2"));

        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("dev"));
        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("xxx"));
        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("DEV"));
        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("Development"));
        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("FishDev"));
        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("xxxxxxxx"));
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
