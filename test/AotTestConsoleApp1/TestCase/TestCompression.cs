using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AotTestConsoleApp1.TestCase;
internal class TestCompression
{
    public static async Task Run()
    {
        await Task.CompletedTask;

        string s1 = "用GZIP压缩一个字符串，并以BASE64字符串的形式返回压缩后的结果";

        string s2 = GzipHelper.Compress(s1);
        string s3 = GzipHelper.Decompress(s2);
        Assert.AreEqual(s1, s3);
    }
}
