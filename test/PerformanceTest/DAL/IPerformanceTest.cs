namespace PerformanceTest.DAL;

public interface IPerformanceTest : IDisposable
{
    List<OrderInfo> Run();
}
