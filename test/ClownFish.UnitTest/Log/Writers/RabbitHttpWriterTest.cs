using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Writers;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ClownFish.UnitTest.Log.Writers;
[TestClass]
public class RabbitHttpWriterTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        HttpClientMockResults.Clear();
    }


    [TestMethod]
    public void Test1()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        List<InvokeLog> list = new List<InvokeLog>();
        list.Add(new InvokeLog());

        long count1 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        writer.WriteList(list);
        long count2 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        Assert.AreEqual(count1, count2);


        LogConfiguration config = LogConfiguration.LoadFromFile("ClownFish.Log.config");
        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_TestConnection", ClownFish.Base.Void.Value);
        writer.InternalInit(config, "rabbit_config");

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_SendMessage", ClownFish.Base.Void.Value);
        long count3 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        Assert.AreEqual(count3 + 1, count4);
    }

    [TestMethod]
    public void Test_InternalInit()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        LogConfiguration config = LogConfiguration.LoadFromFile("ClownFish.Log.config");
        Assert.AreEqual(-1, writer.InternalInit(config, "rabbit_config_xxx"));
        Assert.AreEqual(-2, writer.InternalInit(config, "key3"));

        foreach( var t in config.Types ) {
            t.TypeObject = Type.GetType(t.DataType);
            t.Writers += ",RabbitHttp";
        }

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_QueueDeclare", ClownFish.Base.Void.Value, false);
        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_QueueBind", ClownFish.Base.Void.Value, false);
        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_TestConnection", ClownFish.Base.Void.Value, false);
        Assert.AreEqual(1, writer.InternalInit(config, "rabbit_config"));
    }

    [TestMethod]
    public void Test_Write()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        LogConfiguration config = LogConfiguration.LoadFromFile("ClownFish.Log.config");
        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_TestConnection", ClownFish.Base.Void.Value);
        writer.InternalInit(config, "rabbit_config");

        List<OprLog> list = new List<OprLog>();
        list.Add(new OprLog());

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_SendMessage", ClownFish.Base.Void.Value);
        long count3 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        Assert.AreEqual(count3 + 1, count4);
    }


    [TestMethod]
    public void Test_BatchWrite()
    {
        RabbitHttpWriter writer = new RabbitHttpWriter();

        LogConfiguration config = LogConfiguration.LoadFromFile("ClownFish.Log.config");
        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_TestConnection", ClownFish.Base.Void.Value);
        writer.InternalInit(config, "rabbit_config");

        List<InvokeLog> list = new List<InvokeLog>();
        for( int i = 0; i < 100; i++ ) {
            list.Add(new InvokeLog());
        }

        HttpClientMockResults.SetMockResult("ClownFish_RabbitHttpClient_SendMessage", ClownFish.Base.Void.Value, false);
        long count3 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.Rabbit2WriteCount.Get();
        Assert.AreEqual(count3 + 100, count4);
    }

}
