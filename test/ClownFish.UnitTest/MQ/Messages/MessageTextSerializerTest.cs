using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.MQ.Messages;

#if NETCOREAPP
[TestClass]
public class MessageTextSerializerTest
{
    [TestMethod]
    public void Test_Serialize()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MessageTextSerializer.Instance.Serialize(null);
        });


        string s1 = "中文汉字最优秀！";
        string s2 = MessageTextSerializer.Instance.Serialize(s1);
        Assert.IsTrue(object.ReferenceEquals(s1, s2));

        string base64 = MessageTextSerializer.Instance.Serialize(s1.ToUtf8Bytes());
        Assert.AreEqual("5Lit5paH5rGJ5a2X5pyA5LyY56eA77yB", base64);

        NameInt64 x1 = new NameInt64("x1", 123);
        string s3 = MessageTextSerializer.Instance.Serialize(x1);

        NameInt64 x2 = s3.FromJson<NameInt64>();
        Assert.AreEqual("x1", x2.Name);
        Assert.AreEqual(123, x2.Value);

        // 还有一些场景，例如：NHttpRequest,  ITextSerializer 在其它测试用例中已覆盖，这里忽略
    }

    [TestMethod]
    public void Test_Deserialize()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MessageTextSerializer.Instance.Deserialize<NameInt64>(null);
        });

        string s1 = "中文汉字最优秀！";
        string s2 = MessageTextSerializer.Instance.Deserialize<string>(s1);
        Assert.IsTrue(object.ReferenceEquals(s1, s2));

        byte[] bb = MessageTextSerializer.Instance.Deserialize<byte[]>("5Lit5paH5rGJ5a2X5pyA5LyY56eA77yB");
        Assert.AreEqual(s1, bb.ToUtf8String());


        NameInt64 x1 = new NameInt64("x1", 123);
        NameInt64 x2 = MessageTextSerializer.Instance.Deserialize<NameInt64>(x1.ToJson());
        Assert.AreEqual("x1", x2.Name);
        Assert.AreEqual(123, x2.Value);

        // 还有一些场景，例如：NHttpRequest,  ITextSerializer 在其它测试用例中已覆盖，这里忽略
    }
}
#endif
