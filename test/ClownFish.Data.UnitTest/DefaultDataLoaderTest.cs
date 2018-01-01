using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class DefaultDataLoaderTest : BaseTest
	{
		[TestMethod]
		public void Test_DefaultDataLoader_ToSingle()
		{
			DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
			string sql = "select top 1 * from dbo.Customers order by CustomerID";

			using(DbContext db = DbContext.Create() ) {
				DataTable table = db.CPQuery.Create(sql).ToDataTable();

				Customer customer1 = loader.ToSingle(table.Rows[0]);
				Customer customer2 = db.CPQuery.Create(sql).ToSingle<Customer>();

				Assert.IsTrue(ObjectIsEquals(customer1, customer2));

				using(DbDataReader reader = db.CPQuery.Create(sql).ExecuteReader() ) {
					Customer customer3 = loader.ToSingle(reader);

					Assert.IsTrue(ObjectIsEquals(customer1, customer3));
				}
			}

		}




		[TestMethod]
		public void Test_DefaultDataLoader_ToList()
		{
			DefaultDataLoader<Customer> loader = new DefaultDataLoader<Customer>();
			string sql = "select top 10 * from dbo.Customers order by CustomerID";

			using( DbContext db = DbContext.Create() ) {
				DataTable table = db.CPQuery.Create(sql).ToDataTable();

				List<Customer> list1 = loader.ToList(table);
				List<Customer> list2 = db.CPQuery.Create(sql).ToList<Customer>();

				Assert.IsTrue(ObjectIsEquals(list1, list2));

				using( DbDataReader reader = db.CPQuery.Create(sql).ExecuteReader() ) {
					List<Customer> list3 = loader.ToList(reader);

					Assert.IsTrue(ObjectIsEquals(list1, list3));
				}
			}

		}









	}
}
