namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class EnvUtilsTest
{
    [TestMethod]
    public void Test_1()
    {
        Assert.IsTrue(EnvUtils.IsDevEnv);
        Assert.IsFalse(EnvUtils.IsProdEnv);
        Assert.IsFalse(EnvUtils.IsTestEnv);        
    }

    [TestMethod]
    public void Test_2()
    {
        Assert.IsTrue(EnvUtils.IsDevEnv);
        Assert.IsFalse(AsmHelper.IsSingleFileDeploy);

        Assert.AreEqual("ClownFish.UnitTest", EnvUtils.GetAppName());
        Assert.AreEqual("FishDev", EnvUtils.RunEnv);
        Assert.AreEqual("ClownFish_TEST", EnvUtils.ClusterName);
        Assert.AreEqual("ClownFish_TEST", EnvUtils.GetClusterName());

        // 下面2个结果没有写断言
        Console.WriteLine(EnvUtils.GetHostName());
        Console.WriteLine(EnvUtils.GetTempPath());
    }

    [TestMethod]
    public void Test_EvnKind()
    {
        Assert.AreEqual(RunEnvEnum.Prod, EnvUtils.GetRunEnvEnum(""));
        Assert.AreEqual(RunEnvEnum.Prod, EnvUtils.GetRunEnvEnum("Prod"));
        Assert.AreEqual(RunEnvEnum.Prod, EnvUtils.GetRunEnvEnum("Product"));
        Assert.AreEqual(RunEnvEnum.Prod, EnvUtils.GetRunEnvEnum("production"));
        Assert.AreEqual(RunEnvEnum.Prod, EnvUtils.GetRunEnvEnum("Product_2"));

        Assert.AreEqual(RunEnvEnum.Test, EnvUtils.GetRunEnvEnum("Test"));
        Assert.AreEqual(RunEnvEnum.Test, EnvUtils.GetRunEnvEnum("Test2"));

        Assert.AreEqual(RunEnvEnum.Dev, EnvUtils.GetRunEnvEnum("dev"));
        Assert.AreEqual(RunEnvEnum.Dev, EnvUtils.GetRunEnvEnum("xxx"));
        Assert.AreEqual(RunEnvEnum.Dev, EnvUtils.GetRunEnvEnum("DEV"));
        Assert.AreEqual(RunEnvEnum.Dev, EnvUtils.GetRunEnvEnum("Development"));
        Assert.AreEqual(RunEnvEnum.Dev, EnvUtils.GetRunEnvEnum("FishDev"));
        Assert.AreEqual(RunEnvEnum.Dev, EnvUtils.GetRunEnvEnum("xxxxxxxx"));
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

    [TestMethod]
    public void Test_IsInK8s()
    {
        // 肯定不可能在 K8S 环境中跑单元测试
        // 如果不调用这些方法，代码覆盖率就是 0，所以没办法~~~
        Assert.IsFalse(EnvUtils.IsInK8s);
        Assert.IsNull(EnvUtils.K8sNamespace);
    }
}
