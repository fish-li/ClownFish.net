using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;

namespace ClownFish.Data.PerformanceTest
{
	public static class TestHelper
	{
		public static readonly string QueryText = @"
select top (@TopN) d.OrderID, d.OrderDate, d.SumMoney, d.Comment, d.Finished,
dt.ProductID, dt.UnitPrice, dt.Quantity, 
p.ProductName, p.CategoryID, p.Unit, p.Remark,
c.CustomerID, c.CustomerName, c.ContactName, c.Address, c.PostalCode, c.Tel
from Orders d 
inner join [Order Details] dt on d.OrderId = dt.OrderId
inner join Products p on dt.ProductId = p.ProductId
left join Customers c on d.CustomerId = c.CustomerId
";

		private static DataTable s_OrderInfoTable;

		public static DataTable GetOrderInfoTable()
		{
			// 把结果用静态变量缓存起来，避免影响测试时间
			// 由于在运行测试前，会有一次单独的调用，所以并没有线程安全问题。

			if( s_OrderInfoTable == null ) {

				using( ConnectionScope scope = ConnectionScope.Create() ) {
					s_OrderInfoTable = CPQuery.Create(QueryText, new { TopN = 50 }).ToDataTable();
				}
			}

			return s_OrderInfoTable;
		}
	}
}
