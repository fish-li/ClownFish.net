using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Tasks;

#if NETCOREAPP
using ClownFish.Tasks;

[TestClass]
public class BackgroundTaskTest
{
    [TestMethod]
    public void Test1()
    {
        var task = new BackgroundTask1();
        task.Run();

        Assert.IsTrue(task.ExitFlag);
        Assert.AreEqual(3, task.Count);
    }

    [TestMethod]
    public void Test2()
    {
        var task = new BackgroundTask2();
        task.Run();

        Assert.IsTrue(task.ExitFlag);
        Assert.AreEqual(3, task.Count);
    }

    [TestMethod]
    public void Test3()
    {
        var task = new BackgroundTask3();
        task.Run();

        Assert.IsFalse(task.ExitFlag);
        Assert.AreEqual(0, task.Count);
    }

    [TestMethod]
    public void Test4()
    {
        var task = new BackgroundTask4();
        task.Run();

        Assert.IsNotNull(task.UnhandledException);
        Assert.IsInstanceOfType(task.UnhandledException, typeof(InvalidCodeException));
    }

    [TestMethod]
    public void Test5()
    {
        var task = new BackgroundTask5();
        task.Run();

        Assert.IsTrue(task.ExitFlag);
        Assert.AreEqual(1, task.Count);
    }

    [TestMethod]
    public void Test6()
    {
        var task = new BackgroundTask6();
        task.Run();

        Assert.AreEqual(1, task.Count);
    }

    [TestMethod]
    public void Test7()
    {
        var task = new BackgroundTask7();
        task.Run();

        Assert.AreEqual(1, task.Count);
    }


}



internal class BackgroundTask1 : BackgroundTask
{
    public override int? SleepSeconds => 1;

    private int _count = 0;

    public int Count => _count;

    public override void Execute()
    {
        int count = ++_count;

        if( count >= 3) {
            this.ExitTask();
            Console.WriteLine("exit...");
            return;
        }

        if( count < 2 )
            Console.WriteLine(_count.ToString());
        else
            throw new ApplicationException("xxxxxxxxxxxxx");
    }
}

internal class BackgroundTask2 : BackgroundTask1
{
    public override int? SleepSeconds => 0;

    public override string CronValue => "* * * * * ?";

}

internal class BackgroundTask3 : BackgroundTask1
{
    public override bool Init()
    {
        return false;
    }
}

internal class BackgroundTask4 : BackgroundTask1
{
    public override int? SleepSeconds => 0;
}

internal class BackgroundTask5 : BackgroundTask1
{
    public override bool FirstRun => true;

    public override void Execute()
    {
        base.Execute();
        this.ExitTask();
    }
}
internal class BackgroundTask6 : BackgroundTask1
{
    private int _sleepSec = 1;

    public override int? SleepSeconds => _sleepSec;

    public override void Execute()
    {
        base.Execute();
        _sleepSec = -1;
    }
}

internal class BackgroundTask7 : BackgroundTask2
{
    public override bool FirstRun => true;

    public override void Execute()
    {
        base.Execute();
        this.ExitTask();
    }
}


#endif
