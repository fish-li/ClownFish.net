using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AotTestConsoleApp1.TestCase;
internal class TestConfig
{
    public static async Task Run()
    {
        await Task.CompletedTask;

        Assert.AreEqual("abcd", LocalSettings.GetSetting("key1"));
        Assert.AreEqual("a8352a32841e420c8d0395edb696f85f", LocalSettings.GetSetting("env_demo_1"));

        DbConfig dbconf1 = AppConfig.GetDbConfig("master");
        Assert.AreEqual("MyNorthwind", dbconf1.Database);

        var dbconn = AppConfig.GetConnectionString("mysql2");
        Assert.AreEqual("MySql.Data.MySqlClient", dbconn.ProviderName);

    }
}
