using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.Pipleline;
[TestClass]
public class NHttpResponseTest
{
    [TestMethod]
    public void Test_SetCookie()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        httpContext.Response.SetCookie("name1", "123456", TimeSpan.FromHours(5));

        MockHttpResponse resp = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(1, resp.OutCookies.Count);
    }

    [TestMethod]
    public void Test_Error()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        MyAssert.IsError<ArgumentNullException>(() => {
            httpContext.Response.AccessHeaders(null);
        });
    }

    [TestMethod]
    public void Test_SetOrUpdateHeader()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        httpContext.Response.SetHeader("name1", "11111");
        httpContext.Response.SetOrUpdateHeader("name1", "2222");

        MockHttpResponse resp = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(1, resp.OutHeaders.Count);
        Assert.AreEqual("name1", resp.OutHeaders.AllKeys.First());
    }

    [TestMethod]
    public void Test_SetCacheControl()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();
        MockHttpContext httpContext = new MockHttpContext(requestData);

        httpContext.Response.SetCacheControl(TimeSpan.FromSeconds(1));
        httpContext.Response.SetCacheControl(TimeSpan.FromHours(5));

        MockHttpResponse resp = (MockHttpResponse)httpContext.Response;
        Assert.AreEqual(1, resp.OutHeaders.Count);
        Assert.AreEqual("Cache-Control", resp.OutHeaders.AllKeys.First());
    }
}
