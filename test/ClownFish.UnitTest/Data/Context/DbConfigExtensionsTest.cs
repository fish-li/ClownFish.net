using MySql.Data.MySqlClient;
using Npgsql;

namespace ClownFish.UnitTest.Data.Context;
[TestClass]
public class DbConfigExtensionsTest
{
    [TestMethod]
    public void Test_CreateDbContext()
    {
        DbConfig s1 = AppConfig.GetDbConfig("s1");
        using DbContext db1 = s1.CreateDbContext(true);
        Assert.AreEqual(DatabaseType.SQLSERVER, db1.DatabaseType);
        Assert.AreEqual("s1", db1.ConnName);


        DbConfig m2 = AppConfig.GetDbConfig("m2");
        using DbContext db2 = m2.CreateDbContext(false);
        Assert.AreEqual(DatabaseType.MySQL, db2.DatabaseType);
        Assert.AreEqual("m2", db2.ConnName);

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DbConfigExtensions.CreateDbContext(null);
        });

    }


    [TestMethod]
    public void Test_CreateDbContext_MySQL()
    {
        DbConfig m1 = AppConfig.GetDbConfig("m1");

        using DbContext db1 = m1.CreateDbContext(true, "MySqlConnector");
        Assert.IsInstanceOfType(db1.Connection, typeof(MySqlConnector.MySqlConnection));
        Assert.AreEqual("m1", db1.ConnName);

        using DbContext db2 = m1.CreateDbContext(false, "MySql.Data");
        Assert.IsInstanceOfType(db2.Connection, typeof(MySql.Data.MySqlClient.MySqlConnection));
        Assert.AreEqual("m1", db2.ConnName);
    }


    [TestMethod]
    public void Test_GetProviderName()
    {
        DbConfig s1 = AppConfig.GetDbConfig("s1");
        Assert.AreEqual("System.Data.SqlClient", s1.GetProviderName());

        DbConfig m1 = AppConfig.GetDbConfig("m1");
        Assert.AreEqual("MySql.Data.MySqlClient", m1.GetProviderName());

        DbConfig pg1 = AppConfig.GetDbConfig("pg1");
        Assert.AreEqual("Npgsql", pg1.GetProviderName());


        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DbConfigExtensions.GetProviderName(null);
        });

        MyAssert.IsError<NotSupportedException>(() => {
            DbConfig dbConfig1 = new DbConfig { DbType = (DatabaseType)66666666 };
            _ = DbConfigExtensions.GetProviderName(dbConfig1);
        });
    }


    [TestMethod]
    public void Test_GetProviderName_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DbConfigExtensions.GetConnectionString(null);
        });

        MyAssert.IsError<NotSupportedException>(() => {
            DbConfig dbConfig1 = new DbConfig { DbType = (DatabaseType)66666666 };
            _ = DbConfigExtensions.GetConnectionString(dbConfig1);
        });
    }


    [TestMethod]
    public void Test_GetMsSqlConnectionString()
    {
        DbConfig s1 = AppConfig.GetDbConfig("s1");

        string connectionString1 = s1.GetConnectionString(true);
        Assert.AreEqual("Data Source=MsSqlHost;Initial Catalog=MyNorthwind;User ID=user1;Password=qaz1@wsx;Application Name=ClownFish.UnitTest", connectionString1);


        string connectionString2 = s1.GetConnectionString(false);
        Assert.AreEqual("Data Source=MsSqlHost;User ID=user1;Password=qaz1@wsx;Application Name=ClownFish.UnitTest", connectionString2);



        DbConfig s2 = AppConfig.GetDbConfig("s1");
        s2.Port = 1025;

        string connectionString3 = s2.GetConnectionString(true);
        Assert.AreEqual("Data Source=MsSqlHost,1025;Initial Catalog=MyNorthwind;User ID=user1;Password=qaz1@wsx;Application Name=ClownFish.UnitTest", connectionString3);


        string connectionString4 = s2.GetConnectionString(false);
        Assert.AreEqual("Data Source=MsSqlHost,1025;User ID=user1;Password=qaz1@wsx;Application Name=ClownFish.UnitTest", connectionString4);
    }

    [TestMethod]
    public void Test_GetMySqlConnectionString()
    {
        DbConfig m1 = AppConfig.GetDbConfig("m1");

        string connectionString1 = m1.GetConnectionString(true);
        Assert.AreEqual("Server=MySqlHost;User ID=user1;Password=qaz1@wsx;Database=MyNorthwind", connectionString1);


        string connectionString2 = m1.GetConnectionString(false);
        Assert.AreEqual("Server=MySqlHost;User ID=user1;Password=qaz1@wsx", connectionString2);



        DbConfig m2 = AppConfig.GetDbConfig("m2");

        string connectionString3 = m2.GetConnectionString(true);
        Assert.AreEqual("Server=MySqlHost;User ID=user1;Password=\"qaz1=;@wsx\";Database=MyNorthwind;Allow Zero Datetime=True;Convert Zero Datetime=True;", connectionString3);


        string connectionString4 = m2.GetConnectionString(false);
        Assert.AreEqual("Server=MySqlHost;User ID=user1;Password=\"qaz1=;@wsx\";Allow Zero Datetime=True;Convert Zero Datetime=True;", connectionString4);


        DbConfig m3 = AppConfig.GetDbConfig("m1");
        m3.Port = 1025;
        Assert.IsTrue(m3.GetConnectionString(true).Contains("Port=1025;"));
    }

    [TestMethod]
    public void Test_GetPostgreSQLConnectionString()
    {
        DbConfig pg1 = AppConfig.GetDbConfig("pg1");

        string connectionString1 = pg1.GetConnectionString(true);
        Assert.AreEqual("Host=PgSqlHost;Port=5432;Database=mynorthwind;Username=postgres;Password=1qaz7410;Application Name=ClownFish.UnitTest", connectionString1);


        string connectionString2 = pg1.GetConnectionString(false);
        Assert.AreEqual("Host=PgSqlHost;Port=5432;Database=mynorthwind;Username=postgres;Password=1qaz7410;Application Name=ClownFish.UnitTest", connectionString2);



        DbConfig pg3 = AppConfig.GetDbConfig("pg1");
        pg3.Port = 1025;

        string connectionString3 = pg3.GetConnectionString(true);
        Assert.AreEqual("Host=PgSqlHost;Port=1025;Database=mynorthwind;Username=postgres;Password=1qaz7410;Application Name=ClownFish.UnitTest", connectionString3);


        string connectionString4 = pg3.GetConnectionString(false);
        Assert.AreEqual("Host=PgSqlHost;Port=1025;Database=mynorthwind;Username=postgres;Password=1qaz7410;Application Name=ClownFish.UnitTest", connectionString4);
    }


    //[TestMethod]
    //public void Test_GetMongoDbConnectionString()
    //{
    //    DbConfig g1 = new DbConfig {
    //        DbType = DatabaseType.MongoDB,
    //        Server = "localhost",
    //        Database = "MyNorthwind",
    //        Port = 1025,
    //        UserName = "root",
    //        Password = "fish",
    //        Args = "charset=utf8"
    //    };

    //    Console.WriteLine(g1.GetConnectionString(true));

    //    Assert.AreEqual("mongodb://root:fish@localhost:1025/MyNorthwind?charset=utf8", g1.GetConnectionString(true));
    //}


