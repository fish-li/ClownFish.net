using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

namespace ClownFish.UnitTest.Base.Common;

#if NETCOREAPP

[TestClass]
public class BytesListTest
{
    [TestMethod]
    public void Test_ToArray()
    {
        string input = "中华文明#123456789";
        byte[] bb = input.ToUtf8Bytes();

        BytesList list = new BytesList();
        list.Write(bb.Length);
        list.WriteLn();
        list.Write(bb);
        
        byte[] bytes = list.ToArray();
        Assert.AreEqual(4 + 1 + bb.Length, bytes.Length);

        Span<byte> span = bytes;
        int len = BitConverter.ToInt32(span.Slice(0, 4));
        Assert.AreEqual(bb.Length, len);

        string text = Encoding.UTF8.GetString(span.Slice(5));
        Assert.AreEqual(input, text);

        byte[] bb2 = list.ToArray();
        Assert.AreEqual(0, bb2.Length);
    }

    [TestMethod]
    public void Test_CopyToStream()
    {
        string input = "中华文明#123456789";
        byte[] bb = input.ToUtf8Bytes();

        BytesList list = new BytesList();
        list.Write(bb.Length);
        list.WriteLn();
        list.Write(bb);

        MemoryStream ms = new MemoryStream();
        int len = list.CopyToStream(ms);
        Assert.AreEqual(4 + 1 + bb.Length, len);

        MemoryStream ms2 = new MemoryStream();
        int len2 = list.CopyToStream(ms2);
        Assert.AreEqual(0, len2);

        Assert.AreEqual(-1, list.CopyToStream((MemoryStream)null));
    }


    [TestMethod]
    public void Test_ToGzip()
    {
        string input = "中华文明#123456789";
        byte[] bb = input.ToUtf8Bytes();

        BytesList list = new BytesList();

        list.Write((byte[])null);
        list.Write(Empty.Array<byte>());

        byte[] emptyGzipBytes = list.ToGzip();
        Assert.AreEqual(0, emptyGzipBytes.Length);

        list.Write(bb.Length);
        list.WriteLn();
        list.Write(bb);

        byte[] gzipBytes = list.ToGzip();

        byte[] bb2 = list.ToArray();
        Assert.AreEqual(0, bb2.Length);

        byte[] bb3 = gzipBytes.UnGzip();

        Span<byte> span = bb3;
        int len = BitConverter.ToInt32(span.Slice(0, 4));
        Assert.AreEqual(bb.Length, len);

        string text = Encoding.UTF8.GetString(span.Slice(5));
        Assert.AreEqual(input, text);
    }
}

#endif
