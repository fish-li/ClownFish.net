using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class TransparentOutStreamTest
{
    [TestMethod]
    public async Task Test_1()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
          _ = new TransparentOutStream((Stream)null);
        });


        MemoryStream ms = new MemoryStream();
        TransparentOutStream stream = new TransparentOutStream(ms);

        Assert.AreEqual(0L, stream.GetOutSize());
        Assert.IsFalse(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsTrue(stream.CanWrite);

        MyAssert.IsError<NotImplementedException>(() => {
            _ = stream.Length;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            _ = stream.Position;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            stream.Position = 0;
        });        

        MyAssert.IsError<NotImplementedException>(() => {
            byte[] buffer = new byte[4];
            _ = stream.Read(buffer, 0, buffer.Length);
        });

        MyAssert.IsError<NotImplementedException>(() => {
            _ = stream.Seek(2, SeekOrigin.Begin);
        });

        MyAssert.IsError<NotImplementedException>(() => {
            stream.SetLength(0);
        });

        stream.Flush();

        byte[] data1 = Guid.NewGuid().ToByteArray();
        stream.Write(data1, 0, data1.Length);

        byte[] data2 = "中华文明".ToUtf8Bytes();
        await stream.WriteAsync(data2, 0, data2.Length);

        byte b1 = (byte)23;
        stream.WriteByte(b1);

        long len1 = data1.Length + data2.Length + 1;
        Assert.AreEqual(len1, stream.GetOutSize());
        Assert.AreEqual(len1, ms.ToArray().Length);

        await stream.FlushAsync();

#if NETCOREAPP
        byte[] data3 = new string('a', 100).ToUtf8Bytes();
        ReadOnlySpan<byte> buffer1 = data3;
        stream.Write(buffer1);


        byte[] data4 = new string('b', 200).ToUtf8Bytes();
        ReadOnlyMemory<byte> buffer2 = data4;
        await stream.WriteAsync(buffer2, CancellationToken.None);


        long len2 = data1.Length + data2.Length + 1 + 100 + 200;
        Assert.AreEqual(len2, stream.GetOutSize());
        Assert.AreEqual(len2, ms.ToArray().Length);
#endif

    }
}
