namespace ClownFish.UnitTest.Data.Context;

[TestClass]
public class DbContextTest
{
    [TestMethod]
    public async Task Test_OpenConnection()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder("server=xx;database=xx;uid=xx;pwd=xx");
        builder.ConnectTimeout = 1;

        using( DbContext db = DbContext.Create(builder.ConnectionString, "") ) {

            MyAssert.IsError<SqlException>(() => {
                db.OpenConnection();
            });


           await  MyAssert.IsErrorAsync<SqlException>(async () => {
               await  db.OpenConnectionAsync();
            });
        }
    }


    [TestMethod]
    public void Test_BeginTransaction()
    {
        using( DbContext db = DbContext.Create() ) {
            db.OpenConnection();

            MyAssert.IsError<InvalidOperationException>(() => {
                db.Commit();
            });


            db.BeginTransaction();

            MyAssert.IsError<InvalidOperationException>(() => {
                db.BeginTransaction();
            });


            MyAssert.IsError<InvalidOperationException>(() => {
                db.BeginTransaction(IsolationLevel.ReadCommitted);
            });

            db.Commit();
        }
    }


    [TestMethod]
    public void Test_CreateParameter()
    {
        using( DbContext db = DbContext.Create() ) {

            DbParameter p = db.CreateParameter(DbType.String, "abc", 40);

            Assert.IsInstanceOfType(p, typeof(SqlParameter));

            SqlParameter p2 = (SqlParameter)p;
            Assert.AreEqual(40, p2.Size);
            Assert.AreEqual("abc", p2.Value.ToString());
        }
    }

    [TestMethod]
    public void Test_CreateOutParameter()
    {
        using( DbContext db = DbContext.Create() ) {

            DbParameter p = db.CreateOutParameter(DbType.String, "abc", 40);

            Assert.IsInstanceOfType(p, typeof(SqlParameter));

            SqlParameter p2 = (SqlParameter)p;
            Assert.AreEqual(ParameterDirection.Output, p2.Direction);
            Assert.AreEqual(40, p2.Size);
            Assert.AreEqual("abc", p2.Value.ToString());
        }
    }


    [TestMethod]
    public void Test_Ctor_MsSQL()
    {
        ConnectionStringSetting setting = AppConfig.GetConnectionString("sqlserver");
        Assert.AreEqual("sqlserver", setting.Name);

        using System.Data.SqlClient.SqlConnection connection = new (setting.ConnectionString);
        connection.Open();

        // 检验下面这种构造方式
        using DbContext dbContext = DbContext.Create(connection, setting.ProviderName);

        Assert.AreEqual(DatabaseType.SQLSERVER, dbContext.DatabaseType);
        Assert.AreEqual("System.Data.SqlClient", dbContext.ProviderName);

        dbContext.ExtData = "abc123";
        Assert.AreEqual("abc123", dbContext.ExtData.ToString());

        // 执行一个只有 SQLSERVER 才能理解的SQL
        string username = dbContext.CPQuery.Create("select SUSER_NAME()").ExecuteScalar<string>();

        Assert.AreEqual("user1", username);

        MyAssert.IsError<ArgumentNullException>(() => { 
            _ = DbContext.Create((DbConnection)null, setting.ProviderName);
        });        

        MyAssert.IsError<NotSupportedException>(() => {
            _ = DbContext.Create(connection, "xxxxxxxx");
        });

        var db2 = DbContext.Create(connection, "");
        Assert.AreEqual(DatabaseType.SQLSERVER, db2.DatabaseType);
        Assert.AreEqual("System.Data.SqlClient", db2.ProviderName);
    }


    [TestMethod]
    public void Test_Ctor_MySQL()
    {
        ConnectionStringSetting setting = AppConfig.GetConnectionString("mysql");
        MySqlConnector.MySqlConnection connection = new (setting.ConnectionString);
        connection.Open();

        // 检验下面这种构造方式
        using( DbContext dbContext = DbContext.Create(connection, setting.ProviderName, false) ) { 

            Assert.AreEqual(DatabaseType.MySQL, dbContext.DatabaseType);
            Assert.AreEqual("MySql.Data.MySqlClient", dbContext.ProviderName);   // DbContext.Create 第2个参数指定了ProviderName

            // 执行一个只有 MySQL 才能理解的SQL
            string username = dbContext.CPQuery.Create("select CURRENT_USER()").ExecuteScalar<string>();   // user1@%

            Assert.IsTrue(username.StartsWith0("user1@"));
        }
        Assert.IsTrue(connection.State == ConnectionState.Open);


        using( DbContext dbContext2 = DbContext.Create(connection) ) {   // auto close connection
            Assert.AreEqual(DatabaseType.MySQL, dbContext2.DatabaseType);
            Assert.AreEqual("MySqlConnector", dbContext2.ProviderName);      // 没有指定，所以 ProviderName = “命名空间”
        }
        Assert.IsTrue(connection.State == ConnectionState.Closed);

    }
}
