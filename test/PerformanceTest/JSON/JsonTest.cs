namespace PerformanceTest.JSON;

public class JsonTest
{
    private static readonly InvokeLog s_logData = CreateList(1)[0];
    private static readonly List<InvokeLog> s_testList = CreateList(10);
    
    internal static readonly int RunCount = 100_0000;
    internal static readonly int RunCount2 = 10_0000;

    private static List<InvokeLog> CreateList(int count)
    {
        List<InvokeLog> list = new List<InvokeLog>(count);

        for(int i = 0; i < count; i++ ) {
            list.Add(new InvokeLog {
                ProcessId = Guid.NewGuid().ToString("N"),
                ActionType = 200,
                AppName = "XDemo.WebSiteApp",
                ExecuteTime = TimeSpan.FromSeconds(10),
                IsSlow = 1,
                StartTime = DateTime.Now,
                Status = 200,
                //Title = "序列化/反序列化"
            });
        }

        return list;
    }

    [MenuMethod(Title = "Json 测试")]
    public static void Test111()
    {
        List<InvokeLog> testList = CreateList(10);

        string json = testList[0].ToJson();
        Console.WriteLine(json);
        Console.WriteLine(json.Length);  // 195
        Console.WriteLine("-----------------------------------------------");


        List<InvokeLog> testList2 = new List<InvokeLog>() { testList[0] };
        Console.WriteLine(testList2.ToJson());
        Console.WriteLine("-----------------------------------------------");


        string text = testList.ToJson();
        Console.WriteLine(text.Length);   // 1961
    }


    [MenuMethod(Title = "Newtonsoft.Json/System.Text.Json 对比测试")]
    public static void Test()
    {
        for( int i = 0; i < 3; i++ ) {
            Console.WriteLine($"-----------------------------------------------------------------第 {i+1} 轮");
            Test1();
            Test2();
            Console.WriteLine();
            Test3();
            Test4();
            Console.WriteLine();
            Test5();
            Test6();
            Console.WriteLine();
        }
    }

    //[MenuMethod(Title = "Newtonsoft.Json 单个对象-100W次")]
    public static void Test1()
    {
        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < RunCount; i++ ) {
            string json = s_logData.ToJson();
            InvokeLog log2 = json.FromJson<InvokeLog>();
        }

        watch.Stop();
        Console.WriteLine("Newtonsoft.Json 单个对象-100W次: ".PadRight(40) + watch.Elapsed.ToString());                
    }

    //[MenuMethod(Title = "System.Text.Json 单个对象-100W次")]
    public static void Test2()
    {
        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < RunCount; i++ ) {
            string json = s_logData.ToJson2();
            InvokeLog log2 = json.FromJson2<InvokeLog>();
        }

        watch.Stop();
        Console.WriteLine("System.Text.Json 单个对象-100W次: ".PadRight(40) + watch.Elapsed.ToString());
    }

    //[MenuMethod(Title = "Newtonsoft.Json List<T>-10W次")]
    public static void Test3()
    {
        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < RunCount2; i++ ) {
            string json = s_testList.ToJson();
            List<InvokeLog> log2 = json.FromJson<List<InvokeLog>>();
        }

        watch.Stop();
        Console.WriteLine("Newtonsoft.Json 标准列表-10W次: ".PadRight(40) + watch.Elapsed.ToString());
    }

    //[MenuMethod(Title = "System.Text.Json List<T>-10W次")]
    public static void Test4()
    {
        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < RunCount2; i++ ) {
            string json = s_testList.ToJson2();
            List<InvokeLog> log2 = json.FromJson2<List<InvokeLog>>();
        }

        watch.Stop();
        Console.WriteLine("System.Text.Json 标准列表-10W次: ".PadRight(40) + watch.Elapsed.ToString());
    }


    //[MenuMethod(Title = "Newtonsoft.Json MultiLine-10W次")]
    public static void Test5()
    {
        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < RunCount2; i++ ) {
            string json = s_testList.ToMultiLineJson();
            List<InvokeLog> log2 = json.FromMultiLineJson<InvokeLog>();
        }

        watch.Stop();
        Console.WriteLine("Newtonsoft.Json 多行列表-10W次: ".PadRight(40) + watch.Elapsed.ToString());
    }

    //[MenuMethod(Title = "System.Text.Json MultiLine-10W次")]
    public static void Test6()
    {
        Stopwatch watch = Stopwatch.StartNew();

        for( int i = 0; i < RunCount2; i++ ) {
            var json = s_testList.ToMultiLineJson2();
            List<InvokeLog> log2 = json.FromMultiLineJson2<InvokeLog>();
        }

        watch.Stop();
        Console.WriteLine("System.Text.Json 多行列表-10W次: ".PadRight(40) + watch.Elapsed.ToString());
    }
}
