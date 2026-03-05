namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class ConfigFileTest
{
    [TestMethod]
    public void Test_GetLocalFile()
    {
        Assert.IsNull(DefaultConfigFileImpl.GetLocalFile("file1.txt"));
        Assert.IsNotNull(DefaultConfigFileImpl.GetLocalFile("ClownFish.UnitTest.config.ini"));

        string filePath = Path.Combine(AppContext.BaseDirectory, "_config/file2.txt");
        RetryFile.WriteAllText(filePath, "aaaaa");
        Assert.IsNotNull(DefaultConfigFileImpl.GetLocalFile("file2.txt"));

        RetryFile.Delete(filePath);
        Assert.IsNull(DefaultConfigFileImpl.GetLocalFile("file2.txt"));
    }

    [TestMethod]
    public void Test_GetFile()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ConfigFile.GetFile("");
        });

        MyAssert.IsError<FileNotFoundException>(() => {
            _ = ConfigFile.GetFile("file1.txt", true);
        });

        Assert.IsNull(ConfigFile.GetFile("file1.txt"));
        Assert.IsNotNull(ConfigFile.GetFile("ClownFish.UnitTest.config.ini"));


        //-----------------------------------------------------------
        MyTestConfigClient client = new MyTestConfigClient();
        client.SetConfigFile("file1.txt", "4fd85608524747168b135c6116432966b5d2e1c73f244b098cc3b98967e870cc");
        ConfigClient.Instance.SetClient(client);

        Assert.IsNotNull(ConfigFile.GetFile("file1.txt"));
        ConfigClient.Instance.ResetNull();


        //-----------------------------------------------------------
        Assert.IsNull(ConfigFile.GetFile("file1.txt"));
        MemoryConfig.AddFile("file1.txt", "4fd85608524747168b135c6116432966b5d2e1c73f244b098cc3b98967e870cc");
        Assert.IsNotNull(ConfigFile.GetFile("file1.txt"));
    }

    [TestMethod]
    public void Test_IConfigFile()
    {
        ConfigFile.SetImpl(new XConfigFileImpl());
        Assert.AreEqual("ClownFish.UnitTest.config.ini_2384321076a9425683bae0809cb0f456", ConfigFile.GetFile("ClownFish.UnitTest.config.ini"));

        ConfigFile.SetImpl(null);
        string text = ConfigFile.GetFile("ClownFish.UnitTest.config.ini");
        Assert.IsFalse(text.Contains("_2384321076a9425683bae0809cb0f456"));
    }
}


internal class XConfigFileImpl : IConfigFile
{
    public string GetFile(string filename, bool checkExist)
    {
        return filename + "_2384321076a9425683bae0809cb0f456";
    }
}