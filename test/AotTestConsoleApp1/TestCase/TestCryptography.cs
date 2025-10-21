using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AotTestConsoleApp1.TestCase;
internal class TestCryptography
{
    public static async Task Run()
    {
        await Task.CompletedTask;

        string password = "中文密钥#123";
        string s1 = "用GZIP压缩一个字符串，并以BASE64字符串的形式返回压缩后的结果";

        string s2 = AesHelper.Encrypt(s1, password);
        string s3 = AesHelper.Decrypt(s2, password);
        Assert.AreEqual(s1, s3);
    }
}
