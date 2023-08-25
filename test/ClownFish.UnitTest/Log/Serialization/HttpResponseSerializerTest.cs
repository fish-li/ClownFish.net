#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient.V2;
using ClownFish.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Serialization;
[TestClass]
public class HttpResponseSerializerTest
{
    [TestMethod]
    public void Test_1()
    {
        HttpResponseMessage response = null;
        string text = response.ToLoggingText();

        Assert.AreEqual("", text);
    }

    [TestMethod]
    public void Test_2()
    {
        HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        string text = response.ToLoggingText();
        Console.WriteLine(text);

        Assert.AreEqual("HTTP/1.1 403 Forbidden", text.Trim());
    }

    [TestMethod]
    public void Test_3()
    {
        HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        response.Headers.Add("x-name1", "11111111");
        string text = response.ToLoggingText();
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("HTTP/1.1 403 Forbidden"));
        Assert.IsTrue(text.Contains("x-name1: 11111111"));
    }

    [TestMethod]
    public void Test_4()
    {
        HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        response.Headers.Add("x-name1", "11111111");
        response.Content = HttpObjectUtils.CreateRequestMessageBody3(SerializeFormat.Form, new { a = 2, b = 3, c = "abc" });

        string text = response.ToLoggingText();
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("HTTP/1.1 200 OK"));
        Assert.IsTrue(text.Contains("x-name1: 11111111"));
        Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        Assert.IsFalse(text.Contains("a=2&b=3&c=abc"));
    }

    [TestMethod]
    public void Test_5()
    {
        HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        response.Headers.Add("x-name1", "11111111");
        response.Content = HttpObjectUtils.CreateRequestMessageBody2(SerializeFormat.Binary, "a=2&b=3&c=abc".GetBytes());
        response.Content.Headers.TryAddWithoutValidation("Content-Length", "11");

        string text = response.ToLoggingText();
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("HTTP/1.1 200 OK"));
        Assert.IsTrue(text.Contains("x-name1: 11111111"));
        Assert.IsTrue(text.Contains("Content-Type: application/octet-stream"));
        Assert.IsFalse(text.Contains("a=2&b=3&c=abc"));
    }

    [TestMethod]
    public void Test_6()
    {
        HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        response.Headers.Add("x-name1", "11111111");
        response.Content = HttpObjectUtils.CreateRequestMessageBody3(SerializeFormat.Form, new { a = 2, b = 3, c = "abc" });
        response.Content.Headers.TryAddWithoutValidation("Content-Length", "11");

        string text = response.ToLoggingText();
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("HTTP/1.1 200 OK"));
        Assert.IsTrue(text.Contains("x-name1: 11111111"));
        Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        Assert.IsTrue(text.Contains("a=2&b=3&c=abc"));
    }
}
#endif
