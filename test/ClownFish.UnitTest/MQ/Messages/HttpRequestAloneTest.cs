namespace ClownFish.UnitTest.MQ.Messages;
#if NETCOREAPP

[TestClass]
public class HttpRequestAloneTest
{
    internal static readonly string RequestRawText = @"
POST http://localhost:8206/v20/api/WebSiteApp/test/Databus.aspx?id=12&name=abc HTTP/1.1
Content-Type: application/json; chartset=utf-8
x-client-app: TestApp
x-client-reqid: 1111111111111
x-client-url: http://www.fish-test.com/aaa/bb.aspx
xc-dbname: {dbname}
x-aaaaa: aaaaaaaaaaaaaaaaa
Cookie: a1=111111111; b1=2222222222; c1=333333333333
User-Agent: HttpTestUI/1.22.805.100

[
    {
        ""orderID"": 12,
        ""productID"": 550,
        ""unitPrice"": 115.0,
        ""quantity"": 1,
        ""productName"": ""新光饰品 新光发饰 新光发夹 施华洛世奇水钻横夹 送女友生日礼物"",
        ""unit"": ""个"",
        ""unitPrice1"": 115.0,
        ""customerName"": ""上海东佳铸锻厂""
    },
    {
        ""orderID"": 12,
        ""productID"": 561,
        ""unitPrice"": 29.8,
        ""quantity"": 1,
        ""productName"": ""★新光饰品★【钻石信誉】银底黑锆幸运四叶花新光发簪"",
        ""unit"": ""个"",
        ""unitPrice1"": 29.8,
        ""customerName"": ""上海东佳铸锻厂""
    }
]
".Trim();

    internal static readonly string RequestRawText2 = @"
POST http://localhost:8206/v20/api/WebSiteApp/test/Databus.aspx HTTP/1.1
Content-Type: application/json

{
    ""orderID"": 12,
    ""productID"": 550,
    ""unitPrice"": 115.0,
    ""quantity"": 1,
    ""productName"": ""新光饰品 新光发饰 新光发夹 施华洛世奇水钻横夹 送女友生日礼物"",
    ""unit"": ""个"",
    ""unitPrice1"": 115.0,
    ""customerName"": ""上海东佳铸锻厂""
}
".Trim();

    internal static readonly string RequestRawText3 = @"
GET http://localhost:8206/v20/api/WebSiteApp/test/Databus.aspx HTTP/1.1
Content-Type: application/json; chartset=utf-8
x-client-app: TestApp
x-client-reqid: 1111111111111
x-client-url: http://www.fish-test.com/aaa/bb.aspx
xc-dbname: {dbname}
x-aaaaa: aaaaaaaaaaaaaaaaa
".Trim();

    internal static readonly string RequestRawText4 = @"
GET http://localhost:8206/v20/api/WebSiteApp/test/Databus.aspx HTTP/1.1
".Trim();




    [TestMethod]
    public void Test1()
    {
        // 测试从二进制数据中还原对象
        RequestData data1 = new RequestData();
        (data1 as IBinarySerializer).LoadData(RequestDataTest.RequestBytes);

        Test_Serialize(data1);
    }


    [TestMethod]
    public void Test2()
    {
        // 测试不同的 HttpRequest 场景

        RequestData data1 = RequestData.FromRawText(RequestRawText);
        Test_Serialize(data1);

        RequestData data2 = RequestData.FromRawText(RequestRawText2);
        Test_Serialize(data2);

        RequestData data3 = RequestData.FromRawText(RequestRawText3);
        Test_Serialize(data3);

        RequestData data4 = RequestData.FromRawText(RequestRawText4);
        Test_Serialize(data4);
    }

