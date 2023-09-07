// 指示需要在运行时生成实体代理类
[assembly: ClownFish.Data.EntityAssembly]

namespace HttpClientPerformanceTest;

class Program
{
    static async Task Main(string[] args)
    {
        ClownFishInit.InitBase();
        ClownFishInit.InitDAL();
        // 这个程序不使用持久化日志，因此不必初始化 ClownFish.Log

        await TestMenu.Run();
    }
}
