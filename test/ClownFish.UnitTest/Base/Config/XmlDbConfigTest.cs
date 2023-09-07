using ClownFish.Base.Config.Models;

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
