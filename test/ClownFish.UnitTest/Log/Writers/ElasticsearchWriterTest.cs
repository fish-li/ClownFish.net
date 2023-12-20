using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers;
[TestClass]
public class ElasticsearchWriterTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        HttpClientMockResults.Clear();
    }

    [TestMethod]
    public void Test1()
    {
        ElasticsearchWriter writer = new ElasticsearchWriter();

        List<InvokeLog> list = new List<InvokeLog>();
        list.Add(new InvokeLog());


        long count1 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.Write(list);
        long count2 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count1, count2);


        writer.InternalInit("es_config");

        string esResponse = "{\"took\":6,\"errors\":false,\"items\":[{\"index\":{\"_index\":\"oprlog-20231219-18\",\"_type\":\"_doc\",\"_id\":\"4b617341f22f4e36a631f0eccaab84be\",\"_version\":1,\"result\":\"created\",\"_shards\":{\"total\":2,\"successful\":1,\"failed\":0},\"_seq_no\":83599,\"_primary_term\":1,\"status\":201}}]}";
        HttpClientMockResults.SetMockResult("ClownFish_SimpleEsClient_WriteList", esResponse);

        long count3 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.Write(list);
        long count4 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count3 +1, count4);
    }
}
