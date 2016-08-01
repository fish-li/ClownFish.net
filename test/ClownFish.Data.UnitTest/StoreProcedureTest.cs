using System;
using System.Collections.Generic;
using System.Data;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class StoreProcedureTest : BaseTest
	{
		private static readonly string s_newName = Guid.NewGuid().ToString();


		[TestMethod]
		public void Test_基于SP的CRUD()
		{
			using( DbContext db = DbContext.Create() ) {
				db.BeginTransaction();

				var newCustomer = new {
					// 下行代码创建一个输出参数
					CustomerID = db.CreateOutParameter(DbType.Int32),
					CustomerName = s_newName,
					ContactName = Guid.NewGuid().ToString(),
					Address = "111111 Address",
					PostalCode = "111111",
					Tel = "123456789"
				};

				// 插入一条记录
				db.StoreProcedure.Create("InsertCustomer", newCustomer).ExecuteNonQuery();
				// 获取输出参数的返回值
				int newCustomerId = (int)newCustomer.CustomerID.Value;
				Assert.IsTrue(newCustomerId > 0);


				// 读取刚插入的记录
				var queryArgument = new { CustomerID = newCustomerId };
				Customer customer = db.StoreProcedure.Create("GetCustomerById", queryArgument).ToSingle<Customer>();

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
				db.StoreProcedure.Create("UpdateCustomer", updateArgument).ExecuteNonQuery();

				// 读取刚更新的记录
				var queryArgument2 = new { CustomerID = customer.CustomerID };
				Customer customer2 = db.StoreProcedure.Create("GetCustomerById", queryArgument2).ToSingle<Customer>();

				// 验证更新与读取
				Assert.IsNotNull(customer2);
				Assert.AreEqual(updateArgument.Address, customer2.Address);


				// 删除记录
				var deleteArgument = new { CustomerID = customer.CustomerID };
				db.StoreProcedure.Create("DeleteCustomer", deleteArgument).ExecuteNonQuery();

				// 验证删除			
				Customer customer3 = db.StoreProcedure.Create("GetCustomerById", queryArgument2).ToSingle<Customer>();
				Assert.IsNull(customer3);


				db.Commit();
			}
		}

	

		[TestMethod]
		public void Test_SP加载实体列表_OUTPUT参数()
		{
			using( DbContext db = DbContext.Create() ) {
				var queryArgument = new {
					SearchWord = "上海",
					PageIndex = 1,
					PageSize = 10,
					TotalRecords = 0		// output
				};
				List<Customer> list = db.StoreProcedure.Create("GetCustomerList", queryArgument).ToList<Customer>();

				Assert.IsNotNull(list);
			}
		}

	}
}
