namespace ClownFish.UnitTest.Data.Context;
[TestClass]
public class DbConfigTest
{
    [TestMethod]
    public void Test()
    {
        DbConfig config = new DbConfig {
            Id = 123,
            Name = "db1",
            DbType = DatabaseType.MySQL,
            Server = "localhost",
            Database = "MyNorthwind",
            Port = 1025,
            UserName = "admin",
            Password = "fish",
            Args = "aa=2;bb=3"
        };

        Assert.AreEqual(123, config.Id);
        Assert.AreEqual("db1", config.Name);
        Assert.AreEqual("localhost", config.Server);
        Assert.AreEqual("MyNorthwind", config.Database);
        Assert.AreEqual(1025, config.Port);
        Assert.AreEqual("admin", config.UserName);
        Assert.AreEqual("fish", config.Password);
        Assert.AreEqual("aa=2;bb=3", config.Args);

        Assert.AreEqual($"MySQL/localhost/MyNorthwind", config.ToString());
    }
}
