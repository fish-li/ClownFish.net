using ClownFish.Base.Json;
using Newtonsoft.Json.Serialization;

namespace ClownFish.UnitTest.Base.Json;

[TestClass]
public class JsonSerializerSettingsUtilsTest
{
    [TestMethod]
    public void Test_GetSerializerSettings()
    {
        JsonSerializerSettings s1 = JsonSerializerSettingsUtils.Get(JsonStyle.CamelCase);
        Assert.IsInstanceOfType(s1.ContractResolver, typeof(CamelCasePropertyNamesContractResolver));
        Assert.AreEqual(DateTimeZoneHandling.Local, s1.DateTimeZoneHandling);
        Assert.AreEqual(NullValueHandling.Ignore, s1.NullValueHandling);

        JsonSerializerSettings s2 = JsonSerializerSettingsUtils.Get(JsonStyle.NameToLower);
        Assert.IsInstanceOfType(s2.ContractResolver, typeof(JsonSerializerSettingsUtils.LowerCaseContractResolver));

        JsonSerializerSettings s3 = JsonSerializerSettingsUtils.Get(JsonStyle.Indented);
        Assert.AreEqual(Formatting.Indented, s3.Formatting);

        JsonSerializerSettings s4 = JsonSerializerSettingsUtils.Get(JsonStyle.KeepType);
        Assert.AreEqual(TypeNameHandling.Auto, s4.TypeNameHandling);

        JsonSerializerSettings s5 = JsonSerializerSettingsUtils.Get(JsonStyle.TimeFormat19);
        Assert.AreEqual(DateTimeStyle.Time19, s5.DateFormatString);

        JsonSerializerSettings s6 = JsonSerializerSettingsUtils.Get(JsonStyle.None);
        Assert.AreEqual(DateTimeZoneHandling.Local, s6.DateTimeZoneHandling);
        Assert.AreEqual(NullValueHandling.Ignore, s6.NullValueHandling);
    }

    [TestMethod]
    public void Test_GetSerializerSettings2()
    {
        JsonConvert.DefaultSettings = () => {
            return new JsonSerializerSettings() {
                MaxDepth = 2021
            };
        };

        JsonSerializerSettings settings = JsonSerializerSettingsUtils.Get(JsonStyle.None);
        JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

        Assert.AreEqual(2021, jsonSerializer.MaxDepth);
    }

    public class MyTestData
    {
        public string NAME { get; set; }
    }

    [TestMethod]
    public void Test_NameToLower()
    {
        MyTestData data = new MyTestData {
            NAME = "aaaaaaaa"
        };

        string json = data.ToJson(JsonStyle.NameToLower);
        Assert.IsTrue(json.Contains("name"));
    }


    [TestMethod]
    public void Test_GetSerializerSettings3()
    {
        JsonSerializerSettings settings1 = JsonSerializerSettingsUtils.Get(JsonStyle.KeepNull);
        Assert.AreEqual(NullValueHandling.Include, settings1.NullValueHandling);

        JsonSerializerSettings settings2 = JsonSerializerSettingsUtils.Get(JsonStyle.None);
        Assert.AreEqual(NullValueHandling.Ignore, settings2.NullValueHandling);
    }
}
