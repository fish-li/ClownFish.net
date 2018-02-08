using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class MsSqlHelperTest
	{
		[TestMethod]
		public void Test_MsSqlHelper()
		{
			string connectionString = ConnectionManager.GetConnection().ConnectionString;

			// 测试连接是否有效
			MsSqlHelper.TestConnection(connectionString, 5);

			// 获取SQLSERVER版本
			int version = MsSqlHelper.GetVersion(connectionString);
			Assert.IsTrue(version > 2);

			var fields = MsSqlHelper.GetTableFields(connectionString, "Customers");
			var field = fields.FirstOrDefault(x => x.Name == "CustomerID");
			Assert.IsNotNull(field);

			var tableNames = MsSqlHelper.GetTableNames(connectionString);
			var table = tableNames.FirstOrDefault(x => x.EqualsIgnoreCase("Customers"));
			Assert.IsNotNull(table);

			//TODO: 以后还要补充测试用例
		}
	}
}
