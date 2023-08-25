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
            Assert.AreEqual(DatabaseType.SQLSERVER, DbClientFactory.GetProvider(DatabaseClients.SqlClient).DatabaseType);
            Assert.AreEqual(DatabaseType.SQLSERVER, DbClientFactory.GetProvider(DatabaseClients.SqlClient2).DatabaseType);
        }


        [TestMethod]
        public void Test_ProviderFactory()
        {
            Assert.AreEqual(Microsoft.Data.SqlClient.SqlClientFactory.Instance, DbClientFactory.GetProvider(DatabaseClients.SqlClient).ProviderFactory);
            Assert.AreEqual(Microsoft.Data.SqlClient.SqlClientFactory.Instance, DbClientFactory.GetProvider(DatabaseClients.SqlClient2).ProviderFactory);
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

   

        }


        [TestMethod]
        public void Test_IsDuplicateInsertException()
        {

            using( DbContext dbContext = DbContext.Create("sqlserver") ) {

                var ex1 = ExceptionHelper.CreateSqlException(1061);
                var ex2 = ExceptionHelper.CreateSqlException(2601);
                var ex3 = ExceptionHelper.CreateSqlException(1063);
                var ex4 = ExceptionHelper.CreateSqlException(2627);

                Assert.IsFalse(dbContext.IsDuplicateInsert(ex1));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex2));
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex3));
                Assert.IsTrue(dbContext.IsDuplicateInsert(ex4));

                var ex5 = ExceptionHelper.CreateMyDbException();
                Assert.IsFalse(dbContext.IsDuplicateInsert(ex5));
            }

        }

    }
}
