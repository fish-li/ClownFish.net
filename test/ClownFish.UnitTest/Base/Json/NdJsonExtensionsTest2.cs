//#if NET9_0_OR_GREATER

//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Text.Json.Serialization.Metadata;
//using ClownFish.Base;
//using ClownFish.Base.Json;


//namespace ClownFish.UnitTest.Base.Json;

//public struct FidesClientObject
//{
//    public DateTime LastConnTime { get; set; }

//    public EndClientUserInfo ClientInfo { get; set; }
//}

//[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
//[JsonSerializable(typeof(FidesClientObject))]
//[JsonSerializable(typeof(EndClientUserInfo))]
//internal partial class FidesClientObjectJsonContext : JsonSerializerContext { }

////[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
////[JsonSerializable(typeof(EndClientUserInfo))]
////internal partial class EndClientUserInfoJsonContext : JsonSerializerContext { }


//[TestClass]
//public class NdJsonExtensionsTest2
//{
    
//    private static EndClientUserInfo CreateClientUserInfo()
//    {
//        return new EndClientUserInfo {
//            TenantId = "my5ddf894c653xx",
//            ClientId = "ts089c76120ddxx",
//            AppId = "b4ca0f5b62644dfeb9470ef5ffba55a5",
//            AppName = "TxClientX",
//            Version = "5.25.10716.10/5.10.0.136/.NET 8.0.6",
//            ClientRole = "TxClient",
//            HostName = "k8s-master-1",
//            Ip = "172.20.9.101",
//            Cluster = "xc-cdwhzbtest-test",
//            OsKind = 2,
//            OsName = "Ubuntu 24.04.2 LTS",
//            CpuKind = "X64",
//            TimeZone = "Asia/Shanghai",
//            DeployMode = 7,
//            RunMode = 200,
//            GrayFlag = 0,
//            ClientData = Guid.NewGuid().ToString("N"),
//            ExtData = Guid.NewGuid().ToString("N")
//        };
//    }


//    [TestMethod]
//    public void Test_performance()
//    {
//        List<FidesClientObject> list = new List<FidesClientObject>(5000);
//        DateTime now = DateTime.Now;

//        for( int i = 0; i < 5000; i++ ) {
//            EndClientUserInfo client = CreateClientUserInfo();
//            list.Add(new FidesClientObject { ClientInfo = client, LastConnTime = now.AddSeconds(1) });
//        }

//        int count = 100;
//        string lastJson1 = null, lastJson2 = null;
//        object client1 = null, client2 = null;

//        var x = list.First().ToJson().FromJson<FidesClientObject>();  // 预热
//        //var x2 = list.First().ObjToList().ToNdJson().FromNdJson<FidesClientObject>();

//        Stopwatch sw = Stopwatch.StartNew();

//        for( int i = 0; i < count; i++ ) {
//            string s = list.ToJson();
//            lastJson1 = s;
//        }
//        sw.Stop();
//        Console.Write("ToJson: ".PadRight(20));
//        Console.WriteLine(sw.Elapsed.ToString());


//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            string s = list.ToNdjson();
//            lastJson2 = s;
//        }
//        sw.Stop();
//        Console.Write("ToNdjson: ".PadRight(20));
//        Console.WriteLine(sw.Elapsed.ToString());


//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            List<FidesClientObject> list2 = lastJson1.FromJson<List<FidesClientObject>>();
//            client1 = list2.First();
//        }
//        sw.Stop();
//        Console.Write("FromJson: ".PadRight(20));
//        Console.WriteLine(sw.Elapsed.ToString());


//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            List<FidesClientObject> list2 = lastJson2.FromNdjson<FidesClientObject>(5000);
//            client2 = list2.First();
//        }
//        sw.Stop();
//        Console.Write("FromNdjson: ".PadRight(20));
//        Console.WriteLine(sw.Elapsed.ToString());


//        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
//            TypeInfoResolver = FidesClientObjectJsonContext.Default
//            //TypeInfoResolver = JsonTypeInfoResolver.Combine(FidesClientObjectJsonContext.Default, EndClientUserInfoJsonContext.Default)
//        };

//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            string s = list.ToNdJson2(jsonSerializerOptions);
//            lastJson2 = s;
//        }
//        sw.Stop();
//        Console.Write("ToNdJson2: ".PadRight(20));
//        Console.WriteLine(sw.Elapsed.ToString());

