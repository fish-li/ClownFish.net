﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Clients.Elastic;

namespace ClownFish.UnitTest.Http.Clients.Elastic;
[TestClass]
public class SimpleEsClientTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        HttpClientMockResults.Clear();
    }

    [TestMethod]
    public void Test_1()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new SimpleEsClient(null);
        });

        SimpleEsClient client = CreateClient();

        HttpOption httpOption = client.CreateHttpOption("GET", "/aa/bb");
        Assert.AreEqual("http://localpc:12345/aa/bb", httpOption.Url);

        string auth = httpOption.Headers["Authorization"];
        Assert.AreEqual("Basic cm9vdDp4eHh4", auth);
    }


    private SimpleEsClient CreateClient()
    {
        EsConnOption opt = EsConnOption.Create("es_conn");
        Assert.AreEqual("-yyyyMMdd", opt.IndexNameTimeFormat);

        return new SimpleEsClient(opt);
    }

    [TestMethod]
    public async Task Test_WriteOne()
    {
        SimpleEsClient client = CreateClient();
        client.WriteOne<InvokeLog>(null);
        await client.WriteOneAsync<InvokeLog>(null);

        HttpClientMockResults.SetMockResult("ClownFish_SimpleEsClient_WriteOne", "", false);

        client.WriteOne(new InvokeLog());        
        await client.WriteOneAsync(new InvokeLog());
    }

    [TestMethod]
    public async Task Test_WriteList()
    {
        List<InvokeLog> list = new List<InvokeLog>();

        SimpleEsClient client = CreateClient();
        client.WriteList(list);
        await client.WriteListAsync(list);


        list.Add(new InvokeLog());

        string esResponse = "{\"took\":6,\"errors\":false,\"items\":[{\"index\":{\"_index\":\"oprlog-20231219-18\",\"_type\":\"_doc\",\"_id\":\"4b617341f22f4e36a631f0eccaab84be\",\"_version\":1,\"result\":\"created\",\"_shards\":{\"total\":2,\"successful\":1,\"failed\":0},\"_seq_no\":83599,\"_primary_term\":1,\"status\":201}}]}";
        HttpClientMockResults.SetMockResult("ClownFish_SimpleEsClient_WriteList", esResponse, false);

        client.WriteList(list);
        await client.WriteListAsync(list);
    }

    [TestMethod]
    public async Task Test_Search()
    {
        SimpleEsClient client = CreateClient();

        var result = new SimpleEsClient.SearchResponse<NameInt64>();
        result.Hits = new SimpleEsClient.SearchHit<NameInt64>();
        result.Hits.Hits = new List<SimpleEsClient.HitData<NameInt64>>();
        result.Hits.Hits.Add(new SimpleEsClient.HitData<NameInt64> { Data = new NameInt64("aa", 11) });

        HttpClientMockResults.SetMockResult("ClownFish_SimpleEsClient_Search", result, false);

        var result1 = client.Search<NameInt64>("111", new object());
        var result2 = await client.SearchAsync<NameInt64>("222", new object());
    }

    [TestMethod]
    public void Test_GetIndexName_1()
    {
        SimpleEsClient client = new SimpleEsClient(EsConnOption.Create("es_conn"));

        string postfix = DateTime.Today.ToString("-yyyyMMdd");
        string s1 = (string)client.InvokeMethod("GetIndexName", typeof(OprLog));
        Assert.AreEqual("oprlog" + postfix, s1);
    }

    [TestMethod]
    public void Test_GetIndexName_2()
    {
        SimpleEsClient client = new SimpleEsClient(EsConnOption.Create("es_conn").SetIndexNameTimeFormat(null));

        string s1 = (string)client.InvokeMethod("GetIndexName", typeof(OprLog));
        Assert.AreEqual("oprlog", s1);
    }

    [TestMethod]
    public void Test_GetIndexName_3()
    {
        SimpleEsClient client = new SimpleEsClient(EsConnOption.Create("es_conn").SetIndexNameTimeFormat("-yyyy"));

        string postfix = DateTime.Today.Year.ToString();
        string s1 = (string)client.InvokeMethod("GetIndexName", typeof(OprLog));
        Assert.AreEqual("oprlog-" + postfix, s1);
    }
}
