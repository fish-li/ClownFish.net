namespace ClownFish.UnitTest.Data.Context;

[TestClass]
public class ConnectionManagerTest : BaseTest
{
    [TestMethod]
    public void Test_GetConnection_OK()
    {
        ConnectionInfo connection1 = ConnectionManager.GetFirstConnection();
        Assert.IsNotNull(connection1);

        ConnectionInfo connection2 = ConnectionManager.GetConnection("sqlserver");
        Assert.IsNotNull(connection2);
        Assert.AreEqual("sqlserver", connection2.Name);

        Assert.AreEqual(connection1.ConnectionString, connection2.ConnectionString);
        Assert.AreEqual(connection1.ProviderName, connection2.ProviderName);
    }



    [TestMethod]
    public void Test_GetConnection_名称不存在()
    {
        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            _ = ConnectionManager.GetConnection("xxxxx");
        });
    }


    [TestMethod]
    public void Test_ConnectionScope_Create()
    {
        ConnectionInfo info = ConnectionManager.GetConnection("sqlserver");
        Assert.AreEqual("sqlserver", info.Name);

        using( DbContext
                db1 = DbContext.Create(),
                db2 = DbContext.Create("sqlserver"),
                db3 = DbContext.Create(info.ConnectionString, info.ProviderName)
            ) {

            ConnectionInfo connection1 = db1.ConnectionInfo;
            ConnectionInfo connection2 = db2.ConnectionInfo;
            ConnectionInfo connection3 = db3.ConnectionInfo;


            Assert.AreEqual(connection1.ConnectionString, connection2.ConnectionString);
            Assert.AreEqual(connection1.ProviderName, connection2.ProviderName);

            Assert.AreEqual(connection1.ConnectionString, connection3.ConnectionString);
            Assert.AreEqual(connection1.ProviderName, connection3.ProviderName);
        }
    }


    [TestMethod]
    public void Test_DbConfigs()
    {
        var configs = AppConfig.GetAccessor().GetConfObject().DbConfigs;
        Assert.IsNotNull(AppConfig.GetDbConfig("s1"));
        Assert.IsNotNull(AppConfig.GetDbConfig("s2"));
        Assert.IsNotNull(AppConfig.GetDbConfig("m1"));
        Assert.IsNotNull(AppConfig.GetDbConfig("m2"));
        Assert.IsNotNull(AppConfig.GetDbConfig("pg1"));
        Assert.IsNotNull(AppConfig.GetDbConfig("dm1"));

        DbConfig s1 = AppConfig.GetDbConfig("s1");
        Assert.AreEqual("s1", s1.Name);
        Assert.AreEqual(DatabaseType.SQLSERVER, s1.DbType);
        Assert.AreEqual("MsSqlHost", s1.Server);
        Assert.AreEqual(0, s1.Port);
        Assert.AreEqual("MyNorthwind", s1.Database);
        Assert.AreEqual("user1", s1.UserName);
        Assert.AreEqual("qaz1@wsx", s1.Password);


        DbConfig m2 = AppConfig.GetDbConfig("m2");
        Assert.AreEqual("m2", m2.Name);
        Assert.AreEqual(DatabaseType.MySQL, m2.DbType);
        Assert.AreEqual("MySqlHost", m2.Server);
        Assert.AreEqual(0, m2.Port);
        Assert.AreEqual("MyNorthwind", m2.Database);
        Assert.AreEqual("user1", m2.UserName);
        Assert.AreEqual("qaz1=;@wsx", m2.Password);
        Assert.AreEqual("Allow Zero Datetime=True;Convert Zero Datetime=True;", m2.Args);


        DbConfig dm1 = AppConfig.GetDbConfig("dm1");
        Assert.AreEqual("dm1", dm1.Name);
        Assert.AreEqual(DatabaseType.DaMeng, dm1.DbType);
        Assert.AreEqual("PgSqlHost", dm1.Server);
        Assert.AreEqual(15236, dm1.Port);
    }


    [TestMethod]
    public void Test_GetDbConfig()
    {
        ConnectionInfo conn1 = ConnectionManager.GetConnection("sqlserver");
        Assert.AreEqual("sqlserver", conn1.Name);

        DbConfig config1 = ConnectionManager.GetDbConfig("s1");
        Assert.AreEqual("s1", config1.Name);

        Console.WriteLine(conn1.ConnectionString);
        Console.WriteLine(config1.GetConnectionString(true));

        AssertMsSqlConnectionString(conn1.ConnectionString, config1.GetConnectionString(true));
    }


    private void AssertMsSqlConnectionString(string connectionString1, string connectionString2)
    {
        SqlConnectionStringBuilder b1 = new SqlConnectionStringBuilder(connectionString1);
        SqlConnectionStringBuilder b2 = new SqlConnectionStringBuilder(connectionString2);

        Assert.AreEqual(b1.DataSource, b2.DataSource);
        Assert.AreEqual(b1.InitialCatalog, b2.InitialCatalog);
        Assert.AreEqual(b1.UserID, b2.UserID);
        Assert.AreEqual(b1.Password, b2.Password);
    }

    [TestMethod]
    public void Test_ConnName()
    {
        Test_ConnName0("sqlserver2");
        Test_ConnName0("mysql");
        Test_ConnName0("postgresql");
        Test_ConnName0("kingbase");
        Test_ConnName0("master");
        Test_ConnName0("s2");
        Test_ConnName0("m2");
        Test_ConnName0("pg1");
        Test_ConnName0("tenant_xsql_my57a04574bf635");

#if NET8_0_OR_GREATER
        Test_ConnName0("kingbase3");
#endif
    }

    private static void Test_ConnName0(string connName)
    {
        using DbContext db = DbContext.Create(connName);
        Assert.AreEqual(connName, db.ConnectionInfo.Name);
        Assert.AreEqual(connName, db.ConnName);
    }
}
