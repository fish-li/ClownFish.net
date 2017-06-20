using System;
using System.Collections.Generic;
using ClownFish.Data.UnitTest.Models;
using ClownFish.Data.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class CPQueryTest : BaseTestWithConnectionScope
	{
		private static readonly string s_newName = Guid.NewGuid().ToString();

		internal static string GetSql(string xmlcommandName)
		{
			// 这个测试类为了简单，就直接借用XmlCommand中定义的SQL语句

			XmlCommandItem x1 = XmlCommandManager.GetCommand(xmlcommandName);
			return x1.CommandText;
		}

		[TestMethod]
		public void Test_CPQuery的基本CRUD操作()
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
				db.CPQuery.Create(GetSql("InsertCustomer"), newCustomer).ExecuteNonQuery();

				// 读取刚插入的记录
				var queryArgument = new { CustomerName = s_newName };
				Customer customer = db.CPQuery.Create(GetSql("GetCustomerByName"), queryArgument).ToSingle<Customer>();

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
				db.CPQuery.Create(GetSql("UpdateCustomer"), updateArgument).ExecuteNonQuery();

				// 读取刚更新的记录
				var queryArgument2 = new { CustomerID = customer.CustomerID };
				Customer customer2 = db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingle<Customer>();

				// 验证更新与读取
				Assert.IsNotNull(customer2);
				Assert.AreEqual(updateArgument.Address, customer2.Address);


				// 删除记录
				var deleteArgument = new { CustomerID = customer.CustomerID };
				db.CPQuery.Create(GetSql("DeleteCustomer"), deleteArgument).ExecuteNonQuery();

				// 验证删除			
				Customer customer3 = db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingle<Customer>();
				Assert.IsNull(customer3);


				db.Commit();
			}
		}


		

		[TestMethod]
		public void Test_CPQuery加载实体列表()
		{
			using( DbContext db = DbContext.Create() ) {
				var queryArgument = new { MaxCustomerID = 10 };
				List<Customer> list = db.CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToList<Customer>();

				Assert.IsNotNull(list);
			}
		}

		[TestMethod]
		public void Test_CPQuery参数支持INT数组()
		{
			string sql = @"select * from dbo.Customers where CustomerID in ({CustomerID})";

			int[] customerIdArray = { 1, 2, 3, 4, 5 };
			var queryArgument = new { CustomerID = customerIdArray };

			CPQuery query = CPQuery.Create(sql, queryArgument);

			string commandText = query.ToString();
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


		[TestMethod]
		public void Test_CPQuery参数支持STRING数组()
		{
			string sql = @"select * from dbo.Customers where CustomerID in ({CustomerID})";
			string[] customerIdArray = { "1", "2", "3", "4", "5" };
			var queryArgument = new { CustomerID = customerIdArray };

			CPQuery query = CPQuery.Create(sql, queryArgument);

			string commandText = query.ToString();
			Console.WriteLine(commandText);


			// 断言占位符已被替换
			Assert.AreEqual(
				"select * from dbo.Customers where CustomerID in (@x1,@x2,@x3,@x4,@x5)",
				commandText
				);

			// 断言参数已产生
			Assert.AreEqual(5, query.Command.Parameters.Count);
		}



		[TestMethod]
		public void Test_CPQuery嵌套使用()
		{
			CPQuery query1 = "P2 = ".AsCPQuery() + 2;
			CPQuery query2 = "P3 = ".AsCPQuery() + DateTime.Now;
			CPQuery query = CPQuery.Create("select * from t1 where id=@id and {subquery1} and {subquery2}",
					new { id = 3, subquery1 = query1, subquery2 = query2 }
				);

			string commandText = query.ToString();
			Console.WriteLine(commandText);

			Assert.AreEqual("select * from t1 where id=@id and P2 = @p1 and P3 = @p2", commandText);
			Assert.AreEqual(3, query.Command.Parameters.Count);
		}


		[TestMethod]
		public void Test_CPQuery占位符参数()
		{
			CPQuery query = CPQuery.Create("select * from {table} where id=@id",
				new { id = 2, table = "t1".AsSql() });

			string commmandText = query.ToString();
			Console.WriteLine(commmandText);

			Assert.AreEqual("select * from t1 where id=@id", commmandText);
		}


		[TestMethod]
		public void Test_CPQuery与CPQuery相加()
		{
			CPQuery query1 = CPQuery.Create("select * from t1 where id=@id", new { id = 2 });
			CPQuery query2 = CPQuery.Create(";select * from t2 where name=@name", new { name = "abc" });
			CPQuery query3 = query1 + query2;

			string commmandText = query3.ToString();
			Console.WriteLine(commmandText);

			Assert.AreEqual("select * from t1 where id=@id;select * from t2 where name=@name", commmandText);
			Assert.AreEqual(2, query3.Command.Parameters.Count);
		}


        [TestMethod]
        public void Test_CPQuery设置命令超时时间()
        {
            CPQuery query1 = CPQuery.Create("select * from t1").SetCommand(x => x.CommandTimeout = 2);
            Assert.AreEqual(2, query1.Command.CommandTimeout);
        }


    }
}
