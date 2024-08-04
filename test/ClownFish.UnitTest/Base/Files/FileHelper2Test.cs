using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Files;
[TestClass]
public class FileHelper2Test
{
    private static string ReadFileTails2(string filePath, int maxRows, Encoding encoding = null)
    {
        // 说明：读取尾部 N 行的实现太慢了（需要一个字符一个字符的判断，且频繁的文件IO操作，尤其是当N值比较大的时候，
        // 所以，这个重新实现的方法就忽略行数，改成 数据长度size，

        int size = maxRows * 200;   // 这里取预估值，每行 200 个字符

        encoding = encoding ?? Encoding.UTF8;

        using( FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) ) {

            if( size > 0 && file.Length > size ) {
                file.Seek(-1 * size, SeekOrigin.End);

                // 为了阅读日志更“自然”，这里忽略 “半行” 内容，将文件指针定位到某一行的末尾
                while( file.Position > 0 ) {

                    int value = file.ReadByte();
                    if( value < 0           // 文件结束符
                        || value == 10 )    // 换行符
                        break;
                }
            }

            using( StreamReader reader = new StreamReader(file, encoding) ) {
                return reader.ReadToEnd();
            }
        }
    }

    private static readonly string s_filePath = "./temp/FileHelperTest_ReadFileTails_3.txt";
    private static readonly int s_lineCount = 155092;


    static FileHelper2Test()
    {
        if( File.Exists(s_filePath) == false ) {
            string a100 = new string('a', 100);
            string b100 = new string('b', 100);

            using( FileStream file = RetryFile.Create(s_filePath) ) {
                using( StreamWriter writer = new StreamWriter(file, Encoding.UTF8) ) {
                    for( int i = 0; i < s_lineCount; i++ ) {
                        writer.Write($"中华文明-{a100}-{i + 1}-{b100}-大明王朝\r\n");
                    }
                    // 注意：这个文件最后一行是个 “空行”，模拟日志行为
                }
            }
        }
    }

    [TestMethod]
    public void Test_1()
    {
        string text = ReadFileTails2(s_filePath, 5000);
        File.WriteAllText("./temp/FileHelperTest2_out_111.txt", text, Encoding.UTF8);

        Assert.IsTrue(text.HasValue());
        // ReadFileTails2 的实现方式不方便做断言判断~~~
    }

    [TestMethod]
    public void Test_2()
    {
        string text = FileHelper.ReadFileTails(s_filePath, 5000 +1);

        File.WriteAllText("./temp/FileHelperTest2_out_222.txt", text, Encoding.UTF8);
        int count = text.Count(x => x == '\n');
        Assert.AreEqual(5000, count);
    }

    [TestMethod]
    public void Test_3()
    {
        string text = FileHelper.ReadFileTails(s_filePath, 100 + 1);

        int count = text.Count(x => x == '\n');
        Assert.AreEqual(100, count);
    }

    [TestMethod]
    public void Test_4()
    {
        string text = FileHelper.ReadFileTails(s_filePath, 10000 + 1);

        int count = text.Count(x => x == '\n');
        Assert.AreEqual(10000, count);
    }

    [TestMethod]
    public void Test_5()
    {
        string text = FileHelper.ReadFileTails(s_filePath, s_lineCount + 1);

        int count = text.Count(x => x == '\n');
        Assert.AreEqual(s_lineCount, count);
    }

    [TestMethod]
    public void Test_6()
    {
        string text = FileHelper.ReadFileTails(s_filePath, 0);

        int count = text.Count(x => x == '\n');
        Assert.AreEqual(s_lineCount, count);
    }


    //[TestMethod]  // 这个方法太花时间，如果需要测试就取消注释吧~~~
    public void Test_N()
    {
        for( int i = 10; i < 1000; i = i + 100 ) {
            string text2 = FileHelper.ReadFileTails(s_filePath, i + 1);
            int count = text2.Count(x => x == '\n');
            int expected = i.Max(s_lineCount);
            Assert.AreEqual(expected, count);
        }

        for( int i = 3000; i < 5000; i = i + 100 ) {
            string text2 = FileHelper.ReadFileTails(s_filePath, i + 1);
            int count = text2.Count(x => x == '\n');
            int expected = i.Max(s_lineCount);
            Assert.AreEqual(expected, count);
        }

        for( int i = 1_0000; i < 20_0000; i = i + 3000 ) {
            string text2 = FileHelper.ReadFileTails(s_filePath, i + 1);
            int count = text2.Count(x => x == '\n');
            int expected = i.Max(s_lineCount);
            Assert.AreEqual(expected, count);
        }
    }
}
