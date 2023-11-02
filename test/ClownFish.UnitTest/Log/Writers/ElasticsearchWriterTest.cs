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
        HttpClientMockResults.SetMockResult("ClownFish_SimpleEsClient_WriteList", ClownFish.Base.Void.Value);

        long count3 = ClownFishCounters.Logging.EsWriteCount.Get();
        writer.Write(list);
        long count4 = ClownFishCounters.Logging.EsWriteCount.Get();
        Assert.AreEqual(count3 +1, count4);
    }
}
