using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Common;

#if NETCOREAPP
[TestClass]
public class BrotliHelperTest
{
    [TestMethod]
    public void Test_Brotli压缩字符串()
    {
        string s = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
";

        string b = BrotliHelper.Compress(s);

        string s2 = BrotliHelper.Decompress(b);

        Assert.AreEqual(s, s2);
    }


    [TestMethod]
    public void Test_Brotli压缩二进制字节()
    {
        string s = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
";
        byte[] bb = Encoding.UTF8.GetBytes(s);

        byte[] b1 = BrotliHelper.Compress(bb);

        Assert.IsTrue(b1.Length < bb.Length);

        byte[] b2 = BrotliHelper.Decompress(b1);

        Assert.AreEqual(bb.Length, b2.Length);

        for( int i = 0; i < bb.Length; i++ ) {
            Assert.AreEqual((int)bb[i], (int)b2[i]);
        }
    }

    [TestMethod]
    public void Test_Args_Null()
    {
        Assert.AreEqual(null, BrotliHelper.Compress((string)null));
        Assert.AreEqual(string.Empty, BrotliHelper.Compress(string.Empty));

        Assert.AreEqual(null, BrotliHelper.Decompress((string)null));
        Assert.AreEqual(string.Empty, BrotliHelper.Decompress(string.Empty));
    }

    [TestMethod]
    public void Test_ToBrotli()
    {
        string text = "!@#中文汉字abcd1234";
        byte[] b1 = BrotliHelper.ToBrotli(text);
        byte[] b2 = BrotliHelper.ToBrotli(text, Encoding.UTF8);
        b1.IsEqual(b2);

        byte[] b3 = BrotliHelper.UnBrotli(b2);
        string text2 = Encoding.UTF8.GetString(b3);
        Assert.AreEqual(text, text2);
    }

    [TestMethod]
    public void Test_NULL()
    {
        Assert.IsNull(BrotliHelper.ToBrotli((string)null));
        Assert.IsTrue(BrotliHelper.ToBrotli("").Length == 0);

        Assert.IsNull(BrotliHelper.ToBrotli((byte[])null));
        Assert.IsTrue(BrotliHelper.ToBrotli(Empty.Array<byte>()).Length == 0);

        Assert.IsNull(BrotliHelper.UnBrotli((byte[])null));
        Assert.IsTrue(BrotliHelper.UnBrotli(Empty.Array<byte>()).Length == 0);
    }


    [TestMethod]
    public void Compare_Gzip_Brotli()
    {
        string text = File.ReadAllText("ClownFish.Log.config", Encoding.UTF8);
        string s1 = GzipHelper.Compress(text);
        string s2 = BrotliHelper.Compress(text);
        Console.WriteLine($"input.Length: {text.Length},  gzip.Length: {s1.Length}, br.Length: {s2.Length}");
    }

}
#endif
