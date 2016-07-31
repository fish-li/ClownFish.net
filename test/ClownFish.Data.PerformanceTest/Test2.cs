using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;

namespace ClownFish.Data.PerformanceTest
{
	[TestMethod("ADO.NET-DataTable")]
	public class Test_Adonet_LoadDataTable : IPerformanceTest
	{
		public Test_Adonet_LoadDataTable(int pagesize) { }
		public void Dispose() { }

        public object Run()
        {
			DataTable table = TestHelper.GetOrderInfoTable();

			List<OrderInfo> list = new List<OrderInfo>(table.Rows.Count);
			foreach( DataRow dataRow in table.Rows )
				list.Add(LoadOrderInfo(dataRow));

			return list;
		}

		private OrderInfo LoadOrderInfo(DataRow dataRow)
		{
			OrderInfo info = new OrderInfo();
			info.OrderID = (int)dataRow["OrderID"];
			info.OrderDate = (DateTime)dataRow["OrderDate"];
			info.SumMoney = (decimal)dataRow["SumMoney"];
			info.Comment = (string)dataRow["Comment"];
			info.Finished = (bool)dataRow["Finished"];
			info.ProductID = (int)dataRow["ProductID"];
			info.UnitPrice = (decimal)dataRow["UnitPrice"];
			info.Quantity = (int)dataRow["Quantity"];
			info.ProductName = (string)dataRow["ProductName"];
			info.CategoryID = (int)dataRow["CategoryID"];
			info.Unit = (string)dataRow["Unit"];
			info.Remark = (string)dataRow["Remark"];

			object customerId = dataRow["CustomerID"];
			if( customerId != DBNull.Value ) {
				info.CustomerID = (int)customerId;
				info.CustomerName = (string)dataRow["CustomerName"];
				info.ContactName = (string)dataRow["ContactName"];
				info.Address = (string)dataRow["Address"];
				info.PostalCode = (string)dataRow["PostalCode"];
				info.Tel = (string)dataRow["Tel"];
			}
			return info;
		}
	}



	[TestMethod("ClownFish-DataTable")]
	public class Test_ClownFish_LoadDataTable : IPerformanceTest
	{
		public Test_ClownFish_LoadDataTable(int pagesize) { }
		public void Dispose() { }

        public object Run()
        {
			DataTable table = TestHelper.GetOrderInfoTable();
			return table.ToList<OrderInfo>();
		}
	}
}
