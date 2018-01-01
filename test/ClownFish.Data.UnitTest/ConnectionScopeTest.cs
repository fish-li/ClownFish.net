using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ClownFish.Base.TypeExtend;
using ClownFish.Data.UnitTest.Models;
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
				using(ConnectionScope s2 = ConnectionScope.GetExistOrCreate() ) {
					Assert.IsTrue(object.ReferenceEquals(scope, s2));


					//------------------------------------------------------------
					using( ConnectionScope s3 = ConnectionScope.GetExistOrCreate() ) {
						Assert.IsTrue(object.ReferenceEquals(s3, s2));

						int refCount3 = (int)s3.GetType().InvokeMember("_refCount",
											BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
											null, s3, null);
						Assert.AreEqual(3, refCount3);
					}

					// 确信连接没有被释放
					Assert.IsTrue(connection.ClientConnectionId != Guid.Empty);

					// 检验连接存在
					var name2 = CPQuery.Create("select @@SERVERNAME").ExecuteScalar<string>();
					Assert.AreEqual(name1, name2);


					int refCount2 = (int)s2.GetType().InvokeMember("_refCount",
											BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
											null, s2, null);
					Assert.AreEqual(2, refCount2);
				}

				// 确信连接没有被释放
				Assert.IsTrue(connection.ClientConnectionId != Guid.Empty);

				// 检验连接存在
				var name3 = CPQuery.Create("select @@SERVERNAME").ExecuteScalar<string>();
				Assert.AreEqual(name1, name3);


				int refCount1 = (int)scope.GetType().InvokeMember("_refCount",
											BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic,
											null, scope, null);
				Assert.AreEqual(1, refCount1);
			}


			// 确信连接已被释放
			Assert.IsTrue(connection.ClientConnectionId == Guid.Empty);
		}


		[TestMethod]
		public void Test_ConnectionScope多次启用分段事务()
		{
			ExtenderManager.RegisterSubscriber(typeof(EventManagerExt1));

			using( ConnectionScope scope = ConnectionScope.GetExistOrCreate() ) {
				ExecuteAndAssert(false);


				// ------------------------------------------------------
				// 开启事务
				scope.BeginTransaction();
				ExecuteAndAssert(true);
				scope.Commit();
				// ------------------------------------------------------


				//........................................
				// 没有事务
				ExecuteAndAssert(false);
				//........................................



				// ------------------------------------------------------
				// 开启事务
				scope.BeginTransaction();
				ExecuteAndAssert(true);
				scope.Commit();
				// ------------------------------------------------------

				//........................................
				// 没有事务
				ExecuteAndAssert(false);
				//........................................
			}

			ExtenderManager.RemoveSubscriber(typeof(EventManagerExt1));
		}


		private void ExecuteAndAssert(bool expected)
		{
			var time = CPQuery.Create("select getdate()").ExecuteScalar<DateTime>();
			Assert.AreEqual(expected, EventManagerExt1.LastInTransaction);
		}


		[TestMethod]
		public void Test_DbContext_ChangeDatabase()
		{
			// 去掉默认连接中的数据库名称
			ConnectionInfo info = ConnectionManager.GetConnection();
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(info.ConnectionString);

			string databaseName = builder.InitialCatalog;
			builder.InitialCatalog = "";
			string connectionString = builder.ToString();

			using(DbContext db = DbContext.Create(connectionString, info.ProviderName) ) {
				string name = db.CPQuery.Create("SELECT DB_NAME() AS DataBaseName").ExecuteScalar<string>();
				Assert.AreEqual("master", name);

				db.ChangeDatabase(databaseName);
				name = db.CPQuery.Create("SELECT DB_NAME() AS DataBaseName").ExecuteScalar<string>();
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

	}


	public class EventManagerExt1 : EventSubscriber<EventManager>
	{
		public static bool LastInTransaction { get; private set; }

		public override void SubscribeEvent(EventManager instance)
		{
			instance.BeforeExecute += Instance_BeforeExecute;
		}

		private void Instance_BeforeExecute(object sender, CommandEventArgs e)
		{
			LastInTransaction = e.Command.Command.Transaction != null;
		}
	}
}
