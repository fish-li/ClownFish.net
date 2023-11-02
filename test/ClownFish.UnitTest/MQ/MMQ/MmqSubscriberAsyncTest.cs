#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ.MMQ;

namespace ClownFish.UnitTest.MQ.MMQ;
[TestClass]
public class MmqSubscriberAsyncTest
{
    private static readonly MemoryMesssageQueue<string> s_mmq = new MemoryMesssageQueue<string>(MmqWorkMode.Async, 100);

    [TestMethod]
    public async Task Test_1()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string> {
            Queue = s_mmq,
            RetryCount = 1,
            RetryWaitMilliseconds = 10,
            SubscriberCount = 1,
            CancellationToken = tokenSource.Token
        };

        MmqSubscriber.StartAsync<string, MyTestMessageHandler>(args);

        await s_mmq.WriteAsync("aaaaa");
        await s_mmq.WriteAsync("bbbbb");
        await s_mmq.WriteAsync("xx");    // NotSupportedException
        await s_mmq.WriteAsync("c");     // retry 1

        Thread.Sleep(1000);

        tokenSource.Cancel();

        Assert.AreEqual(5, MyTestMessageHandler.ProcessMessageCounter.Get());
    }

    [TestMethod]
    public void Test_2()
    {
        MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string> {
            Queue = s_mmq,
        };

        MyTestMessageHandler handler = new MyTestMessageHandler();

        MmqSubscriberAsync<string> subscriber = new MmqSubscriberAsync<string>(handler, args);

        CancellationToken token = (CancellationToken)subscriber.GetFieldValue("_cancellationToken");
        Assert.AreEqual(ClownFishInit.AppExitToken, token);
    }

    [TestMethod]
    public async Task Test_3()
    {
        MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string> {
            Queue = s_mmq,
        };

        MyTestMessageHandler handler = new MyTestMessageHandler();
        MmqSubscriberAsync<string> subscriber = new MmqSubscriberAsync<string>(handler, args);

        int result = await subscriber.HandleMessage(null);
        Assert.AreEqual(0, result);

        Exception ex = ExceptionHelper.CreateException();
        TestHelper.SetException(ex);
        result = await subscriber.HandleMessage("aaaaaaaaaaa");
        Assert.AreEqual(-1, result);


        result = await subscriber.HandleMessage("aaaaaaaaaaa");
        Assert.AreEqual(1, result);
    }




    internal class MyTestMessageHandler : AsyncBaseMessageHandler<string>
    {
        internal static readonly ValueCounter ProcessMessageCounter = new ValueCounter();

        public override bool EnableLog => false;

        public override Task ProcessMessage(PipelineContext<string> context)
        {
            ProcessMessageCounter.Increment();

            string data = context.MessageData;

            if( data == "xx" )
                throw new NotSupportedException();

            if( data.Length <= 2 )
                throw new ApplicationException("xxxxxxxxxxxx");
            else
                Console.WriteLine(context.MessageData);

            return Task.CompletedTask;
        }

 
    }
}
#endif
