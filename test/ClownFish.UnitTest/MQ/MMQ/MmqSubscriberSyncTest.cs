#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ.MMQ;
using Microsoft.Identity.Client;

namespace ClownFish.UnitTest.MQ.MMQ;
[TestClass]
public class MmqSubscriberSyncTest
{
    private static readonly MemoryMesssageQueue<string> s_mmq = new MemoryMesssageQueue<string>(MmqWorkMode.Sync, 100);

    [TestMethod]
    public void Test_1()
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string> {
            Queue = s_mmq,
            RetryCount = 1,
            RetryWaitMilliseconds = 10,
            SubscriberCount = 1,
            CancellationToken = tokenSource.Token
        };

        MmqSubscriber.Start<string, MyTestMessageHandler>(args);

        s_mmq.Write("aaaaa");
        s_mmq.Write("bbbbb");
        s_mmq.Write("xx");    // NotSupportedException
        s_mmq.Write("c");     // retry 1

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

        MmqSubscriberSync<string> subscriber = new MmqSubscriberSync<string>(handler, args);

        CancellationToken token = (CancellationToken)subscriber.GetFieldValue("_cancellationToken");
        Assert.AreEqual(ClownFishInit.AppExitToken, token);
    }

    [TestMethod]
    public void Test_3()
    {
        MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string> {
            Queue = s_mmq,
        };

        MyTestMessageHandler handler = new MyTestMessageHandler();
        MmqSubscriberSync<string> subscriber = new MmqSubscriberSync<string>(handler, args);

        Assert.AreEqual(0, subscriber.HandleMessage(null));

        Exception ex = ExceptionHelper.CreateException();
        TestHelper.SetException(ex);
        Assert.AreEqual(-1, subscriber.HandleMessage("aaaaaaaaaaa"));

        Assert.AreEqual(1, subscriber.HandleMessage("aaaaaaaaaaa"));
    }


    internal class MyTestMessageHandler : BaseMessageHandler<string>
    {
        internal static readonly ValueCounter ProcessMessageCounter = new ValueCounter();

        public override bool EnableLog => EnableLogValue;

        public bool EnableLogValue { get; set; } = false;

        public override void ProcessMessage(PipelineContext<string> context)
        {
            ProcessMessageCounter.Increment();

            string data = context.MessageData;

            if( data == "xx" )
                throw new NotSupportedException();

            if( data.Length <= 2 )
                throw new ApplicationException("xxxxxxxxxxxx");
            else
                Console.WriteLine(context.MessageData);
        }
    }
}
#endif
