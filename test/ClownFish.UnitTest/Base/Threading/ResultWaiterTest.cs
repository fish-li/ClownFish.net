using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Threading;

namespace ClownFish.UnitTest.Base.Threading;
[TestClass]
public class ResultWaiterTest
{
    [TestMethod]
    public async Task Test_1()
    {
        using ResultWaiter waiter = new ResultWaiter();

        ThreadPool.QueueUserWorkItem(M1, waiter.ResultId);

        NameInt64 result = (NameInt64)await waiter.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.IsNotNull(result);
        Assert.AreEqual("key1", result.Name);
    }

    private void M1(object state)
    {
        string id = (string)state;
        Thread.Sleep(100);
        ResultWaiter waiter = ResultWaiter.GetById(id);
        waiter.SetResult(new NameInt64("key1", 123));
    }

    [TestMethod]
    public async Task Test_2()
    {
        using ResultWaiter waiter = new ResultWaiter();

        NameInt64 result = (NameInt64)await waiter.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.IsNull(result);
    }


    [TestMethod]
    public async Task Test_3()
    {
        using ResultWaiter waiter = new ResultWaiter();

        ThreadPool.QueueUserWorkItem(M3, waiter.ResultId);

        await MyAssert.IsErrorAsync<MessageException>(async () => {
            NameInt64 result = (NameInt64)await waiter.WaitAsync(TimeSpan.FromSeconds(2));
        });
    }

    private void M3(object state)
    {
        string id = (string)state;
        Thread.Sleep(100);

        Exception ex = ExceptionHelper.CreateException();

        ResultWaiter waiter = ResultWaiter.GetById(id);
        waiter.SetException(ex);
    }


    [TestMethod]
    public async Task Test_4()
    {
        using ResultWaiter waiter = new ResultWaiter();

        ThreadPool.QueueUserWorkItem(M4, waiter.ResultId);

        NameInt64 result = (NameInt64)await waiter.WaitAsync(TimeSpan.FromMilliseconds(20));
        Assert.IsNull(result);
    }

    private void M4(object state)
    {
        string id = (string)state;
        Thread.Sleep(1000);
        ResultWaiter waiter = ResultWaiter.GetById(id);
        waiter.SetResult(new NameInt64("key1", 123));
    }

    [TestMethod]
    public async Task Test_5()
    {
        using ResultWaiter waiter = new ResultWaiter();

        ThreadPool.QueueUserWorkItem(M5, waiter.ResultId);

        NameInt64 result = (NameInt64)await waiter.WaitAsync(TimeSpan.FromMilliseconds(20));
        Assert.IsNull(result);
    }

    private void M5(object state)
    {
        string id = (string)state;
        Thread.Sleep(1000);

        Exception ex = ExceptionHelper.CreateException();

        ResultWaiter waiter = ResultWaiter.GetById(id);
        waiter.SetException(ex);
    }


}
