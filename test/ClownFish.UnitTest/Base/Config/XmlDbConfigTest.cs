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

        string value = conf.ToString();
        Assert.IsTrue(value.Contains("Name=test1"));
        Assert.IsTrue(value.Contains("DbType=PostgreSQL"));
        Assert.IsTrue(value.Contains("Server=localhost"));
        Assert.IsTrue(value.Contains("Port=0"));
        Assert.IsTrue(value.Contains("Database=MyNorthwind"));
        Assert.IsTrue(value.Contains("UserName="));
        Assert.IsTrue(value.Contains("Password="));

    }
}
