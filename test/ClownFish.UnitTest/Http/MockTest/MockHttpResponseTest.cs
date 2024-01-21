using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.MockTest;
[TestClass]
public class MockHttpResponseTest
{
    [TestMethod]
    public void Test1()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.IsNotNull(mock.HttpContext.Response);
            Assert.IsNotNull(mock.HttpContext.Response.OutputStream);

            MockHttpResponse response = (MockHttpResponse)mock.HttpContext.Response;
            Assert.IsNotNull(response.OutCookies);
            Assert.IsNotNull(response.OutHeaders);
            Assert.IsNull(response.ContentEncoding);
            Assert.IsNull(response.OriginalHttpResponse);

            Assert.IsFalse(response.HasStarted);
            Assert.AreEqual(0, response.StatusCode);

            Assert.AreEqual(string.Empty, response.GetResponseAsText());

            MyAssert.IsError<ArgumentNullException>(() => {
                response.SetHeader("", "xx", false);
            });

            response.SetHeader("x-aa", "aaaaaaaaaaa", true);
            response.SetHeaders("x-bb", new string[] { "b1", "b2"}, true);

            response.SetHeader("x-cc", "ccccccccc", true);
            response.RemoveHeader("x-cc");

            Assert.AreEqual(2, response.GetAllHeaders().Count());

            response.ClearHeaders();
            Assert.AreEqual(0, response.GetAllHeaders().Count());

            response.Write("");
            response.Write(Empty.Array<byte>());
            Assert.IsFalse(response.HasStarted);
            Assert.AreEqual(0, response.StatusCode);

            response.Write("111");
            Assert.IsTrue(response.HasStarted);

            response.Write("_222".ToUtf8Bytes());
            response.WriteAsync("_333").GetAwaiter().GetResult();
            response.WriteAsync("_444".ToUtf8Bytes()).GetAwaiter().GetResult();
            Assert.AreEqual("111_222_333_444", response.GetResponseAsText());

            response.WriteAll("_555".ToUtf8Bytes());
            response.WriteAllAsync("_666".ToUtf8Bytes()).GetAwaiter().GetResult();
            Assert.AreEqual("111_222_333_444_555_666", response.GetResponseAsText());
        }
    }

}
