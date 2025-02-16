using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Threading;
[TestClass]
public class ThreadUtilsTest
{
    private static readonly TSafeDictionary<string, Exception> s_errors = new TSafeDictionary<string, Exception>();

    static ThreadUtilsTest()
    {
        ThreadUtils.ExceptionHandler = HandleThreadUtilsException;
    }

    private static void HandleThreadUtilsException(string operatorName, Exception ex)
    {
        s_errors[operatorName] = ex;
    }

    private static Exception GetLastException(string operatorName)
    {
        if( s_errors.TryRemove(operatorName, out Exception ex) ) {
            return ex;
        }
        return null;
    }

    private static void Action1()
    {
        Thread.Sleep(100);

        if( DateTime.Now.Year > 1 )
            throw new ApplicationException("Action1_ERROR_4ac06f7062104255bba3749b102c8d5b");
    }

    private static async Task Action2()
    {
        await Task.Delay(100);

        if( DateTime.Now.Year > 1 )
            throw new ApplicationException("Action2_ERROR_005f036942124c11a4a8dad1c4d989e0");
    }

    private static void Action3(object xx)
    {
        Thread.Sleep(100);

        if( DateTime.Now.Year > 1 )
            throw new ApplicationException("Action3_ERROR_afc714bcc5a24590b90cc63b082dbe89");
    }


    [TestMethod]
    public void Test_RunTask()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ThreadUtils.RunTask("", Action1);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ThreadUtils.RunTask("operatorName", (Action)null);
        });

        Task task = ThreadUtils.RunTask("ThreadUtilsTest_Test_RunTask", Action1);
        Assert.IsNotNull(task);

        Thread.Sleep(1000);

        // 检测 ThreadUtils 的异常处理是否发挥作用
        Exception ex = GetLastException("ThreadUtilsTest_Test_RunTask");
        Assert.IsNotNull(ex);
        Assert.AreEqual("Action1_ERROR_4ac06f7062104255bba3749b102c8d5b", ex.Message);
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

        Assert.IsNull(GetLastException("ThreadUtilsTest_Test_RunAsync"));

        // 下面调用 RunAsync 方法时，不使用 await ，因为实际使用时，通常的场景就是开启一个【长时间运行】的异步过程，所以肯定是不需要等待结束的
        _ = ThreadUtils.RunAsync("ThreadUtilsTest_Test_RunAsync", Action2);

        Thread.Sleep(1000);

        // 检测 ThreadUtils 的异常处理是否发挥作用
        Exception ex = GetLastException("ThreadUtilsTest_Test_RunAsync");
        Assert.IsNotNull(ex);
        Assert.AreEqual("Action2_ERROR_005f036942124c11a4a8dad1c4d989e0", ex.Message);
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

        ThreadUtils.Run("ThreadUtilsTest_Test_Run1", Action1);

        Thread.Sleep(1000);

        // 检测 ThreadUtils 的异常处理是否发挥作用
        Exception ex = GetLastException("ThreadUtilsTest_Test_Run1");
        Assert.IsNotNull(ex);
        Assert.AreEqual("Action1_ERROR_4ac06f7062104255bba3749b102c8d5b", ex.Message);
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

        ThreadUtils.Run("ThreadUtilsTest_Test_Run1b", Action3, args);

        Thread.Sleep(1000);

        // 检测 ThreadUtils 的异常处理是否发挥作用
        Exception ex = GetLastException("ThreadUtilsTest_Test_Run1b");
        Assert.IsNotNull(ex);
        Assert.AreEqual("Action3_ERROR_afc714bcc5a24590b90cc63b082dbe89", ex.Message);
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

        ThreadUtils.Run2("ThreadUtilsTest_Test_Run2", "Test_Run2", Action1);

        Thread.Sleep(1000);

        // 检测 ThreadUtils 的异常处理是否发挥作用
        Exception ex = GetLastException("ThreadUtilsTest_Test_Run2");
        Assert.IsNotNull(ex);
        Assert.AreEqual("Action1_ERROR_4ac06f7062104255bba3749b102c8d5b", ex.Message);
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

        ThreadUtils.Run2("ThreadUtilsTest_Test_Run2b", "Test_Run2b", Action3, args);

        Thread.Sleep(1000);

        // 检测 ThreadUtils 的异常处理是否发挥作用
        Exception ex = GetLastException("ThreadUtilsTest_Test_Run2b");
        Assert.IsNotNull(ex);
        Assert.AreEqual("Action3_ERROR_afc714bcc5a24590b90cc63b082dbe89", ex.Message);
    }

    [TestMethod]
    public void Test_LogException()
    {
        Exception ex = ExceptionHelper.CreateException();

        ThreadUtils.LogException("Test_LogException", ex);
    }

    [TestMethod]
    public void Test_LogException2()
    {
        Exception ex = ExceptionHelper.CreateException();
        Exception ex2 = ExceptionHelper.CreateException("this is ex2");

        ThreadUtils.LogException2("Test_LogException2", ex, ex2);
    }
}
