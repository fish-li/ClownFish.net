using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.MockTest;
[TestClass]
public class MockHttpContextTest
{
    [TestMethod]
    public void Test1()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();

        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.IsNull(mock.HttpContext.OriginalHttpContext);
            Assert.IsNull(mock.HttpContext.User);

            Assert.IsNotNull(mock.HttpContext.Request);
            Assert.IsNotNull(mock.HttpContext.MRequest);
            Assert.IsNotNull(mock.HttpContext.Response);

            Assert.IsNull(mock.HttpContext.GetFieldValue("_items"));
            Assert.IsNotNull(mock.HttpContext.Items);

            Assert.IsFalse(mock.HttpContext.SkipAuthorization);
            mock.HttpContext.SkipAuthorization = true;
            Assert.IsTrue(mock.HttpContext.SkipAuthorization);
        }
    }

    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MockRequestData requestData = null;
            _ = new MockHttpContext(requestData);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            MockRequestData requestData = null;
            _ = new MockHttpPipeline(requestData);
        });
    }
}