    private void Test_Serialize(RequestData data1)
    {
        HttpRequestAlone req = new HttpRequestAlone(data1);

        // 对象还原
        RequestData data2 = RequestData.Create(req);
        Assert.AreEqual(data1.RequestLine, data2.RequestLine);
        Assert.AreEqual(data1.Headers, data2.Headers);
        Assert.IsTrue(data1.Body.IsEqual(data2.Body));


        // 二进制序列化/反序列化
        byte[] b1 = MessageBinSerializer.Instance.Serialize(req);
        HttpRequestAlone req2 = MessageBinSerializer.Instance.Deserialize<HttpRequestAlone>(b1);
        Assert.AreEqual(req.HttpMethod, req2.HttpMethod);
        Assert.AreEqual(req.FullUrl, req2.FullUrl);
        Assert.AreEqual(req.ContentType, req2.ContentType);
        Assert.AreEqual(req.ReadBodyAsText(), req2.ReadBodyAsText());

        // 文本序列化/反序列化
        string text = MessageTextSerializer.Instance.Serialize(req);
        HttpRequestAlone req3 = MessageTextSerializer.Instance.Deserialize<HttpRequestAlone>(text);
        Assert.AreEqual(req.HttpMethod, req3.HttpMethod);
        Assert.AreEqual(req.FullUrl, req3.FullUrl);
        Assert.AreEqual(req.ContentType, req3.ContentType);
        Assert.AreEqual(req.ReadBodyAsText(), req3.ReadBodyAsText());


        // 测试多次转换并还原
        HttpRequestAlone req4 = new HttpRequestAlone(data2);
        RequestData data4 = RequestData.Create(req4);
        Assert.AreEqual(data1.RequestLine, data4.RequestLine);
        Assert.AreEqual(data1.Headers, data4.Headers);
        Assert.IsTrue(data1.Body.IsEqual(data4.Body));
    }


    [TestMethod]
    public void Test_FirstLine()
    {
        RequestData data1 = RequestData.FromRawText(RequestRawText);
        HttpRequestAlone req = new HttpRequestAlone(data1);

        Assert.AreEqual("POST", req.HttpMethod);
        Assert.AreEqual("/v20/api/WebSiteApp/test/Databus.aspx", req.Path);
        Assert.AreEqual("?id=12&name=abc", req.Query);
        Assert.AreEqual("http://localhost:8206/v20/api/WebSiteApp/test/Databus.aspx", req.FullPath);
        Assert.AreEqual("http://localhost:8206/v20/api/WebSiteApp/test/Databus.aspx?id=12&name=abc", req.FullUrl);
        Assert.AreEqual("/v20/api/WebSiteApp/test/Databus.aspx?id=12&name=abc", req.RawUrl);
        Assert.AreEqual("http://localhost:8206", req.RootUrl);

        Assert.IsFalse(req.IsHttps);
    }

    [TestMethod]
    public void Test_Query()
    {
        RequestData data1 = RequestData.FromRawText(RequestRawText);
        HttpRequestAlone req = new HttpRequestAlone(data1);

        string[] names = req.QueryStringKeys;
        Assert.AreEqual(2, names.Length);

        Assert.AreEqual("id", names[0]);
        Assert.AreEqual("12", req.QueryString("id"));

        Assert.AreEqual("name", names[1]);
        Assert.AreEqual("abc", req.QueryString("name"));
    }


    [TestMethod]
    public void Test_Header()
    {
        RequestData data1 = RequestData.FromRawText(RequestRawText);
        HttpRequestAlone req = new HttpRequestAlone(data1);

        string[] names = req.HeaderKeys;
        Assert.AreEqual(8, names.Length);

        Assert.AreEqual("Content-Type", names[0]);
        Assert.AreEqual("application/json; chartset=utf-8", req.Header("Content-Type"));
        Assert.AreEqual("application/json", req.ContentType);

        Assert.AreEqual("x-client-app", names[1]);
        Assert.AreEqual("TestApp", req.Header("x-client-app"));

        Assert.AreEqual("x-client-reqid", names[2]);
        Assert.AreEqual("1111111111111", req.Header("x-client-reqid"));

        Assert.AreEqual("x-client-url", names[3]);
        Assert.AreEqual("xc-dbname", names[4]);
        Assert.AreEqual("x-aaaaa", names[5]);
        Assert.AreEqual("Cookie", names[6]);
        Assert.AreEqual("User-Agent", names[7]);
        Assert.AreEqual("HttpTestUI/1.22.805.100", req.UserAgent);
    }


    [TestMethod]
    public void Test_Header2()
    {
        RequestData data1 = RequestData.FromRawText(RequestRawText4);
        HttpRequestAlone req = new HttpRequestAlone(data1);

        Assert.IsNull(req.ContentType);
        Assert.IsNull(req.UserAgent);
    }


    [TestMethod]
    public void Test_Cookie()
    {
        RequestData data1 = RequestData.FromRawText(RequestRawText);
        HttpRequestAlone req = new HttpRequestAlone(data1);

        string[] names = req.CookieKeys;
        Assert.AreEqual(3, names.Length);

        Assert.AreEqual("a1", names[0]);
        Assert.AreEqual("111111111", req.Cookie("a1"));

        Assert.AreEqual("b1", names[1]);
        Assert.AreEqual("2222222222", req.Cookie("b1"));

        Assert.AreEqual("c1", names[2]);
        Assert.AreEqual("333333333333", req.Cookie("c1"));
    }

}
#endif
