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
public class ConfigClientTest
{
    [TestMethod]
    public void Test_1()
    {
        Assert.IsNull(ConfigClient.Instance.GetSetting("key1"));
        Assert.IsNull(ConfigClient.Instance.GetSetting<NameValue>("key2"));
        Assert.IsNull(ConfigClient.Instance.GetAppDbConfig("conn1", false));
        Assert.IsNull(ConfigClient.Instance.GetTntDbConfig("xsql", "t123456789", "", false));
        Assert.IsNull(ConfigClient.Instance.GetConfigFile("file1.txt"));
    }

    [TestMethod]
    public void Test_2()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ConfigClient.Instance.GetSetting("");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ConfigClient.Instance.GetSetting<NameValue>("");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ConfigClient.Instance.GetAppDbConfig("");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ConfigClient.Instance.GetTntDbConfig("xsql", "");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ConfigClient.Instance.GetConfigFile("");
        });
    }

    [TestMethod]
    public void Test_3()
    {
        MyTestConfigClient client = new MyTestConfigClient();
        client.SetAppDbConfig("conn1", new DbConfig {
            Name = "conn1",
            Server = "mssql_host",
            Port = 1234,
            Database = "config1",
            UserName = "sa",
            Password = "xxxxxxx",
        });
        client.SetTntDbConfig("xsql", "t123456789", "", new DbConfig {
            Name = "conn2",
            Server = "mssql_host",
            Port = 1235,
            Database = "config2",
            UserName = "sa",
            Password = "zzzzzzzz",
        });

        client.SetSetting("key1", "9689399fd016412c9bb958b5b76e7824");
        client.SetSetting("key2", "Name=abc;Value=12345");
        client.SetConfigFile("file1.txt", "4fd85608524747168b135c6116432966b5d2e1c73f244b098cc3b98967e870cc");

        ConfigClient.Instance.SetClient(client);
        Assert.IsNotNull(ConfigClient.Instance.GetSetting("key1"));
        Assert.IsNotNull(ConfigClient.Instance.GetSetting<NameValue>("key2"));
        Assert.IsNotNull(ConfigClient.Instance.GetAppDbConfig("conn1"));
        Assert.IsNotNull(ConfigClient.Instance.GetTntDbConfig("xsql", "t123456789"));
        Assert.IsNotNull(ConfigClient.Instance.GetConfigFile("file1.txt"));


        ConfigClient.Instance.ResetNull();
    }


    [TestMethod]
    public void Test_4()
    {
        MyTestConfigClient client = new MyTestConfigClient();
        ConfigClient.Instance.SetClient(client);

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = ConfigClient.Instance.GetSetting("key1", true);
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = ConfigClient.Instance.GetSetting<NameValue>("key2", true);
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = ConfigClient.Instance.GetAppDbConfig("conn1", true);
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = ConfigClient.Instance.GetTntDbConfig("xsql", "t123456789", "", true);
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = ConfigClient.Instance.GetConfigFile("file1.txt", true);
        });

        ConfigClient.Instance.ResetNull();
    }
}



public class MyTestConfigClient : IConfigClient
{
    private readonly Dictionary<string, DbConfig> _appDbDict = new Dictionary<string, DbConfig>();
    private readonly Dictionary<string, DbConfig> _tntDbDict = new Dictionary<string, DbConfig>();
    private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _files = new Dictionary<string, string>();

    public DbConfig GetAppDbConfig(string name) => _appDbDict.TryGet(name);

    public void SetAppDbConfig(string name, DbConfig value) => _appDbDict[name] = value;

    public string GetConfigFile(string filename) => _files.TryGet(filename);

    public void SetConfigFile(string filename, string body) => _files[filename] = body;

    public string GetSetting(string name) => _settings.TryGet(name);

    public void SetSetting(string name, string value) => _settings[name] = value;

    public DbConfig GetTntDbConfig(string connType, string tenantId, string flag) => _tntDbDict.TryGet($"{connType}-{tenantId}-{flag}");

    public void SetTntDbConfig(string connType, string tenantId, string flag, DbConfig value) => _tntDbDict[$"{connType}-{tenantId}-{flag}"] = value;
}