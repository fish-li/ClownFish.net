namespace ClownFish.UnitTest.Base.Json;

[TestClass]
public class NdJsonExtensionsTest
{
    [TestMethod]
    public void Test_ToNdjson()
    {
        List<Product3> list = new List<Product3>();
        list.Add(Product3.CreateByFixedData());
        list.Add(Product3.CreateByFixedData());
        list.Add(Product3.CreateByFixedData());

        string lines = list.ToNdjson().TrimEnd();
        Assert.AreEqual(2, lines.Where(x => x == '\n').Count());
        Assert.IsTrue(lines.StartsWith("{"));
        Assert.IsTrue(lines.EndsWith("}"));


        List<Product3> list2 = lines.FromNdjson<Product3>();
        Assert.AreEqual(3, list2.Count);

        MyAssert.AreEqual(list, list2);
    }


    [TestMethod]
    public void Test_FromNdjson()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Product3.CreateByFixedData().ToJson()).Append("\n");
        sb.Append(Product3.CreateByRandomData().ToJson()).Append("\n");
        sb.Append(Product3.CreateByRandomData().ToJson()).Append("\n");

        List<Product3> list = sb.ToString().FromNdjson<Product3>();
        Assert.AreEqual(3, list.Count);


        List<Product3> list2 = NdJsonExtensions.FromNdjson<Product3>(string.Empty);
        Assert.AreEqual(0, list2.Count);

        List<Product3> list3 = NdJsonExtensions.FromNdjson<Product3>((string)null);
        Assert.IsNull(list3);
    }


    [TestMethod]
    public void Test_x_args()
    {
        List<InvokeLog> list = null;
        Assert.AreEqual("", list.ToNdjson());
        Assert.AreEqual(0, list.ToNdjson((TextWriter)null));

        string nullString = null;
        Assert.IsNull(nullString.FromNdjson<InvokeLog>());
        Assert.AreEqual(0, string.Empty.FromNdjson<InvokeLog>().Count);

        TextReader nullReader = null;
        Assert.AreEqual(0, nullReader.FromNdjson<InvokeLog>().Count);


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


    [TestMethod]
    public void Test_NdJsonReader()
    {
        List<Product3> list = Product3.CreateTestDataList(900);

        MemoryStream gzipData = new MemoryStream();

        using( StreamWriter writer = gzipData.CreateGzipWriter() ) {
            list.ToNdjson(writer);
        }
        

        gzipData.Position = 0;
        List<Product3> list2 = new List<Product3>();

        using( NdJsonReader reader = new NdJsonReader(gzipData, "gzip") ) {
            foreach( Product3 p in reader.ReadLines<Product3>() ) {
                list2.Add(p);
            }
        }

        Assert.AreEqual(900, list2.Count);

        gzipData.Position = 0;
        using StreamReader reader1 = gzipData.CreateGzipReader();
        string ndjson = reader1.ReadToEnd();

        Assert.IsTrue(ndjson.Contains(list.First().ToJson()));
        Assert.IsTrue(ndjson.Contains(list.Last().ToJson()));

        // =====================================================================

        MemoryStream notgData = new MemoryStream();

        using( StreamWriter writer2 = new StreamWriter(notgData, Encoding.UTF8, 1024, true) ) {
            list.ToNdjson(writer2);
        }

        notgData.Position = 0;
        List<Product3> list3 = new List<Product3>();

        using( NdJsonReader reader2 = new NdJsonReader(notgData, null) ) {
            foreach( Product3 p in reader2.ReadLines<Product3>() ) {
                list3.Add(p);
            }
        }

        Assert.AreEqual(900, list3.Count);

        notgData.Position = 0;
        using StreamReader reader3 = new StreamReader(notgData, Encoding.UTF8, true, 1024, true);
        string ndjson2 = reader3.ReadToEnd();

        Assert.IsTrue(ndjson2.Contains(list.First().ToJson()));
        Assert.IsTrue(ndjson2.Contains(list.Last().ToJson()));

        // =====================================================================

        Console.WriteLine($"gzipData.Length = {gzipData.Length}");
        Console.WriteLine($"notgData.Length = {notgData.Length}");
        Assert.IsTrue(notgData.Length > gzipData.Length);
    }

}