#if TEST_DM
    [TestMethod]
    public void Test_GetDamengConnectionString()
    {
        DbConfig dm1 = AppConfig.GetDbConfig("dm1");
        Console.WriteLine(dm1.GetConnectionString(true));

        Assert.AreEqual("Dm", dm1.GetProviderName());

        Assert.IsTrue(dm1.GetConnectionString(true).Contains("server=PgSqlHost;"));
        Assert.IsTrue(dm1.GetConnectionString(true).Contains("port=15236;"));
        Assert.IsTrue(dm1.GetConnectionString(true).Contains("schema=MyNorthwind;"));
        Assert.IsTrue(dm1.GetConnectionString(true).Contains("user=SYSDBA;"));
        Assert.IsTrue(dm1.GetConnectionString(true).Contains("password=SYSDBA001;"));
        Assert.IsFalse(dm1.GetConnectionString(false).Contains("schema="));

        DbConfig dm3 = AppConfig.GetDbConfig("dm1");
        dm3.Port = 0;
        Assert.IsFalse(dm3.GetConnectionString(false).Contains("port="));
    }
#endif


    [TestMethod]
    public void Test_GetConnectionString_SQLSERVER()
    {
        DbConfig config = new DbConfig {
            DbType = ClownFish.Data.DatabaseType.SQLSERVER,
            Server = "server1",
            UserName = "user1",
            Password = "xxx",
            Database = "db12"
        };


        string expected1 = "Data Source=server1;Initial Catalog=db12;User ID=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        string connectionString1 = config.GetConnectionString(true);
        Assert.AreEqual(expected1, connectionString1);


        string expected2 = "Data Source=server1;User ID=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        string connectionString2 = config.GetConnectionString(false);
        Assert.AreEqual(expected2, connectionString2);
    }

    [TestMethod]
    public void Test_GetConnectionString_SQLSERVER_WithPort()
    {
        DbConfig config = new DbConfig {
            DbType = ClownFish.Data.DatabaseType.SQLSERVER,
            Server = "server1",
            Port = 123,
            UserName = "user1",
            Password = "xxx",
            Database = "db12"
        };


        string expected1 = "Data Source=server1,123;Initial Catalog=db12;User ID=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        string connectionString1 = config.GetConnectionString(true);
        Assert.AreEqual(expected1, connectionString1);

        string expected2 = "Data Source=server1,123;User ID=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        string connectionString2 = config.GetConnectionString(false);
        Assert.AreEqual(expected2, connectionString2);
    }


    [TestMethod]
    public void Test_GetConnectionString_PostgreSQL()
    {
        DbConfig config = new DbConfig {
            DbType = ClownFish.Data.DatabaseType.PostgreSQL,
            Server = "server1",
            UserName = "user1",
            Password = "xxx",
            Database = "db12"
        };


        string expected1 = "Host=server1;Database=db12;Username=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        string connectionString1 = config.GetConnectionString(true);
        Assert.AreEqual(expected1, connectionString1);


        //string expected2 = "Host=server1;Username=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        //string connectionString2 = config.GetConnectionString(false);
        //Console.WriteLine(connectionString2);
        //Assert.AreEqual(expected2, connectionString2);
    }

    [TestMethod]
    public void Test_GetConnectionString_PostgreSQL_WithPort()
    {
        DbConfig config = new DbConfig {
            DbType = ClownFish.Data.DatabaseType.PostgreSQL,
            Server = "server1",
            Port = 123,
            UserName = "user1",
            Password = "xxx",
            Database = "db12"
        };


        string expected1 = "Host=server1;Port=123;Database=db12;Username=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        string connectionString1 = config.GetConnectionString(true);
        Assert.AreEqual(expected1, connectionString1);


        //string expected2 = "Host=server1;Port=123;Username=user1;Password=xxx;Application Name=ClownFish.UnitTest";
        //string connectionString2 = config.GetConnectionString(false);
        //Console.WriteLine(connectionString2);
        //Assert.AreEqual(expected2, connectionString2);
    }



    [TestMethod]
    public void Test_GetConnectionString_MySQL()
    {
        DbConfig config = new DbConfig {
            DbType = ClownFish.Data.DatabaseType.MySQL,
            Server = "server1",
            UserName = "user1",
            Password = "xxx"
        };

        string expected = "Server=server1;User ID=user1;Password=xxx";
        string connectionString = config.GetConnectionString();
        Assert.AreEqual(expected, connectionString);
    }



    [TestMethod]
    public void Test_GetConnectionString_MySQL_Port_2()
    {
        DbConfig config = new DbConfig {
            DbType = ClownFish.Data.DatabaseType.MySQL,
            Server = "server1",
            Port = 147,
            UserName = "user1",
            Password = "xxx"
        };

        string expected = "Server=server1;Port=147;User ID=user1;Password=xxx";
        string connectionString = config.GetConnectionString();
        Assert.AreEqual(expected, connectionString);
    }


}