//        object client3 = null;
//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            List<FidesClientObject> list2 = lastJson2.FromNdJson2<FidesClientObject>(5000, jsonSerializerOptions);
//            client3 = list2.First();
//        }
//        sw.Stop();
//        Console.Write("FromNdJson2: ".PadRight(20));
//        Console.WriteLine(sw.Elapsed.ToString());

//        Console.WriteLine($"JsonArray.Length:   {lastJson1.Length}");
//        Console.WriteLine($"NdJson.Length:      {lastJson2.Length}");

//        Assert.AreEqual(list.First().ToJson(), client1.ToJson());
//        Assert.AreEqual(list.First().ToJson(), client2.ToJson());
//        Assert.AreEqual(list.First().ToJson(), client3.ToJson());

//        //Console.WriteLine("---------------------------------");
//        //Console.WriteLine(lastJson1);
//        //Console.WriteLine("---------------------------------");
//        //Console.WriteLine(lastJson2);

//        // output:  (count = 100)
//        //ToJson:             00:00:01.3771499
//        //ToNdjson:           00:00:00.8930489
//        //FromJson:           00:00:01.9413256
//        //FromNdjson:         00:00:02.8036464

//        //ToNdJson2:          00:00:01.1619535
//        //FromNdJson2:        00:00:02.2545541

//        // 结论：json-array 和 ndjson 在序列化和反序列化的时间总和差不多
//    }


//    [TestMethod]
//    public void Test_performance2()
//    {
//        List<FidesClientObject> list = new List<FidesClientObject>(5000);
//        DateTime now = DateTime.Now;

//        for( int i = 0; i < 5000; i++ ) {
//            EndClientUserInfo client = CreateClientUserInfo();
//            list.Add(new FidesClientObject { ClientInfo = client, LastConnTime = now.AddSeconds(1) });
//        }

//        int count = 100;
//        int len1 = 0, len2 = 0, len3 = 0, len4 = 0;

//        var x = list.First().ToJson().FromJson<FidesClientObject>();  // 预热

//        Stopwatch sw = Stopwatch.StartNew();

//        for( int i = 0; i < count; i++ ) {
//            string json1 = list.ToNdjson();
//            len1 = json1.Length;
//        }
//        sw.Stop();
//        Console.Write("ToNdjson-ToString: ".PadRight(32));
//        Console.WriteLine(sw.Elapsed.ToString());


//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            using MemoryStream stream = MemoryStreamPool.GetStream();
//            using( StreamWriter writer = new StreamWriter(stream, EncodingUtils.UTF8NoBOM, 1024, true) ) {
//                list.ToNdjson(writer);
//            }
//            len2 = (int)stream.Length;
//        }
//        sw.Stop();
//        Console.Write("ToNdjson-ToStream: ".PadRight(32));
//        Console.WriteLine(sw.Elapsed.ToString());


//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            using MemoryStream stream = MemoryStreamPool.GetStream();
//            using( GZipStream gzip = new GZipStream(stream, CompressionMode.Compress, true) ) {
//                using( StreamWriter writer = new StreamWriter(gzip, EncodingUtils.UTF8NoBOM, 1024 * 4, true) ) {

//                    list.ToNdjson(writer);
//                }
//            }
//            len3 = (int)stream.Length;
//        }
//        sw.Stop();
//        Console.Write("ToNdjson-ToGzipStream: ".PadRight(32));
//        Console.WriteLine(sw.Elapsed.ToString());



//        sw.Restart();
//        for( int i = 0; i < count; i++ ) {
//            string json1 = list.ToNdjson();
//            byte[] bytes = json1.ToGzip();
//            len4 = bytes.Length;
//        }
//        sw.Stop();
//        Console.Write("ToNdjson-ToGzipBytes: ".PadRight(32));
//        Console.WriteLine(sw.Elapsed.ToString());


//        Console.WriteLine($"ndjson.Length:        {len1}");
//        Console.WriteLine($"stream.Length:        {len2}");
//        Console.WriteLine($"gzip-stream.Length:   {len3}");
//        Console.WriteLine($"gzip-bytes.Length:    {len4}");



//        // output:  (count = 100)
//        //ToNdjson - ToString:       00:00:01.4197390
//        //ToNdjson - ToStream:       00:00:00.6168259
//        //ToNdjson - ToGzipStream:   00:00:00.8357746
//        //ToNdjson - ToGzipBytes:    00:00:01.4432242
//        //ndjson.Length:        2650000
//        //stream.Length:        2650000
//        //gzip - stream.Length:   13798
//        //gzip - bytes.Length:    13262
//    }



//}
//#endif
