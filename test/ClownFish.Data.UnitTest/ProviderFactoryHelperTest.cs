using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class ProviderFactoryHelperTest : BaseTest
	{
		[TestMethod]
		public void Test_GetDbProviderFactory()
		{
			DbProviderFactory factory1 = ProviderFactoryHelper.GetDbProviderFactory("");
			DbProviderFactory factory2 = ProviderFactoryHelper.GetDbProviderFactory("System.Data.SqlClient");
			DbProviderFactory factory3 = ProviderFactoryHelper.GetDbProviderFactory("System.Data.Odbc");
			DbProviderFactory factory4 = ProviderFactoryHelper.GetDbProviderFactory("System.Data.OleDb");

			Assert.IsTrue(object.ReferenceEquals(System.Data.SqlClient.SqlClientFactory.Instance, factory1));
			Assert.IsTrue(object.ReferenceEquals(System.Data.SqlClient.SqlClientFactory.Instance, factory2));
			Assert.IsTrue(object.ReferenceEquals(System.Data.Odbc.OdbcFactory.Instance, factory3));
			Assert.IsTrue(object.ReferenceEquals(System.Data.OleDb.OleDbFactory.Instance, factory4));


		}

	}
}
