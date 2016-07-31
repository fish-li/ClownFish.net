using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class XmlCommandTest : BaseTest
	{
		private static readonly string s_newName = Guid.NewGuid().ToString();
		
		
		[TestMethod]
		public void Test_XmlCommand_CRUD()
		{
			using( DbContext db = DbContext.Create() ) {
				db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

				var newCustomer = new {
					CustomerName = s_newName,
					ContactName = Guid.NewGuid().ToString(),
					Address = "111111 Address",
					PostalCode = "111111",
					Tel = "123456789"
				};

				// 插入一条记录
				db.XmlCommand.Create("InsertCustomer", newCustomer).ExecuteNonQuery();

				// 读取刚插入的记录
				var queryArgument = new {CustomerName = s_newName};
				Customer customer = db.XmlCommand.Create("GetCustomerByName", queryArgument).ToSingle<Customer>();

				// 验证插入与读取
				Assert.IsNotNull(customer);
				Assert.AreEqual(newCustomer.ContactName, customer.ContactName);





				// 准备更新数据
				Customer updateArgument = new Customer {
					CustomerID = customer.CustomerID,
					CustomerName = newCustomer.CustomerName,
					ContactName = newCustomer.ContactName,
					Address = Guid.NewGuid().ToString(),
					PostalCode = newCustomer.PostalCode,
					Tel = newCustomer.Tel
				};

				// 更新记录
				db.XmlCommand.Create("UpdateCustomer", updateArgument).ExecuteNonQuery();

				// 读取刚更新的记录
				var queryArgument2 = new { CustomerID = customer.CustomerID };
				Customer customer2 = db.XmlCommand.Create("GetCustomerById", queryArgument2).ToSingle<Customer>();

				// 验证更新与读取
				Assert.IsNotNull(customer2);
				Assert.AreEqual(updateArgument.Address, customer2.Address);


				// 删除记录
				var deleteArgument = new { CustomerID = customer.CustomerID };
				db.XmlCommand.Create("DeleteCustomer", deleteArgument).ExecuteNonQuery();

				// 验证删除			
				Customer customer3 = db.XmlCommand.Create("GetCustomerById", queryArgument2).ToSingle<Customer>();
				Assert.IsNull(customer3);


				db.Commit();
			}
		}


		[TestMethod]
		public void Test_XmlCommand_GetList()
		{
			using( DbContext db = DbContext.Create() ) {
				var queryArgument = new { MaxCustomerID = 10 };
				List <Customer> list = db.XmlCommand.Create("GetCustomerList", queryArgument).ToList<Customer>();

				Assert.IsNotNull(list);
			}
		}


		[TestMethod]
		public void Test_XmlCommand_IntArray()
		{
			int[] customerIdArray = { 1, 2, 3, 4, 5 };
			// 注意：下面二个参数名，它们只是SQL语句中的占位符，在替换时是区分大小写的。
			var queryArgument = new { table = "dbo.Customers",  CustomerID = customerIdArray };

			using( ConnectionScope scope = ConnectionScope.Create() ) {
				XmlCommand query = XmlCommand.Create("GetCustomerListById", queryArgument);

				string commandText = query.Command.CommandText;
				Console.WriteLine(commandText);

				// 断言占位符已被替换
				Assert.AreEqual(
					// 注意：int[] 不会生成命令参数，将直接输出到SQL中
					"select * from dbo.Customers where CustomerID in (1,2,3,4,5)",
					commandText
					);

				// 断言参数已产生
				Assert.AreEqual(0, query.Command.Parameters.Count);	
			}
		}


		[TestMethod]
		public void Test_XmlCommand_StringArray()
		{
			string[] customerIdArray = { "1", "2", "3", "4", "5" };
			// 注意：下面二个参数名，它们只是SQL语句中的占位符，在替换时是区分大小写的。
			var queryArgument = new { table = "dbo.Customers", CustomerID = customerIdArray };

			using( ConnectionScope scope = ConnectionScope.Create() ) {
				XmlCommand query = XmlCommand.Create("GetCustomerListById", queryArgument);

				string commandText = query.Command.CommandText;
				Console.WriteLine(commandText);
				
				// 断言占位符已被替换
				Assert.AreEqual(
					"select * from dbo.Customers where CustomerID in (@x1,@x2,@x3,@x4,@x5)",
					commandText
					);

				// 断言参数已产生
				Assert.AreEqual(5, query.Command.Parameters.Count);
			}
		}

		[TestMethod]
		public void Test_XmlCommand_SubQuery()
		{
			using( ConnectionScope scope = ConnectionScope.Create() ) {
				CPQuery subQuery = CPQuery.Create() + "Tel like " + "021%".AsQueryParameter();

				int[] customerIdArray = { 1, 2, 3, 4, 5 };
				// 注意：下面二个参数名，它们只是SQL语句中的占位符，在替换时是区分大小写的。
				var queryArgument = new {
					table = "dbo.Customers", 
					CustomerID = customerIdArray,
					filter = subQuery
				};


				XmlCommand query = XmlCommand.Create("FindCustomers", queryArgument);

				string commandText = query.Command.CommandText;
				Console.WriteLine(commandText);


				// 断言占位符已被替换
				Assert.AreEqual(
					"select * from dbo.Customers where CustomerID in (1,2,3,4,5) and Tel like @p1",
					commandText
					);

				// 断言参数已产生
				Assert.AreEqual(1, query.Command.Parameters.Count);


				List<Customer> list = query.ToList<Customer>();

				// 这里不需要检查 list 的结果，因为结果不重要，只要能正确拼接成有效的SQL就行了。
				Assert.IsNotNull(list);
				Console.WriteLine("list.Count: " + list.Count);

				foreach( Customer c in list )
					Assert.IsTrue(c.CustomerID <= 5);
			}
		}


		

	}
}
