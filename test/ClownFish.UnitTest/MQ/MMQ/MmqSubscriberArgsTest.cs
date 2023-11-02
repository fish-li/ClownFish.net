#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ.MMQ;

namespace ClownFish.UnitTest.MQ.MMQ;
[TestClass]
public class MmqSubscriberArgsTest
{
    [TestMethod]
    public void Test1()
    {
        MmqSubscriberArgs<string> args = new MmqSubscriberArgs<string>();
        args.Queue = null;
        args.SubscriberCount = 3;
        args.RetryCount = 5;
        args.RetryWaitMilliseconds = 10;

        Assert.IsNull(args.Queue);
        Assert.AreEqual(3, args.SubscriberCount);
        Assert.AreEqual(5, args.RetryCount);
        Assert.AreEqual(10, args.RetryWaitMilliseconds);
    }
}
#endif
