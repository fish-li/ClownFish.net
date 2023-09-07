namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class GzipHelperTest
{
    [TestMethod]
    public void Test_Gzip压缩字符串()
    {
        string s = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
";

        string b = GzipHelper.Compress(s);

        string s2 = GzipHelper.Decompress(b);

        Assert.AreEqual(s, s2);
    }


    [TestMethod]
    public void Test_Gzip压缩二进制字节()
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

        byte[] b1 = GzipHelper.Compress(bb);

        Assert.IsTrue(b1.Length < bb.Length);

        byte[] b2 = GzipHelper.Decompress(b1);

        Assert.AreEqual(bb.Length, b2.Length);

        for( int i = 0; i < bb.Length; i++ ) {
            Assert.AreEqual((int)bb[i], (int)b2[i]);
        }
    }

    [TestMethod]
    public void Test_Args_Null()
    {
        Assert.AreEqual(null, GzipHelper.Compress((string)null));
        Assert.AreEqual(string.Empty, GzipHelper.Compress(string.Empty));

        Assert.AreEqual(null, GzipHelper.Decompress((string)null));
        Assert.AreEqual(string.Empty, GzipHelper.Decompress(string.Empty));
    }

    [TestMethod]
    public void Test_ToGzip()
    {
        string text = "!@#中文汉字abcd1234";
        byte[] b1 = GzipHelper.ToGzip(text);
        byte[] b2 = GzipHelper.ToGzip(text, Encoding.UTF8);
        b1.IsEqual(b2);

        byte[] b3 = GzipHelper.UnGzip(b2);
        string text2 = Encoding.UTF8.GetString(b3);
        Assert.AreEqual(text, text2);
    }

    [TestMethod]
    public void Test_NULL()
    {
        Assert.IsNull(GzipHelper.ToGzip((string)null));
        Assert.IsTrue(GzipHelper.ToGzip("").Length == 0);

        Assert.IsNull(GzipHelper.ToGzip((byte[])null));
        Assert.IsTrue(GzipHelper.ToGzip(Empty.Array<byte>()).Length == 0);

        Assert.IsNull(GzipHelper.UnGzip((byte[])null));
        Assert.IsTrue(GzipHelper.UnGzip(Empty.Array<byte>()).Length == 0);
    }

}
