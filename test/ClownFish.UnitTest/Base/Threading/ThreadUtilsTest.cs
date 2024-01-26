using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Threading;
[TestClass]
public class ThreadUtilsTest
{
    [TestMethod]
    public void Test_RunTask()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ThreadUtils.RunTask("", Action1);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ThreadUtils.RunTask("operatorName", (Action)null);
        });

        Task task = ThreadUtils.RunTask(nameof(Test_RunTask), Action1);
        Assert.IsNotNull(task);

        task.GetAwaiter().GetResult();
    }

    private static void Action1()
    {
        Thread.Sleep(100);

        if( DateTime.Now.Year < 1000 )
            throw new ApplicationException("xxxxxxxxxxxxx");
    }

    private static Task Action2()
    {
        Thread.Sleep(100);

        if( DateTime.Now.Year < 1000 )
            throw new ApplicationException("xxxxxxxxxxxxx");

        return Task.Delay(100);
    }

    private static void Action3(object xx)
    {
        Thread.Sleep(100);

        if( DateTime.Now.Year < 1000 )
            throw new ApplicationException("xxxxxxxxxxxxx");
    }

    [TestMethod]
    public void Test_RunAsync()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ThreadUtils.RunAsync("", Action2);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ThreadUtils.RunAsync("operatorName", (Func<Task>)null);
        });

        Task task = ThreadUtils.RunAsync(nameof(Test_RunAsync), Action2);
        Assert.IsNotNull(task);

        task.GetAwaiter().GetResult();
    }


    [TestMethod]
    public void Test_Run1()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run("", Action1);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run("operatorName", (Action)null);
        });

        ThreadUtils.Run(nameof(Test_Run1), Action1);
    }

    [TestMethod]
    public void Test_Run1b()
    {
        object args = null;

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run("", Action3, args);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run("operatorName", (Action<object>)null, args);
        });

        ThreadUtils.Run(nameof(Test_Run1b), Action3, args);
    }

    [TestMethod]
    public void Test_Run2()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run2("", "threadName", Action1);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run2("operatorName", "", Action1);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run2("operatorName", "threadName", (Action)null);
        });

        ThreadUtils.Run2("operatorName", "threadName", Action1);
    }

    [TestMethod]
    public void Test_Run2b()
    {
        object args = null;

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run2("", "threadName", Action3, args);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run2("operatorName", "", Action3, args);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ThreadUtils.Run2("operatorName", "threadName", (Action<object>)null, args);
        });

        ThreadUtils.Run2("operatorName", "threadName", Action3, args);
    }

    [TestMethod]
    public void Test_LogError()
    {
        Exception ex = ExceptionHelper.CreateException();

        ThreadUtils.LogError("Test_LogError", ex);
    }
}
