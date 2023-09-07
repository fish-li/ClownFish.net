#if NETCOREAPP
using System.Net.Http;

namespace ClownFish.UnitTest.Log.Serialization;
[TestClass]
public class HttpRequestSerializerTest
{
    [TestMethod]
    public void Test_1()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "http://www.abc.com/aa/bb.aspx");
        string text = TextUtils.GetLogText(request);
        Console.WriteLine(text);

        Assert.AreEqual("GET http://www.abc.com/aa/bb.aspx HTTP/1.1", text.Trim());
    }


    [TestMethod]
    public void Test_2()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb.aspx",
            Format = SerializeFormat.Form,
            Data = new { a = 2, b = 3, c = "abc" },
            Header = new {
                Content_Length = 100, // 随便写个数字
            }
        };
        HttpRequestMessage request = ClownFish.WebClient.V2.HttpObjectUtils.CreateRequestMessage(http);

        string text = TextUtils.GetLogText(request);
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("POST http://www.abc.com/aa/bb.aspx HTTP/1.1"));
        Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        Assert.IsTrue(text.Contains("Content-Length: 100"));
        Assert.IsTrue(text.Contains("a=2&b=3&c=abc"));
    }

    [TestMethod]
    public void Test_3()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb.aspx",
            Format = SerializeFormat.Form,
            Data = new { a = 2, b = 3, c = "abc" },
            Header = new {
                Content_Length = 100, // 随便写个数字
            }
        };
        HttpRequestMessage request = ClownFish.WebClient.V2.HttpObjectUtils.CreateRequestMessage(http);

        request.Dispose();    // 注意这里！ ##################

        string text = TextUtils.GetLogText(request);
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("POST http://www.abc.com/aa/bb.aspx HTTP/1.1"));
        Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        Assert.IsTrue(text.Contains("Content-Length: 100"));
        Assert.IsTrue(text.Contains("a=2&b=3&c=abc"));
    }

    [TestMethod]
    public void Test_4()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb.aspx",
            Format = SerializeFormat.Form,
            Data = new { a = 2, b = 3, c = "abc" },
            //Header = new {             // 没有指定头，将不会读取 Body ######################
            //    Content_Length = 100, // 随便写个数字
            //}
        };
        HttpRequestMessage request = ClownFish.WebClient.V2.HttpObjectUtils.CreateRequestMessage(http);

        string text = TextUtils.GetLogText(request);
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("POST http://www.abc.com/aa/bb.aspx HTTP/1.1"));
        Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        Assert.IsFalse(text.Contains("a=2&b=3&c=abc"));
    }

    [TestMethod]
    public void Test_5()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb.aspx",
            Format = SerializeFormat.None,    // 内容格式不是文本，将不会读取 Body ######################
            Data = "a=2&b=3&c=abc",
            Header = new {
                Content_Length = 100, // 随便写个数字
            }
        };
        HttpRequestMessage request = ClownFish.WebClient.V2.HttpObjectUtils.CreateRequestMessage(http);

        string text = TextUtils.GetLogText(request);
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("POST http://www.abc.com/aa/bb.aspx HTTP/1.1"));
        Assert.IsFalse(text.Contains("a=2&b=3&c=abc"));
    }

    [TestMethod]
    public void Test_6()
    {
        HttpRequestMessage request = null;
        string text = request.ToLoggingText();

        Assert.AreEqual("", text);
    }


}
#endif