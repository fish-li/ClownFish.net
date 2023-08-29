namespace PerformanceTest.DAL;

public sealed class Test_Adonet_ShareConnection : IPerformanceTest
{
    private readonly int _pagesize;
    private readonly SqlConnection _conn;

    public Test_Adonet_ShareConnection(int pagesize)
    {
        _pagesize = pagesize;
        _conn = new SqlConnection(TestDAL.ConnectionString);
        _conn.Open();
    }

    public List<OrderInfo> Run()
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
