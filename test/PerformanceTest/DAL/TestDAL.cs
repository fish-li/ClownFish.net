namespace PerformanceTest.DAL;

public static class TestDAL
{
    internal static string ConnectionString { get; private set; }

    private static bool s_initFlag = false;

    private static void Init()
    {
        if( s_initFlag == false ) {

            // 给静态变量赋值，连接字符串保存在ClownFish.App.config中
            ConnectionString = ConnectionManager.GetConnection("default").ConnectionString;

            s_initFlag = true;
        }

    }


    [MenuMethod(Title = "ADO.NET-SQLSERVER")]
    public static void Test1()
    {
        RunTest<Test_Adonet_ShareConnection>();
    }

    [MenuMethod(Title = "ClownFish-SQLSERVER")]
    public static void Test2()
    {
        RunTest<Test_ClownFish_ShareConnection>();
    }

    [MenuMethod(Title = "ADO.NET-DataTable")]
    public static void Test3()
    {
        RunTest<Test_Adonet_LoadDataTable>();
    }

    [MenuMethod(Title = "ClownFish-DataTable")]
    public static void Test4()
    {
        RunTest<Test_ClownFish_LoadDataTable>();
    }

    [MenuMethod(Title = "ClownFish/ADO.NET 4种场景对比")]
    public static void Test5()
    {
        RunAllTest();
    }

    [MenuMethod(Title = "比较4种数据查询的结果是否一致")]
    public static void CheckResult()
    {
        Init();

        IPerformanceTest test1 = new Test_Adonet_ShareConnection(50);
        IPerformanceTest test2 = new Test_ClownFish_ShareConnection(50);
        IPerformanceTest test3 = new Test_Adonet_LoadDataTable(50);
        IPerformanceTest test4 = new Test_ClownFish_LoadDataTable(50);

        var result1 = test1.Run().ToJson(JsonStyle.Indented);
        var result2 = test2.Run().ToJson(JsonStyle.Indented);
        var result3 = test3.Run().ToJson(JsonStyle.Indented);
        var result4 = test4.Run().ToJson(JsonStyle.Indented);

        Console.WriteLine(result1);

        bool isEquals = result1 == result2 && result2 == result3 && result3 == result4;
        Console.WriteLine($"测试结果：{isEquals}");
    }

    private class GroupData
    {
        public string Group { get; set; }
        public List<TimeSpan> List { get; set; }
    }

    private static void RunTest<T>() where T : IPerformanceTest
    {
        RunTest<T>(x => Console.WriteLine(x));
    }

    private static GroupData RunTest2<T>(string method) where T : IPerformanceTest
    {
        MethodInfo m = typeof(TestDAL).GetMethod(method, BindingFlags.Public | BindingFlags.Static);

        GroupData data = new GroupData();
        data.List = new List<TimeSpan>();
        data.Group = m.GetMyAttribute<MenuMethodAttribute>().Title;

        RunTest<T>(x => data.List.Add(x));
        return data;
    }

    private static void RunAllTest()
    {
        GroupData data1 = RunTest2<Test_Adonet_ShareConnection>(nameof(Test1));
        GroupData data2 = RunTest2<Test_ClownFish_ShareConnection>(nameof(Test2));
        GroupData data3 = RunTest2<Test_Adonet_LoadDataTable>(nameof(Test3));
        GroupData data4 = RunTest2<Test_ClownFish_LoadDataTable>(nameof(Test4));

        Console.WriteLine("{0,-22}{1,-22}{2,-22}{3,-22}", data1.Group, data2.Group, data3.Group, data4.Group);

        for( int i = 0; i < data1.List.Count; i++ )
            Console.WriteLine("{0,-22}{1,-22}{2,-22}{3,-22}", data1.List[i], data2.List[i], data3.List[i], data4.List[i]);

    }


    private static void RunTest<T>(Action<TimeSpan> action) where T : IPerformanceTest
    {
        Init();

        int pagesize = 50;
        IPerformanceTest instance = Activator.CreateInstance(typeof(T), pagesize) as IPerformanceTest;

        long sumTicks = 0;
        int count1 = 10;        // 大循环次数
        int count2 = 3000;      // 每轮循环中调用方法次数

        for( int i = 0; i < count1; i++ ) {

            Stopwatch watch = Stopwatch.StartNew();

            for( int k = 0; k < count2; k++ ) {
                var result = instance.Run();
            }

            watch.Stop();

            sumTicks += watch.Elapsed.Ticks;
            action(watch.Elapsed);
        }

        instance.Dispose();

        TimeSpan ts = new TimeSpan(sumTicks / count1);
        action(ts);
    }
}
