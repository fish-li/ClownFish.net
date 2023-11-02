using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Http.Mock;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class HttpContextExtensionsTest
{
    [TestMethod]
    public void Test_HttpReply()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        httpContext.HttpReply("abc");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(200, response.StatusCode);
        Assert.AreEqual("abc", response.GetResponseAsText());
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);


        MyAssert.IsError<ArgumentNullException>(() => {
            HttpContextExtensions.HttpReply((NHttpContext)null, "abc");
        });
    }

    [TestMethod]
    public void Test_HttpReply_204()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        httpContext.HttpReply("");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);
    }

    [TestMethod]
    public void Test_HttpReply_321()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        httpContext.HttpReply(321, "abc", ResponseContentType.TextUtf8);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(321, response.StatusCode);
        Assert.AreEqual("abc", response.GetResponseAsText());
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);

        MyAssert.IsError<ArgumentNullException>(() => {
            HttpContextExtensions.HttpReply((NHttpContext)null, 321, "abc", "text/plain");
        });
    }

    [TestMethod]
    public void Test_HttpReply_204_2()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        httpContext.HttpReply(500, "", ResponseContentType.TextUtf8);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);
    }


    [TestMethod]
    public async Task Test_HttpReplyAsync()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        await httpContext.HttpReplyAsync("abc");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(200, response.StatusCode);
        Assert.AreEqual("abc", response.GetResponseAsText());
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync((NHttpContext)null, "abc");
        });
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_204()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        await httpContext.HttpReplyAsync("");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_321()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        await httpContext.HttpReplyAsync(321, "abc", ResponseContentType.TextUtf8);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(321, response.StatusCode);
        Assert.AreEqual("abc", response.GetResponseAsText());
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync((NHttpContext)null, 321, "abc", "text/plain");
        });
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_204_2()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        await httpContext.HttpReplyAsync(500, "", ResponseContentType.TextUtf8);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_HttpResult_string()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        NameValueCollection headers = new NameValueCollection();
        headers.Add("Content-Type", ResponseContentType.TextUtf8);

        HttpResult<string> httpResult = new HttpResult<string>(321, headers, "abc");
        await httpContext.HttpReplyAsync(httpResult);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(321, response.StatusCode);
        Assert.AreEqual("abc", response.GetResponseAsText());
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);


        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync(httpContext, (HttpResult<string>)null);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync((MockHttpContext)null, httpResult);
        });
    }




    [TestMethod]
    public async Task Test_Http500Async()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        Exception ex = ExceptionHelper.CreateException();
        await httpContext.Http500Async(ex);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(500, response.StatusCode);
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);


        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.Http500Async(httpContext, (Exception)null);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.Http500Async((MockHttpContext)null, ex);
        });
    }

#if NETCOREAPP

    [TestMethod]
    public async Task Test_HttpReplyAsync_HttpWebResponse()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        HttpWebResponse webResponse = CreateHttpResponseMessage().ToHttpWebResponse();
        await httpContext.HttpReplyAsync(webResponse);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);


        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions6.HttpReplyAsync(httpContext, (HttpWebResponse)null);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions6.HttpReplyAsync((MockHttpContext)null, webResponse);
        });
    }
        

    private HttpResponseMessage CreateHttpResponseMessage()
    {
        HttpResponseMessage responseMessage = new HttpResponseMessage();
        responseMessage.StatusCode = HttpStatusCode.Unauthorized;

        HttpContent content = new ByteArrayContent("abc".ToUtf8Bytes());
        content.Headers.TryAddWithoutValidation(HttpHeaders.Request.ContentType, ResponseContentType.TextUtf8);

        responseMessage.Content = content;
        return responseMessage;
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_HttpResponseMessage()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        HttpResponseMessage responseMessage = CreateHttpResponseMessage();
        await httpContext.HttpReplyAsync(responseMessage);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("text/plain; charset=utf-8", response.ContentType);


        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions6.HttpReplyAsync(httpContext, (HttpResponseMessage)null);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions6.HttpReplyAsync((MockHttpContext)null, responseMessage);
        });
    }
#endif


}
