using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.UnitTest.Models;
using ClownFish.Data.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class ConnectionScopeTest : BaseTest
	{
		[TestMethod]
		public void Test_ConnectionScope事务中使用StoreProcedure_XmlCommand_CPQuery()
		{
			using( ConnectionScope scope = ConnectionScope.Create() ) {
				scope.BeginTransaction();

				var newCustomer = new {
					// 下行代码创建一个输出参数
					CustomerID = scope.Context.CreateOutParameter(DbType.Int32),
					CustomerName = Guid.NewGuid().ToString(),
					ContactName = Guid.NewGuid().ToString(),
					Address = "111111 Address",
					PostalCode = "111111",
					Tel = "123456789"
				};

				// 插入一条记录
				StoreProcedure.Create("InsertCustomer", newCustomer).ExecuteNonQuery();
				// 获取输出参数的返回值
				int newCustomerId = (int)newCustomer.CustomerID.Value;



				var queryArgument = new { CustomerID = newCustomerId };
				Customer customer1 = XmlCommand.Create("GetCustomerById", queryArgument).ToSingle<Customer>();


				Customer customer2 = StoreProcedure.Create("GetCustomerById", queryArgument).ToSingle<Customer>();

				string sql = CPQueryTest.GetSql("GetCustomerById");
				Customer customer3 = CPQuery.Create(sql, queryArgument).ToSingle<Customer>();

				Assert.AreEqual(customer1.CustomerID, customer2.CustomerID);
				Assert.AreEqual(customer1.CustomerID, customer3.CustomerID);

				Assert.AreEqual(customer1.CustomerName, customer2.CustomerName);
				Assert.AreEqual(customer1.CustomerName, customer3.CustomerName);

				Assert.AreEqual(customer1.Address, customer2.Address);
				Assert.AreEqual(customer1.Address, customer3.Address);
				
				scope.Commit();
			}
		}

	}
}
