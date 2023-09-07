namespace ClownFish.UnitTest.Data.MultiDB;

[TestClass]
public class ClientProviderTest : BaseTest
{
    [TestMethod]
    public void Test_DatabaseType()
    {
        Assert.AreEqual(DatabaseType.MySQL, DbClientFactory.GetProvider(DatabaseClients.MySqlClient).DatabaseType);

    }


    [TestMethod]
    public void Test_ProviderFactory()
    {
        Assert.AreEqual(MySql.Data.MySqlClient.MySqlClientFactory.Instance, DbClientFactory.GetProvider(DatabaseClients.MySqlClient).ProviderFactory);
    }




    [TestMethod]
    public void Test_GetPagedCommand()
    {
        string sql = GetSql("GetCustomerList");
        var args = new { MaxCustomerID = 100 };

        PagingInfo pagingInfo = new PagingInfo() {
            PageIndex = 0,
            PageSize = 20,
        };



        using( DbContext db = DbContext.Create("mysql") ) {

            var query = db.CPQuery.Create(sql, args);
            var queries = db.ClientProvider.GetPagedCommand(query, pagingInfo);

            Assert.IsNotNull(queries.ListQuery);
            Assert.IsNotNull(queries.CountQuery);
            Assert.AreEqual(1, queries.ListQuery.Command.Parameters.Count);
            Assert.AreEqual(1, queries.CountQuery.Command.Parameters.Count);
        }


    }

    [TestMethod]
    public void Test_GetPagedCommand_NeedCount_isFalse()
    {
        string sql = GetSql("GetCustomerList");
        var args = new { MaxCustomerID = 100 };

        PagingInfo pagingInfo = new PagingInfo() {
            PageIndex = 0,
            PageSize = 20,
            NeedCount = false
        };


        using( DbContext db = DbContext.Create("mysql") ) {

            var query = db.CPQuery.Create(sql, args);
            var queries = db.ClientProvider.GetPagedCommand(query, pagingInfo);

            Assert.IsNotNull(queries.ListQuery);
            Assert.IsNull(queries.CountQuery);
            Assert.AreEqual(1, queries.ListQuery.Command.Parameters.Count);
        }



    }


    [TestMethod]
    public void Test_IsDuplicateInsertException()
    {

        using( DbContext dbContext = DbContext.Create("mysql") ) {

            var ex1 = ExceptionHelper.CreateMySqlException1(1061);
            var ex2 = ExceptionHelper.CreateMySqlException1(1062);
            var ex3 = ExceptionHelper.CreateMySqlException1(1063);

            Assert.IsFalse(dbContext.IsDuplicateInsert(ex1));
            Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
            Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));

            var ex4 = ExceptionHelper.CreateMyDbException();
            Assert.IsFalse(dbContext.IsDuplicateInsert(ex4));
        }

    }

}
