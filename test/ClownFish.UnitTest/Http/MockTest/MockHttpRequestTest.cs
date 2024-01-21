using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.MockTest;
[TestClass]
public class MockHttpRequestTest
{
    [TestMethod]
    public void Test1()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.IsNull(mock.HttpContext.Request.OriginalHttpRequest);

            NHttpRequest request = mock.HttpContext.Request;

            Assert.AreEqual("POST", request.HttpMethod);
            Assert.AreEqual("http://www.abc.com:14752", request.RootUrl);
            Assert.AreEqual("/aaa/bb/ccc.aspx", request.Path);
            Assert.AreEqual("?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3", request.Query);
            Assert.AreEqual("/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3", request.RawUrl);
            Assert.AreEqual("http://www.abc.com:14752/aaa/bb/ccc.aspx", request.FullPath);
            Assert.AreEqual("http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3", request.FullUrl);
            Assert.AreEqual("application/json", request.ContentType);
            Assert.IsNull(request.UserAgent);
            Assert.AreEqual(false, request.IsHttps);
            Assert.AreEqual(false, request.HttpContext.IsAuthenticated);
            Assert.AreEqual(true, request.HasBody);
            Assert.AreEqual(true, request.LogRequestBody);

            Assert.AreEqual(3, request.HeaderKeys.Length);
            Assert.IsNotNull(request.Header("Content-Type"));
            Assert.IsNotNull(request.Header("x-client-app"));
            Assert.IsNotNull(request.Header("Cookie"));
            Assert.AreEqual("application/json", request.Header("Content-Type"));
            Assert.AreEqual("HttpTest1", request.Header("x-client-app"));

            Assert.AreEqual(2, request.CookieKeys.Length);
            Assert.IsNotNull(request.Cookie("c1"));
            Assert.IsNotNull(request.Cookie("c2"));
            Assert.AreEqual("1111111", request.Cookie("c1"));
            Assert.AreEqual("22222222", request.Cookie("c2"));

            Assert.AreEqual(2, request.QueryStringKeys.Length);
            Assert.IsNotNull(request.QueryString("tenantId"));
            Assert.IsNotNull(request.QueryString("checkType"));
            Assert.AreEqual("my57972739adc90", request.QueryString("tenantId"));
            Assert.AreEqual("系统应用水平", request.QueryString("checkType"));

            Console.WriteLine(request.GetBodyText());

        }
    }
}
