namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class DbConnManagerTest
{
    [TestMethod]
    public void Test_GetAppDbConfig()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DbConnManager.GetAppDbConfig("");
        });

        Assert.IsNull(DbConnManager.GetAppDbConfig("sqlserver", false));
        Assert.IsNull(DbConnManager.GetAppDbConfig("mysql", false));

        Assert.IsNotNull(DbConnManager.GetAppDbConfig("s1"));
        Assert.IsNotNull(DbConnManager.GetAppDbConfig("s2"));

        Assert.IsNotNull(DbConnManager.GetAppDbConfig("m1"));
        Assert.IsNotNull(DbConnManager.GetAppDbConfig("m2"));

        Assert.IsNotNull(DbConnManager.GetAppDbConfig("pg1"));
        Assert.IsNotNull(DbConnManager.GetAppDbConfig("dm1"));
    }

    [TestMethod]
    public void Test_GetTntDbConfig()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DbConnManager.GetTntDbConfig("", "xsql", false);
        });

        Assert.IsNull(DbConnManager.GetTntDbConfig("t23432412134", "xsql", false, false));

        DbConfig conf1 = DbConnManager.GetTntDbConfig("my57a04574bf635", "xsql", false);

        MyAssert.IsError<DatabaseNotFoundException>(() => {
            _ = DbConnManager.GetTntDbConfig("my57a04574bf635", "xsql", true);
        });

        DbConfig conf2 = DbConnManager.GetTntDbConfig("my57a197beed7d2", "xsql", true);

        Assert.IsNotNull(conf1);
        Assert.IsNotNull(conf2);
        Assert.AreEqual("tenant_xsql_my57a04574bf635", conf1.Name);
        Assert.AreEqual("tenant_xsql_my57a197beed7d2_readonly", conf2.Name);


        MyAssert.IsError<DatabaseNotFoundException>(() => {
            _ = DbConnManager.GetTntDbConfig("xsql", "my57a04574bf635", true);
        });

    }

    [TestMethod]
    public void Test_CreateMaster()
    {
        using DbContext db = DbConnManager.CreateMaster();
        Assert.IsNotNull(db);
    }


    [TestMethod]
    public void Test_CreateAppDb()
    {
        using DbContext db = DbConnManager.CreateAppDb("s1");
        Assert.IsNotNull(db);
    }

    [TestMethod]
    public void Test_CreateTenant()
    {
        using DbContext db1 = DbConnManager.CreateTenant("my57a04574bf635");
        Assert.IsNotNull(db1);

        using DbContext db2 = DbConnManager.CreateTenant("my57a197beed7d2", true);
        Assert.IsNotNull(db2);
    }

    [TestMethod]
    public void Test_IDbConnManager()
    {
        DbConnManager.SetImpl(new XDbConnManagerImpl());
        Assert.IsNotNull(DbConnManager.GetAppDbConfig("xx1"));
        Assert.IsNotNull(DbConnManager.GetTntDbConfig("tnt123"));


        DbConnManager.SetImpl(null);
        Assert.IsNull(DbConnManager.GetAppDbConfig("xx1", false));
        Assert.IsNull(DbConnManager.GetTntDbConfig("tnt123", null, false, false));
    }
}



internal sealed class XDbConnManagerImpl : IDbConnManager
{
    public DbConfig GetAppDbConfig(string connName, bool checkExist)
    {
        return new DbConfig {
            Server = "localpc",
            Database = "app1",
            UserName = connName,
        };
    }

    public DbConfig GetTntDbConfig(string tenantId, string connType, bool readonlyDB, bool checkExist)
    {
        return new DbConfig {
            Server = "localpc",
            Database = "app2",
            UserName = tenantId
        };
    }

    public DbContext CreateMaster()
    {
        throw new NotImplementedException();
    }

    public DbContext CreateAppDb(string connName, bool longConnection, string providerName)
    {
        throw new NotImplementedException();
    }

    public DbContext CreateTenant(string tenantId, bool readonlyDB, string providerName)
    {
        throw new NotImplementedException();
    }
}