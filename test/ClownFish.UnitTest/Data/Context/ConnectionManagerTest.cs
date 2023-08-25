using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Data;
using ClownFish.Base;

namespace ClownFish.UnitTest.Data.Context
{
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

			Assert.AreEqual(connection1.ConnectionString, connection2.ConnectionString);
			Assert.AreEqual(connection1.ProviderName, connection2.ProviderName);
		}



		[TestMethod]
		public void Test_GetConnection_名称不存在()
		{
            MyAssert.IsError<ArgumentOutOfRangeException>(() => { 
                _ = ConnectionManager.GetConnection("xxxxx");
            });


            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                _ = ConnectionManager.GetDbConfig("xxxxx");
            });
        }


		[TestMethod]
		public void Test_ConnectionScope_Create()
		{
			ConnectionInfo info = ConnectionManager.GetConnection("sqlserver");

			using( ConnectionScope 
					scope1 = ConnectionScope.Create(),
					scope2 = ConnectionScope.Create("sqlserver"),
					scope3 = ConnectionScope.Create(info.ConnectionString, info.ProviderName)
				) {

				ConnectionInfo connection1 = scope1.Context.ConnectionInfo;
				ConnectionInfo connection2 = scope2.Context.ConnectionInfo;
				ConnectionInfo connection3 = scope3.Context.ConnectionInfo;


				Assert.AreEqual(connection1.ConnectionString, connection2.ConnectionString);
				Assert.AreEqual(connection1.ProviderName, connection2.ProviderName);

				Assert.AreEqual(connection1.ConnectionString, connection3.ConnectionString);
				Assert.AreEqual(connection1.ProviderName, connection3.ProviderName);
			}
		}


        [TestMethod]
        public void Test_DbConfigs()
        {
            var configs = AppConfig.GetConfigObject().GetConfiguration().DbConfigs;
            Assert.IsNotNull(AppConfig.GetDbConfig("s1"));
            Assert.IsNotNull(AppConfig.GetDbConfig("s2"));
            Assert.IsNotNull(AppConfig.GetDbConfig("m1"));
            Assert.IsNotNull(AppConfig.GetDbConfig("m2"));
            Assert.IsNotNull(AppConfig.GetDbConfig("pg1"));
            Assert.IsNotNull(AppConfig.GetDbConfig("dm1"));

            DbConfig s1 = AppConfig.GetDbConfig("s1");
            Assert.AreEqual(DatabaseType.SQLSERVER, s1.DbType);
            Assert.AreEqual("MsSqlHost", s1.Server);
            Assert.AreEqual(0, s1.Port);
            Assert.AreEqual("MyNorthwind", s1.Database);
            Assert.AreEqual("user1", s1.UserName);
            Assert.AreEqual("qaz1@wsx", s1.Password);


            DbConfig m2 = AppConfig.GetDbConfig("m2");
            Assert.AreEqual(DatabaseType.MySQL, m2.DbType);
            Assert.AreEqual("MySqlHost", m2.Server);
            Assert.AreEqual(0, m2.Port);
            Assert.AreEqual("MyNorthwind", m2.Database);
            Assert.AreEqual("user1", m2.UserName);
            Assert.AreEqual("qaz1@wsx", m2.Password);
            Assert.AreEqual("Allow Zero Datetime=True;Convert Zero Datetime=True;", m2.Args);

            DbConfig dm1 = AppConfig.GetDbConfig("dm1");
            Assert.AreEqual(DatabaseType.DaMeng, dm1.DbType);
            Assert.AreEqual("PgSqlHost", dm1.Server);
            Assert.AreEqual(15236, dm1.Port);
        }


        [TestMethod]
        public void Test_GetDbConfig()
        {
            ConnectionInfo conn1 = ConnectionManager.GetConnection("sqlserver");
            DbConfig config1 = ConnectionManager.GetDbConfig("s1");

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
    }
}
