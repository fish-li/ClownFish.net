using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Model;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class SqlInfoTest
	{
		[TestMethod]
		public void Test1()
		{
			// 测试命令参数超过最大值的情况

			FieldInfo field = typeof(SqlInfo).GetField("MaxParameterCount", BindingFlags.Static | BindingFlags.NonPublic);
			field.SetValue(null, 3);

			DbCommand command = CreateDbCommand();

			SqlInfo info = SqlInfo.Create(command);

			Assert.AreEqual(4, info.Parameters.Count);
			Assert.AreEqual("#####", info.Parameters[3].Name);
		}


		[TestMethod]
		public void Test2()
		{
			// 测试特殊参数场景

			DbCommand command = CreateDbCommand();

			SqlInfo info = SqlInfo.Create(command);

			Assert.AreEqual(command.CommandText, info.SqlText.ToString());



			SqlInfo info2 = SqlInfo.Create(null);
			Assert.IsNull(info2);


			command.Parameters.Clear();
			SqlInfo info3 = SqlInfo.Create(command);

			Assert.IsNull(info3.Parameters);
		}


		


		internal static DbCommand CreateDbCommand()
		{
			SqlCommand command = new SqlCommand();
			command.CommandText = "select * from Table1";
			command.Parameters.AddWithValue("@p_int", 123);
			command.Parameters.AddWithValue("@p_string", "abc");
			command.Parameters.AddWithValue("@p_datetime", DateTime.Now);
			command.Parameters.AddWithValue("@p_null", null);
			command.Parameters.AddWithValue("@p_dbnull", DBNull.Value);
			return command;
		}
	}
}
