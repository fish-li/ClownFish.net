namespace PerformanceTest.DAL;

public sealed class Test_ClownFish_LoadDataTable : IPerformanceTest
{
    public Test_ClownFish_LoadDataTable(int pagesize) { }
    public void Dispose() { }

    public List<OrderInfo> Run()
    {
        DataTable table = TestHelper.GetOrderInfoTable();
        return table.ToList<OrderInfo>();
    }
}
