namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class RetryTest
{
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_Retry_指定错误重试次数()
    {
        TestRetryTask1 task = new TestRetryTask1();

        string text = Retry.Create(-5, 10).Run(() => {
            return task.Exec1();
        });
    }


    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task Test_Retry_指定错误重试次数_Async()
    {
        AsyncTestRetryTask2 task2 = new AsyncTestRetryTask2();

        string text = await Retry.Create(-5, 10).RunAsync(async () => {
            return await task2.Exec1Async();
        });
    }


    [TestMethod]
    public void Test_Retry_不指定过滤器()
    {
        TestRetryTask1 task = new TestRetryTask1();

        string text = Retry.Create(5, 10).Run(() => {
            return task.Exec1();
        });

        Assert.AreEqual(TestRetryTask1.Result, text);
    }

    [TestMethod]
    public async Task Test_Retry_不指定过滤器_Async()
    {
        AsyncTestRetryTask2 task2 = new AsyncTestRetryTask2();

        string text2 = await Retry.Create(5, 10).RunAsync(async () => {
            return await task2.Exec1Async();
        });

        Assert.AreEqual(AsyncTestRetryTask2.Result, text2);
    }


    [TestMethod]
    public void Test_Retry_指定全部过滤器()
    {
        TestRetryTask1 task = new TestRetryTask1();

        string text = Retry.Create(5, 10)
            .Filter<NotSupportedException>()
            .Filter<ArgumentOutOfRangeException>(ex => ex.ParamName == "name")
            .Filter<ArgumentNullException>(ex => ex.ParamName == "key")
            .Run(() => {                    
                return task.Exec2();
            });

        Assert.AreEqual(TestRetryTask1.Result, text);
    }

    [TestMethod]
    public async Task Test_Retry_指定全部过滤器_Async()
    {
        AsyncTestRetryTask2 task2 = new AsyncTestRetryTask2();

        string text2 = await Retry.Create(5, 10)
            .Filter<NotSupportedException>()
            .Filter<ArgumentOutOfRangeException>(ex => ex.ParamName == "name")
            .Filter<ArgumentNullException>(ex => ex.ParamName == "key")
            .RunAsync(async () => {
                return await task2.Exec2Async();
            });

        Assert.AreEqual(AsyncTestRetryTask2.Result, text2);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Retry_指定部分过滤器_将会出现异常()
    {
        TestRetryTask1 task = new TestRetryTask1();

        string text = Retry.Create(5, 10)
                .Filter<NotSupportedException>()
                .Filter<ArgumentOutOfRangeException>(ex => ex.ParamName == "name")
                  .Run(() => {                          
                      return task.Exec2();
                  });

    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task Test_Retry_指定部分过滤器_将会出现异常_Async()
    {
        AsyncTestRetryTask2 task2 = new AsyncTestRetryTask2();

        string text = await Retry.Create(5, 10)
                .Filter<NotSupportedException>()
                .Filter<ArgumentOutOfRangeException>(ex => ex.ParamName == "name")
                  .RunAsync(async () => {
                      return await task2.Exec2Async();
                  });

    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Test_Retry_指定部分过滤器_将会出现异常2()
    {
        TestRetryTask1 task = new TestRetryTask1();

        string text = Retry.Create(5, 10)
                .Filter<NotSupportedException>()
                  .Run(() => {
                      return task.Exec2();
                  });

    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public async Task Test_Retry_指定部分过滤器_将会出现异常2_Async()
    {
        AsyncTestRetryTask2 task2 = new AsyncTestRetryTask2();

        string text = await Retry.Create(5, 10)
                .Filter<NotSupportedException>()
                  .RunAsync(async () => {
                      return await task2.Exec2Async();
                  });

    }



    [TestMethod]
    public void Test_Retry_指定过滤器_指定回调方法()
    {
        int count = 0;
        int count2 = 0;
        TestRetryTask1 task = new TestRetryTask1();

        string text = Retry.Create(5, 10)
                .Filter<NotSupportedException>()
                .OnException((ex, n)=> { count++; })
                .OnException((ex, n) => { count2+=n; })
                  .Run(() => {
                      return task.Exec3();
                  });

        Assert.AreEqual(TestRetryTask1.Result, text);
        Assert.AreEqual(3, count);
        Assert.AreEqual(6, count2);
    }


    [TestMethod]
    public async Task Test_Retry_指定过滤器_指定回调方法_Async()
    {
        int count = 0;
        int count2 = 0;
        AsyncTestRetryTask2 task2 = new AsyncTestRetryTask2();

        string text = await Retry.Create(5, 10)
                .Filter<NotSupportedException>()
                .OnException((ex, n) => { count++; })
                .OnException((ex, n) => { count2 += n; })
                  .RunAsync(async () => {
                      return await task2.Exec3Async();
                  });

        Assert.AreEqual(AsyncTestRetryTask2.Result, text);
        Assert.AreEqual(3, count);
        Assert.AreEqual(6, count2);
    }


    [TestMethod]
    public async Task Test_Error()
    {
        Retry retry = Retry.Create(5, 10);

        MyAssert.IsError<ArgumentNullException>(() => {
            _= retry.Filter<Exception>(null);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            _ = retry.OnException(null);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            retry.Run<string>(null);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
             string value = await retry.RunAsync<string>(null);
        });
    }

}


public class TestRetryTask1
{
    private int _count = 0;


    public static readonly string Result = "1111111111111111";


    public string Exec1()
    {
        _count++;

        if( _count < 3 )
            throw new InvalidOperationException();


        return Result;
    }


    public string Exec2()
    {
        _count++;

        if( _count == 1 )
            throw new NotSupportedException();

        if( _count == 2 )
            throw new ArgumentOutOfRangeException("name");

        if( _count == 3 )
            throw new ArgumentNullException("key");


        return Result;
    }


    public string Exec3()
    {
        _count++;

        if( _count <= 3 )
            throw new NotSupportedException();


        return Result;
    }

}



public class AsyncTestRetryTask2
{
    private int _count = 0;


    public static readonly string Result = "22222222222222222";


    public Task<string> Exec1Async()
    {
        _count++;

        if( _count < 3 )
            throw new InvalidOperationException();


        return Task.FromResult(Result);
    }


    public Task<string> Exec2Async()
    {
        _count++;

        if( _count == 1 )
            throw new NotSupportedException();

        if( _count == 2 )
            throw new ArgumentOutOfRangeException("name");

        if( _count == 3 )
            throw new ArgumentNullException("key");


        return Task.FromResult(Result);
    }


    public Task<string> Exec3Async()
    {
        _count++;

        if( _count <= 3 )
            throw new NotSupportedException();


        return Task.FromResult(Result);
    }

}
