using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.MultiDB.MsSQL;
using ClownFish.Data.MultiDB.MySQL;
using ClownFish.Data.MultiDB.PostgreSQL;
using ClownFish.Data.MultiDB.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.MultiDB
{
    [TestClass]
    public class ClientProviderTest : BaseTest
    {
        [TestMethod]
        public void Test_DatabaseType()
        {
            Assert.AreEqual(DatabaseType.SQLSERVER, DbClientFactory.GetProvider(null).DatabaseType);
            Assert.AreEqual(DatabaseType.SQLSERVER, DbClientFactory.GetProvider(DatabaseClients.SqlClient).DatabaseType);

            Assert.AreEqual(DatabaseType.MySQL, DbClientFactory.GetProvider(DatabaseClients.MySqlClient).DatabaseType);
            Assert.AreEqual(DatabaseType.MySQL, DbClientFactory.GetProvider("MySql.Data").DatabaseType);
            Assert.AreEqual(DatabaseType.MySQL, DbClientFactory.GetProvider("MySqlConnector").DatabaseType);

            Assert.AreEqual(DatabaseType.PostgreSQL, DbClientFactory.GetProvider(DatabaseClients.PostgreSQL).DatabaseType);
            Assert.AreEqual(DatabaseType.SQLite, DbClientFactory.GetProvider(DatabaseClients.SQLite).DatabaseType);
        }


        [TestMethod]
        public void Test_ProviderFactory()
        {
            Assert.AreEqual(SqlClientFactory.Instance, DbClientFactory.GetProvider(null).ProviderFactory);
            Assert.AreEqual(SqlClientFactory.Instance, DbClientFactory.GetProvider(DatabaseClients.SqlClient).ProviderFactory);

            Assert.AreEqual(MySqlConnector.MySqlConnectorFactory.Instance, DbClientFactory.GetProvider(DatabaseClients.MySqlClient).ProviderFactory);
            Assert.AreEqual(MySql.Data.MySqlClient.MySqlClientFactory.Instance, DbClientFactory.GetProvider("MySql.Data").ProviderFactory);
            Assert.AreEqual(MySqlConnector.MySqlConnectorFactory.Instance, DbClientFactory.GetProvider("MySqlConnector").ProviderFactory);

            Assert.AreEqual(Npgsql.NpgsqlFactory.Instance, DbClientFactory.GetProvider(DatabaseClients.PostgreSQL).ProviderFactory);
            Assert.AreEqual(System.Data.SQLite.SQLiteFactory.Instance, DbClientFactory.GetProvider(DatabaseClients.SQLite).ProviderFactory);

        }


        [TestMethod]
        public void Test_GetSymbolFullName()
        {
            Assert.AreEqual("[xx]", MsSqlClientProvider.Instance.GetObjectFullName("xx"));
            Assert.AreEqual("`xx`", MySqlDataClientProvider.Instance.GetObjectFullName("xx"));
            Assert.AreEqual("`xx`", MySqlConnectorClientProvider.Instance.GetObjectFullName("xx"));
            Assert.AreEqual("\"xx\"", PostgreSqlClientProvider.Instance.GetObjectFullName("xx"));
            Assert.AreEqual("[xx]", SQLiteClientProvider.Instance.GetObjectFullName("xx"));
        }


        [TestMethod]
        public void Test_GetParamterName()
        {
            Assert.AreEqual("@xx", MsSqlClientProvider.Instance.GetParamterName("xx"));
            Assert.AreEqual("@xx", MySqlDataClientProvider.Instance.GetParamterName("xx"));
            Assert.AreEqual("@xx", MySqlConnectorClientProvider.Instance.GetParamterName("xx"));
            Assert.AreEqual("@xx", PostgreSqlClientProvider.Instance.GetParamterName("xx"));
            Assert.AreEqual("@xx", SQLiteClientProvider.Instance.GetParamterName("xx"));
        }

        [TestMethod]
        public void Test_GetParamterPlaceholder()
        {
            Assert.AreEqual("@xx", MsSqlClientProvider.Instance.GetParamterPlaceholder("xx"));
            Assert.AreEqual("@xx", MySqlDataClientProvider.Instance.GetParamterPlaceholder("xx"));
            Assert.AreEqual("@xx", MySqlConnectorClientProvider.Instance.GetParamterPlaceholder("xx"));
            Assert.AreEqual("@xx", PostgreSqlClientProvider.Instance.GetParamterPlaceholder("xx"));
            Assert.AreEqual("@xx", SQLiteClientProvider.Instance.GetParamterPlaceholder("xx"));
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


            using( DbContext db = DbContext.Create("sqlserver") ) {

                var query = db.CPQuery.Create(sql, args);
                var queries = db.ClientProvider.GetPagedCommand(query, pagingInfo);

                Assert.IsNotNull(queries.ListQuery);
                Assert.IsNotNull(queries.CountQuery);
                Assert.AreEqual(1, queries.ListQuery.Command.Parameters.Count);
                Assert.AreEqual(1, queries.CountQuery.Command.Parameters.Count);
            }

            using( DbContext db = DbContext.Create("mysql") ) {

                var query = db.CPQuery.Create(sql, args);
                var queries = db.ClientProvider.GetPagedCommand(query, pagingInfo);

                Assert.IsNotNull(queries.ListQuery);
                Assert.IsNotNull(queries.CountQuery);
                Assert.AreEqual(1, queries.ListQuery.Command.Parameters.Count);
                Assert.AreEqual(1, queries.CountQuery.Command.Parameters.Count);
            }

            using( DbContext db = DbContext.Create("postgresql") ) {

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


            using( DbContext db = DbContext.Create("sqlserver") ) {

                var query = db.CPQuery.Create(sql, args);
                var queries = db.ClientProvider.GetPagedCommand(query, pagingInfo);

                Assert.IsNotNull(queries.ListQuery);
                Assert.IsNull(queries.CountQuery);
                Assert.AreEqual(1, queries.ListQuery.Command.Parameters.Count);
            }

            using( DbContext db = DbContext.Create("mysql") ) {

                var query = db.CPQuery.Create(sql, args);
                var queries = db.ClientProvider.GetPagedCommand(query, pagingInfo);

                Assert.IsNotNull(queries.ListQuery);
                Assert.IsNull(queries.CountQuery);
                Assert.AreEqual(1, queries.ListQuery.Command.Parameters.Count);
            }

            using( DbContext db = DbContext.Create("postgresql") ) {

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
            using( DbContext dbContext = DbContext.Create("sqlserver") ) {

                SqlException ex1 = ExceptionHelper.CreateSqlException(2601);
                SqlException ex2 = ExceptionHelper.CreateSqlException(2627);
                SqlException ex3 = ExceptionHelper.CreateSqlException(2222);

                Assert.IsTrue(dbContext.IsDuplicateInsert(ex1));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));

                var ex4 = ExceptionHelper.CreateMyDbException();
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex4));
            }

            using( DbContext dbContext = DbContext.Create("mysql") ) {

                var ex1 = ExceptionHelper.CreateMySqlException2(1061);
                var ex2 = ExceptionHelper.CreateMySqlException2(1062);
                var ex3 = ExceptionHelper.CreateMySqlException2(1063);

                Assert.IsFalse(dbContext.IsDuplicateInsert(ex1));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));

                var ex4 = ExceptionHelper.CreateMyDbException();
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex4));
            }


            using( DbContext dbContext = DbContext.Create("mysql", "MySqlConnector") ) {

                var ex1 = ExceptionHelper.CreateMySqlException2(1061);
                var ex2 = ExceptionHelper.CreateMySqlException2(1062);
                var ex3 = ExceptionHelper.CreateMySqlException2(1063);

                Assert.IsFalse(dbContext.IsDuplicateInsert(ex1));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));

                var ex4 = ExceptionHelper.CreateMyDbException();
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex4));
            }

            using( DbContext dbContext = DbContext.Create("mysql", "MySql.Data") ) {

                var ex1 = ExceptionHelper.CreateMySqlException1(1061);
                var ex2 = ExceptionHelper.CreateMySqlException1(1062);
                var ex3 = ExceptionHelper.CreateMySqlException1(1063);

                Assert.IsFalse(dbContext.IsDuplicateInsert(ex1));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));

                var ex4 = ExceptionHelper.CreateMyDbException();
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex4));
            }


            using( DbContext dbContext = DbContext.Create("postgresql") ) {

                var ex1 = ExceptionHelper.CreatePostgresException("23504");
                var ex2 = ExceptionHelper.CreatePostgresException("23505");
                var ex3 = ExceptionHelper.CreatePostgresException("23506");

                Assert.IsFalse(dbContext.IsDuplicateInsert(ex1));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));

                var ex4 = ExceptionHelper.CreateMyDbException();
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex4));
            }


            using( DbContext dbContext = DbContext.Create("xxxxxx", DatabaseClients.SQLite) ) {

                var ex1 = ExceptionHelper.CreateSQLiteException(2066);
                var ex2 = ExceptionHelper.CreateSQLiteException(2067);
                var ex3 = ExceptionHelper.CreateSQLiteException(2068);                

                Assert.IsFalse(dbContext.IsDuplicateInsert(ex1));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));

                var ex4 = ExceptionHelper.CreateMyDbException();
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex4));
            }

        }

    }
}
