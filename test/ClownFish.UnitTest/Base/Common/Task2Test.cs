using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class Task2Test
{
    [TestMethod]
    public async Task Test1()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        await Task.Delay(100, tokenSource.Token);

        Task task = Task.Delay(30_000, tokenSource.Token);

        tokenSource.Cancel();

        MyAssert.IsError<TaskCanceledException>(() => {
            task.GetAwaiter().GetResult();
        });
        
        Assert.IsTrue(task.IsCompleted);
        Assert.IsTrue(task.IsCanceled);
    }


    [TestMethod]
    public async Task Test2()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        await Task2.Delay(100, tokenSource.Token);

        Task task = Task2.Delay(30_000, tokenSource.Token);

        tokenSource.Cancel();

        task.GetAwaiter().GetResult();

        Assert.IsTrue(task.IsCompleted);
        Assert.IsFalse(task.IsCanceled);
    }

    [TestMethod]
    public void Test_CompletedTask()
    {
        Task task = Task2.CompletedTask;
        Assert.IsTrue(task.IsCompleted);
#if NETCOREAPP
        Assert.IsTrue(task.IsCompletedSuccessfully);
#endif
        Assert.IsFalse(task.IsCanceled);
        Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
    }
}
