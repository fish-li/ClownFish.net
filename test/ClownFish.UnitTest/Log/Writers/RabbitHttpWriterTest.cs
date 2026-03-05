using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers;
[TestClass]
public class RabbitHttpWriterTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        HttpClientMockResults.Clear();
    }

    private void SetupMockResults()
    {
        HttpClientMockResults.SetMockResult("Rabbit_TestConnection", ClownFish.Base.Void.Value, false);

        HttpClientMockResults.SetMockResult("Rabbit_QueueDeclare", ClownFish.Base.Void.Value, false);
        HttpClientMockResults.SetMockResult("Rabbit_QueueBind", ClownFish.Base.Void.Value, false);
        
        HttpClientMockResults.SetMockResult("Rabbit_SendMessage", ClownFish.Base.Void.Value, false);
    }

    [TestMethod]
    public void Test1()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        List<InvokeLog> list = new List<InvokeLog>();
        list.Add(new InvokeLog());

        long count1 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        writer.WriteList(list);   // 忽略调用
        long count2 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        Assert.AreEqual(count1, count2);
    }

    [TestMethod]
    public void Test_InternalInit()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        Assert.AreEqual(-1, writer.InternalInit(typeof(InvokeLog), "rabbit_config_xxx"));
        Assert.AreEqual(-2, writer.InternalInit(typeof(InvokeLog), "key3"));

        SetupMockResults();
        Assert.AreEqual(1, writer.InternalInit(typeof(InvokeLog), "rabbit_config"));
    }

    [TestMethod]
    public void Test_Write()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        SetupMockResults();
        writer.InternalInit(typeof(OprLog), "rabbit_config");

        List<OprLog> list = new List<OprLog>();
        list.Add(new OprLog());

        long count3 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        Assert.AreEqual(count3 + 1, count4);
    }


    [TestMethod]
    public void Test_BatchWrite()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        SetupMockResults();
        writer.InternalInit(typeof(InvokeLog), "rabbit_config");

        List<InvokeLog> list = new List<InvokeLog>();
        for( int i = 0; i < 100; i++ ) {
            list.Add(new InvokeLog());
        }


        long count3 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        Assert.AreEqual(count3 + 100, count4);
    }

}
