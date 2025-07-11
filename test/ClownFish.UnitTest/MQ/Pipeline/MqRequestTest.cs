using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ClownFish.UnitTest.MQ.Pipeline;

#if NETCOREAPP
[TestClass]
public class MqRequestTest
{
    [TestMethod]
    public void Test1()
    {
        string text = "中华文明5000年!";

        MqRequest req = new MqRequest {
            MqKind = "rabbit",
            Original = new object(),
            Body = text.ToUtf8Bytes(),
            MessageObject = text
        };

        Assert.AreEqual(24, req.MessageId.Length);
        Assert.AreEqual("rabbit", req.MqKind);
        Assert.IsNotNull(req.Original);
        Assert.AreEqual(text, (string)req.MessageObject);


        string logtext = (req as ILoggingObject).GetLogText();
        Assert.AreEqual(text, logtext);
    }

    [TestMethod]
    public void Test2()
    {
        string text = new string('a', 6000);

        MqRequest req = new MqRequest {
            MqKind = "rabbit",
            Original = new object(),
            Body = text.ToUtf8Bytes(),
            MessageObject = text
        };

        string logtext = (req as ILoggingObject).GetLogText();
        Assert.AreEqual(LoggingLimit.HttpBodyMaxLen, logtext.Length);
    }

    [TestMethod]
    public void Test3()
    {
        NameInt64 item = new NameInt64("abc", 123);

        MqRequest req = new MqRequest {
            MqKind = "mmq",
            MessageObject = item
        };

        string logtext = (req as ILoggingObject).GetLogText();
        string json = item.ToJson();
        Assert.AreEqual(json, logtext);
    }


    [TestMethod]
    public void Test4()
    {
        byte[] bb = Guid.NewGuid().ToByteArray();

        MqRequest req = new MqRequest {
            MqKind = "rabbit",
            Body = bb,
        };

        string logtext = (req as ILoggingObject).GetLogText();   // 得到一个 “乱码” 字符串
        Assert.IsTrue(logtext.Length > 0);
    }
}
#endif
