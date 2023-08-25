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
            DbProviderFactory factory3 = DbClientFactory.GetDbProviderFactory(DatabaseClients.SqlClient);
            Assert.IsTrue(object.ReferenceEquals(Microsoft.Data.SqlClient.SqlClientFactory.Instance, factory3));

            DbProviderFactory factory4 = DbClientFactory.GetDbProviderFactory(DatabaseClients.SqlClient2);
            Assert.IsTrue(object.ReferenceEquals(Microsoft.Data.SqlClient.SqlClientFactory.Instance, factory4));

        }

	}
}
