#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;
using ClownFish.UnitTest.Http.Pipleline.Test;
using ClownFish.UnitTest.WebClient;
using ClownFish.WebClient.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class ResponseUtilsTest6
{
    private static readonly ConstructorInfo s_ctor = typeof(HttpWebResponse).GetConstructor(
                                                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                                        null, new Type[] { typeof(HttpResponseMessage), typeof(Uri), typeof(CookieContainer) }, null);

    internal static HttpWebResponse CreateHttpWebResponse(HttpResponseMessage responseMessage, 
                                    string url = "http://www.abc.com/aa/bb", CookieContainer cookieContainer = null)
    {
        Uri requestUri = new Uri(url);
        return (HttpWebResponse)s_ctor.Invoke(new object[] { responseMessage, requestUri, null });
    }

    [TestMethod]
    public void Test_GetResult_Null()
    {
        Assert.IsNull(ResponseUtils.GetResult(null));
    }

    [TestMethod]
    public void Test_GetResult()
    {
        HttpResponseMessage responseMessage = new HttpResponseMessage();
        responseMessage.StatusCode = HttpStatusCode.OK;
        responseMessage.Content = HttpObjectUtils.CreateRequestMessageBody2(SerializeFormat.Json, @"{""a"": 2, ""b"": 3}".ToUtf8Bytes());
        responseMessage.Headers.Add("Connection", "keep-alive");
        responseMessage.Headers.Add("Pragma", "no-cache");
        responseMessage.Headers.Add("Vary", "Accept-Encoding");
        responseMessage.Headers.Add("X-Content-Type-Options", "nosniff");
        responseMessage.Headers.Add("X-XSS-Protection", "1; mode=block");
        responseMessage.Headers.Add("Strict-Transport-Security", "max-age=15724800; includeSubDomains");
        responseMessage.Headers.Add("Content-Security-Policy", "frame-ancestors *.aaa.com *.bbb.com.cn *.ccc.com");
        responseMessage.Headers.Add("Cache-Control", "no-cache");
        responseMessage.Headers.Add("Date", "Thu, 07 Mar 2024 06:38:26 GMT");
        //responseMessage.Content.Headers.Add("Expires", "-1");  // The format of value '-1' is invalid.
        responseMessage.Headers.Add("x-name1", "aaaa");
        responseMessage.Headers.Add("Location", "/aa/bb/cc.html");


        HttpWebResponse response = CreateHttpWebResponse(responseMessage);

        HttpResult<string> httpResult = ResponseUtils.GetResult(response);

        Assert.AreEqual(200, httpResult.StatusCode);
        Assert.AreEqual("application/json; charset=utf-8", httpResult.GetHeader("Content-Type"));
        Assert.AreEqual("keep-alive", httpResult.GetHeader("Connection"));
        Assert.AreEqual("no-cache", httpResult.GetHeader("Pragma"));
        Assert.AreEqual("Accept-Encoding", httpResult.GetHeader("Vary"));
        Assert.AreEqual("nosniff", httpResult.GetHeader("X-Content-Type-Options"));
        Assert.AreEqual("1; mode=block", httpResult.GetHeader("X-XSS-Protection"));
        Assert.AreEqual("max-age=15724800; includeSubDomains", httpResult.GetHeader("Strict-Transport-Security"));
        Assert.AreEqual("frame-ancestors *.aaa.com *.bbb.com.cn *.ccc.com", httpResult.GetHeader("Content-Security-Policy"));
        Assert.AreEqual("no-cache", httpResult.GetHeader("Cache-Control"));
        Assert.AreEqual("Thu, 07 Mar 2024 06:38:26 GMT", httpResult.GetHeader("Date"));
        Assert.AreEqual("aaaa", httpResult.GetHeader("x-name1"));
        Assert.AreEqual("/aa/bb/cc.html", httpResult.GetHeader("Location"));
    }

    [Obsolete]
    [TestMethod]
    public void Test_ToResponseMessage()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ResponseUtils.ToResponseMessage((HttpWebResponse)null);
        });

        using( HttpWebResponse response = HttpResultTest.CreateHttpWebResponse() ) {

            HttpResponseMessage msg = response.ToResponseMessage();
            Assert.IsNotNull(msg);
        }

        MyAssert.IsError<ObjectDisposedException>(() => {
            _ = ResponseUtils.ToResponseMessage(new HttpWebResponse());
        });
    }


    [TestMethod]
    public void Test_CloneAllHeaders()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ResponseUtils.CloneAllHeaders((HttpResponseMessage)null);
        });

        using( HttpWebResponse response = HttpResultTest.CreateHttpWebResponse() ) {

            HttpResponseMessage msg = response.ToResponseMessage();

            NameValueCollection headers = msg.CloneAllHeaders();

            Assert.IsNotNull(headers);
        }
    }


    [TestMethod]
    public void Test_GetContentType()
    {
        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb",
            Data = new {
                a = 1,
                b = 2
            },
            Format = SerializeFormat.Json
        };

        HttpResponseMessage responseMessage = new HttpResponseMessage();
        responseMessage.Content = HttpObjectUtils.CreateRequestMessageBody(httpOption);

        string contentType = responseMessage.GetContentType();
        Assert.AreEqual("application/json; charset=utf-8", contentType);

        responseMessage.Content.Headers.Remove("Content-Type");
        Assert.IsNull(responseMessage.GetContentType());


        MyAssert.IsError<ArgumentNullException>(() => {
             _= ResponseUtils.GetContentType(null);
        });
    }

    [TestMethod]
    public void Test_GetContentType2()
    {
        HttpResponseMessage responseMessage = new HttpResponseMessage();
        Assert.IsNull(responseMessage.GetContentType());
    }


    [TestMethod]
    public void Test_GetHeaderValues()
    {
        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/aa/bb",
            Data = new {
                a = 1,
                b = 2
            },
            Format = SerializeFormat.Json,
        };

        HttpResponseMessage responseMessage = new HttpResponseMessage();
        responseMessage.Headers.Add("x-a", "aaa");
        responseMessage.Headers.Add("x-b", "bbb");
        responseMessage.Content = HttpObjectUtils.CreateRequestMessageBody(httpOption);

        string[] values = responseMessage.GetHeaders(HttpHeaders.Response.ContentType).ToArray();
        Assert.AreEqual(1, values.Length);
        Assert.AreEqual("application/json; charset=utf-8", values[0]);


        string[] values2 = responseMessage.GetHeaders("x-a").ToArray();
        Assert.AreEqual(1, values2.Length);
        Assert.AreEqual("aaa", values2[0]);


        Assert.IsNull(ResponseUtils.GetHeaders(responseMessage, "xxxx"));

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ResponseUtils.GetHeaders(null,  "xx");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ResponseUtils.GetHeaders(responseMessage, null);
        });
    }


    [TestMethod]
    public void Test_CopyResponseHeaders()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        Assert.AreEqual(0, httpContext.Response.SetResponseHeaders((NameValueCollection)null));
        Assert.AreEqual(0, httpContext.Response.SetResponseHeaders(new NameValueCollection()));

        NameValueCollection headers = new NameValueCollection();
        headers.Set("x-a", "aaa");
        headers.Set("x-b", "bbb");
        headers.Set(HttpHeaders.Response.ContentType, "application/json");
        headers.Set("Server", "test");
        headers.Set("Location", "/aa/bb.html");

        int count = httpContext.Response.SetResponseHeaders(headers);
        Assert.AreEqual(5, count);

        Assert.AreEqual("aaa", httpContext.Response.GetHeader("x-a"));
        Assert.AreEqual("bbb", httpContext.Response.GetHeader("x-b"));
        Assert.AreEqual("application/json", httpContext.Response.ContentType);
        Assert.AreEqual("/aa/bb.html", httpContext.Response.GetHeader("Location"));
        Assert.AreEqual("test", httpContext.Response.GetHeader("Server"));
    }

    [TestMethod]
    public void Test_SetResponseHeader()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);


        Assert.AreEqual(0, ResponseUtils.SetResponseHeader(httpContext.Response, "xxx", null));
        Assert.AreEqual(0, ResponseUtils.SetResponseHeader(httpContext.Response, "xxx", ""));

        Assert.AreEqual(1, ResponseUtils.SetResponseHeader(httpContext.Response, "x-x1", "xxxxxxxxxx"));
        Assert.AreEqual(-1, ResponseUtils.SetResponseHeader(httpContext.Response, null, "xxxxxxxxxx"));
    }

    [TestMethod]
    public void Test_SetResponseHeaders()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);


        Assert.AreEqual(0, ResponseUtils.SetResponseHeaders(httpContext.Response, "xxx", null));
        Assert.AreEqual(0, ResponseUtils.SetResponseHeaders(httpContext.Response, "xxx", new string[0]));

        Assert.AreEqual(1, ResponseUtils.SetResponseHeaders(httpContext.Response, "x-x1", new string[] { "xxxxxxxxxx" }));
        Assert.AreEqual(-1, ResponseUtils.SetResponseHeaders(httpContext.Response, null, new string[] { "xxxxxxxxxx" }));
    }

}
#endif
