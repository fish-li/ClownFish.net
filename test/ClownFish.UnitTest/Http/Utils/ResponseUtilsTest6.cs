#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;
using ClownFish.UnitTest.Http.Mock;
using ClownFish.UnitTest.Http.Pipleline.Test;
using ClownFish.UnitTest.WebClient;
using ClownFish.WebClient.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class ResponseUtilsTest6
{
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
        Assert.AreEqual("application/json", contentType);

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

        string[] values = responseMessage.GetHeaderValues(HttpHeaders.Response.ContentType);
        Assert.AreEqual(1, values.Length);
        Assert.AreEqual("application/json", values[0]);


        string[] values2 = responseMessage.GetHeaderValues("x-a");
        Assert.AreEqual(1, values2.Length);
        Assert.AreEqual("aaa", values2[0]);


        Assert.IsNull(ResponseUtils.GetHeaderValues(responseMessage, "xxxx"));

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ResponseUtils.GetHeaderValues(null,  "xx");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ResponseUtils.GetHeaderValues(responseMessage, null);
        });
    }


    [TestMethod]
    public void Test_CopyResponseHeaders()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        Assert.AreEqual(0, ResponseUtils.CopyResponseHeaders((NameValueCollection)null, httpContext.Response));
        Assert.AreEqual(0, ResponseUtils.CopyResponseHeaders(new NameValueCollection(), httpContext.Response));

        NameValueCollection headers = new NameValueCollection();
        headers.Set("x-a", "aaa");
        headers.Set("x-b", "bbb");
        headers.Set(HttpHeaders.Response.ContentType, "application/json");
        headers.Set("Server", "test");

        int count = ResponseUtils.CopyResponseHeaders(headers, httpContext.Response);
        Assert.AreEqual(3, count);

        bool flagA = false, flagB = false;

        foreach( var x in httpContext.Response.GetAllHeaders() ) {
            if( x.Key == "x-a" && x.Value.First() == "aaa" )
                flagA = true;

            if( x.Key == "x-b" && x.Value.First() == "bbb" )
                flagB = true;
        }

        Assert.AreEqual(true, flagA);
        Assert.AreEqual(true, flagB);
        Assert.AreEqual("application/json", httpContext.Response.ContentType);
    }

    [TestMethod]
    public void Test_SetResponseHeader()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);


        Assert.AreEqual(0, ResponseUtils.SetResponseHeader(httpContext.Response, "xxx", null));
        Assert.AreEqual(0, ResponseUtils.SetResponseHeader(httpContext.Response, "xxx", new string[0]));

        Assert.AreEqual(1, ResponseUtils.SetResponseHeader(httpContext.Response, "x-x1", new string[] { "xxxxxxxxxx" }));
        Assert.AreEqual(-1, ResponseUtils.SetResponseHeader(httpContext.Response, null, new string[] { "xxxxxxxxxx" }));
    }

}
#endif
