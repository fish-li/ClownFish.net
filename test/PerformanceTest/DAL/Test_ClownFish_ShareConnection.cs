namespace PerformanceTest.DAL;

public sealed class Test_ClownFish_ShareConnection : IPerformanceTest
{
    private readonly int _pagesize;
    private readonly DbContext _db;

    public Test_ClownFish_ShareConnection(int pagesize)
    {
        _pagesize = pagesize;
        _db = DbContext.Create();
    }

    public List<OrderInfo> Run()
    {
        var parameter = new { TopN = _pagesize };
        return _db.CPQuery.Create(TestHelper.QueryText, parameter).ToList<OrderInfo>();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
