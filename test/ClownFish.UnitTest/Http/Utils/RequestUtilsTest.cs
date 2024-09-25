#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class RequestUtilsTest
{
    [TestMethod]
    public void Test_SetOptionValue()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            RequestUtils.SetOptionValue((HttpRequestMessage)null, "key", "value");
        });

        HttpRequestMessage requestMessage = new HttpRequestMessage();

        MyAssert.IsError<ArgumentNullException>(() => {
            RequestUtils.SetOptionValue(requestMessage, "", "value");
        });

        requestMessage.SetOptionValue("key1", "bdb0f91aeb4d4dba958b761bb6d34168");
        Assert.AreEqual("bdb0f91aeb4d4dba958b761bb6d34168", requestMessage.GetOptionValue<string>("key1"));
    }

    [TestMethod]
    public void Test_GetOptionValue()
    {
        Assert.IsNull(RequestUtils.GetOptionValue<string>((HttpRequestMessage)null, "key"));

        HttpRequestMessage requestMessage = new HttpRequestMessage();
        Assert.IsNull(requestMessage.GetOptionValue<string>("key1"));

        FieldInfo field = typeof(HttpRequestMessage).GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
        object options1 = field.GetValue(requestMessage);
        Assert.IsNull(options1);

        requestMessage.SetOptionValue("key1", "bdb0f91aeb4d4dba958b761bb6d34168");
        Assert.AreEqual("bdb0f91aeb4d4dba958b761bb6d34168", requestMessage.GetOptionValue<string>("key1"));

        Assert.IsNull(requestMessage.GetOptionValue<HttpOption>("key1"));

    }
}
#endif
