using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using ClownFish.UnitTest.Base.WebClient;
using ClownFish.Base.WebClient;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;

namespace ClownFish.UnitTest.Base.Exceptions;

[TestClass]
public class RemoteWebExceptionTest
{
    public static readonly string DefaultUrl = "http://www.abc.com/aa/bb.aspx";


    public static readonly string ResponseText = @"x-RequestId: 65a09b23a7644aa7a27a641a43a5c657
x-HostName: 1b5b8b3075a2
x-AppName: XDemo.WebSiteApp
x-dotnet: .NET 6.0.5
x-Nebula: 6.22.615.100
x-req-url: /v20/api/WebSiteApp/perftest/database2?provider=MySqlConnector&tenantId=596c871f21722
x-ReuestCount: 2439508
x-PreRequestExecute-ThreadId: 41
x-PostRequestExecute-ThreadId: 40
x-result-datatype: String
Content-Length: 103
Content-Type: text/plain; charset=utf-8
Date: Wed, 15 Jun 2022 11:03:38 GMT
Server: Kestrel";


    [TestMethod]
    public void Test()
    {
        string url = "http://www.abc.com/aa/bb.txt";

        var ex = CreateRemoteWebException(null, 503, url, "error5", "testxxx");
        Assert.AreEqual(503, ex.StatusCode);
        Assert.AreEqual(url, ex.Url);
        Assert.AreEqual("testxxx", ex.ResponseText);

        string text = ex.ToLoggingText();
        //Console.WriteLine(text);

        string[] lines = text.ToLines();
        Assert.IsTrue(lines.Contains("ClownFish.Base.Exceptions.RemoteWebException: error5"));
        Assert.IsTrue(lines.Contains("HTTP/1.1 503 ServiceUnavailable"));
        Assert.IsTrue(lines.Contains("testxxx"));
        Assert.IsTrue(lines.Contains("-------------------------Response-------------------------"));
    }

    [TestMethod]
    public void Test2()
    {
        WebException ex1 = CreateWebException("xxxxxx001");
        RemoteWebException ex2 = new RemoteWebException(ex1, DefaultUrl);

        Assert.AreEqual(200, ex2.StatusCode);
        Assert.AreEqual(DefaultUrl, ex2.Url);
        Assert.IsNotNull(ex2.Result);
        Assert.IsNotNull(ex2.ResponseText);

        Assert.IsTrue(ex2.ResponseText.Contains("Hello Test!"));
        Assert.IsTrue(ex2.Message.Contains("Hello Test!"));
    }

    [TestMethod]
    public void TestError()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            _ = new RemoteWebException(null, DefaultUrl);
        });
    }

    internal static RemoteWebException CreateRemoteWebException(Exception ex = null, int statusCode = 0, string url = null, string exmessage = null, string responseText = null)
    {
        if( ex == null )
            ex = ExceptionHelper.CreateException();

        RemoteWebException ex2 = new RemoteWebException(ex, DefaultUrl);

        HttpResult<string> result = new (statusCode, new NameValueCollection(), responseText);

        typeof(RemoteWebException).InvokeMember("Result", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null, ex2, new object[] { result });

        if( url != null ) {
            typeof(RemoteWebException).InvokeMember("Url", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null, ex2, new object[] { url });
        }

        if( exmessage != null ) {
            typeof(RemoteWebException).InvokeMember("_message", BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic,
                null, ex2, new object[] { exmessage });
        }

        return ex2;
    }


    internal static WebException CreateWebException(string message)
    {
        HttpWebResponse response = HttpResultTest.CreateHttpWebResponse();

        return new WebException(message, null, WebExceptionStatus.ProtocolError, response);
    }

    [TestMethod]
    public void Test_StatusCode()
    {
        var ex = ExceptionHelper.CreateException();
        RemoteWebException ex2 = new RemoteWebException(ex, DefaultUrl);

        Assert.AreEqual(0, ex2.StatusCode);
        Assert.AreEqual(500, (ex2 as IErrorCode).GetErrorCode());
    }



