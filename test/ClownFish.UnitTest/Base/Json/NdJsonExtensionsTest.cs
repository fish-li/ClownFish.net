using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest.Data;

namespace ClownFish.UnitTest.Base.Json;
[TestClass]
public class NdJsonExtensionsTest
{
    [TestMethod]
    public void Test_ToMultiLineJson()
    {
        List<Product2> list = new List<Product2>();
        list.Add(Product2.CreateByFixedData());
        list.Add(Product2.CreateByFixedData());
        list.Add(Product2.CreateByFixedData());

        string lines = list.ToMultiLineJson().TrimEnd();
        Assert.AreEqual(2, lines.Where(x => x == '\n').Count());
        Assert.IsTrue(lines.StartsWith("{"));
        Assert.IsTrue(lines.EndsWith("}"));


        List<Product2> list2 = lines.FromMultiLineJson<Product2>();
        Assert.AreEqual(3, list2.Count);

        MyAssert.AreEqual(list, list2);
    }


    [TestMethod]
    public void Test_FromMultiLineJson()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Product2.CreateByFixedData().ToJson()).Append("\n");
        sb.Append(Product2.CreateByRandomData().ToJson()).Append("\n");
        sb.Append(Product2.CreateByRandomData().ToJson()).Append("\n");

        List<Product2> list = sb.ToString().FromMultiLineJson<Product2>();
        Assert.AreEqual(3, list.Count);


        List<Product2> list2 = NdJsonExtensions.FromMultiLineJson<Product2>(string.Empty);
        Assert.AreEqual(0, list2.Count);

        List<Product2> list3 = NdJsonExtensions.FromMultiLineJson<Product2>((string)null);
        Assert.IsNull(list3);
    }



    [TestMethod]
    public void Test_performance()
    {
        List<InvokeLog> list = new List<InvokeLog>(30);

        for( int i = 0; i < 30; i++ ) {
            list.Add(CreateInvokeLog());
        }

        int count = 1_0000;
        string lastJson1 = null, lastJson2 = null;

        var x = list.First().ToJson().FromJson<InvokeLog>();  // 预热

        Stopwatch sw = Stopwatch.StartNew();

        for( int i = 0; i < count; i++ ) {
            string s = list.ToJson();
            List<InvokeLog> list2 = s.FromJson<List<InvokeLog>>();
            lastJson1 = s;
        }
        sw.Stop();
        Console.Write("ToJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            string s = list.ToMultiLineJson();
            lastJson2 = s;
        }
        sw.Stop();
        Console.Write("ToMultiLineJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            List<InvokeLog> list2 = lastJson1.FromJson<List<InvokeLog>>();
        }
        sw.Stop();
        Console.Write("FromJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());


        sw.Restart();
        for( int i = 0; i < count; i++ ) {
            List<InvokeLog> list2 = lastJson2.FromMultiLineJson<InvokeLog>();
        }
        sw.Stop();
        Console.Write("FromMultiLineJson: ".PadRight(20));
        Console.WriteLine(sw.Elapsed.ToString());


        Console.WriteLine("---------------------------------");
        Console.WriteLine(lastJson1);
        Console.WriteLine("---------------------------------");
        Console.WriteLine(lastJson2);

        // output:  (count = 1_0000)
        //ToJson:             00:00:00.7952054
        //ToMultiLineJson:    00:00:00.1526433
        //FromJson:           00:00:00.2956272
        //FromMultiLineJson:  00:00:00.4294308
    }


    private static InvokeLog CreateInvokeLog()
    {
        return new InvokeLog {
            ProcessId = LogIdMaker.GetNewId(),
            ActionType = 100,
            AppName = "ClownFish.UnitTest",
            StartTime = DateTime.Now,
            ExecuteTime = TimeSpan.FromSeconds(5),
            Status = 200,
            IsSlow = 1,
            IsLongTask = 1,
            HasError = 1,
            Title = "xxxxxxxxx"
        };
    }


    [TestMethod]
    public void Test_x_args()
    {
        List<InvokeLog> list = null;
        Assert.AreEqual("", list.ToMultiLineJson());
        Assert.AreEqual(0, list.ToMultiLineJson((TextWriter)null));

        string nullString = null;
        Assert.IsNull(nullString.FromMultiLineJson<InvokeLog>());
        Assert.AreEqual(0, string.Empty.FromMultiLineJson<InvokeLog>().Count);

        TextReader nullReader = null;
        Assert.AreEqual(0, nullReader.FromMultiLineJson<InvokeLog>().Count);


        MyAssert.IsError<ArgumentNullException>(() => {
            DbDataReader reader = null;
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            _ = reader.DbReaderToNdJson(100, writer);
        });


        using( DbContext db = DbContext.Create("sqlserver") ) {
            CPQuery query = db.CPQuery.Create("select 1 as aa");
            using( DbDataReader reader = query.ExecuteReader() ) {

                MyAssert.IsError<ArgumentNullException>(() => {
                    TextWriter writer = null;
                    _ = reader.DbReaderToNdJson(100, writer);
                });
            }
        }
    }
}
