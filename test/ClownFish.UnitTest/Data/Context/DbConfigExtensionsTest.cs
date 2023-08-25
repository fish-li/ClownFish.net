using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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


        DbConfig m2 = AppConfig.GetDbConfig("m2");
        using DbContext db2 = m2.CreateDbContext(false);
        Assert.AreEqual(DatabaseType.MySQL, db2.DatabaseType);


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

        using DbContext db2 = m1.CreateDbContext(false, "MySql.Data");
        Assert.IsInstanceOfType(db2.Connection, typeof(MySql.Data.MySqlClient.MySqlConnection));
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

        DbConfig dm1 = AppConfig.GetDbConfig("dm1");
        Assert.AreEqual("Dm", dm1.GetProviderName());


        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DbConfigExtensions.GetProviderName(null);
        });

        MyAssert.IsError<NotSupportedException>(() => {
            DbConfig dbConfig1 = new DbConfig { DbType = DatabaseType.Unknow };
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
            DbConfig dbConfig1 = new DbConfig { DbType = DatabaseType.Unknow };
            _ = DbConfigExtensions.GetConnectionString(dbConfig1);
        });
    }


    [TestMethod]
    public void Test_GetMsSqlConnectionString()
    {
        DbConfig s1 = AppConfig.GetDbConfig("s1");
        Console.WriteLine(s1.GetConnectionString(true));

        Assert.IsTrue(s1.GetConnectionString(true).Contains("Server=MsSqlHost;"));
        Assert.IsTrue(s1.GetConnectionString(true).Contains("Database=MyNorthwind;"));
        Assert.IsTrue(s1.GetConnectionString(true).Contains("Uid=user1;"));
        Assert.IsTrue(s1.GetConnectionString(true).Contains("Pwd=qaz1@wsx;"));
        Assert.IsTrue(s1.GetConnectionString(true).Contains("Application Name=ClownFish.UnitTest;"));
        Assert.IsFalse(s1.GetConnectionString(false).Contains("Database="));


        DbConfig s2 = AppConfig.GetDbConfig("s1");
        s2.Port = 1025;
        Console.WriteLine(s2.GetConnectionString(true));

        Assert.IsTrue(s2.GetConnectionString(true).Contains("Server=MsSqlHost,1025;"));
        Assert.IsTrue(s2.GetConnectionString(true).Contains("Database=MyNorthwind;"));
        Assert.IsTrue(s2.GetConnectionString(true).Contains("Uid=user1;"));
        Assert.IsTrue(s2.GetConnectionString(true).Contains("Pwd=qaz1@wsx;"));
        Assert.IsTrue(s2.GetConnectionString(true).Contains("Application Name=ClownFish.UnitTest;"));
        Assert.IsFalse(s2.GetConnectionString(false).Contains("Database="));
    }

    [TestMethod]
    public void Test_GetMySqlConnectionString()
    {
        DbConfig m1 = AppConfig.GetDbConfig("m1");
        Console.WriteLine(m1.GetConnectionString(true));
        
        Assert.IsTrue(m1.GetConnectionString(true).Contains("Server=MySqlHost;"));
        Assert.IsTrue(m1.GetConnectionString(true).Contains("Database=MyNorthwind;"));
        Assert.IsTrue(m1.GetConnectionString(true).Contains("Uid=user1;"));
        Assert.IsTrue(m1.GetConnectionString(true).Contains("Pwd=qaz1@wsx;"));
        Assert.IsFalse(m1.GetConnectionString(false).Contains("Database="));



        DbConfig m2 = AppConfig.GetDbConfig("m2");
        Console.WriteLine(m2.GetConnectionString(true));

        Assert.IsTrue(m2.GetConnectionString(true).Contains("Server=MySqlHost;"));
        Assert.IsTrue(m2.GetConnectionString(true).Contains("Database=MyNorthwind;"));
        Assert.IsTrue(m2.GetConnectionString(true).Contains("Uid=user1;"));
        Assert.IsTrue(m2.GetConnectionString(true).Contains("Pwd=qaz1@wsx;"));
        Assert.IsTrue(m2.GetConnectionString(true).Contains("Allow Zero Datetime=True;"));
        Assert.IsTrue(m2.GetConnectionString(true).Contains("Convert Zero Datetime=True;"));
        Assert.IsFalse(m2.GetConnectionString(false).Contains("Database="));


        DbConfig m3 = AppConfig.GetDbConfig("m1");
        m3.Port = 1025;
        Assert.IsTrue(m3.GetConnectionString(true).Contains("Port=1025;"));
    }

    [TestMethod]
    public void Test_GetPostgreSQLConnectionString()
    {
        DbConfig pg1 = AppConfig.GetDbConfig("pg1");
        Console.WriteLine(pg1.GetConnectionString(true));

        Assert.IsTrue(pg1.GetConnectionString(true).Contains("Host=PgSqlHost;"));
        Assert.IsTrue(pg1.GetConnectionString(true).Contains("Database=mynorthwind;"));
        Assert.IsTrue(pg1.GetConnectionString(true).Contains("Username=postgres;"));
        Assert.IsTrue(pg1.GetConnectionString(true).Contains("Password=1qaz7410;"));
        Assert.IsTrue(pg1.GetConnectionString(true).Contains("Application Name=ClownFish.UnitTest;"));
        Assert.IsFalse(pg1.GetConnectionString(false).Contains("Database="));



        DbConfig pg3 = AppConfig.GetDbConfig("pg1");
        pg3.Port = 1025;
        Assert.IsTrue(pg3.GetConnectionString(true).Contains("Port=1025;"));
    }


    [TestMethod]
    public void Test_GetMongoDbConnectionString()
    {
        DbConfig g1 = new DbConfig {
            DbType = DatabaseType.MongoDB,
            Server = "localhost",
            Database = "MyNorthwind",
            Port = 1025,
            UserName = "root",
            Password = "fish",
            Args = "charset=utf8"
        };

        Console.WriteLine(g1.GetConnectionString(true));

        Assert.AreEqual("mongodb://root:fish@localhost:1025/MyNorthwind?charset=utf8", g1.GetConnectionString(true));
    }

    [TestMethod]
    public void Test_GetDamengConnectionString()
    {
        DbConfig dm1 = AppConfig.GetDbConfig("dm1");
        Console.WriteLine(dm1.GetConnectionString(true));

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

  
}
