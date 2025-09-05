using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Json;
[TestClass]
public class NdJsonReaderTest
{
    [TestMethod]
    public void Test_1()
    {
        List<Product3> list = Product3.CreateTestDataList(900);

        MemoryStream gzipData = new MemoryStream();

        using( StreamWriter writer = gzipData.CreateGzipWriter() ) {
            list.ToNdjson(writer);
        }


        gzipData.Position = 0;
        List<Product3> list2 = new List<Product3>();

        using( NdJsonReader reader = new NdJsonReader(gzipData, "gzip") ) {
            foreach( Product3 p in reader.ReadLines<Product3>() ) {
                list2.Add(p);
            }
        }

        Assert.AreEqual(900, list2.Count);

        gzipData.Position = 0;
        using StreamReader reader1 = gzipData.CreateGzipReader();
        string ndjson = reader1.ReadToEnd();

        Assert.IsTrue(ndjson.Contains(list.First().ToJson()));
        Assert.IsTrue(ndjson.Contains(list.Last().ToJson()));

        // =====================================================================

        MemoryStream notgData = new MemoryStream();

        using( StreamWriter writer2 = new StreamWriter(notgData, Encoding.UTF8, 1024, true) ) {
            list.ToNdjson(writer2);
        }

        notgData.Position = 0;
        List<Product3> list3 = new List<Product3>();

        using( NdJsonReader reader2 = new NdJsonReader(notgData, null) ) {
            foreach( Product3 p in reader2.ReadLines<Product3>() ) {
                list3.Add(p);
            }
        }

        Assert.AreEqual(900, list3.Count);

        notgData.Position = 0;
        using StreamReader reader3 = new StreamReader(notgData, Encoding.UTF8, true, 1024, true);
        string ndjson2 = reader3.ReadToEnd();

        Assert.IsTrue(ndjson2.Contains(list.First().ToJson()));
        Assert.IsTrue(ndjson2.Contains(list.Last().ToJson()));

        // =====================================================================

        Console.WriteLine($"gzipData.Length = {gzipData.Length}");
        Console.WriteLine($"notgData.Length = {notgData.Length}");
        Assert.IsTrue(notgData.Length > gzipData.Length);
    }


    [TestMethod]
    public void Test_ArgumentNullException()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            Stream httpStream = null;
            NdJsonReader reader = new NdJsonReader(httpStream);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            NHttpRequest request = null;
            NdJsonReader reader = NdJsonReader.Create(request);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            HttpResult<Stream> httpResult = null;
            NdJsonReader reader = NdJsonReader.Create(httpResult);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            HttpResult<Stream> httpResult = new HttpResult<Stream>(200, null, null);
            NdJsonReader reader = NdJsonReader.Create(httpResult);
        });
    }


    [TestMethod]
    public void Test_Create_NHttpRequest()
    {
        List<Product3> list = Product3.CreateTestDataList(900);
        string ndjson = list.ToNdjson();

        string requestText = @$"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: application/x-ndjson

{ndjson}".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        using NdJsonReader reader = NdJsonReader.Create(request);
        Assert.IsNotNull(reader);

        int count = 0;
        foreach( var item in reader.ReadLines<Product3>() ) {
            if( item != null ) { count++; }
        }

        Assert.AreEqual(900, count);

        InvalidOperationException ex = MyAssert.IsError<InvalidOperationException>(() => {
            NdJsonReader reader2 = NdJsonReader.Create(request);
        });

        Assert.IsTrue(ex.Message.Contains("请求体不可读或者已被读取过了"));
    }

    [TestMethod]
    public void Test_HttpRequest_Error()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
aa: 11
bb: 22

xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
".Trim();
        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        InvalidOperationException ex = MyAssert.IsError<InvalidOperationException>(() => {
            NdJsonReader reader = NdJsonReader.Create(request);
        });

        Assert.IsTrue(ex.Message.Contains("请求体数据类型不是预期的ndjson格式"));
    }


    [TestMethod]
    public void Test_Create_HttpResult_Stream()
    {
        List<Product3> list = Product3.CreateTestDataList(900);
        string ndjson = list.ToNdjson();

        MemoryStream ms = new MemoryStream(ndjson.ToUtf8Bytes().ToGzip());

        NameValueCollection headers = new NameValueCollection();
        headers.Add("Content-Type", "application/x-ndjson");

        // 一般来说，httpclient的场景，这个头是不会有的，因为已经设置了 SocketsHttpHandler.AutomaticDecompression
        headers.Add("Content-Encoding", "gzip");

        HttpResult<Stream> httpResult = new HttpResult<Stream>(200, headers, ms);

        using NdJsonReader reader = NdJsonReader.Create(httpResult);
        Assert.IsNotNull(reader);

        int count = 0;
        foreach( var item in reader.ReadLines<Product3>() ) {
            if( item != null ) { count++; }
        }

        Assert.AreEqual(900, count);

        reader.Dispose();
    }


    [TestMethod]
    public void Test_HttpResultStream_Error()
    {
        MemoryStream ms = new MemoryStream("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx".ToUtf8Bytes());

        NameValueCollection headers = new NameValueCollection();
        headers.Add("Content-Type", "text/plain");

        HttpResult<Stream> httpResult = new HttpResult<Stream>(200, headers, ms);

        InvalidOperationException ex = MyAssert.IsError<InvalidOperationException>(() => {
            NdJsonReader reader = NdJsonReader.Create(httpResult);
        });

        Assert.IsTrue(ex.Message.Contains("响应体数据类型不是预期的ndjson格式"));
    }


}
