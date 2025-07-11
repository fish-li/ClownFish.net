using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers;

[TestClass]
public class HttpJsonWriterTest 
{
    private static List<OprLog> CreateTestData()
    {
        List<OprLog> logs = new List<OprLog>();
        logs.Add(new OprLog {
            OprId = "79ac9c4110574453a8295a1f92679980",
            OprName = "aaaa"
        });
        logs.Add(new OprLog {
            OprId = "35fd6ab07dda4570843bf6dae51f00cd",
            OprName = "bbbb"
        });

        return logs;
    }

    [TestMethod]
    public void Test_InitUrl()
    {
        HttpJsonWriter writer = new HttpJsonWriter();

        int result1 = writer.InitUrl(null);
        Assert.AreEqual(0, result1 );

        int result2 = writer.InitUrl("");
        Assert.AreEqual(0, result2);

        int result3 = writer.InitUrl("http://abc.com/");
        Assert.AreEqual(1, result3);
        Assert.AreEqual("http://abc.com/v20/api/loggate/save/{datatype}?app=ClownFish.UnitTest", (string)writer.GetFieldValue("_url"));
        Assert.AreEqual("http://abc.com/v20/api/loggate/save/OprLog?app=ClownFish.UnitTest", (string)writer.GetFieldValue("_urlOprlog"));

        HttpJsonWriter writer2 = new HttpJsonWriter();
        int result4 = writer2.InitUrl("http://abc.com/v20/api/loggate/save/{datatype}");
        Assert.AreEqual(1, result4);
        Assert.AreEqual("http://abc.com/v20/api/loggate/save/{datatype}?app=ClownFish.UnitTest", (string)writer2.GetFieldValue("_url"));
        Assert.AreEqual("http://abc.com/v20/api/loggate/save/OprLog?app=ClownFish.UnitTest", (string)writer2.GetFieldValue("_urlOprlog"));
    }


    [TestMethod]
    public void Test1()
    {
        List<OprLog> logs = CreateTestData();

        HttpJsonWriter2 writer = new HttpJsonWriter2();
        writer.SetUrl("http://xxxxxxxxxxxxxxxxxxx");
        (writer as ILogWriter).WriteList(logs);

        string response = writer.ResponseOut.ToString();
        Console.WriteLine(response);

        Assert.IsTrue(response.Contains("Content-Length ="));
        Assert.IsTrue(response.Contains("Content-Type = application/x-ndjson"));
        Assert.IsTrue(response.Contains("Content-Encoding = gzip"));
        Assert.IsTrue(response.Contains("x-datatype = ClownFish.Log.Logging.OprLog"));
    }

    [TestMethod]
    public void Test2()
    {
        List<OprLog> logs = CreateTestData();

        HttpJsonWriter3 writer = new HttpJsonWriter3();
        writer.SetUrl("http://xxxxxxxxxxxxxxxxxxx");

        (writer as ILogWriter).WriteList(logs);

        // 确认异常不会抛出
    }

    [TestMethod]
    public void Test_CheckResponse()
    {
        HttpResult<string> result = new HttpResult<string>(200, null, "-22");
        Assert.AreEqual(-22, HttpJsonWriter.CheckResponse(result, "xxx"));


        string returnId = Guid.NewGuid().ToString("N");

        MyAssert.IsError<InvalidOperationException>(() => {
            HttpResult<string> result2 = new HttpResult<string>(200, null, "123");
            HttpJsonWriter.CheckResponse(result2, returnId);
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("x-returnid", "xxxxxxxxxxx");
            HttpResult<string> result3 = new HttpResult<string>(200, headers, "123");
            HttpJsonWriter.CheckResponse(result3, returnId);
        });


        NameValueCollection headers = new NameValueCollection();
        headers.Add("x-returnid", returnId);
        HttpResult<string> result4 = new HttpResult<string>(200, headers, "123");
        Assert.AreEqual(1, HttpJsonWriter.CheckResponse(result4, returnId));
    }
}



internal class HttpJsonWriter2 : HttpJsonWriter
{
    public readonly StringBuilder ResponseOut = new StringBuilder();

    protected override void SendRequest(HttpOption httpOption)
    {
        httpOption.Url = "http://www.fish-test.com/show-request2.aspx";
        //httpOption.Url = "http://linuxtest:8206/v20/api/WebSiteApp/test/ShowRequest.aspx";

        string response = httpOption.GetResult();
        ResponseOut.AppendLine(response);
    }
}

internal class HttpJsonWriter3 : HttpJsonWriter
{
    protected override void SendRequest(HttpOption httpOption)
    {
        httpOption.Url = "http://www.xxxxxxxxxxxxxx.com/show-request2.aspx";
        httpOption.Timeout = 100;
        base.SendRequest(httpOption);
    }
}