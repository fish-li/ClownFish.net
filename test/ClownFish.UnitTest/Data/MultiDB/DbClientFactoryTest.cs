using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.MultiDB.MsSQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.MultiDB
{
    [TestClass]
    public class DbClientFactoryTest
    {
        [TestMethod]
        public void Test()
        {
            DbProviderFactory factory1 = DbClientFactory.GetDbProviderFactory(null);
            Assert.AreEqual(SqlClientFactory.Instance, factory1);

            DbProviderFactory factory2 = DbClientFactory.GetDbProviderFactory("");
            Assert.AreEqual(SqlClientFactory.Instance, factory2);

            DbProviderFactory factory3 = DbClientFactory.GetDbProviderFactory("System.Data.SqlClient");
            Assert.AreEqual(SqlClientFactory.Instance, factory3);


            DbProviderFactory factory4 = DbClientFactory.GetDbProviderFactory("MySql.Data.MySqlClient");
            Assert.AreEqual(MySqlConnector.MySqlConnectorFactory.Instance, factory4);

            DbProviderFactory factory5 = DbClientFactory.GetDbProviderFactory("Npgsql");
            Assert.AreEqual(Npgsql.NpgsqlFactory.Instance, factory5);



            DbProviderFactory factory6 = DbClientFactory.GetDbProviderFactory("MySql.Data");
            Assert.AreEqual(MySql.Data.MySqlClient.MySqlClientFactory.Instance, factory6);

            DbProviderFactory factory7 = DbClientFactory.GetDbProviderFactory("MySqlConnector");
            Assert.AreEqual(MySqlConnector.MySqlConnectorFactory.Instance, factory7);
        }


        [TestMethod]
        public void Test2()
        {
            using( DbContext dbContext = DbContext.Create("mysql") ) {

                string value1 = dbContext.CPQuery.Create("select now();").ExecuteScalar<string>();

                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySqlConnector.MySqlConnection"));
            }


            var css = ClownFish.Base.AppConfig.GetConnectionString("mysql");


            using( DbContext dbContext = DbContext.Create(css.ConnectionString, "MySql.Data") ) {

                string value1 = dbContext.CPQuery.Create("select now();").ExecuteScalar<string>();

                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySql.Data.MySqlClient.MySqlConnection"));
            }


            using( DbContext dbContext = DbContext.Create(css.ConnectionString, "MySqlConnector") ) {

                string value1 = dbContext.CPQuery.Create("select now();").ExecuteScalar<string>();

                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySqlConnector.MySqlConnection"));
            }

            using( DbContext dbContext = DbContext.Create(css.ConnectionString, "MySql.Data.MySqlClient") ) {

                string value1 = dbContext.CPQuery.Create("select now();").ExecuteScalar<string>();

                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySqlConnector.MySqlConnection"));
            }
        }



        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<NotSupportedException>(() => {
                _ = DbClientFactory.GetDbProviderFactory("xxx");
            });


            MyAssert.IsError<ArgumentNullException>(() => {
                DbClientFactory.RegisterProvider(null, MsSqlClientProvider.Instance);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DbClientFactory.RegisterProvider("xxx", null);
            });
        }

    }
}
