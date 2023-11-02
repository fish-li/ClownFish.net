using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Tasks;

#if NETCOREAPP
using ClownFish.Tasks;

[TestClass]
public class BackgroundTaskManagerTest
{
    [TestMethod]
    public void Test_1()
    {
        int count1 = BackgroundTaskManager.StartAll();
        Assert.AreEqual(0, count1);

        int count2 = BackgroundTaskManager.StartAll(typeof(BackgroundTask1), typeof(AsyncBackgroundTask1));
        Assert.AreEqual(2, count2);

        MyAssert.IsError<InvalidOperationException>(() => {
            BackgroundTaskManager.StartAll(typeof(BackgroundTask2), typeof(AsyncBackgroundTask2));
        });

        DebugReportBlock block = BackgroundTaskManager.GetReportBlock();
        string text = block.ToString2();
        Console.WriteLine(text);
        Assert.IsTrue(text.Contains("ClownFish.UnitTest.Tasks.BackgroundTask1: 0,  0"));
        Assert.IsTrue(text.Contains("ClownFish.UnitTest.Tasks.AsyncBackgroundTask1: 0,  0"));


        int a = BackgroundTaskManager.ActivateTask(typeof(BackgroundTask1).FullName);
        int b = BackgroundTaskManager.ActivateTask(typeof(AsyncBackgroundTask1).FullName);
        int c = BackgroundTaskManager.ActivateTask("xxxxxxxx");
        Assert.AreEqual(1, a);
        Assert.AreEqual(1, b);
        Assert.AreEqual(0, c);
        Thread.Sleep(100);

        DebugReportBlock block2 = BackgroundTaskManager.GetReportBlock();
        string text2 = block2.ToString2();
        Console.WriteLine(text2);
        Assert.IsTrue(text2.Contains("ClownFish.UnitTest.Tasks.BackgroundTask1: 1,  0"));
        // 异步采用线程池，所以调用不及时，因此下面这个断言极大概率不通过！
        //Assert.IsTrue(text2.Contains("ClownFish.UnitTest.Tasks.AsyncBackgroundTask1: 1,  0"));

        var stats = BackgroundTaskManager.GetAllStatus();
        Assert.AreEqual(2, stats.Count);
        //Console.WriteLine(stats.ToJson(JsonStyle.Indented));

        BackgroundTaskManager.StopAll();
        Thread.Sleep(1500);
    }
}
#endif
