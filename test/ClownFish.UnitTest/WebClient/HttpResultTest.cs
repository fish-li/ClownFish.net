using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class HttpResultTest
{
    [TestMethod]
    public void Test_1()
    {
        HttpResult<string> result2 = new (600, new NameValueCollection(), "abc");
        Assert.AreEqual(600, result2.StatusCode);
        Assert.AreEqual("abc", result2.Result);
        Assert.IsNotNull(result2.Headers);
        Assert.AreEqual(0, result2.Headers.Count);
    }


    [TestMethod]
    public void Test_2()
    {
        using( HttpWebResponse response = CreateHttpWebResponse() ) {

            HttpResult<string> result = response.GetResult();

            string text = result.ToAllText();

            Console.WriteLine(text);

            Assert.IsTrue(text.Contains("x-status: OK"));
            Assert.IsTrue(text.Contains("Set-Cookie: c1=; expires="));
            Assert.IsTrue(text.Contains("Set-Cookie: c2=xxxxxxx; expires="));
            Assert.IsTrue(text.Contains("Content-Type: text/html; charset=utf-8"));
        }
    }


    internal static HttpWebResponse CreateHttpWebResponse(string url = null)
    {
        HttpOption option = new HttpOption {
            Url = url ?? HttpOptionTest.TestUrl
        };

        return option.GetResult<HttpWebResponse>();
    }

#if NETCOREAPP

    [TestMethod]
    public void Test_HttpResult_string_Serializer()
    {
        NameValueCollection headers = new NameValueCollection();
        headers.Add("name1", "111");
        headers.Add("name2", "222");
        string body = Guid.NewGuid().ToString() + " 中华文明";

        HttpResult<string> httpResult = new HttpResult<string>(666, headers, body);
        string text1 = (httpResult as ITextSerializer).ToText();

        HttpResult<string> httpResult2 = new HttpResult<string>(200, null, null);
        (httpResult2 as ITextSerializer).LoadData(text1);

        Assert.AreEqual(666, httpResult2.StatusCode);
        Assert.IsNotNull(httpResult2.Headers);
        Assert.AreEqual("111", httpResult2.GetHeader("name1"));
        Assert.AreEqual("222", httpResult2.GetHeader("name2"));
        Assert.AreEqual(body, httpResult2.Result);

        byte[] bb = (httpResult as IBinarySerializer).ToBytes();
        HttpResult<string> httpResult3 = new HttpResult<string>(200, null, null);
        (httpResult3 as IBinarySerializer).LoadData(bb);

        Assert.AreEqual(666, httpResult3.StatusCode);
        Assert.IsNotNull(httpResult3.Headers);
        Assert.AreEqual("111", httpResult3.GetHeader("name1"));
        Assert.AreEqual("222", httpResult3.GetHeader("name2"));
        Assert.AreEqual(body, httpResult3.Result);
    }

    [TestMethod]
    public void Test_HttpResult_string_Serializer_2()
    {
        NameValueCollection headers = null;
        string body = Guid.NewGuid().ToString() + " 中华文明";

        HttpResult<string> httpResult = new HttpResult<string>(666, headers, body);
        string text1 = (httpResult as ITextSerializer).ToText();

        HttpResult<string> httpResult2 = new HttpResult<string>(200, null, null);
        (httpResult2 as ITextSerializer).LoadData(text1);

        Assert.AreEqual(666, httpResult2.StatusCode);
        Assert.IsNotNull(httpResult2.Headers);
        Assert.AreEqual(body, httpResult2.Result);

        byte[] bb = (httpResult as IBinarySerializer).ToBytes();
        HttpResult<string> httpResult3 = new HttpResult<string>(200, null, null);
        (httpResult3 as IBinarySerializer).LoadData(bb);

        Assert.AreEqual(666, httpResult3.StatusCode);
        Assert.IsNotNull(httpResult3.Headers);
        Assert.AreEqual(body, httpResult3.Result);
    }

    [TestMethod]
    public void Test_HttpResult_string_Serializer_3()
    {
        NameValueCollection headers = null;
        string body = "";

        HttpResult<string> httpResult = new HttpResult<string>(666, headers, body);
        string text1 = (httpResult as ITextSerializer).ToText();

        HttpResult<string> httpResult2 = new HttpResult<string>(200, null, null);
        (httpResult2 as ITextSerializer).LoadData(text1);

        Assert.AreEqual(666, httpResult2.StatusCode);
        Assert.IsNotNull(httpResult2.Headers);
        Assert.IsNull(httpResult2.Result);

        byte[] bb = (httpResult as IBinarySerializer).ToBytes();
        HttpResult<string> httpResult3 = new HttpResult<string>(200, null, null);
        (httpResult3 as IBinarySerializer).LoadData(bb);

        Assert.AreEqual(666, httpResult3.StatusCode);
        Assert.IsNotNull(httpResult3.Headers);
        Assert.IsNull(httpResult3.Result);
    }

    [TestMethod]
    public void Test_HttpResult_long_Serializer()
    {
        NameValueCollection headers = new NameValueCollection();
        headers.Add("name1", "111");
        headers.Add("name2", "222");
        long body = 123L;

        HttpResult<long> httpResult = new HttpResult<long>(666, headers, body);
        string text1 = (httpResult as ITextSerializer).ToText();

        HttpResult<long> httpResult2 = new HttpResult<long>(200, null, 0);
        (httpResult2 as ITextSerializer).LoadData(text1);

        Assert.AreEqual(666, httpResult2.StatusCode);
        Assert.IsNotNull(httpResult2.Headers);
        Assert.AreEqual("111", httpResult2.GetHeader("name1"));
        Assert.AreEqual("222", httpResult2.GetHeader("name2"));
        Assert.AreEqual(body, httpResult2.Result);

        byte[] bb = (httpResult as IBinarySerializer).ToBytes();
        HttpResult<long> httpResult3 = new HttpResult<long>(200, null, 0);
        (httpResult3 as IBinarySerializer).LoadData(bb);

        Assert.AreEqual(666, httpResult3.StatusCode);
        Assert.IsNotNull(httpResult3.Headers);
        Assert.AreEqual("111", httpResult3.GetHeader("name1"));
        Assert.AreEqual("222", httpResult3.GetHeader("name2"));
        Assert.AreEqual(body, httpResult3.Result);
    }

    [TestMethod]
    public void Test_HttpResult_bytes_Serializer()
    {
        NameValueCollection headers = new NameValueCollection();
        headers.Add("name1", "111");
        headers.Add("name2", "222");
        string body = Guid.NewGuid().ToString() + " 中华文明";

        HttpResult<byte[]> httpResult = new HttpResult<byte[]>(666, headers, body.GetBytes());
        string text1 = (httpResult as ITextSerializer).ToText();

        HttpResult<byte[]> httpResult2 = new HttpResult<byte[]>(200, null, null);
        (httpResult2 as ITextSerializer).LoadData(text1);

        Assert.AreEqual(666, httpResult2.StatusCode);
        Assert.IsNotNull(httpResult2.Headers);
        Assert.AreEqual("111", httpResult2.GetHeader("name1"));
        Assert.AreEqual("222", httpResult2.GetHeader("name2"));
        Assert.AreEqual(body, httpResult2.Result.ToUtf8String());

        byte[] bb = (httpResult as IBinarySerializer).ToBytes();
        HttpResult<byte[]> httpResult3 = new HttpResult<byte[]>(200, null, null);
        (httpResult3 as IBinarySerializer).LoadData(bb);

        Assert.AreEqual(666, httpResult3.StatusCode);
        Assert.IsNotNull(httpResult3.Headers);
        Assert.AreEqual("111", httpResult3.GetHeader("name1"));
        Assert.AreEqual("222", httpResult3.GetHeader("name2"));
        Assert.AreEqual(body, httpResult3.Result.ToUtf8String());
    }


    [TestMethod]
    public void Test_HttpResult_object_Serializer()
    {
        NameValueCollection headers = new NameValueCollection();
        headers.Add("name1", "111");
        headers.Add("name2", "222");

        Product2 body = new Product2 {
            PId = 123,
            PName = Guid.NewGuid().ToString(),
            Unt = "x",
            CID = 999,
            Quantity2 = 321,
            Remark2 = Guid.NewGuid().ToString() + " 中华文明",
            UPrice = 23.45m
        };
        string bodyJson = body.ToJson();

        HttpResult<Product2> httpResult = new HttpResult<Product2>(666, headers, body);
        string text1 = (httpResult as ITextSerializer).ToText();

        HttpResult<Product2> httpResult2 = new HttpResult<Product2>(200, null, null);
        (httpResult2 as ITextSerializer).LoadData(text1);

        Assert.AreEqual(666, httpResult2.StatusCode);
        Assert.IsNotNull(httpResult2.Headers);
        Assert.AreEqual("111", httpResult2.GetHeader("name1"));
        Assert.AreEqual("222", httpResult2.GetHeader("name2"));
        Assert.AreEqual(bodyJson, httpResult2.Result.ToJson());

        byte[] bb = (httpResult as IBinarySerializer).ToBytes();
        HttpResult<Product2> httpResult3 = new HttpResult<Product2>(200, null, null);
        (httpResult3 as IBinarySerializer).LoadData(bb);

        Assert.AreEqual(666, httpResult3.StatusCode);
        Assert.IsNotNull(httpResult3.Headers);
        Assert.AreEqual("111", httpResult3.GetHeader("name1"));
        Assert.AreEqual("222", httpResult3.GetHeader("name2"));
        Assert.AreEqual(bodyJson, httpResult3.Result.ToJson());
    }

#endif

}
