using ClownFish.Base.Json;

namespace ClownFish.UnitTest.Base.Json;
[TestClass]
public class NdJsonExtensionsTest
{
    [TestMethod]
    public void Test_ToMultiLineJson()
    {
        List<Product2> list = new List<Product2>();
        list.Add(Product2.CreateByFixedData());
        list.Add(Product2.CreateByFixedData());
        list.Add(Product2.CreateByFixedData());

        string lines = list.ToMultiLineJson().TrimEnd();
        Assert.AreEqual(2, lines.Where(x => x == '\n').Count());
        Assert.IsTrue(lines.StartsWith("{"));
        Assert.IsTrue(lines.EndsWith("}"));


        List<Product2> list2 = lines.FromMultiLineJson<Product2>();
        Assert.AreEqual(3, list2.Count);

        MyAssert.AreEqual(list, list2);
    }


    [TestMethod]
    public void Test_FromMultiLineJson()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Product2.CreateByFixedData().ToJson()).Append("\n");
        sb.Append(Product2.CreateByRandomData().ToJson()).Append("\n");
        sb.Append(Product2.CreateByRandomData().ToJson()).Append("\n");

        List<Product2> list = sb.ToString().FromMultiLineJson<Product2>();
        Assert.AreEqual(3, list.Count);


        List<Product2> list2 = NdJsonExtensions.FromMultiLineJson<Product2>(string.Empty);
        Assert.AreEqual(0, list2.Count);

        List<Product2> list3 = NdJsonExtensions.FromMultiLineJson<Product2>((string)null);
        Assert.IsNull(list3);
    }

