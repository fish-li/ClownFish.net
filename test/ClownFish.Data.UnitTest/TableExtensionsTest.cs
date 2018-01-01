using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class TableExtensionsTest : BaseTest
	{
		[TestMethod]
		public void Test_DataTable_ToList()
		{
			using( ConnectionScope scope = ConnectionScope.Create() ) {
				var queryArgument = new { MaxCustomerID = 10 };
				DataTable table = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToDataTable();

				List<Customer> list1 = table.ToList< Customer>();
				List<Customer> list2 = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToList<Customer>();

				string json1 = list1.ToJson();
				string json2 = list2.ToJson();

				Assert.AreEqual(json1, json2);
			}
		}


		[TestMethod]
		public void Test_DataTable_ToSingle()
		{
			using( ConnectionScope scope = ConnectionScope.Create() ) {
				var queryArgument = new { MaxCustomerID = 10 };
				DataTable table = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToDataTable();

				Customer customer1 = table.Rows[0].ToSingle<Customer>();
				Customer customer2 = CPQuery.Create(GetSql("GetCustomerList"), queryArgument).ToSingle<Customer>();

				string json1 = customer1.ToJson();
				string json2 = customer2.ToJson();

				Assert.AreEqual(json1, json2);
			}
		}

	}
}
