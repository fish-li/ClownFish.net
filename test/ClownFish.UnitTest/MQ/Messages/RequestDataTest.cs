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


    

}
#endif
