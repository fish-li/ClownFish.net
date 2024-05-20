using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Tasks;

#if NETCOREAPP

namespace ClownFish.UnitTest.Tasks;
[TestClass]
public class BaseBackgroundTaskTest
{
    [TestMethod]
    public void Test_1()
    {
        var test1 = new Test1BackgroundTask();
        Assert.IsFalse(test1.Init0());
    }

    [TestMethod]
    public void Test_2()
    {
        var test1 = new Test2BackgroundTask();
        Assert.IsFalse(test1.Init0());
    }

    [TestMethod]
    public void Test_3()
    {
        var test1 = new Test3BackgroundTask();
        test1.OnError0(null);

        // 不出现异常，就是测试通过
    }
}

internal class Test1BackgroundTask : BaseBackgroundTask
{
     public override string CronValue => "xxxxxxxx";
}

internal class Test2BackgroundTask : BaseBackgroundTask
{
    public override int? SleepSeconds => 3;

    public override bool Init()
    {
        throw new NotImplementedException();
    }
}

internal class Test3BackgroundTask : BaseBackgroundTask
{
    public override int? SleepSeconds => 3;

    public override void OnError(Exception ex)
    {
        throw new NotImplementedException();
    }
}

#endif
