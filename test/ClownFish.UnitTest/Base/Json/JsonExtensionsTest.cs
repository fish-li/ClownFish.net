namespace ClownFish.UnitTest.Base.Json;

[TestClass]
public class JsonExtensionsTest
{
    [TestMethod]
    public void Test_ToJson_FromJson()
    {
        Product2 p = Product2.CreateByRandomData();

        string json = p.ToJson();


        Product2 p2 = json.FromJson<Product2>();

        Assert.IsTrue(p.IsEqual(p2));
    }


    public class TestData
    {
        public object Data { get; set; }
    }

    [TestMethod]
    public void Test_ToJsonKeepTypeInfo()
    {
        Product2 p = Product2.CreateByFixedData();
        TestData data = new TestData { Data = p };

        string json = data.ToJson(JsonStyle.KeepType);

        Assert.IsTrue(json.Contains("{\"$type\":\"ClownFish.UnitTest.Base.Product2, ClownFish.UnitTest\""));
    }

    [TestMethod]
    public void Test_FromJson_ObjectType()
    {
        Product2 p = Product2.CreateByRandomData();

        string json = p.ToJson();
        Product2 p2 = json.FromJson(typeof(Product2)) as Product2;

        Assert.IsTrue(p.IsEqual(p2));
    }

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


        List<Product2> list2 = JsonExtensions.FromMultiLineJson<Product2>(string.Empty);
        Assert.AreEqual(0, list2.Count);

        List<Product2> list3 = JsonExtensions.FromMultiLineJson<Product2>(null);
        Assert.IsNull(list3);
    }

    [TestMethod]
    public void Test_ToJsonSerializerSettings()
    {
        JsonSerializerSettings jss = JsonStyle.Indented.ToJsonSerializerSettings();
        Assert.AreEqual(Formatting.Indented, jss.Formatting);

    }
}
