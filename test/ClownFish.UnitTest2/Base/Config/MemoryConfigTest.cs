using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
}
