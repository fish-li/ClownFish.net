namespace ClownFish.UnitTest.Http.Utils;

[TestClass]
public class SysNetInitializerTest
{
    [TestMethod]
    public void Test()
    {
        HttpOption httpOption = new HttpOption {
            Url = "https://www.baidu.com/"
        };

        httpOption.GetResult<ClownFish.Base.Void>();

        // 不报错就认可通过
    }



    [TestMethod]
    public void Test_Callback()
    {
        // 下面这个回调在 .net core 中根本不会进入，所以就直接反射调用

        object value = typeof(SysNetInitializer).InvokeMember("RemoteCertificateValidationCallback",
            BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic,
            null, null, new object[] {null, null, null, null });

        Assert.IsTrue((bool)value);
    }
}
