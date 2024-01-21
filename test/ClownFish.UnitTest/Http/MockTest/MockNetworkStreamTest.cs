using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.MockTest;
[TestClass]
public class MockNetworkStreamTest
{
    [TestMethod]
    public void Test1()
    {
        byte[] data = "1d46dd22e94d46f29bbc6c3191da5e33".ToUtf8Bytes();
        MockNetworkStream stream = new MockNetworkStream(data);

        Assert.IsTrue(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsFalse(stream.CanWrite);

        MyAssert.IsError<NotImplementedException>(() => {
            bool flag = stream.Length > 0;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            bool flag = stream.Position > 0;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            stream.Position = 0;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            stream.Seek(0, SeekOrigin.Begin);
        });

        MyAssert.IsError<NotImplementedException>(() => {
            stream.SetLength(0);
        });

        MyAssert.IsError<NotImplementedException>(() => {
            byte[] bb = "aaaa".ToUtf8Bytes();
            stream.Write(bb, 0, bb.Length);
        });

        stream.Flush();

        byte[] data2 = new byte[1];
        int len = stream.Read(data2, 0, data2.Length);
        Assert.AreEqual(1, len);
    }

    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            byte[] data = null;
            _ = new MockNetworkStream(data);
        });
    }
}
