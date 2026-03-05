namespace ClownFish.UnitTest.Log;

[TestClass]
public class LogConfigTest
{
    [TestMethod]
    public void Test_LogConfiguration()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            LogConfig.LoadFromXml("");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            LogConfig.LoadFromFile("");
        });

        MyAssert.IsError<FileNotFoundException>(() => {
            LogConfig.LoadFromFile("xxxxxxxxxx.config", true);
        });


        string filePath = PathUtils.GetFileAbsolutePath("ClownFish.UnitTest.config.ini");
        string ini = File.ReadAllText(filePath, Encoding.UTF8);

        LogConfiguration cfg1 = LogConfig.LoadFromIni(ini);
        LogConfiguration cfg2 = LogConfig.LoadFromFile(filePath, true);
    }

    [TestMethod]
    public void Test_GetDebugReportBlock()
    {
        DebugReportBlock block = LogConfig.GetDebugReportBlock();
        Console.WriteLine(block.ToString2());
        // 这个用例不关心结果，只保证 GetDebugReportBlock 这个方法能成功调用就可以了。
    }


}
