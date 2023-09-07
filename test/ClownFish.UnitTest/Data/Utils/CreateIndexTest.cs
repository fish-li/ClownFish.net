namespace ClownFish.UnitTest.Data.Utils;

[TestClass]
public class CreateIndexTest
{
    private static readonly string[] s_names = new string[] { "rid" , "intA" , "timeA", "moneyA", "stringA", "boolA", "guidA"
                    , "intB", "moneyB", "guidB", "shortB", "charA", "charB", "img", "g2", "ts"
        };


    [TestMethod]
    public void Test_FindIndex()
    {
        int x1 = DataExtensions.FindIndex(s_names, "intA");
        Assert.AreEqual(1, x1);

        int x2 = DataExtensions.FindIndex(s_names, "boolA");
        Assert.AreEqual(5, x2);


        List<string> list = s_names.ToList();

        int x3 = DataExtensions.FindIndex(list, "timeA");
        Assert.AreEqual(2, x3);

        int x4 = DataExtensions.FindIndex(list, "moneyA");
        Assert.AreEqual(3, x4);
    }

    //[TestMethod]
    public void Test()
    {
        int count = 100 * 10000;
        int[] result = null;

        Stopwatch watch1 = Stopwatch.StartNew();

        for( int i = 0; i < count; i++ )
            result = Test1();

        watch1.Stop();
        Console.WriteLine(watch1.Elapsed.ToString());


        Stopwatch watch2 = Stopwatch.StartNew();

        for( int i = 0; i < count; i++ )
            result = Test2();

        watch2.Stop();
        Console.WriteLine(watch2.Elapsed.ToString());

    }


    public int[] Test1()
    {
        int[] colIndex = new int[19];
        colIndex[0] = DataExtensions.FindIndex(s_names, "rid");
        colIndex[1] = DataExtensions.FindIndex(s_names, "intA");
        colIndex[2] = DataExtensions.FindIndex(s_names, "timeA");
        colIndex[3] = DataExtensions.FindIndex(s_names, "moneyA");
        colIndex[4] = DataExtensions.FindIndex(s_names, "stringA");
        colIndex[5] = DataExtensions.FindIndex(s_names, "boolA");
        colIndex[6] = DataExtensions.FindIndex(s_names, "guidA");
        colIndex[7] = DataExtensions.FindIndex(s_names, "intB");
        colIndex[8] = DataExtensions.FindIndex(s_names, "moneyB");
        colIndex[9] = DataExtensions.FindIndex(s_names, "guidB");
        colIndex[13] = DataExtensions.FindIndex(s_names, "shortB");
        colIndex[14] = DataExtensions.FindIndex(s_names, "charA");
        colIndex[15] = DataExtensions.FindIndex(s_names, "charB");
        colIndex[16] = DataExtensions.FindIndex(s_names, "img");
        colIndex[17] = DataExtensions.FindIndex(s_names, "g2");
        colIndex[18] = DataExtensions.FindIndex(s_names, "ts");
        return colIndex;
    }

    public int[] Test2()
    {
        return CreateNameMapIndex(s_names, 19
        , new KeyValuePair<int, string>(0, "rid")
        , new KeyValuePair<int, string>(1, "intA")
        , new KeyValuePair<int, string>(2, "timeA")
        , new KeyValuePair<int, string>(3, "moneyA")
        , new KeyValuePair<int, string>(4, "stringA")
        , new KeyValuePair<int, string>(5, "boolA")
        , new KeyValuePair<int, string>(6, "guidA")
        , new KeyValuePair<int, string>(7, "intB")
        , new KeyValuePair<int, string>(8, "moneyB")
        , new KeyValuePair<int, string>(9, "guidB")
        , new KeyValuePair<int, string>(13, "shortB")
        , new KeyValuePair<int, string>(14, "charA")
        , new KeyValuePair<int, string>(15, "charB")
        , new KeyValuePair<int, string>(16, "img")
        , new KeyValuePair<int, string>(17, "g2")
        , new KeyValuePair<int, string>(18, "ts")
        );
    }



    private static int[] CreateNameMapIndex(string[] names, int len, params KeyValuePair<int, string>[] kvs)
    {
        // 优化这段代码的性能
        //int[] colIndex = new int[19];
        //colIndex[0] = DataExtensions.FindIndex(names, "rid");
        //colIndex[1] = DataExtensions.FindIndex(names, "intA");
        //colIndex[2] = DataExtensions.FindIndex(names, "timeA");
        //colIndex[3] = DataExtensions.FindIndex(names, "moneyA");
        // ............................

        int[] colIndex = new int[len];

        foreach( var kv in kvs ) {
            int index = -1;

            for( int i = 0; i < names.Length; i++ ) {
                if( names[i] != null
                    && string.Compare(names[i], kv.Value, StringComparison.OrdinalIgnoreCase) == 0 ) {

                    index = i;
                    names[i] = null;    // 节省后续查找时间
                    break;
                }
            }
            colIndex[kv.Key] = index;
        }
        return colIndex;
    }



}
