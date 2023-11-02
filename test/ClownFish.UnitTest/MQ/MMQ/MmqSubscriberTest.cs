#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ.MMQ;

namespace ClownFish.UnitTest.MQ.MMQ;
[TestClass]
public class MmqSubscriberTest
{
    [TestMethod]
    public void Test_CheckArgs()
    {
        MemoryMesssageQueue<string> mmq = new MemoryMesssageQueue<string>(MmqWorkMode.Sync, 100);


        MyAssert.IsError<ArgumentNullException>(() => {
            MmqSubscriber.CheckArgs((MmqSubscriberArgs<string>)null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string>();
            args.Queue = null;
            MmqSubscriber.CheckArgs(args);
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string>();
            args.Queue = mmq;
            args.SubscriberCount = -1;
            MmqSubscriber.CheckArgs(args);
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string>();
            args.Queue = mmq;
            args.SubscriberCount = 1;
            args.RetryCount = -1;
            MmqSubscriber.CheckArgs(args);
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string>();
            args.Queue = mmq;
            args.SubscriberCount = 1;
            args.RetryCount = 1;
            args.RetryWaitMilliseconds = -1;
            MmqSubscriber.CheckArgs(args);
        });
    }
}
#endif
