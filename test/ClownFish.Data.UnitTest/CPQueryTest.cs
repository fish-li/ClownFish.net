using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data.UnitTest.Models;
using ClownFish.Data.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class CPQueryTest : BaseTestWithConnectionScope
	{
		private static readonly string s_newName = Guid.NewGuid().ToString();

	
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
		public async Task Test_CPQuery的基本CRUD操作_Async()
		{
			ShowCurrentThread();

			using( DbContext db = DbContext.Create() ) {
				db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

				var newCustomer = new {
					CustomerName = s_newName,
					ContactName = Guid.NewGuid().ToString(),
					Address = "111111 Address",
					PostalCode = "111111",
					Tel = "123456789"
				};

				ShowCurrentThread();
				// 插入一条记录
				await db.CPQuery.Create(GetSql("InsertCustomer"), newCustomer).ExecuteNonQueryAsync();
				ShowCurrentThread();

				// 读取刚插入的记录
				var queryArgument = new { CustomerName = s_newName };
				Customer customer = await db.CPQuery.Create(GetSql("GetCustomerByName"), queryArgument).ToSingleAsync<Customer>();

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
				await db.CPQuery.Create(GetSql("UpdateCustomer"), updateArgument).ExecuteNonQueryAsync();

				// 读取刚更新的记录
				var queryArgument2 = new { CustomerID = customer.CustomerID };
				Customer customer2 = await db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingleAsync<Customer>();

				// 验证更新与读取
				Assert.IsNotNull(customer2);
				Assert.AreEqual(updateArgument.Address, customer2.Address);


				// 删除记录
				var deleteArgument = new { CustomerID = customer.CustomerID };
				await db.CPQuery.Create(GetSql("DeleteCustomer"), deleteArgument).ExecuteNonQueryAsync();

				// 验证删除			
				Customer customer3 = await db.CPQuery.Create(GetSql("GetCustomerById"), queryArgument2).ToSingleAsync<Customer>();
				Assert.IsNull(customer3);

				db.Commit();
			}
		}


		[TestMethod]
		public async Task Test_CPQuery加载实体列表()
		{
			List<Customer> list1 = null;
			using( DbContext db = DbContext.Create() ) {
				var queryArgument = new { MaxCustomerID = 10 };
				list1 = db.CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToList<Customer>();

				Assert.IsNotNull(list1);
			}


			List<Customer> list2 = null;
			using( DbContext db = DbContext.Create() ) {
				var queryArgument = new { MaxCustomerID = 10 };
				list2 = await db.CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToListAsync<Customer>();

				Assert.IsNotNull(list2);
			}


			// 确认二次查询的结果一致
			Assert.AreEqual(list1.Count, list2.Count);

			string json1 = list1.ToJson();
			string json2 = list2.ToJson();
			Assert.AreEqual(json1, json2);
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

			// 断言没有参数产生
			Assert.AreEqual(0, query.Command.Parameters.Count);
		}


		[TestMethod]
		public void Test_CPQuery参数支持Guid数组()
		{
			string sql = @"select * from xx where xxGuid in ({guidList})";

			Guid guid1 = new Guid("c7423fc3-fc56-4b0a-9119-dcbdbaac56db");
			Guid guid2 = new Guid("74c5608c-e6b9-40f7-a2c7-999916f3bd40");
			List<Guid> guidArray = new List<Guid> { guid1, guid2 };
			var queryArgument = new { guidList = guidArray };

			CPQuery query = CPQuery.Create(sql, queryArgument);

			string commandText = query.ToString();
			Console.WriteLine(commandText);

			// 断言占位符已被替换
			Assert.AreEqual(
				// 注意：Guid[] 不会生成命令参数，将直接输出到SQL中
				"select * from xx where xxGuid in ('c7423fc3-fc56-4b0a-9119-dcbdbaac56db','74c5608c-e6b9-40f7-a2c7-999916f3bd40')",
				commandText
				);

			// 断言没有参数产生
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


			CPQuery query2 = CPQuery.Create("select * from t1").SetTimeout(2);
			Assert.AreEqual(2, query2.Command.CommandTimeout);
		}

		[TestMethod]
		public async Task Test_CPQuery_ToScalarList()
		{
			// 结果集的第一列其实是个数字列，这里强制写成 string
			List<string> list1 = CPQuery.Create("select * from Categories").ToScalarList<string>();

			using( DbContext db = DbContext.Create() ) {
				List<string> list2 = await db.CPQuery.Create("select * from Categories").ToScalarListAsync<string>();

				string json1 = list1.ToJson();
				string json2 = list2.ToJson();

				Console.WriteLine(json1);
			}
		}


		[TestMethod]
		public async Task Test_CPQuery_ExecuteReader()
		{
			int id1 = -1, id2 = -2;

			CPQuery query1 = CPQuery.Create("select * from Categories");
			using(DbDataReader reader1 = query1.ExecuteReader() ) {
				if( reader1.Read() )
					id1 = reader1.GetInt32(0);
			}


			

			using( DbContext db = DbContext.Create() ) {
				CPQuery query2 = db.CPQuery.Create("select * from Categories");

				using( DbDataReader reader2 = await query1.ExecuteReaderAsync() ) {
					if( reader2.Read() )
						id2 = reader2.GetInt32(0);
				}
			}

			Assert.AreEqual(id1, id2);
			Assert.IsTrue(id1 > 0);
		}


		[TestMethod]
		public async Task Test_CPQuery_测试同名列()
		{
			string sql = @"select a.* , b.* 
from sys.all_objects as a inner join sys.all_columns as b 
on a.object_id = b.object_id";

			string sql2 = @"
select * from sys.all_objects;
select * from sys.all_columns;

select a.* , b.* 
from sys.all_objects as a inner join sys.all_columns as b 
on a.object_id = b.object_id;
";


			DataTable table1 = CPQuery.Create(sql).ToDataTable();
			// 上面的调用如果不出现异常，基本就算是成功了，所以这里就只是检验下列有没有出现。
			Assert.IsTrue(table1.Rows.Count > 0);


			DataSet ds1 = CPQuery.Create(sql2).ToDataSet();
			Assert.AreEqual(3, ds1.Tables.Count);


			using( DbContext db = DbContext.Create() ) {

				DataTable table2 = await db.CPQuery.Create(sql).ToDataTableAsync();
				Assert.IsTrue(table2.Rows.Count > 0);

				// 确认同步和异步方式得到的DataTable的 行和列的数量一致
				// 因为二者不是一样的获取方式
				Assert.AreEqual(table1.Columns.Count, table2.Columns.Count);
				Assert.AreEqual(table1.Rows.Count, table2.Rows.Count);


				DataSet ds2 = await db.CPQuery.Create(sql2).ToDataSetAsync();
				Assert.AreEqual(3, ds2.Tables.Count);


				// 用最简单的方式确认二个 DataSet 一致
				Assert.AreEqual(ds1.Tables.Count, ds2.Tables.Count);

				Assert.AreEqual(ds1.Tables[0].Columns.Count, ds2.Tables[0].Columns.Count);
				Assert.AreEqual(ds1.Tables[0].Rows.Count, ds2.Tables[0].Rows.Count);

				Assert.AreEqual(ds1.Tables[1].Columns.Count, ds2.Tables[1].Columns.Count);
				Assert.AreEqual(ds1.Tables[1].Rows.Count, ds2.Tables[1].Rows.Count);

				Assert.AreEqual(ds1.Tables[2].Columns.Count, ds2.Tables[2].Columns.Count);
				Assert.AreEqual(ds1.Tables[2].Rows.Count, ds2.Tables[2].Rows.Count);
			}
		}


		[TestMethod]
		public void Test_BaseCommand_CloneParameters()
		{
			var newCustomer = new {
				CustomerName = s_newName,
				ContactName = Guid.NewGuid().ToString(),
				Address = "111111 Address",
				PostalCode = "111111",
				Tel = "123456789"
			};

			using( ConnectionScope scope = ConnectionScope.Create() ) {
				XmlCommand command = XmlCommand.Create("InsertCustomer", newCustomer);

				DbParameter[] parameters1 = command.Command.Parameters.Cast<DbParameter>().ToArray();
				DbParameter[] parameters2 = command.CloneParameters();

				AssertAreEqual_DbParameterArray(parameters1, parameters2);
			}
		}


		private void AssertAreEqual_DbParameterArray(DbParameter[] parameters1, DbParameter[] parameters2)
		{
			Assert.AreEqual(parameters1.Length, parameters2.Length);

			for( int i = 0; i < parameters2.Length; i++ ) {
				DbParameter p1 = parameters1[i];
				DbParameter p2 = parameters2[i];

				Assert.AreEqual(p1.ParameterName, p2.ParameterName);
				Assert.AreEqual(p1.DbType, p2.DbType);
				Assert.AreEqual(p1.Direction, p2.Direction);
				Assert.AreEqual(p1.IsNullable, p2.IsNullable);
				Assert.AreEqual(p1.Size, p2.Size);
				Assert.AreEqual(p1.SourceColumn, p2.SourceColumn);
				Assert.AreEqual(p1.SourceColumnNullMapping, p2.SourceColumnNullMapping);
				Assert.AreEqual(p1.Value.ToString(), p2.Value.ToString());
			}
		}

		[TestMethod]
		public void Test_CPQuery_Init_Dictionary()
		{
			DateTime now = DateTime.Now;
			Guid guid = Guid.NewGuid();

			Dictionary<string, object> dict = new Dictionary<string, object>();
			dict["key1"] = 1;
			dict["key2"] = "abc";
			dict["key3"] = now;
			dict["key4"] = guid;


			Hashtable table = new Hashtable();
			table["key1"] = 1;
			table["key2"] = "abc";
			table["key3"] = now;
			table["key4"] = guid;

			CPQuery query1 = CPQuery.Create("select ............", dict);
			CPQuery query2 = CPQuery.Create("select ............", table);

			DbParameter[] parameters1 = (from x in query1.Command.Parameters.Cast<DbParameter>()
										 orderby x.ParameterName
										 select x).ToArray();

			DbParameter[] parameters2 = (from x in query2.Command.Parameters.Cast<DbParameter>()
										 orderby x.ParameterName
										 select x).ToArray();

			AssertAreEqual_DbParameterArray(parameters1, parameters2);
		}


		[TestMethod]
		public void Test_CPQuery_Init_DbParameters()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			dict["key1"] = 1;
			dict["key2"] = "abc";
			dict["key3"] = DateTime.Now;
			dict["key4"] = Guid.NewGuid();


			CPQuery query1 = CPQuery.Create("select ............", dict);

			DbParameter[] parameters = query1.CloneParameters();
			CPQuery query2 = CPQuery.Create("select ............", parameters);


			DbParameter[] parameters1 = (from x in query1.Command.Parameters.Cast<DbParameter>()
										 orderby x.ParameterName
										 select x).ToArray();

			DbParameter[] parameters2 = (from x in query2.Command.Parameters.Cast<DbParameter>()
										 orderby x.ParameterName
										 select x).ToArray();
			AssertAreEqual_DbParameterArray(parameters1, parameters2);
		}
	}
}
