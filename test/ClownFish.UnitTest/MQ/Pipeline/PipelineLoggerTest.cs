using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ;

namespace ClownFish.UnitTest.MQ.Pipeline;

#if NETCOREAPP
using ClownFish.UnitTest.MQ.MMQ;

[TestClass]
public class PipelineLoggerTest
{
    [TestMethod]
    public void Test1()
    {
        string s1 = "中文汉字最优秀！";
        byte[] buffer = s1.ToUtf8Bytes();
        MqRequest req = new MqRequest {
            MqKind = MQSource.RabbitMQ,
            Original = buffer,
            Body = buffer,
            MessageObject = s1
        };

        var handler = new MmqSubscriberSyncTest.MyTestMessageHandler();
        handler.EnableLogValue = false;

        long logCount1 = ClownFishCounters.Logging.WriteCount.Get();
        using( PipelineContext<string> context1 = new PipelineContext<string>(req, handler, false, 0) ) {
            Assert.IsTrue(context1.OprLogScope.IsNull);
        }
        long logCount2 = ClownFishCounters.Logging.WriteCount.Get();

        // 没有生成日志
        Assert.IsTrue(logCount1 == logCount2);



        handler.EnableLogValue = true;
        using( PipelineContext<string> context2 = new PipelineContext<string>(req, handler, false, 3) ) {
            Assert.IsFalse(context2.OprLogScope.IsNull);
            Assert.IsFalse(context2.IsAsync);
            Assert.AreEqual(3, context2.RetryN);
            Assert.IsTrue(object.ReferenceEquals(s1, context2.GetRequest()));
            Console.WriteLine(context2.GetTitle());
        }
        long logCount3 = ClownFishCounters.Logging.WriteCount.Get();

        // 有日志生成
        Assert.IsTrue(logCount3 > logCount2);        
    }
}
#endif