#if NETCOREAPP

    internal static WebException CreateWebException(string exMessage, int statusCode, string responseText, Dictionary<string, string> headers = null)
    {
        HttpResponseMessage message = new HttpResponseMessage {
            StatusCode = (HttpStatusCode)statusCode,
            Content = new StringContent(responseText, Encoding.UTF8, "text/html")
        };

        if( headers.HasValue() ) {
            foreach(var kv in headers ) {
                if( ClownFish.Base.WebClient.V2.HttpObjectUtils.IsWellKnownContentHeader(kv.Key) )
                    message.Content.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                else
                    message.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            }
        }
        message.Headers.TryAddWithoutValidation("Server", "ClownFish.UnitTest");


        ConstructorInfo ctor = typeof(HttpWebResponse).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] {
            // internal HttpWebResponse(HttpResponseMessage _message, Uri requestUri, CookieContainer cookieContainer)
            typeof(HttpResponseMessage),typeof(Uri),typeof(CookieContainer)
        }, null);

        HttpWebResponse response = (HttpWebResponse)ctor.Invoke(new object[] {
            message, new Uri(DefaultUrl), new CookieContainer()
        });

        var ex = ExceptionHelper.CreateException();
        return new WebException(exMessage, ex, WebExceptionStatus.ProtocolError, response);
    }


    [TestMethod]
    public void Test_StatusCode_401()
    {
        WebException ex = CreateWebException("请登录", 401, "xx_<title>请登录2</title>");
        RemoteWebException ex2 = new RemoteWebException(ex, DefaultUrl);

        Assert.AreEqual(401, ex2.StatusCode);
        Assert.AreEqual(701, (ex2 as IErrorCode).GetErrorCode());
        Assert.AreEqual("请登录2", ex2.ServerMessage);
        Assert.AreEqual("xx_<title>请登录2</title>", ex2.ResponseText);
    }

    [TestMethod]
    public void Test_StatusCode_500()
    {
        Dictionary<string, string> header1 = new Dictionary<string, string> {
            {HttpHeaders.XResponse.ErrorMessage, "字段XX的值不能为空！".UrlEncode() }
        };
        WebException ex = CreateWebException("服务端异常XXX", 500, "xx_<title>服务端异常XXX</title>", header1);
        RemoteWebException ex2 = new RemoteWebException(ex, DefaultUrl);

        Assert.AreEqual(500, ex2.StatusCode);
        Assert.AreEqual(500, (ex2 as IErrorCode).GetErrorCode());
        Assert.AreEqual("字段XX的值不能为空！", ex2.ServerMessage.UrlDecode());
        Assert.AreEqual("xx_<title>服务端异常XXX</title>", ex2.ResponseText);
    }

    [TestMethod]
    public void Test_StatusCode_503()
    {
        WebException ex = CreateWebException("服务不可用", 503, "xx_<title>服务不可用2</title>");
        RemoteWebException ex2 = new RemoteWebException(ex, DefaultUrl);

        Assert.AreEqual(503, ex2.StatusCode);
        Assert.AreEqual(503, (ex2 as IErrorCode).GetErrorCode());
        Assert.AreEqual("服务不可用2", ex2.ServerMessage);
        Assert.AreEqual("xx_<title>服务不可用2</title>", ex2.ResponseText);
    }


    [TestMethod]
    public void Test_IHttpResultString()
    {
        Dictionary<string, string> header1 = new Dictionary<string, string> {
            {"x-test1", "d203de885b93463b91d33d1375eefef4" }
        };
        WebException ex = CreateWebException("服务端异常XXX", 500, "xx_<title>服务端异常XXX</title>", header1);
        RemoteWebException ex2 = new RemoteWebException(ex, DefaultUrl);

        Assert.AreEqual(500, ex2.StatusCode);
        Assert.AreEqual(500, (ex2 as IErrorCode).GetErrorCode());
        Assert.AreEqual("xx_<title>服务端异常XXX</title>", ex2.ResponseText);

        HttpResult<string> httpResult = (ex2 as IHttpResultString).Response;
        string text = httpResult.ToAllText();
        //Console.WriteLine(text);

        Assert.AreEqual(@"HTTP/1.1 500 InternalServerError
x-test1: d203de885b93463b91d33d1375eefef4
Server: ClownFish.UnitTest
Content-Type: text/html; charset=utf-8

xx_<title>服务端异常XXX</title>", text);
    }



    [TestMethod]
    public void Test_ToLoggingText()
    {
        Dictionary<string, string> header1 = new Dictionary<string, string> {
            {"x-test1", "d203de885b93463b91d33d1375eefef4" }
        };
        WebException ex = CreateWebException("服务端异常XXX", 500, "xx_<title>服务端异常XXX</title>", header1);
        RemoteWebException ex2 = new RemoteWebException(ex, DefaultUrl);

        Assert.AreEqual(500, ex2.StatusCode);
        Assert.AreEqual(500, (ex2 as IErrorCode).GetErrorCode());

        //Console.WriteLine(ex2.ToLoggingText());

        string outText = @"ClownFish.Base.Exceptions.RemoteWebException: 服务端异常XXX
=)本次调用的目标地址：http://www.abc.com/aa/bb.aspx
 ---> System.Net.WebException: 服务端异常XXX
 ---> ClownFish.Base.Exceptions.MessageException: 一个用于测试的异常
   at ClownFish.UnitTest.ExceptionHelper.CreateException() in x:\xxxxxx\Nebula.net\test\ClownFish.UnitTest\ExceptionHelper.cs:line 9999999999999
   --- End of inner exception stack trace ---
   --- End of inner exception stack trace ---
-------------------------Response-------------------------
HTTP/1.1 500 InternalServerError
x-test1: d203de885b93463b91d33d1375eefef4
Server: ClownFish.UnitTest
Content-Type: text/html; charset=utf-8

xx_<title>服务端异常XXX</title>".DeleteErrorLineNo();

        Assert.AreEqual(outText, ex2.ToLoggingText().DeleteErrorLineNo());
        Assert.AreEqual(outText, ex2.ToAllText().DeleteErrorLineNo());
        Assert.AreEqual(outText, ex2.ToString2().DeleteErrorLineNo());


    }


    



#endif



}


internal static class RemoteWebExceptionTestUtils
{
    public static string DeleteErrorLineNo(this string text)
    {
        string[] lines = text.ToArray('\r', '\n');

        if( lines[4].StartsWith0("at ClownFish.UnitTest.ExceptionHelper.CreateException() in") )
            lines[4] = "at ClownFish.UnitTest.ExceptionHelper.CreateException() in";

        return string.Join("\r\n", lines);
    }
}