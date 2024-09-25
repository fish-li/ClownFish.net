using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers;
[TestClass]
public class OprlogEsWriterTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        HttpClientMockResults.Clear();
    }


    [TestMethod]
    public void Test1()
    {
        OprlogEsWriter writer = new OprlogEsWriter();

        List<OprLog> list = new List<OprLog>();
        list.Add(new OprLog());

        long count1 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.WriteList(list);
        long count2 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count1, count2);

        writer.InternalInit("es_conn");

        HttpClientMockResults.SetMockResult("Elasticsearch_WriteList", ClownFish.Base.Void.Value);
        long count3 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count3 + 1, count4);
    }

    [TestMethod]
    public void Test_InternalInit_fail()
    {
        OprlogEsWriter writer = new OprlogEsWriter();

        Assert.IsFalse(writer.InternalInit("rabbit_config_xxx"));
    }

    [TestMethod]
    public void Test_Write()
    {
        OprlogEsWriter writer = new OprlogEsWriter();

        writer.InternalInit("es_conn");

        List<OprLog> list = new List<OprLog>();
        list.Add(new OprLog());

        HttpClientMockResults.SetMockResult("Elasticsearch_WriteList", ClownFish.Base.Void.Value);
        long count3 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count3 + 1, count4);
    }

    [TestMethod]
    public void Test_Write_2()
    {
        OprlogEsWriter writer = new OprlogEsWriter();

        writer.InternalInit("es_conn");

        List<InvokeLog> list = new List<InvokeLog>();    // OprlogEsWriter 只支持 OprLog 的写入操作
        list.Add(new InvokeLog());

        long count3 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count3, count4);
    }


    [TestMethod]
    public void Test_BatchWrite()
    {
        OprlogEsWriter writer = new OprlogEsWriter();

        writer.InternalInit("es_conn");

        List<OprLog> list = new List<OprLog>();
        for( int i = 0; i < 100; i++ ) {
            list.Add(new OprLog());
        }

        HttpClientMockResults.SetMockResult("Elasticsearch_WriteList", ClownFish.Base.Void.Value, false);
        long count3 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.WriteList(list);
        long count4 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count3 + 100, count4);
    }
}
