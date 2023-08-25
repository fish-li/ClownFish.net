using System;
using System.Data;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Data;
using System.Reflection;

namespace ClownFish.UnitTest.Data.Context
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

				string sql = GetSql("GetCustomerById");
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


		[TestMethod]
		public void Test_重用ConnectionScope实例()
		{
			SqlConnection connection = null;

			using( ConnectionScope scope = ConnectionScope.Create() ) {
				connection = scope.Context.Connection as SqlConnection;

				var name1 = CPQuery.Create("select @@SERVERNAME").ExecuteScalar<string>();


				//------------------------------------------------------------
				using(ConnectionScope s2 = ConnectionScope.Get() ) {
					Assert.IsTrue(object.ReferenceEquals(scope.Context, s2.Context));


					//------------------------------------------------------------
					using( ConnectionScope s3 = ConnectionScope.Get() ) {
						Assert.IsTrue(object.ReferenceEquals(s2.Context, s3.Context));
					}

					// 确信连接没有被释放
					Assert.IsTrue(connection.ClientConnectionId != Guid.Empty);

					// 检验连接存在
					var name2 = CPQuery.Create("select @@SERVERNAME").ExecuteScalar<string>();
					Assert.AreEqual(name1, name2);
				}

				// 确信连接没有被释放
				Assert.IsTrue(connection.ClientConnectionId != Guid.Empty);

				// 检验连接存在
				var name3 = CPQuery.Create("select @@SERVERNAME").ExecuteScalar<string>();
				Assert.AreEqual(name1, name3);
			}


			// 确信连接已被释放
			Assert.IsTrue(connection.ClientConnectionId == Guid.Empty);
		}

			

		[TestMethod]
		public void Test_DbContext_ChangeDatabase()
		{
			// 去掉默认连接中的数据库名称
			ConnectionInfo info = ConnectionManager.GetFirstConnection();
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(info.ConnectionString);

			string databaseName = builder.InitialCatalog;
			builder.InitialCatalog = "";
			string connectionString = builder.ToString();

			using(DbContext db = DbContext.Create(connectionString, info.ProviderName) ) {
				//string name = db.CPQuery.Create("SELECT DB_NAME() AS DataBaseName").ExecuteScalar<string>();
				//Assert.AreEqual("master", name);

				db.ChangeDatabase(databaseName);
				string name = db.CPQuery.Create("SELECT DB_NAME() AS DataBaseName").ExecuteScalar<string>();
				Assert.AreEqual(databaseName, name);
			}

			using( DbContext db = DbContext.Create(connectionString, info.ProviderName) ) {
				db.ChangeDatabase(databaseName);
				string name = db.CPQuery.Create("SELECT DB_NAME() AS DataBaseName").ExecuteScalar<string>();
				Assert.AreEqual(databaseName, name);

				Assert.IsNull(db.Transaction);

				db.BeginTransaction(IsolationLevel.ReadCommitted);
				Assert.IsNotNull(db.Transaction);
			}
		}


		[TestMethod]
		public void Test_Error()
        {
			MyAssert.IsError<InvalidProgramException>(() => {
				_= ConnectionScope.Get();
			});

			MyAssert.IsError<InvalidProgramException>(() => {
				_ = ConnectionScope.GetCurrentDbConext();
			});


			MyAssert.IsError<TargetInvocationException, ArgumentNullException>(() => {
				ConstructorInfo ctor = typeof(ConnectionScope).GetConstructor(
					BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(DbContext), typeof(bool) }, null);
				_ = ctor.Invoke(new object[] { null, true });
			});
		}


		[TestMethod]
		public void Test_Dispose()
        {
			DbContext db = DbContext.Create();
			int count = db.CPQuery.Create("select count(*) from categories").ExecuteScalar<int>();
			Assert.IsTrue(count > 0);
			db.Dispose();

			MyAssert.IsError<InvalidOperationException>(() => {
				int count2 = db.CPQuery.Create("select count(*) from categories").ExecuteScalar<int>();
				Assert.IsTrue(count2 > 0);
			});

		}

	}


}
