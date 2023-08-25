using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.MultiDB
{
	[TestClass]
	public class ProviderFactoryHelperTest : BaseTest
	{
		[TestMethod]
		public void Test_GetDbProviderFactory()
		{
			DbProviderFactory factory1 = DbClientFactory.GetDbProviderFactory("");
			DbProviderFactory factory2 = DbClientFactory.GetDbProviderFactory(DatabaseClients.SqlClient);
            DbProviderFactory factory3 = DbClientFactory.GetDbProviderFactory(DatabaseClients.MySqlClient);
            DbProviderFactory factory4 = DbClientFactory.GetDbProviderFactory(DatabaseClients.PostgreSQL);
            DbProviderFactory factory5 = DbClientFactory.GetDbProviderFactory(DatabaseClients.SQLite);

            //DbProviderFactory factory6 = ProviderFactoryHelper.GetDbProviderFactory("System.Data.Odbc");
            //DbProviderFactory factory7 = ProviderFactoryHelper.GetDbProviderFactory("System.Data.OleDb");

            Assert.IsTrue(object.ReferenceEquals(SqlClientFactory.Instance, factory1));
			Assert.IsTrue(object.ReferenceEquals(SqlClientFactory.Instance, factory2));
            Assert.IsTrue(object.ReferenceEquals(MySqlConnector.MySqlConnectorFactory.Instance, factory3));
            Assert.IsTrue(object.ReferenceEquals(Npgsql.NpgsqlFactory.Instance, factory4));
            Assert.IsTrue(object.ReferenceEquals(System.Data.SQLite.SQLiteFactory.Instance, factory5));


            //Assert.IsTrue(object.ReferenceEquals(System.Data.Odbc.OdbcFactory.Instance, factory6));
            //Assert.IsTrue(object.ReferenceEquals(System.Data.OleDb.OleDbFactory.Instance, factory7));


        }

	}
}
