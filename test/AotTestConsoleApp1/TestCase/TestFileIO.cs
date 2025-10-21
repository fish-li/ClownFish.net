using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AotTestConsoleApp1.TestCase;
internal class TestFileIO
{
    public static async Task Run()
    {
        await Task.CompletedTask;

        string s1 = "用GZIP压缩一个字符串，并以BASE64字符串的形式返回压缩后的结果";
        byte[] bb = s1.ToUtf8Bytes();
        using MemoryStream ms = new MemoryStream(bb, false);

        using TempFile file1 = TempFile.CreateFile(ms);

        byte[] bb2 = RetryFile.ReadAllBytes(file1.FilePath);
        MyAssert.AreEqual(bb, bb2);




        NameValue nv = new NameValue { Name = "abc", Value = Guid.NewGuid().ToString() };
        using TempFile file2 = TempFile.CreateFile();

        ReliableFile.WriteObject(nv, file2.FilePath, 2);
        NameValue nv2 = ReliableFile.ReadObject<NameValue>(file2.FilePath, 2);
        MyAssert.AreEqual(nv, nv2);

    }
}
