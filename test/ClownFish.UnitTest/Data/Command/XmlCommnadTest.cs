using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Command
{
	[TestClass]
	public class XmlCommandTest : BaseTest
	{
		private static readonly string s_newName = Guid.NewGuid().ToString("N");
		
		
		[TestMethod]
		public void Test_CRUD()
		{
            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    Console.WriteLine("ConnectionString: " + db.Connection.ConnectionString);
                    Console.WriteLine("Database Type: " + db.DatabaseType);

                    db.OpenConnection();
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
                    var queryArgument = new { CustomerName = s_newName };
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
		}

		[TestMethod]
		public async Task Test_CRUD_Async()
		{
            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    Console.WriteLine("ConnectionString: " + db.Connection.ConnectionString);
                    Console.WriteLine("Database Type: " + db.DatabaseType);

                    db.OpenConnection();
                    db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                    var newCustomer = new {
                        CustomerName = s_newName,
                        ContactName = Guid.NewGuid().ToString(),
                        Address = "111111 Address",
                        PostalCode = "111111",
                        Tel = "123456789"
                    };

                    // 插入一条记录
                    await db.XmlCommand.Create("InsertCustomer", newCustomer).ExecuteNonQueryAsync();

                    // 读取刚插入的记录
                    var queryArgument = new { CustomerName = s_newName };
                    Customer customer = await db.XmlCommand.Create("GetCustomerByName", queryArgument).ToSingleAsync<Customer>();

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
                    await db.XmlCommand.Create("UpdateCustomer", updateArgument).ExecuteNonQueryAsync();

                    // 读取刚更新的记录
                    var queryArgument2 = new { CustomerID = customer.CustomerID };
                    Customer customer2 = await db.XmlCommand.Create("GetCustomerById", queryArgument2).ToSingleAsync<Customer>();

                    // 验证更新与读取
                    Assert.IsNotNull(customer2);
                    Assert.AreEqual(updateArgument.Address, customer2.Address);


                    // 删除记录
                    var deleteArgument = new { CustomerID = customer.CustomerID };
                    await db.XmlCommand.Create("DeleteCustomer", deleteArgument).ExecuteNonQueryAsync();

                    // 验证删除			
                    Customer customer3 = await db.XmlCommand.Create("GetCustomerById", queryArgument2).ToSingleAsync<Customer>();
                    Assert.IsNull(customer3);


                    db.Commit();
                }
            }
		}

        [TestMethod]
        public void Test_Create()
        {
            using( ConnectionScope scope = ConnectionScope.Create() ) {

                var query1 = XmlCommand.Create("DeleteCustomer");

                var args2 = new { ProductID = 2 };
                var query2 = XmlCommand.Create("DeleteCustomer", args2);

                Hashtable table = new Hashtable();
                table["CustomerID"] = 2;
                var query3 = XmlCommand.Create("DeleteCustomer", table);

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["CustomerID"] = 2;
                var query4 = XmlCommand.Create("DeleteCustomer", dictionary);


                string expected = "delete from Customers where CustomerID = @CustomerID";
                Assert.AreEqual(expected, query1.Command.CommandText.Trim());
                Assert.AreEqual(expected, query2.Command.CommandText.Trim());
                Assert.AreEqual(expected, query3.Command.CommandText.Trim());
                Assert.AreEqual(expected, query4.Command.CommandText.Trim());

                Assert.AreEqual(1, query2.Command.Parameters.Count);
                Assert.AreEqual(1, query3.Command.Parameters.Count);
                Assert.AreEqual(1, query4.Command.Parameters.Count);

                Assert.AreEqual("@CustomerID", query2.Command.Parameters[0].ParameterName);
                Assert.AreEqual("@CustomerID", query3.Command.Parameters[0].ParameterName);
                Assert.AreEqual("@CustomerID", query4.Command.Parameters[0].ParameterName);
            }
        }


        [TestMethod]
        public void Test_Create2()
        {
            using( DbContext db = DbContext.Create() ) {

                var query1 = db.XmlCommand.Create("DeleteCustomer");

                var args2 = new { ProductID = 2 };
                var query2 = db.XmlCommand.Create("DeleteCustomer", args2);

                Hashtable table = new Hashtable();
                table["CustomerID"] = 2;
                var query3 = db.XmlCommand.Create("DeleteCustomer", table);

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["CustomerID"] = 2;
                var query4 = db.XmlCommand.Create("DeleteCustomer", dictionary);


                string expected = "delete from Customers where CustomerID = @CustomerID";
                Assert.AreEqual(expected, query1.Command.CommandText.Trim());
                Assert.AreEqual(expected, query2.Command.CommandText.Trim());
                Assert.AreEqual(expected, query3.Command.CommandText.Trim());
                Assert.AreEqual(expected, query4.Command.CommandText.Trim());

                Assert.AreEqual(1, query2.Command.Parameters.Count);
                Assert.AreEqual(1, query3.Command.Parameters.Count);
                Assert.AreEqual(1, query4.Command.Parameters.Count);

                Assert.AreEqual("@CustomerID", query2.Command.Parameters[0].ParameterName);
                Assert.AreEqual("@CustomerID", query3.Command.Parameters[0].ParameterName);
                Assert.AreEqual("@CustomerID", query4.Command.Parameters[0].ParameterName);
            }
        }


        [TestMethod]
        public void Test_Init_No_Params()
        {
            using( DbContext db = DbContext.Create() ) {

                var query1 = db.XmlCommand.Create("DeleteCustomer", (Hashtable)null);
                var query2 = db.XmlCommand.Create("DeleteCustomer", new Hashtable());

                var query3 = db.XmlCommand.Create("DeleteCustomer", (Dictionary<string, object>)null);
                var query4 = db.XmlCommand.Create("DeleteCustomer", new Dictionary<string, object>());

                Assert.AreEqual(1, query1.Command.Parameters.Count);
                Assert.AreEqual(1, query2.Command.Parameters.Count);
                Assert.AreEqual(1, query3.Command.Parameters.Count);
                Assert.AreEqual(1, query4.Command.Parameters.Count);

                Assert.IsNull(query1.Command.Parameters[0].Value);
                Assert.IsNull(query2.Command.Parameters[0].Value);
                Assert.IsNull(query3.Command.Parameters[0].Value);
                Assert.IsNull(query4.Command.Parameters[0].Value);
            }
        }


        [TestMethod]
        public void Test_SetCommand_Error()
        {
            using( DbContext db = DbContext.Create() ) {

                MyAssert.IsError<ArgumentNullException>(()=> {
                    var x = db.XmlCommand.Create("");
                });

                MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                    var x = db.XmlCommand.Create("DeleteCustomer_xxxx");
                });
            }
        }


        [TestMethod]
        public async Task Test_GetList()
        {
            var queryArgument = new { MaxCustomerID = 10 };

            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    List<Customer> list1 = db.XmlCommand.Create("GetCustomerList", queryArgument).ToList<Customer>();
                    Assert.IsNotNull(list1);


                    List<Customer> list2 = await db.XmlCommand.Create("GetCustomerList", queryArgument).ToListAsync<Customer>();
                    Assert.IsNotNull(list2);


                    // 确认二次查询的结果一致
                    Assert.AreEqual(list1.Count, list2.Count);

                    string json1 = list1.ToJson();
                    string json2 = list2.ToJson();
                    Assert.AreEqual(json1, json2);
                }
            }
        }


		[TestMethod]
		public void Test_IntArray()
		{
			int[] customerIdArray = { 1, 2, 3, 4, 5 };
			// 注意：下面二个参数名，它们只是SQL语句中的占位符，在替换时是区分大小写的。
			var queryArgument = new { table = "Customers",  CustomerID = customerIdArray };

            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    this.ResetCPQueryParamIndex();

                    XmlCommand query = db.XmlCommand.Create("GetCustomerListById", queryArgument);

                    string commandText = query.Command.CommandText;
                    Console.WriteLine(commandText);

                    // 断言占位符已被替换
                    Assert.AreEqual(
                        // 注意：int[] 不会生成命令参数，将直接输出到SQL中
                        "select * from Customers where CustomerID in (1,2,3,4,5)",
                        commandText
                        );

                    // 断言参数已产生
                    Assert.AreEqual(0, query.Command.Parameters.Count);
                }
            }
		}


		[TestMethod]
		public void Test_StringArray()
		{
			string[] customerIdArray = { "1", "2", "3", "4", "5" };
			// 注意：下面二个参数名，它们只是SQL语句中的占位符，在替换时是区分大小写的。
			var queryArgument = new { table = "Customers", CustomerID = customerIdArray };

            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    this.ResetCPQueryParamIndex();

                    XmlCommand query = db.XmlCommand.Create("GetCustomerListById", queryArgument);

                    string commandText = query.Command.CommandText;
                    Console.WriteLine(commandText);

                    // 断言占位符已被替换
                    if( db.DatabaseType == DatabaseType.DaMeng ) {
                        Assert.AreEqual("select * from Customers where CustomerID in (:x1,:x2,:x3,:x4,:x5)", commandText);
                    }
                    else {
                        Assert.AreEqual("select * from Customers where CustomerID in (@x1,@x2,@x3,@x4,@x5)", commandText);
                    }

                    // 断言参数已产生
                    Assert.AreEqual(5, query.Command.Parameters.Count);
                }
            }
		}

		[TestMethod]
		public void Test_SubQuery()
		{
            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext db = DbContext.Create(conn) ) {

                    this.ResetCPQueryParamIndex();

                    CPQuery subQuery = db.CPQuery.Create("Tel like ") + "021%".AsQueryParameter();

                    int[] customerIdArray = { 1, 2, 3, 4, 5 };
                    // 注意：下面二个参数名，它们只是SQL语句中的占位符，在替换时是区分大小写的。
                    var queryArgument = new {
                        table = "Customers",
                        CustomerID = customerIdArray,
                        filter = subQuery
                    };


                    XmlCommand query = db.XmlCommand.Create("FindCustomers", queryArgument);

                    string commandText = query.Command.CommandText;
                    Console.WriteLine(commandText);


                    // 断言占位符已被替换
                    if( db.DatabaseType == DatabaseType.DaMeng ) {
                        Assert.AreEqual("select * from Customers where CustomerID in (1,2,3,4,5) and Tel like :p1", commandText);
                    }
                    else {
                        Assert.AreEqual("select * from Customers where CustomerID in (1,2,3,4,5) and Tel like @p1", commandText);
                    }

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
}
