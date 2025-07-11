using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Void = ClownFish.Base.Void;

namespace ClownFish.UnitTest.MQ.Messages;

#if NETCOREAPP
[TestClass]
public class MessageBinSerializerTest
{
    [TestMethod]
    public void Test_Serialize()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MessageBinSerializer.Instance.Serialize(null);
        });

        string s1 = "中文汉字最优秀！";
        byte[] b1 = MessageBinSerializer.Instance.Serialize(s1);
        Assert.AreEqual(s1, b1.ToUtf8String());

        ReadOnlyMemory<byte> mem = s1.ToUtf8Bytes();
        byte[] b2 = MessageBinSerializer.Instance.Serialize(mem);
        Assert.AreEqual(s1, b2.ToUtf8String());

        byte[] b3 = MessageBinSerializer.Instance.Serialize(b1);
        Assert.IsTrue(object.ReferenceEquals(b1, b3));

        NameInt64 x1 = new NameInt64("x1", 123);
        byte[] b4 = MessageBinSerializer.Instance.Serialize(x1);
        NameInt64 x2 = b4.ToUtf8String().FromJson<NameInt64>();
        Assert.AreEqual("x1", x2.Name);
        Assert.AreEqual(123, x2.Value);

        // 还有一些场景，例如：NHttpRequest,  IBinarySerializer 在其它测试用例中已覆盖，这里忽略
    }

    [TestMethod]
    public void Test_Deserialize()
    {
        Assert.IsNull(MessageBinSerializer.Instance.Deserialize<string>(Empty.Array<byte>()));

        string s1 = "中文汉字最优秀！";
        byte[] bb = s1.ToUtf8Bytes();
        ReadOnlyMemory<byte> mem = bb;

        MyAssert.IsError<NotSupportedException>(() => { 
          _ = MessageBinSerializer.Instance.Deserialize<ReadOnlyMemory<byte>>(mem);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            _ = MessageBinSerializer.Instance.Deserialize<byte[]>(mem);
        });


        string s2 = MessageBinSerializer.Instance.Deserialize<string>(mem);
        Assert.AreEqual(s1, s2);


        byte[] bb2 = new NameInt64("x1", 123).ToJson().ToUtf8Bytes();
        ReadOnlyMemory<byte> mem4 = bb2;
        NameInt64 x2 = MessageBinSerializer.Instance.Deserialize<NameInt64>(mem4);
        Assert.AreEqual("x1", x2.Name);
        Assert.AreEqual(123, x2.Value);

        // 还有一些场景，例如：NHttpRequest,  IBinarySerializer 在其它测试用例中已覆盖，这里忽略
    }

    [TestMethod]
    public void Test_SimpleType()
    {
        byte[] b1 = MessageBinSerializer.Instance.Serialize(53L);
        long x1 = MessageBinSerializer.Instance.Deserialize<long>(b1);
        Assert.AreEqual(53L, x1);
    }


    [TestMethod]
    public void Test_NullObject()
    {
        ReadOnlyMemory<byte> mm = Guid.NewGuid().ToByteArray();

        Assert.IsTrue(object.ReferenceEquals(Void.Value, MessageBinSerializer.Instance.Deserialize<Void>(mm)));
    }
}


#endif