#if NET9_0_OR_GREATER

    [TestMethod]
    public void Test_performance()
    {
        List<FidesClientObject> list = new List<FidesClientObject>(5000);
        DateTime now = DateTime.Now;

        for( int i = 0; i < 5000; i++ ) {
            EndClientUserInfo client = CreateClientUserInfo();
            list.Add(new FidesClientObject { ClientInfo = client, LastConnTime = now.AddSeconds(1) });
        }

        int count = 100;
        string lastJson1 = null, lastJson2 = null;
        object client1 = null, client2 = null;

        var x = list.First().ToJson().FromJson<FidesClientObject>();  // 预热
        //var x2 = list.First().ObjToList().ToNdJson().FromNdJson<FidesClientObject>();

        Stopwatch sw = Stopwatch.StartNew();

        for( int i = 0; i < count; i++ ) {
            string s = list.ToJson();
            lastJson1 = s;
        }
        sw.Stop();
        Console.Write("ToJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            string s = list.ToMultiLineJson();
            lastJson2 = s;
        }
        sw.Stop();
        Console.Write("ToMultiLineJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            List<FidesClientObject> list2 = lastJson1.FromJson<List<FidesClientObject>>();
            client1 = list2.First();
        }
        sw.Stop();
        Console.Write("FromJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            List<FidesClientObject> list2 = lastJson2.FromMultiLineJson<FidesClientObject>();
            client2 = list2.First();
        }
        sw.Stop();
        Console.Write("FromMultiLineJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());



        //sw.Restart();
        //for( int i = 0; i < count; i++ ) {
        //    string s = list.ToNdJson();
        //    lastJson2 = s;
        //}
        //sw.Stop();
        //Console.Write("ToNdJson: ".PadRight(20));
        //Console.WriteLine(sw.Elapsed.ToString());

        // object client3 = null;
        //sw.Restart();
        //for( int i = 0; i < count; i++ ) {
        //    List<FidesClientObject> list2 = lastJson2.FromNdJson<FidesClientObject>();
        //    client3 = list2.First();
        //}
        //sw.Stop();
        //Console.Write("FromNdJson: ".PadRight(20));
        //Console.WriteLine(sw.Elapsed.ToString());



        Console.WriteLine($"JsonArray.Length:   {lastJson1.Length}");
        Console.WriteLine($"NdJson.Length:      {lastJson2.Length}");

        //Console.WriteLine("---------------------------------");
        //Console.WriteLine(lastJson1);
        //Console.WriteLine("---------------------------------");
        //Console.WriteLine(lastJson2);

        // output:  (count = 100)
        //ToJson:             00:00:01.3771499
        //ToMultiLineJson:    00:00:00.8930489
        //FromJson:           00:00:01.9413256
        //FromMultiLineJson:  00:00:02.8036464

        //ToNdJson:           00:00:01.1619535
        //FromNdJson:         00:00:02.2545541

        // 结论：json-array 和 ndjson 在序列化和反序列化的时间总和差不多

        Assert.AreEqual(list.First().ToJson(), client1.ToJson());
        Assert.AreEqual(list.First().ToJson(), client2.ToJson());
        //Assert.AreEqual(list.First().ToJson(), client3.ToJson());
    }


    public struct FidesClientObject
    {
        public DateTime LastConnTime { get; set; }

        public EndClientUserInfo ClientInfo { get; set; }
    }
    private static EndClientUserInfo CreateClientUserInfo()
    {
        return new EndClientUserInfo {
            TenantId = "my5ddf894c653xx",
            ClientId = "ts089c76120ddxx",
            AppId = "b4ca0f5b62644dfeb9470ef5ffba55a5",
            AppName = "TxClientX",
            Version = "5.25.10716.10/5.10.0.136/.NET 8.0.6",
            ClientRole = "TxClient",
            HostName = "k8s-master-1",
            Ip = "172.20.9.101",
            Cluster = "xc-cdwhzbtest-test",
            OsKind = 2,
            OsName = "Ubuntu 24.04.2 LTS",
            CpuKind = "X64",
            TimeZone = "Asia/Shanghai",
            DeployMode = 7,
            RunMode = 200,
            GrayFlag = 0,
            ClientData = Guid.NewGuid().ToString("N"),
            ExtData = Guid.NewGuid().ToString("N")
        };
    }

    [TestMethod]
    public void Test_performance2()
    {
        List<FidesClientObject> list = new List<FidesClientObject>(5000);
        DateTime now = DateTime.Now;

        for( int i = 0; i < 5000; i++ ) {
            EndClientUserInfo client = CreateClientUserInfo();
            list.Add(new FidesClientObject { ClientInfo = client, LastConnTime = now.AddSeconds(1) });
        }

        int count = 100;
        int len1 = 0, len2 = 0, len3 = 0, len4 = 0;

        var x = list.First().ToJson().FromJson<FidesClientObject>();  // 预热

        Stopwatch sw = Stopwatch.StartNew();

        for( int i = 0; i < count; i++ ) {
            string json1 = list.ToMultiLineJson();
            len1 = json1.Length;
        }
        sw.Stop();
        Console.Write("ToMultiLineJson-ToString: ".PadRight(32));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            using MemoryStream stream = MemoryStreamPool.GetStream();
            using( StreamWriter writer = new StreamWriter(stream, EncodingUtils.UTF8NoBOM, 1024, true) ) {
                list.ToMultiLineJson(writer);
            }
            len2 = (int)stream.Length;
        }
        sw.Stop();
        Console.Write("ToMultiLineJson-ToStream: ".PadRight(32));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            using MemoryStream stream = MemoryStreamPool.GetStream();
            using( GZipStream gzip = new GZipStream(stream, CompressionMode.Compress, true) ) {
                using( StreamWriter writer = new StreamWriter(gzip, EncodingUtils.UTF8NoBOM, 1024 * 4, true) ) { // 增加数据窗口大小可以提高压缩率

                    list.ToMultiLineJson(writer);
                }
            }
            len3 = (int)stream.Length;
        }
        sw.Stop();
        Console.Write("ToMultiLineJson-ToGzipStream: ".PadRight(32));
        Console.WriteLine(sw.Elapsed.ToString());



        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            string json1 = list.ToMultiLineJson();
            byte[] bytes = json1.ToGzip();
            len4 = bytes.Length;
        }
        sw.Stop();
        Console.Write("ToMultiLineJson-ToGzipBytes: ".PadRight(32));
        Console.WriteLine(sw.Elapsed.ToString());


        Console.WriteLine($"ndjson.Length:        {len1}");
        Console.WriteLine($"stream.Length:        {len2}");
        Console.WriteLine($"gzip-stream.Length:   {len3}");
        Console.WriteLine($"gzip-bytes.Length:    {len4}");



        // output:  (count = 100)
        //ToMultiLineJson - ToString:       00:00:01.4197390
        //ToMultiLineJson - ToStream:       00:00:00.6168259
        //ToMultiLineJson - ToGzipStream:   00:00:00.8357746
        //ToMultiLineJson - ToGzipBytes:    00:00:01.4432242
        //ndjson.Length:        2650000
        //stream.Length:        2650000
        //gzip - stream.Length:   13798
        //gzip - bytes.Length:    13262
    }

#endif


    [TestMethod]
    public void Test_x_args()
    {
        List<InvokeLog> list = null;
        Assert.AreEqual("", list.ToMultiLineJson());
        Assert.AreEqual(0, list.ToMultiLineJson((TextWriter)null));

        string nullString = null;
        Assert.IsNull(nullString.FromMultiLineJson<InvokeLog>());
        Assert.AreEqual(0, string.Empty.FromMultiLineJson<InvokeLog>().Count);

        TextReader nullReader = null;
        Assert.AreEqual(0, nullReader.FromMultiLineJson<InvokeLog>().Count);


        MyAssert.IsError<ArgumentNullException>(() => {
            DbDataReader reader = null;
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            _ = reader.DbReaderToNdJson(100, writer);
        });


        using( DbContext db = DbContext.Create("sqlserver") ) {
            CPQuery query = db.CPQuery.Create("select 1 as aa");
            using( DbDataReader reader = query.ExecuteReader() ) {

                MyAssert.IsError<ArgumentNullException>(() => {
                    TextWriter writer = null;
                    _ = reader.DbReaderToNdJson(100, writer);
                });
            }
        }
    }
}
