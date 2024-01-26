namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class MemoryConfigTest
{
    [TestMethod]
    public void Test_AddSetting()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = MemoryConfig.GetSetting("");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.AddSetting("", "11111111");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.AddSetting("key1", null);
        });

        Assert.IsNull(MemoryConfig.GetSetting("key1"));

        MemoryConfig.AddSetting("key1", "3fde7d2b9d31429cbaa5e08a90e67fca");

        Assert.AreEqual("3fde7d2b9d31429cbaa5e08a90e67fca", MemoryConfig.GetSetting("key1"));
    }

    [TestMethod]
    public void Test_AddDbConfig()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = MemoryConfig.GetDbConfig("");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.AddDbConfig("", new DbConfig());
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.AddDbConfig("key1", null);
        });



        Assert.IsNull(MemoryConfig.GetDbConfig("conn1"));

        MemoryConfig.AddDbConfig("conn1", new DbConfig {
            Name = "conn1",
            Server = "mssql_host",
            Port = 1234,
            Database = "config1",
            UserName = "sa",
            Password = "xxxxxxx",
        });

        Assert.IsNotNull(MemoryConfig.GetDbConfig("conn1"));
    }

    [TestMethod]
    public void Test_AddFile()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = MemoryConfig.GetFile("");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.AddFile("", "xxxxxxxxxxxxx");
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.AddFile("key1", null);
        });


        Assert.IsNull(MemoryConfig.GetFile("file2.txt"));

        MemoryConfig.AddFile("file2.txt", "4fd85608524747168b135c6116432966b5d2e1c73f244b098cc3b98967e870cc");

        Assert.IsNotNull(MemoryConfig.GetFile("file2.txt"));
    }

    [TestMethod]
    public void Test_SetAppConfig()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.SetAppConfig(null);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            MemoryConfig.SetLogConfig(null);
        });


        Assert.IsNull(MemoryConfig.GetFile("ClownFish.UnitTest.App.Config"));
        Assert.IsNull(MemoryConfig.GetFile("ClownFish.UnitTest.Log.Config"));

        MemoryConfig.SetAppConfig("xml_1111111111");
        MemoryConfig.SetLogConfig("xml_2222222222");

        Assert.IsNotNull(MemoryConfig.GetFile("ClownFish.UnitTest.App.Config"));
        Assert.IsNotNull(MemoryConfig.GetFile("ClownFish.UnitTest.Log.Config"));
    }

    [TestMethod]
    public void Test_GetDebugReportBlock()
    {
        MemoryConfig.AddSetting("s95398", "95398a3cd942459cbcf1dd88e0a58d7f");
        MemoryConfig.AddFile("f6ecf0e", "6ecf0ebdc30140adb1d5e8d0b2a2240173f11958a9da43cfb561f304db270b4b");
        MemoryConfig.AddDbConfig("d9b22ee9", new DbConfig {
            Name = "d9b22ee9",
            DbType = DatabaseType.MySQL,
            Database = "9b22ee9385cd4c90be4833343c8b465c"
        });

        DebugReportBlock block = MemoryConfig.GetDebugReportBlock();
        string text = block.ToString2();

        Assert.IsTrue(text.Contains("s95398 = 95398a3cd942459cbcf1dd88e0a58d7f"));
        Assert.IsTrue(text.Contains("9b22ee9385cd4c90be4833343c8b465c"));
    }
}
