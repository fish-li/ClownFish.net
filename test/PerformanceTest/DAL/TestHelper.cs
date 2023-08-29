namespace PerformanceTest.DAL;

public static class TestHelper
{
    public static readonly string QueryText = @"
select top (@TopN) d.OrderID, d.OrderDate, d.SumMoney, d.Comment, d.Finished,
dt.ProductID, dt.UnitPrice, dt.Quantity, 
p.ProductName, p.CategoryID, p.Unit, p.Remark,
c.CustomerID, c.CustomerName, c.ContactName, c.Address, c.PostalCode, c.Tel
from Orders d 
inner join [OrderDetails] dt on d.OrderId = dt.OrderId
inner join Products p on dt.ProductId = p.ProductId
left join Customers c on d.CustomerId = c.CustomerId
";

    private static DataTable s_orderInfoTable;

    public static DataTable GetOrderInfoTable()
    {
        // 把结果用静态变量缓存起来，避免影响测试时间
        // 由于在运行测试前，会有一次单独的调用，所以并没有线程安全问题。

        if( s_orderInfoTable == null ) {

            using( DbContext db = DbContext.Create() ) {
                s_orderInfoTable = db.CPQuery.Create(QueryText, new { TopN = 50 }).ToDataTable();
            }
        }

        return s_orderInfoTable;
    }
}
