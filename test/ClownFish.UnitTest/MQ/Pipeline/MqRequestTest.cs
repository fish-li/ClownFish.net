using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.MQ.Pipeline;

#if NETCOREAPP
[TestClass]
public class MqRequestTest
{
    [TestMethod]
    public void Test1()
    {
        MqRequest req = new MqRequest {
            MqKind = "rabbit",
            Original = new object(),
            Body = "ea8f5c47948b486d9d731c004bf8d6f6".ToUtf8Bytes(),
            MessageObject = null
        };

        Assert.AreEqual(49, req.MessageId.Length);
        Assert.AreEqual("rabbit", req.MqKind);
        Assert.IsNotNull(req.Original);
        Assert.IsNull(req.MessageObject);
        Assert.AreEqual(32, req.BodyLen);
    }
}
#endif
