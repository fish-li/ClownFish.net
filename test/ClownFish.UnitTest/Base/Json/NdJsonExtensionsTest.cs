namespace ClownFish.UnitTest.Base.Json;

[TestClass]
public class NdJsonExtensionsTest
{
    [TestMethod]
    public void Test_ToMultiLineJson()
    {
        List<Product3> list = new List<Product3>();
        list.Add(Product3.CreateByFixedData());
        list.Add(Product3.CreateByFixedData());
        list.Add(Product3.CreateByFixedData());

        string lines = list.ToMultiLineJson().TrimEnd();
        Assert.AreEqual(2, lines.Where(x => x == '\n').Count());
        Assert.IsTrue(lines.StartsWith("{"));
        Assert.IsTrue(lines.EndsWith("}"));


        List<Product3> list2 = lines.FromMultiLineJson<Product3>();
        Assert.AreEqual(3, list2.Count);

        MyAssert.AreEqual(list, list2);
    }


    [TestMethod]
    public void Test_FromMultiLineJson()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Product3.CreateByFixedData().ToJson()).Append("\n");
        sb.Append(Product3.CreateByRandomData().ToJson()).Append("\n");
        sb.Append(Product3.CreateByRandomData().ToJson()).Append("\n");

        List<Product3> list = sb.ToString().FromMultiLineJson<Product3>();
        Assert.AreEqual(3, list.Count);


        List<Product3> list2 = NdJsonExtensions.FromMultiLineJson<Product3>(string.Empty);
        Assert.AreEqual(0, list2.Count);

        List<Product3> list3 = NdJsonExtensions.FromMultiLineJson<Product3>((string)null);
        Assert.IsNull(list3);
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

