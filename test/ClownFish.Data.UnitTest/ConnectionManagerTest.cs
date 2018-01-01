using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class ConnectionManagerTest : BaseTest
	{
		[TestMethod]
		public void Test_RegisterConnection_argsNULL()
		{
			ArgumentNullException exception = null;
			try {
				ConnectionStringSettings setting = null;
				ConnectionManager.RegisterConnection(setting);
			}
			catch( ArgumentNullException  ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);
			Assert.AreEqual("setting", exception.ParamName);
		}


		[TestMethod]
		public void Test_RegisterConnection_nameNULL()
		{
			ArgumentNullException exception = null;
			try {
				ConnectionStringSettings setting = new ConnectionStringSettings(null, "xxxx");
				ConnectionManager.RegisterConnection(setting);
			}
			catch( ArgumentNullException ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);
			Assert.AreEqual("setting.Name", exception.ParamName);
		}


		[TestMethod]
		public void Test_RegisterConnection_connectionStringNULL()
		{
			ArgumentNullException exception = null;
			try {
				ConnectionStringSettings setting = new ConnectionStringSettings("xxx", null);
				ConnectionManager.RegisterConnection(setting);
			}
			catch( ArgumentNullException ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);
			Assert.AreEqual("setting.ConnectionString", exception.ParamName);
		}


		[TestMethod]
		public void Test_RegisterConnection_ConnectionExist()
		{
			InvalidOperationException exception = null;
			try {
				// default 这个名称 已经在 Connections.config 中注册过了，下面调用将会抛出异常
				ConnectionStringSettings setting = new ConnectionStringSettings("default", "xxxx");
				ConnectionManager.RegisterConnection(setting);
			}
			catch( InvalidOperationException ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);
		}


		[TestMethod]
		public void Test_GetConnection_OK()
		{
			ConnectionInfo connection1 = ConnectionManager.GetConnection();
			Assert.IsNotNull(connection1);


			ConnectionInfo connection2 = ConnectionManager.GetConnection("default");
			Assert.IsNotNull(connection2);

			Assert.AreEqual(connection1.ConnectionString, connection2.ConnectionString);
			Assert.AreEqual(connection1.ProviderName, connection2.ProviderName);
		}



		[TestMethod]
		public void Test_GetConnection_名称不存在()
		{
			ArgumentOutOfRangeException exception = null;
			try {
				ConnectionInfo connection = ConnectionManager.GetConnection("default-xx");
			}
			catch( ArgumentOutOfRangeException ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);
		}


		[TestMethod]
		public void Test_ConnectionScope_Create()
		{
			ConnectionInfo info = ConnectionManager.GetConnection("default");

			using( ConnectionScope 
					scope1 = ConnectionScope.Create(),
					scope2 = ConnectionScope.Create("default"),
					scope3 = ConnectionScope.Create(info.ConnectionString, info.ProviderName)
				) {

				ConnectionInfo connection1 = scope1.Context.ConnectionInfo;
				ConnectionInfo connection2 = scope2.Context.ConnectionInfo;
				ConnectionInfo connection3 = scope3.Context.ConnectionInfo;


				Assert.AreEqual(connection1.ConnectionString, connection2.ConnectionString);
				Assert.AreEqual(connection1.ProviderName, connection2.ProviderName);

				Assert.AreEqual(connection1.ConnectionString, connection3.ConnectionString);
				Assert.AreEqual(connection1.ProviderName, connection3.ProviderName);
			}
		}


		[TestMethod]
		public void Test_ConnectionScope_GetDefaultDbConext()
		{
			bool currentValue = ClownFish.Data.Initializer.Instance.IsAutoCreateOneOffDbContext;

			// 设置  IsAutoCreateOneOffDbContext = true;
			ClownFish.Data.Initializer.Instance.AllowCreateOneOffDbContext();

			using( DbContext db = ConnectionScope.GetDefaultDbConext() ) {
				Assert.IsNotNull(db);
				Assert.AreEqual(true, db.AutoDisposable);
			}


			// 设置  IsAutoCreateOneOffDbContext = false;
			typeof(ClownFish.Data.Initializer).InvokeMember(
				"IsAutoCreateOneOffDbContext",
				BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Instance,
				null, ClownFish.Data.Initializer.Instance,
				new object[] { false });


			InvalidProgramException exception = null;
			try {
				DbContext db = ConnectionScope.GetDefaultDbConext();
			}
			catch( InvalidProgramException ex ) {
				exception = ex;
			}

			Assert.IsNotNull(exception);


			// 恢复  IsAutoCreateOneOffDbContext 
			typeof(ClownFish.Data.Initializer).InvokeMember(
				"IsAutoCreateOneOffDbContext",
				BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Instance,
				null, ClownFish.Data.Initializer.Instance,
				new object[] { currentValue });
		}


	}
}
