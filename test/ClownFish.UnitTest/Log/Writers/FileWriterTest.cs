using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers;

[TestClass]
public class FileWriterTest
{
    public static InvokeLog CreateTempInvokeLog(string title)
    {
        return new InvokeLog {
            ActionType = 123,
            AppName = "ClownFish.UnitTest",
            ProcessId = Guid.NewGuid().ToString("N"),
            StartTime = DateTime.Now,
            ExecuteTime = TimeSpan.FromMilliseconds(10),
            Status = 200,
            Title = title
        };
    }


    [TestMethod]
    public void Test_write1()
    {
        string title = "==c1a5bfb0fd45408aaf7ad0dc06d40133==";
        InvokeLog log = CreateTempInvokeLog(title);

        ILogWriter[] writers = WriterFactory.GetWriters(typeof(InvokeLog));

        foreach( ILogWriter w in writers ) {
            if( w is FileWriter fileWriter ) {

                fileWriter.WriteOne(log);
                string filePath = (string)fileWriter.GetFieldValue("_currentFile");
                string body = RetryFile.ReadAllText(filePath);
                Assert.IsTrue(body.Contains(title));
            }
        }
    }

    [TestMethod]
    public void Test_write2()
    {
        string title1 = "==0e574da37a664b9cbae5c65cc9438666==";
        string title2 = "==643d50c97d734904ab0416e4560ed14c==";

        List<InvokeLog> list = new List<InvokeLog> {
            CreateTempInvokeLog(title1),
            CreateTempInvokeLog(title2)
        };

        ILogWriter[] writers = WriterFactory.GetWriters(typeof(InvokeLog));

        foreach( ILogWriter w in writers ) {
            if( w is FileWriter fileWriter ) {

                fileWriter.WriteList(list);
                string filePath = (string)fileWriter.GetFieldValue("_currentFile");
                string body = RetryFile.ReadAllText(filePath);
                Assert.IsTrue(body.Contains(title1));
                Assert.IsTrue(body.Contains(title2));
            }
        }
    }


    private OprLog CreateOprLog()
    {
        return new OprLog {
            OprKind = "test",
            Action = "X-Action",
            Addition = new string('a', 1024),
            AppName = "ClownFish.UnitTest",
            BizId = "111111111111111111111111111111",
            BizName = "222222222222222222222222",
            Controller = "333333333333333333",
            CtxData = "44444444444444444444444444444",
            Duration = 11111,
            EnvName = "5555555555555",
            ExAll = "6666666666666666666666666666",
            ExMessage = "77777777777777777777777777",
            ExType = "88888888888888888888888888888888",
            HasError = 1,
            HostName = "99999999999999999999999999999",
            HttpMethod = "GT",
            HttpRef = "bbbbbbbbbbbbbbbbbbbbbb",
            IsSlow = 0,
            Module = "cccccccccccccccccccccccccccccc",
            OprDetails = new string('a', 300),
            OprName = "dddddddddddddddddddddddddddddddd",
            ParentId = "eeeeeeeeeeeeeeeeeeeeeeeee",
            RootId = "fffffffffffffffffffffffffffff",
            StartTime = DateTime.Now,
            Status = 200,
            TenantId = "gggggggggggggggggggggggggg",
            Url = "hhhhhhhhhhhhhhhhhhhhhhhhhhh",
            UserAgent = "jjjjjjjjjjjjjjjjjjjjjjjj",
            UserCode = "kkkkkkkkkkkkk",
            UserId = "mmmmmmmmmmmmm",
            UserName = "nnnnnnnnnnnnnnnnnnn",
            UserRole = "mmmmmmmmmmmmmmmmmmmmmmmm",
        };
    }

    private void CreateSomeLogFile(XFileWriter writer, OprLog log, string savePath, int fileCount)
    {
        // 先删除遗留文件，确保下面的执行能触发清理动作
        string[] files = Directory.GetFiles(savePath, "*.xlog", SearchOption.TopDirectoryOnly);
        foreach( var file in files )
            RetryFile.Delete(file);


        // 先确保产生足够多的文件
        for( int i = fileCount; i > 0; i-- ) {

            string outFilePath = (string)s_method.Invoke(writer, new object[] { typeof(OprLog), DateTime.Now.AddDays(-1 * i) });
            s_field.SetValue(writer, outFilePath);

            log.OprId = i.ToString() + "_" + Guid.NewGuid().ToString("N");
            writer.WriteOne(log);
        }


        // 确定符合清理文件的条件
        files = Directory.GetFiles(savePath, "*.xlog", SearchOption.TopDirectoryOnly);
        Assert.AreEqual(fileCount, files.Length);
        Assert.IsTrue(files.Length > FileUtils.MaxCount);
    }

    private static readonly FieldInfo s_field = typeof(FileWriter).GetField("_currentFile", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo s_method = typeof(FileWriter).GetMethod("GetFilePath", BindingFlags.Instance | BindingFlags.NonPublic);


    [TestMethod]
    public void Test_write3()
    {
        OprLog log = CreateOprLog();
        XFileWriter writer = new XFileWriter();

        string newFilePath = (string)s_method.Invoke(writer, new object[] { typeof(OprLog), DateTime.Now });
        string savePath = Path.GetDirectoryName(newFilePath);

        // 创建足够多的日志文件
        CreateSomeLogFile(writer, log, savePath, 7);


        // 开始大量写入，触发将一个文件写满，进而引发清理
        List<int> results = new List<int>();

        for( int i = 0; i < 200; i++ ) {
            log.OprId = Guid.NewGuid().ToString("N");
            string text = XmlHelper.XmlSerialize(log, Encoding.UTF8);
            writer.WriteOne(log);
            results.Add(writer.LastWriteResult);
        }

        // 说明：下面几个断言与几个参数有关：
        // ClownFish.Log.config中的  <File MaxLength="200KB" MaxCount="5" />
        // OprLog的长度，主要是 Addition = new string('a', 1024),
        // i 的次数，现在是 200

        Console.WriteLine(results.ToJson());
        Assert.IsTrue(results.Count(x => x == 1) > 0);
        Assert.IsTrue(results.Count(x => x == 2) > 0);
        //Assert.IsTrue(results.Count(x => x == 3) > 0);

        string[] files = Directory.GetFiles(savePath, "*.xlog", SearchOption.TopDirectoryOnly);
        Assert.IsTrue(FileUtils.MaxCount - files.Length <= 1);
    }


    [TestMethod]
    public void Test_write4()
    {
        OprLog log = CreateOprLog();
        XFileWriter writer = new XFileWriter();

        string newFilePath = (string)s_method.Invoke(writer, new object[] { typeof(OprLog), DateTime.Now });
        string savePath = Path.GetDirectoryName(newFilePath);

        // 创建足够多的日志文件
        CreateSomeLogFile(writer, log, savePath, 7);


        // 执行清理动作
        MethodInfo method = typeof(FileWriter).GetMethod("DeleteOldFile", BindingFlags.Instance | BindingFlags.NonPublic);
        int result = (int)method.Invoke(writer, null);

        Assert.AreEqual(7 - FileUtils.MaxCount + 1, result);
    }
}
