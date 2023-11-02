using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MQ;

namespace ClownFish.UnitTest.MQ;
[TestClass]
public class QueueUtilsTest
{
    [TestMethod]
    public void Test_GetQueueName()
    {
        MyAssert.IsError<NotSupportedException>(() => { 
            _ = QueueUtils.GetQueueName(typeof(List<int>));  
        });

        MyAssert.IsError<NotSupportedException>(() => {
            _ = QueueUtils.GetQueueName(typeof(int[]));
        });

        MyAssert.IsError<NotSupportedException>(() => {
            _ = QueueUtils.GetQueueName(typeof(int));
        });

        Assert.AreEqual("ClownFish.UnitTest.MQ.QueueUtilsTest", QueueUtils.GetQueueName(typeof(QueueUtilsTest)));
    }

    [TestMethod]
    public void Test_2()
    {
        Assert.IsTrue(QueueUtils.MinMessageLength >= 5);
    }
}
