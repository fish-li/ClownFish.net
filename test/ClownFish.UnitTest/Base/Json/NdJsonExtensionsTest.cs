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
        sb.AppendLine("// 注释行1111111111111111");
        sb.Append(Product3.CreateByFixedData().ToJson()).Append("\n");
        sb.AppendLine("// 注释行222222222222222");
        sb.Append(Product3.CreateByRandomData().ToJson()).Append("\n");
        sb.AppendLine();
        sb.AppendLineRN("\r\n");
        sb.Append(Product3.CreateByRandomData().ToJson()).Append("\n");
        sb.AppendLine("// 注释行3333333333333");

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




}

