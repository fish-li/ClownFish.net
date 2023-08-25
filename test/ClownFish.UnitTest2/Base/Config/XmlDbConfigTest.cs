using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Config.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class XmlDbConfigTest
{
    [TestMethod]
    public void Test()
    {
        XmlDbConfig conf = new XmlDbConfig {
            Name = "test1",
            DbType = ClownFish.Data.DatabaseType.PostgreSQL,
            Database = "MyNorthwind",
            Server = "localhost"
        };

        Assert.AreEqual("PostgreSQL/localhost/MyNorthwind", conf.ToString());

    }
}
