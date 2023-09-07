namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class ZipHelperTest
{
    [TestMethod]
    public void Test()
    {
        string path = Path.Combine(Environment.CurrentDirectory, "temp");
        string tempPath2 = Path.Combine(Environment.CurrentDirectory, "temp2");
        Directory.CreateDirectory(tempPath2);

        string newFile = Path.Combine(Environment.CurrentDirectory, @"temp\aa\bb.txt");
        RetryFile.WriteAllText(newFile, "aaaaaaaaaaaa");
        

        string zipFile = Path.Combine(tempPath2, "test.zip");
        ZipHelper.CompressDirectory(path, zipFile);

        string tempPath3 = Path.Combine(Environment.CurrentDirectory, "temp3");
        RetryDirectory.Delete(tempPath3, true);  // 这个目录会自动创建

        ZipHelper.ExtractFiles(zipFile, tempPath3);


        string[] files1 = Directory.GetFiles(path);
        string[] files2 = Directory.GetFiles(tempPath3);

        Assert.AreEqual(files1.Length, files2.Length);

        long length1 = (from x in files1
                           let len = (new FileInfo(x)).Length
                           select len).Sum();

        long length2 = (from x in files2
                        let len = (new FileInfo(x)).Length
                        select len).Sum();
        Assert.AreEqual(length1, length2);

        


        List<ZipItem> list = ZipHelper.Read(zipFile);
        //File.Delete(zipFile);

        var item = list.Find(x => x.FullName == @"aa/bb.txt");
        Assert.IsNotNull(item);

        list.Add(new ZipItem {  // 这个节点不是必需的
            FullName = "aa/",
        });

        string configFile = Path.Combine(Environment.CurrentDirectory, "ClownFish.App.config");
        list.Add(new ZipItem {
            FullName = "aa/ClownFish.App.config",
            LocalFilePath = configFile
        });


        string zipFile2 = Path.Combine(tempPath2, "test2.zip");
        ZipHelper.Compress(list, zipFile2);


        List<ZipItem> list2 = ZipHelper.Read(zipFile2);
        var item2 = list2.Find(x => x.FullName == @"aa/ClownFish.App.config");
        Assert.IsNotNull(item2);

        byte[] body = File.ReadAllBytes(configFile);            
        Assert.IsTrue(body.IsEqual(item2.Body));
    }


    [TestMethod]
    public void Test_CreateZipFromText()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ZipHelper.CreateZipFromText(string.Empty);
        });


        string text = "Code Generation指的是编译器将用户的代码翻译为IL(Intermediate language)到assembly code的过程，Code Generation几乎是所有一切的基础，因此对于Code Generation的优化直接影响了最终代码的性能，根据测试以及官方的blog, .Net 6在这个方面有着巨大的提升。";
        using( Stream zipStream = ZipHelper.CreateZipFromText(text) ) {

            using( ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read) ) {

                List<ZipItem> list = ZipHelper.Read(archive);

                Assert.AreEqual(1, list.Count);
                Assert.AreEqual("file1", list[0].FullName);
                Assert.AreEqual(text, Encoding.UTF8.GetString(list[0].Body));
            }
        }
    }


    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ZipHelper.ExtractFiles(string.Empty, "xxx");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            ZipHelper.ExtractFiles(@"d:\aa.zip", "");
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            _= ZipHelper.Read(string.Empty);
        });
        MyAssert.IsError<FileNotFoundException>(() => {
            _ = ZipHelper.Read("xxxxxxxxxxxxx.zip");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ZipHelper.Compress(null, new MemoryStream());
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            ZipHelper.Compress(new List<ZipItem>(), (MemoryStream)null);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            ZipHelper.Compress(null, "xxxxxx");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            ZipHelper.Compress(new List<ZipItem>(), (string)null);
        });
    }
}
