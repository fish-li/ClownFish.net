using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Tasks;

#if NETCOREAPP
using ClownFish.Tasks;

[TestClass]
public class AsyncBackgroundTaskTest
{
    [TestMethod]
    public async Task Test1()
    {
        var task = new AsyncBackgroundTask1();
        await task.RunAsync();

        Assert.IsTrue(task.ExitFlag);
        Assert.AreEqual(3, task.Count);
    }

    [TestMethod]
    public async Task Test2()
    {
        var task = new AsyncBackgroundTask2();
        await task.RunAsync();

        Assert.IsTrue(task.ExitFlag);
        Assert.AreEqual(3, task.Count);
    }

    [TestMethod]
    public async Task Test3()
    {
        var task = new AsyncBackgroundTask3();
        await task.RunAsync();

        Assert.IsFalse(task.ExitFlag);
        Assert.AreEqual(0, task.Count);
    }

    [TestMethod]
    public async Task Test4()
    {
        var task = new AsyncBackgroundTask4();
        await task.RunAsync();

        Assert.IsNotNull(task.UnhandledException);
        Assert.IsInstanceOfType(task.UnhandledException, typeof(InvalidCodeException));
    }

    [TestMethod]
    public async Task Test5()
    {
        var task = new AsyncBackgroundTask5();
        await task.RunAsync();

        Assert.IsTrue(task.ExitFlag);
        Assert.AreEqual(1, task.Count);
    }

    [TestMethod]
    public async Task Test6()
    {
        var task = new AsyncBackgroundTask6();
        await task.RunAsync();

        Assert.AreEqual(1, task.Count);
    }

    [TestMethod]
    public async Task Test7()
    {
        var task = new AsyncBackgroundTask7();
        await task.RunAsync();

        Assert.AreEqual(1, task.Count);
    }

}


internal class AsyncBackgroundTask1 : AsyncBackgroundTask
{
    public override int? SleepSeconds => 1;

    private int _count = 0;

    public int Count => _count;

    public override Task ExecuteAsync()
    {
        int count = ++_count;

        if( count >= 3 ) {
            this.ExitTask();
            Console.WriteLine("exit...");
            return Task.CompletedTask;
        }

        if( count < 2 )
            Console.WriteLine(_count.ToString());
        else
            throw new ApplicationException("xxxxxxxxxxxxx");

        return Task.CompletedTask;
    }
}

internal class AsyncBackgroundTask2 : AsyncBackgroundTask1
{
    public override int? SleepSeconds => 0;

    public override string CronValue => "* * * * * ?";

}

internal class AsyncBackgroundTask3 : AsyncBackgroundTask1
{
    public override bool Init()
    {
        return false;
    }
}

internal class AsyncBackgroundTask4 : AsyncBackgroundTask1
{
    public override int? SleepSeconds => 0;
}

internal class AsyncBackgroundTask5 : AsyncBackgroundTask1
{
    public override bool FirstRun => true;

    public override Task ExecuteAsync()
    {
        base.ExecuteAsync();
        this.ExitTask();
        return Task.CompletedTask;
    }
}
internal class AsyncBackgroundTask6 : AsyncBackgroundTask1
{
    private int _sleepSec = 1;

    public override int? SleepSeconds => _sleepSec;

    public override Task ExecuteAsync()
    {
        base.ExecuteAsync();
        _sleepSec = -1;
        return Task.CompletedTask;
    }
}

internal class AsyncBackgroundTask7 : AsyncBackgroundTask2
{
    public override bool FirstRun => true;

    public override Task ExecuteAsync()
    {
        base.ExecuteAsync();
        this.ExitTask();
        return Task.CompletedTask;
    }
}



#endif
