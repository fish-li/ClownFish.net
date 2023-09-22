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
}
