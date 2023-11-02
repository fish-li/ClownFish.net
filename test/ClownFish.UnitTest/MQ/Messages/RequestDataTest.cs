using System.Xml.Schema;
using Org.BouncyCastle.Ocsp;

namespace ClownFish.UnitTest.MQ.Messages;
#if NETCOREAPP

[TestClass]
public class RequestDataTest
{
    internal static readonly byte[] RequestBytes = System.IO.File.ReadAllBytes("files/Databus_request.bin");
    internal static readonly string RequestText = System.IO.File.ReadAllText("files/Databus_request.txt");

    [TestMethod]
    public void Test1()
    {
        RequestData data1 = new RequestData();
        (data1 as IBinarySerializer).LoadData(RequestBytes);

        RequestData data2 = new RequestData();
        (data2 as ITextSerializer).LoadData(RequestText);

        Assert.AreEqual(data1.RequestLine, data2.RequestLine);
        Assert.AreEqual(data1.Headers, data2.Headers);
        Assert.IsTrue(data1.Body.IsEqual(data2.Body));
    }


    [TestMethod]
    public void Test2()
    {
        RequestData data1 = new RequestData();
        (data1 as IBinarySerializer).LoadData(RequestBytes);


        byte[] b1 = MessageBinSerializer.Instance.Serialize(data1);
        RequestData data2 = MessageBinSerializer.Instance.Deserialize<RequestData>(b1);

        Assert.AreEqual(data1.RequestLine, data2.RequestLine);
        Assert.AreEqual(data1.Headers, data2.Headers);
        Assert.IsTrue(data1.Body.IsEqual(data2.Body));


        string text = MessageTextSerializer.Instance.Serialize(data1);
        data2 = MessageTextSerializer.Instance.Deserialize<RequestData>(text);

        Assert.AreEqual(data1.RequestLine, data2.RequestLine);
        Assert.AreEqual(data1.Headers, data2.Headers);
        Assert.IsTrue(data1.Body.IsEqual(data2.Body));
    }


    [TestMethod] 
    public async Task Test3()
    {
        RequestData data = new RequestData();
        (data as IBinarySerializer).LoadData(RequestBytes);

        HttpRequestAlone req = new HttpRequestAlone(data);

        RequestData data2 = RequestData.Create(req);
        RequestData data3 = await RequestData.CreateAsync(req);

        Assert.AreEqual(data.RequestLine, data2.RequestLine);
        Assert.AreEqual(data2.RequestLine, data3.RequestLine);

        Assert.AreEqual(data.Headers, data3.Headers);
        Assert.IsTrue(data.Body.IsEqual(data3.Body));
    }



    [TestMethod]
    public void Test4()
    {
        RequestData data = new RequestData();
        (data as IBinarySerializer).LoadData(Empty.Array<byte>());

        Assert.AreEqual(string.Empty, data.RequestLine);
        Assert.AreEqual(string.Empty, data.Headers);
        Assert.AreEqual(0, data.Body.Length);
    }

    [TestMethod]
    public void Test5()
    {
        RequestData data = new RequestData();
        (data as ITextSerializer).LoadData("");

        Assert.AreEqual(string.Empty, data.RequestLine);
        Assert.AreEqual(string.Empty, data.Headers);
        Assert.AreEqual(0, data.Body.Length);
    }

    [TestMethod]
    public void Test6()
    {
        string reqLine = "GET http://xxx.com/ HTTP/1.1";
        RequestData data = new RequestData(reqLine, null, null);
        Assert.AreEqual(reqLine, data.ToString());

        RequestData data2 = new RequestData();
        Assert.AreEqual("NULL", data2.ToString());
    }


    [TestMethod]
    public void Test_error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = RequestData.Create(null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = RequestData.FromRawText("");
        });
    }
}
#endif
