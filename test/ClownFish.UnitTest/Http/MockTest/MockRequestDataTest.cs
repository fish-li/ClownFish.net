using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Http.Pipleline.Test;

namespace ClownFish.UnitTest.Http.MockTest;
[TestClass]
public class MockRequestDataTest
{
    [TestMethod]
    public void Test1()
    {
        MockRequestData requestData = HttpTest1.GetRequestData();

        Assert.AreEqual("POST", requestData.HttpMethod);
        Assert.AreEqual("www.abc.com", requestData.Url.Host);
        Assert.AreEqual(14752, requestData.Url.Port);
        Assert.AreEqual(3, requestData.Headers.Count);
        Assert.AreEqual("application/json", requestData.GetHeader("Content-Type"));
        Assert.IsNotNull(requestData.Body);
        Assert.IsNotNull(requestData.InputStream);
        Console.WriteLine(requestData.ToText());
    }

    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = MockRequestData.FromText("");
        });

        Exception ex1 = MyAssert.IsError<FormatException>(() => {
            _ = MockRequestData.FromText("GET http://www.abc.com:14752/aaa/bb/ccc.aspx");
        });
        Assert.AreEqual("开始行格式不正确!", ex1.Message);

        Exception ex2 = MyAssert.IsError<FormatException>(() => {
            string request = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx HTTP/1.1
Content-Type text/palin".Trim();
            _ = MockRequestData.FromText(request);
        });
        Assert.AreEqual("请求头格式不正确!", ex2.Message);
    }
}
