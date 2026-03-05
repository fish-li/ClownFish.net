namespace ClownFish.UnitTest.Data.Context;

[TestClass]
public class LocalDbConnTest
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
            _ = DbConnManager.GetTntDbConfig("", "xsql");
        });

        Assert.IsNull(DbConnManager.GetTntDbConfig("t23432412134", "xsql", false, false));

        DbConfig conf1 = DbConnManager.GetTntDbConfig("my57a04574bf635", "xsql");
        DbConfig conf2 = DbConnManager.GetTntDbConfig("my57a197beed7d2", "xsql", true);

        Assert.IsNotNull(conf1);
        Assert.IsNotNull(conf2);
        Assert.AreEqual("tenant_xsql_my57a04574bf635", conf1.Name);
        Assert.AreEqual("tenant_xsql_my57a197beed7d2_readonly", conf2.Name);

    }
}
