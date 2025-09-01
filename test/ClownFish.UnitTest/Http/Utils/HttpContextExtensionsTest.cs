using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Base;
using ClownFish.UnitTest.Http.Pipleline.Test;
using ClownFish.UnitTest.WebClient;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class HttpContextExtensionsTest
{
    [TestMethod]
    public void Test_HttpReply()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

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
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

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
    public async Task Test_HttpReplyAsync_bytes()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        byte[] bb = "中华文明_abc".GetBytes();
        await httpContext.HttpReplyAsync(221, bb, "text/abc");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(221, response.StatusCode);
        Assert.AreEqual("中华文明_abc", response.GetResponseAsText());
        Assert.AreEqual("text/abc", response.ContentType);

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync((NHttpContext)null, 200, bb);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync(httpContext, 200, (byte[])null);
        });
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_stream()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        byte[] bb = "中华文明_abc".GetBytes();
        using MemoryStream stream = new MemoryStream(bb);
        await httpContext.HttpReplyAsync(221, stream, "text/abc");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(221, response.StatusCode);
        Assert.AreEqual("中华文明_abc", response.GetResponseAsText());
        Assert.AreEqual("text/abc", response.ContentType);

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync((NHttpContext)null, 200, stream);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync(httpContext, 200, (Stream)null);
        });
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_stream204()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        using MemoryStream stream = new MemoryStream();
        await httpContext.HttpReplyAsync(221, stream, "text/abc");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);
    }

    [TestMethod]
    public async Task Test_HttpReplyAsync_stream_xx()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        Stream stream = new CanntReadStream();
 
        await MyAssert.IsErrorAsync<InvalidOperationException>(async () => {
            await HttpContextExtensions.HttpReplyAsync(httpContext, 200, stream);
        });
    }


    public sealed class CanntReadStream : Stream
    {
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }


    [TestMethod]
    public async Task Test_HttpGzipReplyAsync()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        await httpContext.HttpGzipReplyAsync(221, "中华文明_abc", "text/abc");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(221, response.StatusCode);
        Assert.AreEqual("text/abc", response.ContentType);

        string contentEncoding = response.GetHeader("Content-Encoding");
        Assert.AreEqual("gzip", contentEncoding);

        "中华文明_abc".GetBytes().ToGzip().IsEqual(response.OutputStream.ToArray());
        string bodyText = (new HttpStreamReader(response.OutputStream, contentEncoding)).ReadAllText();
        Assert.AreEqual("中华文明_abc", bodyText);        

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpGzipReplyAsync((NHttpContext)null, 200, "abc");
        });
    }

    [TestMethod]
    public async Task Test_HttpGzipReplyAsync_204()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        await httpContext.HttpGzipReplyAsync(221, "", "text/abc");

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);
    }


    [TestMethod]
    public async Task Test_HttpReplyAsync_204()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

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
    public async Task Test_HttpReplyAsync_HttpResult_bytes()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        NameValueCollection headers = new NameValueCollection();
        headers.Add("Content-Type", ResponseContentType.OctetStream);

        HttpResult<byte[]> httpResult = new HttpResult<byte[]>(321, headers, "abc".ToUtf8Bytes());
        await httpContext.HttpReplyAsync(httpResult);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(321, response.StatusCode);
        Assert.AreEqual("abc", response.GetResponseAsText());
        Assert.AreEqual("application/octet-stream", response.ContentType);


        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpReplyAsync(httpContext, (HttpResult<byte[]>)null);
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


    [TestMethod]
    public async Task Test_HttpGzipNdjsonReply_200()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        List<Product3> list = Product3.CreateTestDataList(100);

        await httpContext.HttpGzipNdjsonReply(list);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(200, response.StatusCode);
        Assert.AreEqual("application/x-ndjson", response.ContentType);

        string contentEncoding = response.GetHeader("Content-Encoding");
        Assert.AreEqual("gzip", contentEncoding);
        Assert.IsTrue(pipelineContext.OprLog.OutSize > 0);

        byte[] body = response.OutputStream.ToArray();
        string bodyText = body.UnGzip().ToUtf8String();
        
        string bodyText2 = (new HttpStreamReader(response.OutputStream, contentEncoding)).ReadAllText();

        string inputText = list.ToMultiLineJson();

        Assert.AreEqual(inputText, bodyText);
        Assert.AreEqual(inputText, bodyText2);

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            await HttpContextExtensions.HttpGzipNdjsonReply((NHttpContext)null, list);
        });
    }

    [TestMethod]
    public async Task Test_HttpGzipNdjsonReply_204()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);
        using HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext);

        List<Product3> list = null;

        await httpContext.HttpGzipNdjsonReply(list);

        MockHttpResponse response = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(204, response.StatusCode);
    }

}
