using ClownFish.Data.SqlClient;

namespace ClownFish.UnitTest.Data.SqlClient;

[TestClass]
public class MssqlHelperTest
{
    private readonly string _connectionString = ConnectionManager.GetConnection("sqlserver").ConnectionString;

    [TestMethod]
    public void Test_TestConnection()
    {
        // 测试连接是否有效
        MsSqlHelper.TestConnection(_connectionString, 5);
    }


    [TestMethod]
    public void Test_CreateContext()
    {
        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {
            Assert.IsNotNull(db);
        }

        using( DbContext db = MsSqlHelper.CreateContext(_connectionString, "xxx") ) {
            Assert.IsNotNull(db);
            Assert.IsTrue(db.Connection.ConnectionString.Contains("xxx"));
        }

        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.CreateContext(null);
        });
    }


    [TestMethod]
    public void Test_GetVersion()
    {
        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {
            int version = MsSqlHelper.GetVersion(db);
            Assert.IsTrue(version > 2);
        }

        MyAssert.IsError<ArgumentNullException>(() => {
            int version = MsSqlHelper.GetVersion(null);
        });
    }


    [TestMethod]
    public void Test_HideConnectionStringPassword()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.HideConnectionStringPassword(null);
        });


        string text1 = MsSqlHelper.HideConnectionStringPassword(_connectionString);
        Assert.IsTrue(text1.Contains("######"));
    }


    [TestMethod]
    public void Test_GetFields()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetFields(null, "table1");
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                MsSqlHelper.GetFields(db, "");
            });


            var list = MsSqlHelper.GetFields(db, "Products");
            Assert.IsNotNull(list);
            Assert.AreEqual(7, list.Count);
        }
    }



    [TestMethod]
    public void Test_GetFields2()
    {
        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            var fields = MsSqlHelper.GetFields(db, "Customers");
            var field = fields.FirstOrDefault(x => x.Name == "CustomerID");

            field.DefaultValue = "123";
            field.Formular = "xxxx";

            string xx = field.ToString();

            var field2 = field.JsonCloneObject();
            Assert.AreEqual(field.Name, field2.Name);
        }
    }


    [TestMethod]
    public void Test_GetQueryFields()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetQueryFields(null, "select * from xxx");
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                MsSqlHelper.GetQueryFields(db, "");
            });


            string query = "select ProductID,ProductName,CategoryID from Products where ProductID > 2";

            var list = MsSqlHelper.GetQueryFields(db, query);
            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);
        }
    }

    [TestMethod]
    public void Test_GetDatabases()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetDatabases(null);
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            var list = MsSqlHelper.GetDatabases(db);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);

            string text = list.ToArray().Merge(",");
            Assert.IsTrue(text.Contains("MyNorthwind"));
        }
    }


    [TestMethod]
    public void Test_GetTables()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetTables(null);
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            var list = MsSqlHelper.GetTables(db);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);

            string text = list.ToArray().Merge(",");
            Assert.IsTrue(text.Contains("Products"));
        }
    }



    [TestMethod]
    public void Test_GetViews()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetViews(null);
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            var list = MsSqlHelper.GetViews(db);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);

            string text = list.ToArray().Merge(",");
            Assert.IsTrue(text.Contains("OrderDetailView"));
        }
    }


    [TestMethod]
    public void Test_GetStoreProcedures()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetStoreProcedures(null);
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            var list = MsSqlHelper.GetStoreProcedures(db);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);

            string text = list.ToArray().Merge(",");
            Assert.IsTrue(text.Contains("DeleteCustomer"));
        }
    }


    [TestMethod]
    public void Test_GetProcedureCode()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetProcedureCode(null, "xxxxxx");
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                MsSqlHelper.GetProcedureCode(db, "");
            });


            var text = MsSqlHelper.GetProcedureCode(db, "DeleteCustomer");
            Assert.IsTrue(text.HasValue());
            Assert.IsTrue(text.Contains("delete from Customers"));
        }
    }




    [TestMethod]
    public void Test_GetViewCode()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetViewCode(null, "xxxxxx");
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                MsSqlHelper.GetViewCode(db, "");
            });


            var text = MsSqlHelper.GetViewCode(db, "OrderDetailView");
            Assert.IsTrue(text.HasValue());
            Assert.IsTrue(text.Contains("ON dbo.[OrderDetails].ProductID = dbo.Products.ProductID"));
        }
    }



    [TestMethod]
    public void Test_GetTablesStatisticalInformation()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.GetTablesStatisticalInformation(null);
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            var table = MsSqlHelper.GetTablesStatisticalInformation(db);
            Assert.IsNotNull(table);
            Assert.IsTrue(table.Rows.Count > 0);
        }
    }


    [TestMethod]
    public void Test_ValidSql()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.ValidSql(null, "xxxxxx");
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                MsSqlHelper.ValidSql(db, "");
            });


            var text = MsSqlHelper.ValidSql(db, "selet xxx abc");
            Assert.IsTrue(text.HasValue());
           
            var text2 = MsSqlHelper.ValidSql(db, "select * from OrderDetailView");
            Assert.IsNull(text2);
        }
    }


    [TestMethod]
    public void Test_ExecuteScript()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            MsSqlHelper.ExecuteScript(null, "xxxxxx");
        });


        using( DbContext db = MsSqlHelper.CreateContext(_connectionString) ) {

            MsSqlHelper.ExecuteScript(db, "");


            MsSqlHelper.ExecuteScript(db, @"
select getdate();
GO
select getdate();
");
           
        }
    }



    [TestMethod]
    public void Test_M1()
    {
        string connectionString = ConnectionManager.GetConnection("sqlserver").ConnectionString;

        // 测试连接是否有效
        MsSqlHelper.TestConnection(connectionString, 5);

        using( DbContext context = DbContext.Create(connectionString, null) ) {
            // 获取SQLSERVER版本
            int version = MsSqlHelper.GetVersion(context);
            Assert.IsTrue(version > 2);

            var fields = MsSqlHelper.GetFields(context, "Customers");
            var field = fields.FirstOrDefault(x => x.Name == "CustomerID");
            Assert.IsNotNull(field);

            var tableNames = MsSqlHelper.GetTables(context);
            var table = tableNames.FirstOrDefault(x => x.EqualsIgnoreCase("Customers"));
            Assert.IsNotNull(table);
        }

    }


}
