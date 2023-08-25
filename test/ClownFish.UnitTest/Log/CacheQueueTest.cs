using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Log;
using ClownFish.Log.Writers;
using ClownFish.UnitTest.Log.Writers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log
{
    [TestClass]
    public class CacheQueueTest
    {
        [TestMethod]
        public void Test()
        {
            MemoryWriter memoryWriter = new MemoryWriter();
            ILogWriter[] writers = new ILogWriter[] { memoryWriter };

            CacheQueue<XMessage> queue = new CacheQueue<XMessage>();
            queue.SetFieldValue("_writers", writers);

            long inCount1 = ClownFishCounters.Logging.InQueueCount.Get();
            long xCount1 = ClownFishCounters.Logging.GiveupCount.Get();

            for( int i = 0; i < 15; i++ )
                queue.Add((object)new XMessage(i));


            long inCount2 = ClownFishCounters.Logging.InQueueCount.Get();
            long xCount2 = ClownFishCounters.Logging.GiveupCount.Get();

            Assert.AreEqual(10, inCount2 - inCount1);
            Assert.AreEqual(5, xCount2 - xCount1);  // 5条消息被放弃了，因为超过最大长度

            List<XMessage> list = (List<XMessage>)queue.GetFieldValue("_list");
            Assert.AreEqual(10, list.Count);

            Assert.AreEqual(0, memoryWriter.PullALL().Count);
            queue.Flush();
            Assert.AreEqual(10, memoryWriter.PullALL().Count);

        }

        [TestMethod]
        public void Test2()
        {
            CacheQueue<XMessage> queue = new CacheQueue<XMessage>();
            queue.SetFieldValue("_writers", null);

            // 队列为空
            Assert.AreEqual(0, queue.Flush());

            // 没有关联的写入器
            queue.Add((object)new XMessage(123));
            Assert.AreEqual(-1, queue.Flush());


            ILogWriter[] writers = new ILogWriter[] { new NullWriter() };
            queue.SetFieldValue("_writers", writers);

            // 写入成功
            queue.Add((object)new XMessage(123));
            Assert.AreEqual(1, queue.Flush());

            // 验证批量写入
            for( int i = 0; i < LoggingOptions.WriteListBatchSize + 101; i++ )
                queue.Add((object)new XMessage(i));

            Assert.AreEqual(2, queue.Flush());
        }
    }
}
