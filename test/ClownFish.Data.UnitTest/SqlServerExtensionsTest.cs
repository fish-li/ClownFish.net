using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.SqlClient;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class SqlServerExtensionsTest : BaseTest
	{
		[TestMethod]
		public void Test_InsertReturnNewId()
		{
			using(ConnectionScope scope = ConnectionScope.Create() ) {
				Customer customer = Entity.BeginEdit<Customer>();
				customer.CustomerName = "Name_" + Guid.NewGuid().ToString("N");
				customer.Address = "Address_" + Guid.NewGuid().ToString("N");
				customer.ContactName = "Contact_" + Guid.NewGuid().ToString("N");
				customer.PostalCode = "430076";
				customer.Tel = "123456789";

				// 确认这个这没有赋值
				Assert.AreEqual(0, customer.CustomerID);

				// 插入数据行，并获取自增ID
				int id = customer.InsertReturnNewId();
				customer.CustomerID = id;


				Assert.IsTrue(id > 0);

				Customer customer2 = (from x in Entity.Query<Customer>()
									  where x.CustomerID == id
									  select x
									  ).FirstOrDefault();

				// 确认数据插入成功
				Assert.IsNotNull(customer2);


				// 确认数据有效
				ObjectIsEquals(customer, customer2);
			}
			
		}
		
	}
}
