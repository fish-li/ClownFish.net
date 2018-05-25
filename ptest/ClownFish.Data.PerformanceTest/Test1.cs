using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;


// ##################################################################
//
// ClownFish.Data 性能测试结果解读
//
// http://note.youdao.com/noteshare?id=f45ab5306f6ebdfa6b142322a50f9b32
//
// ##################################################################


namespace ClownFish.Data.PerformanceTest
{
	[TestCase("ADO.NET-SQLSERVER")]
	public sealed class Test_Adonet_ShareConnection : IPerformanceTest
	{
		private int _pagesize;
		private SqlConnection _conn;

		public Test_Adonet_ShareConnection(int pagesize)
		{
			_pagesize = pagesize;
			_conn = new SqlConnection(Program.ConnectionString);
			_conn.Open();
		}

		public object Run()
		{
			SqlCommand command = new SqlCommand(TestHelper.QueryText, _conn);
			command.Parameters.Add("TopN", SqlDbType.Int).Value = _pagesize;

			List<OrderInfo> list = new List<OrderInfo>(_pagesize);

			using( SqlDataReader reader = command.ExecuteReader() ) {
				while( reader.Read() )
					list.Add(LoadOrderInfo(reader));
			}

			return list;
		}

		private static OrderInfo LoadOrderInfo(SqlDataReader reader)
		{
			OrderInfo info = new OrderInfo();
			info.OrderID = (int)reader["OrderID"];
			info.OrderDate = (DateTime)reader["OrderDate"];
			info.SumMoney = (decimal)reader["SumMoney"];
			info.Comment = (string)reader["Comment"];
			info.Finished = (bool)reader["Finished"];
			info.ProductID = (int)reader["ProductID"];
			info.UnitPrice = (decimal)reader["UnitPrice"];
			info.Quantity = (int)reader["Quantity"];
			info.ProductName = (string)reader["ProductName"];
			info.CategoryID = (int)reader["CategoryID"];
			info.Unit = (string)reader["Unit"];
			info.Remark = (string)reader["Remark"];

			object customerId = reader["CustomerID"];
			if( customerId != DBNull.Value ) {
				info.CustomerID = (int)customerId;
				info.CustomerName = (string)reader["CustomerName"];
				info.ContactName = (string)reader["ContactName"];
				info.Address = (string)reader["Address"];
				info.PostalCode = (string)reader["PostalCode"];
				info.Tel = (string)reader["Tel"];
			}
			return info;
		}

		public void Dispose()
		{
			_conn.Dispose();
		}
	}




	[TestCase("ClownFish-SQLSERVER")]
	public sealed class Test_ClownFish_ShareConnection : IPerformanceTest
	{
		private int _pagesize;
		private DbContext _db;

		public Test_ClownFish_ShareConnection(int pagesize)
		{
			_pagesize = pagesize;
			_db = DbContext.Create();
		}

		public object Run()
		{
			var parameter = new { TopN = _pagesize };
			return _db.CPQuery.Create(TestHelper.QueryText, parameter).ToList<OrderInfo>();
		}

		public void Dispose()
		{
			_db.Dispose();
		}
	}
}
