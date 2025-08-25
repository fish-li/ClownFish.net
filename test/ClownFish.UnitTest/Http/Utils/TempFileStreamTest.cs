using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class TempFileStreamTest
{
    [TestMethod]
    public async Task Test_1()
    {
        TempFileStream stream = new TempFileStream();

        Assert.IsTrue(stream.CanRead);
        Assert.IsTrue(stream.CanSeek);
        Assert.IsTrue(stream.CanWrite);
        Assert.AreEqual(0L, stream.Length);
        Assert.AreEqual(0L, stream.Position);

        stream.Flush();
        await stream.FlushAsync();

        // ---------------------------------------------------------
        byte[] data1 = Guid.NewGuid().ToByteArray();
        stream.Write(data1, 0, data1.Length);

        stream.Position = 0;
        byte[] buffer1 = new byte[100];
        int count1 = stream.Read(buffer1, 0, buffer1.Length);
        Assert.AreEqual(data1.Length, count1);
        Assert.AreEqual(stream.Position, count1);

        for( int i = 0; i < count1; i++ ) {
            Assert.AreEqual(data1[i], buffer1[i]);
        }

        stream.SetLength(0);

        // ---------------------------------------------------------
        byte[] data2 = "中华文明".ToUtf8Bytes();
        await stream.WriteAsync(data2, 0, data2.Length);

        stream.Seek(0, SeekOrigin.Begin);
        byte[] buffer2 = new byte[100];
        int count2 = await stream.ReadAsync(buffer2, 0, buffer2.Length);
        Assert.AreEqual(data2.Length, count2);

        for( int i = 0; i < count2; i++ ) {
            Assert.AreEqual(data2[i], buffer2[i]);
        }

        stream.SetLength(0);

        // ---------------------------------------------------------
        byte b1 = (byte)23;
        stream.WriteByte(b1);

        stream.Position = 0;
        int b2 = stream.ReadByte();
        Assert.AreEqual(23, b2);

        stream.SetLength(0);

#if NETCOREAPP
        // ---------------------------------------------------------
        byte[] bytes3 = "中文汉字12345".ToUtf8Bytes();
        ReadOnlyMemory<byte> data3 = bytes3;
        await stream.WriteAsync(data3);

        stream.Seek(0, SeekOrigin.Begin);
        byte[] bufferBytes3 = new byte[100];
        Memory<byte> buffer3 = bufferBytes3;
        int count3 = await stream.ReadAsync(buffer3);
        Assert.AreEqual(data3.Length, count3);

        for( int i = 0; i < count3; i++ ) {
            Assert.AreEqual(bytes3[i], bufferBytes3[i]);
        }

        //stream.SetLength(0);

        // ---------------------------------------------------------

        MemoryStream ms1 = new MemoryStream();
        MemoryStream ms2 = new MemoryStream();

        stream.Position = 0;
        stream.CopyTo(ms1);

        stream.Position = 0;
        await stream.CopyToAsync(ms2);

        Assert.AreEqual(ms1.Length, count3);
        Assert.AreEqual(ms2.Length, count3);

#endif

    }


    [TestMethod]
    public void Test_Error()
    {
        TempFileStream stream = new TempFileStream();

        string filePath = stream.FilePath;
        Assert.IsTrue(File.Exists(filePath));

        byte[] data1 = Guid.NewGuid().ToByteArray();
        stream.Write(data1, 0, data1.Length);

        stream.Dispose();

        Assert.IsFalse(File.Exists(filePath));

        MyAssert.IsError<ObjectDisposedException>(() => {
            stream.Position = 0;
        });

        MyAssert.IsError<ObjectDisposedException>(() => {
            byte[] buffer1 = new byte[100];
            int count1 = stream.Read(buffer1, 0, buffer1.Length);
        });

    }
}

