using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Proxy;

namespace ClownFish.UnitTest.Http.Proxy;

#if NETCOREAPP

using ClownFish.WebClient.V2;

[TestClass]
public class HttpProxyHandler2Test
{
    [TestMethod]
    public void Test_CreateRequestBody_GET()
    {
        MockRequestData reqdata = MockRequestData.FromText("GET http://www.abc.com/test1 HTTP/1.1");
        NHttpContext httpContext = new MockHttpContext(reqdata);

        HttpProxyHandler2 handler = new HttpProxyHandler2("http://www.xyz.com/test1");
        HttpContent body = handler.CreateRequestBody(httpContext.Request);

        Assert.IsInstanceOfType(body, typeof(ByteArrayContent));
        Assert.AreEqual(0, body.ReadAsStream().Length);
    }

    [TestMethod]
    public void Test_CreateRequestBody_POST()
    {
        MockRequestData reqdata = MockRequestData.FromText(@"
POST http://www.abc.com/test1 HTTP/1.1
Content-Type: application/json
Content-Length: 666

{""a"": 2, ""b"": 3}
".Trim());

        reqdata.SetInputStream(new MemoryStream(reqdata.Body, false));
        reqdata.InputStream.Seek(0, SeekOrigin.End);  // 强行指出尾部

        NHttpContext httpContext = new MockHttpContext(reqdata);

        HttpProxyHandler2 handler = new HttpProxyHandler2("http://www.xyz.com/test1");
        HttpContent body = handler.CreateRequestBody(httpContext.Request);

        Assert.IsInstanceOfType(body, typeof(StreamContent));
        Assert.AreEqual(16, body.ReadAsStream().Length);
        Assert.AreEqual(666, body.Headers.ContentLength);
    }


    [TestMethod]
    public void Test_CreateRequest()
    {
        MockRequestData reqdata = MockRequestData.FromText(@"
POST https://localpc:5025/api/ds/query?xx=2 HTTP/1.1
Host: localpc:5025
Connection: keep-alive
Accept: application/json, text/plain, */*
x-grafana-org-id: 1
User-Agent: Mozilla/5.0 Chrome/121.0.0.0 Safari/537.36 Edg/121.0.0.0
Content-Type: application/json; charset=utf-8
Origin: http://localpc:5025
Referer: http://localpc:5025/d/6581e46e4e5c7ba40a07646395ef7b25/podjian-kong?theme=light
Accept-Encoding: gzip, deflate
Accept-Language: zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7
Cookie: grafana_session=a9fa11c9c11b27e1951a95746995aa0c

{""a"": 2, ""b"": 3}
".Trim());

        NHttpContext httpContext = new MockHttpContext(reqdata);

        string destUrl = "http://www.xyz.com/test1";
        HttpProxyHandler2 handler = new HttpProxyHandler2(destUrl);

        HttpRequestMessage requestMessage = handler.CreateRequest(httpContext.Request, new Uri(destUrl));

        Assert.AreEqual(HttpMethod.Post, requestMessage.Method);
        Assert.AreEqual(destUrl, requestMessage.RequestUri.ToString());

        Assert.IsNull(requestMessage.GetHeader("Host"));  // 已忽略
        Assert.AreEqual("Keep-Alive", requestMessage.GetHeader("Connection"));
        Assert.AreEqual("application/json, text/plain, */*", requestMessage.GetHeader("Accept"));
        Assert.AreEqual("1", requestMessage.GetHeader("x-grafana-org-id"));
        Assert.AreEqual("Mozilla/5.0 Chrome/121.0.0.0 Safari/537.36 Edg/121.0.0.0", requestMessage.Headers.UserAgent.ToString());
        Assert.AreEqual("application/json; charset=utf-8", requestMessage.GetHeader("Content-Type"));
        Assert.AreEqual("http://www.xyz.com", requestMessage.GetHeader("Origin"));
        Assert.AreEqual("http://www.xyz.com/d/6581e46e4e5c7ba40a07646395ef7b25/podjian-kong?theme=light", requestMessage.GetHeader("Referer"));
        Assert.AreEqual("gzip, deflate", requestMessage.GetHeader("Accept-Encoding"));
        Assert.AreEqual("zh-CN, zh; q=0.9, en-US; q=0.8, en; q=0.7", requestMessage.GetHeader("Accept-Language"));
        Assert.AreEqual("grafana_session=a9fa11c9c11b27e1951a95746995aa0c", requestMessage.GetHeader("Cookie"));

        Assert.AreEqual("https", requestMessage.GetHeader("X-Forwarded-Proto"));
        Assert.AreEqual("localpc:5025", requestMessage.GetHeader("X-Forwarded-Host"));
        Assert.AreEqual("https://localpc:5025/api/ds/query", requestMessage.GetHeader("X-CfProxy-OrgUrl"));
    }

    //[TestMethod]
    //public void Test_CopyResponseHeaders()
    //{

    //}

    //[TestMethod]
    //public async Task Test_CopyResponseBodyAsync()
    //{
    //    await Task.CompletedTask;
    //}

    [TestMethod]
    public async Task Test_CopyResponseAsync()
    {
        MockRequestData reqdata = MockRequestData.FromText("GET http://www.abc.com/test1 HTTP/1.1");
        NHttpContext httpContext = new MockHttpContext(reqdata);

        HttpResponseMessage responseMessage = new HttpResponseMessage();
        responseMessage.StatusCode = HttpStatusCode.OK;
        responseMessage.Content = HttpObjectUtils.CreateRequestMessageBody2(SerializeFormat.Json, @"{""a"": 2, ""b"": 3}".ToUtf8Bytes());
        responseMessage.Headers.Add("Connection", "keep-alive");
        responseMessage.Content.Headers.Add("Content-Encoding", "");
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

        HttpProxyHandler2 handler = new HttpProxyHandler2("http://www.xyz.com/test1");
        await handler.CopyResponseAsync(responseMessage, httpContext.Response);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(200, response.StatusCode);
        Assert.AreEqual("application/json; charset=utf-8", response.GetHeader("Content-Type"));
        Assert.AreEqual("application/json; charset=utf-8", response.ContentType);
        Assert.AreEqual("keep-alive", response.GetHeader("Connection"));
        Assert.AreEqual(null, response.GetHeader("Content-Encoding"));
        Assert.AreEqual("no-cache", response.GetHeader("Pragma"));
        Assert.AreEqual("Accept-Encoding", response.GetHeader("Vary"));
        Assert.AreEqual("nosniff", response.GetHeader("X-Content-Type-Options"));
        Assert.AreEqual("1; mode=block", response.GetHeader("X-XSS-Protection"));
        Assert.AreEqual("max-age=15724800; includeSubDomains", response.GetHeader("Strict-Transport-Security"));
        Assert.AreEqual("frame-ancestors *.aaa.com *.bbb.com.cn *.ccc.com", response.GetHeader("Content-Security-Policy"));
        Assert.AreEqual("no-cache", response.GetHeader("Cache-Control"));
        Assert.AreEqual("Thu, 07 Mar 2024 06:38:26 GMT", response.GetHeader("Date"));
        Assert.AreEqual("aaaa", response.GetHeader("x-name1"));
    }


    [TestMethod]
    public async Task Test_CopyResponseAsync_Error()
    {
        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            
            MockRequestData reqdata = MockRequestData.FromText("GET http://www.abc.com/test1 HTTP/1.1");
            NHttpContext httpContext = new MockHttpContext(reqdata);
            HttpResponseMessage responseMessage = null;

            HttpProxyHandler2 handler = new HttpProxyHandler2("http://www.xyz.com/test1");
            await handler.CopyResponseAsync(responseMessage, httpContext.Response);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {

            NHttpResponse httpResponse = null;
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            HttpProxyHandler2 handler = new HttpProxyHandler2("http://www.xyz.com/test1");
            await handler.CopyResponseAsync(responseMessage, httpResponse);
        });


        HttpProxyHandler2 handler = new HttpProxyHandler2("http://www.xyz.com/test1");
        Assert.IsNull(handler.Request);
        Assert.IsNull(handler.Response);
    }


    [TestMethod]
    public async Task Test_CopyResponseAsync_204()
    {
        MockRequestData reqdata = MockRequestData.FromText("GET http://www.abc.com/test1 HTTP/1.1");
        NHttpContext httpContext = new MockHttpContext(reqdata);

        HttpResponseMessage responseMessage = new HttpResponseMessage();
        responseMessage.StatusCode = HttpStatusCode.NoContent;

        HttpProxyHandler2 handler = new HttpProxyHandler2("http://www.xyz.com/test1");
        await handler.CopyResponseAsync(responseMessage, httpContext.Response);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);

        Assert.IsFalse(response.HasStarted);   // 没有执行 write 操作
    }

}

#endif
