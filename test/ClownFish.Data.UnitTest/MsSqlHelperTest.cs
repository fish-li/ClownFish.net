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

			using( DbContext context = (DbContext)connectionString ) {
				// 获取SQLSERVER版本
				int version = MsSqlHelper.GetVersion(context);
				Assert.IsTrue(version > 2);

				var fields = MsSqlHelper.GetTableFields(context, "Customers");
				var field = fields.FirstOrDefault(x => x.Name == "CustomerID");
				Assert.IsNotNull(field);

				var tableNames = MsSqlHelper.GetTableNames(context);
				var table = tableNames.FirstOrDefault(x => x.EqualsIgnoreCase("Customers"));
				Assert.IsNotNull(table);
			}

			//TODO: 以后还要补充测试用例
		}
	}
}
