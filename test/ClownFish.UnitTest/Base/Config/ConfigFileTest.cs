namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class ConfigFileTest
{
    [TestMethod]
    public void Test_GetLocalFile()
    {
        Assert.IsNull(ConfigFile.GetLocalFile("file1.txt"));
        Assert.IsNotNull(ConfigFile.GetLocalFile("ClownFish.App.config"));
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
        Assert.IsNotNull(ConfigFile.GetFile("ClownFish.App.config"));
        Assert.IsNotNull(ConfigFile.GetFile("ClownFish.UnitTest.ClownFish.App.config"));


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
}
