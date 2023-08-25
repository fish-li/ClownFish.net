using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Writers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    public void Test1()
    {
        List<OprLog> logs = CreateTestData();

        HttpJsonWriter2 writer = new HttpJsonWriter2();
        writer.SetUrl("http://xxxxxxxxxxxxxxxxxxx");
        (writer as ILogWriter).Write(logs);

        string response = writer.ResponseOut.ToString();
        Console.WriteLine(response);

        Assert.IsTrue(response.Contains("Content-Length ="));
        Assert.IsTrue(response.Contains("Content-Type = application/json-seq"));
        Assert.IsTrue(response.Contains("Content-Encoding = gzip"));
        Assert.IsTrue(response.Contains("x-datatype = ClownFish.Log.Logging.OprLog"));
    }

    [TestMethod]
    public void Test2()
    {
        List<OprLog> logs = CreateTestData();

        HttpJsonWriter3 writer = new HttpJsonWriter3();
        writer.SetUrl("http://xxxxxxxxxxxxxxxxxxx");

        (writer as ILogWriter).Write(logs);

        // 确认异常不会抛出
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
        base.SendRequest(httpOption);
    }
}