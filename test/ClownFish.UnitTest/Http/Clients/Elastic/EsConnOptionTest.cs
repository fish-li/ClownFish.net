using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Clients.Elastic;

namespace ClownFish.UnitTest.Http.Clients.Elastic;
[TestClass]
public class EsConnOptionTest
{
    [TestMethod]
    public void Teset_Create1_1()
    {
        DbConfig dbConfig = new DbConfig {
            Server = "localhost",
            Database = "x",
            Port = 123
        };

        EsConnOption opt = EsConnOption.Create1(dbConfig);
        Assert.AreEqual("localhost", opt.Server);
        Assert.AreEqual(123, opt.Port);
        Assert.AreEqual("http://localhost:123", opt.Url);
        Assert.IsFalse(opt.Https);
        Assert.AreEqual(0, opt.TimeoutMs);
        Assert.IsNull(opt.IndexNameTimeFormat);
    }

    [TestMethod]
    public void Teset_Create1_2()
    {
        DbConfig dbConfig = new DbConfig {
            Server = "localhost",
            Database = "x",
            Port = 123,
            Args = "https=1"
        };

        EsConnOption opt = EsConnOption.Create1(dbConfig);
        Assert.AreEqual("localhost", opt.Server);
        Assert.AreEqual(123, opt.Port);
        Assert.AreEqual("https://localhost:123", opt.Url);
        Assert.AreEqual(0, opt.TimeoutMs);
        Assert.IsNull(opt.IndexNameTimeFormat);
    }

    [TestMethod]
    public void Teset_Create1_3()
    {
        DbConfig dbConfig = new DbConfig {
            Server = "localhost",
            Database = "x",
            Port = 123,
            Args = "https=1;Timeoutms=3000;IndexNameTimeFormat=-yyyyMMdd-HH"
        };

        EsConnOption opt = EsConnOption.Create1(dbConfig);
        Assert.AreEqual("localhost", opt.Server);
        Assert.AreEqual(123, opt.Port);
        Assert.AreEqual("https://localhost:123", opt.Url);
        Assert.AreEqual(3000, opt.TimeoutMs);
        Assert.AreEqual("-yyyyMMdd-HH", opt.IndexNameTimeFormat);
    }

    [TestMethod]
    public void Teset_Create1_4()
    {
        DbConfig dbConfig = new DbConfig {
            Server = "localhost",
            Database = "x",
        };

        EsConnOption opt = EsConnOption.Create1(dbConfig);
        Assert.AreEqual("localhost", opt.Server);
        Assert.AreEqual(0, opt.Port);
        Assert.AreEqual("http://localhost:9200", opt.Url);
        Assert.AreEqual(0, opt.TimeoutMs);
        Assert.IsNull(opt.IndexNameTimeFormat);
    }

    [TestMethod]
    public void Teset_Create1_5()
    {
        DbConfig dbConfig = new DbConfig {
            Server = "10.5.1.1:9300",
            Database = "x",
        };

        EsConnOption opt = EsConnOption.Create1(dbConfig);
        Assert.AreEqual("10.5.1.1:9300", opt.Server);
        Assert.AreEqual(0, opt.Port);
        Assert.AreEqual("http://10.5.1.1:9300", opt.Url);
        Assert.AreEqual(0, opt.TimeoutMs);
        Assert.IsNull(opt.IndexNameTimeFormat);
    }

    [TestMethod]
    public void Test_2()
    {
        DbConfig dbConfig = new DbConfig {
            Server = "10.5.1.1:9300",
            Database = "x",
        };

        EsConnOption opt = EsConnOption.Create1(dbConfig);

        opt.SetTimeoutMs(100)
            .SetIndexNameTimeFormat("abc");

        Assert.IsNull(opt.Password);
        Assert.AreEqual(100, opt.TimeoutMs);
        Assert.AreEqual("abc", opt.IndexNameTimeFormat);

        string text = opt.ToString();
        Assert.IsTrue(text.StartsWith0("Server="));
    }

    [TestMethod]
    public void Test_3()
    {
        EsConnOption opt = new EsConnOption();
        Assert.AreEqual("-yyyyMMdd", opt.IndexNameTimeFormat);
        Assert.AreEqual(30_000, opt.TimeoutMs);

        opt.IndexNameTimeFormat = null;
        opt.TimeoutMs = 0;

        Assert.IsNull(opt.IndexNameTimeFormat);
        Assert.AreEqual(0, opt.TimeoutMs);

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            opt.Validate();
        });

        opt.Server = "s1";
        opt.Validate();

        Assert.AreEqual("-yyyyMMdd", opt.IndexNameTimeFormat);
        Assert.AreEqual(30_000, opt.TimeoutMs);
    }

    [TestMethod]
    public void Test_4()
    {
        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = EsConnOption.Create("xxxxxxxxx", true);
        });

        EsConnOption opt = EsConnOption.Create("xxxxxxxxx", false);
        Assert.IsNull(opt);
    }
}
