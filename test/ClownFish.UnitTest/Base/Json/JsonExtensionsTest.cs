namespace ClownFish.UnitTest.Base.Json;

[TestClass]
public class JsonExtensionsTest
{
    [TestMethod]
    public void Test_1()
    {
        // 这个测试用例没什么实质意义，只为了覆盖率
        _ = JsonExtensions.DefaultCamelCase;
    }

    [TestMethod]
    public void Test_ToJson_FromJson()
    {
        Product3 p = Product3.CreateByRandomData();

        string json = p.ToJson();


        Product3 p2 = json.FromJson<Product3>();

        Assert.IsTrue(p.IsEqual(p2));
    }


    public class TestData
    {
        public object Data { get; set; }
    }

    [TestMethod]
    public void Test_ToJsonKeepTypeInfo()
    {
        Product3 p = Product3.CreateByFixedData();
        TestData data = new TestData { Data = p };

        string json = data.ToJson(JsonStyle.KeepType);

        Assert.IsTrue(json.Contains("{\"$type\":\"ClownFish.UnitTest.Base.Product3, ClownFish.UnitTest\""));
    }

    [TestMethod]
    public void Test_FromJson_ObjectType()
    {
        Product3 p = Product3.CreateByRandomData();

        string json = p.ToJson();
        Product3 p2 = json.FromJson(typeof(Product3)) as Product3;

        Assert.IsTrue(p.IsEqual(p2));
    }

   
    [TestMethod]
    public void Test_ToJsonSerializerSettings()
    {
        JsonSerializerSettings jss = JsonStyle.Indented.ToSettings();
        Assert.AreEqual(Formatting.Indented, jss.Formatting);
    }

#if NET8_0_OR_GREATER
    [TestMethod]
    public void Test_ToJson_UtcTime()
    {
        TestData11 d1 = new TestData11("abc", new DateTime(2024, 2, 3, 4, 5, 6, 789), 33.445m);
        string json1 = d1.ToJson(JsonStyle.UtcTime);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("2024-02-03T04:05:06.789Z"));
    }


    internal record class TestData11(string Name, DateTime Time, decimal Account);

#endif


    //[TestMethod]
    //public void Test_DbNull()
    //{
    //    JsonSerializerSettings settings = ClownFish.Base.Json.JsonSerializerSettingsUtils.Get(JsonStyle.None);
    //    settings.NullValueHandling = NullValueHandling.Ignore;

    //    Dictionary<string, object> dict = new Dictionary<string, object>();
    //    dict["aa"] = 1;
    //    dict["bb"] = DBNull.Value;
    //    dict["cc"] = null;

    //    string json = dict.ToJson(settings);
    //    Console.WriteLine(json);

    //    // output: {"aa":1,"bb":null,"cc":null}
    //}


    [TestMethod]
    public void Test_x_args()
    {
        object nullObject = null;
        Assert.IsNull(nullObject.ToJson());

        Assert.IsNull("".FromJson<InvokeLog>());
        Assert.IsNull("".FromJson(typeof(InvokeLog)));

        TextReader writer = null;
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = writer.FromJson(typeof(InvokeLog));
        });       
    }

    [TestMethod]
    public void Test_Writer()
    {
        List<Product3> list = Product3.CreateTestDataList(10);

        MemoryStream ms = new MemoryStream();

        using( StreamWriter writer = new StreamWriter(ms, Encoding.UTF8, 1024, true) ) {
            list.ToJson(writer);
        }

        string json = ms.ToArray().ToUtf8String();
        Console.WriteLine(json);

        ms.Position = 0;
        StreamReader reader = new StreamReader(ms, Encoding.UTF8, true, 1024, true);

        List<Product3> list2 = (List<Product3>)reader.FromJson(typeof (List<Product3>));

        Assert.AreEqual(10, list2.Count);
    }
}
