using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Json;
using ClownFish.Http.Clients.Elastic;
using Newtonsoft.Json.Serialization;

namespace ClownFish.UnitTest.Base.Json;
[TestClass]
public class JsonStyleTest
{
    internal static readonly InvokeLog OneLog = new InvokeLog() {
        ActionType = 100,
        AppName = "App123",
        ExecuteTime = TimeSpan.FromSeconds(10),
        HasError = 1,
        IsLongTask = 1,
        IsSlow = 1,
        ProcessId = "x123456789",
        StartTime = new DateTime(2024, 1, 2, 11, 22, 33),
        Status = 200,
        Title = "abcd",
    };

    static JsonStyleTest()
    {
        JsonSerializerSettingsUtils.EnableCache = false;
    }

    [TestCleanup]
    public void TestCleanup()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = false;
        ClownFishOptions.JsonSerializer_CamelCase = false;

        Newtonsoft.Json.JsonConvert.DefaultSettings = null;
    }

    [TestMethod]
    public void Test_None_0()
    {
        string json1 = OneLog.ToJson();
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"ActionType\":100"));
        Assert.IsTrue(json1.Contains("\"StartTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));
    }

    [TestMethod]
    public void Test_None_1()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = true;

        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() => {
            return new Newtonsoft.Json.JsonSerializerSettings() {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            };
        });

        string json1 = OneLog.ToJson();

        Assert.IsTrue(json1.Contains("\"actionType\":100"));
        Assert.IsTrue(json1.Contains("\"startTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));

        ClownFishOptions.JsonSerializer_CreateDefault = false;
    }

    [TestMethod]
    public void Test_None_2()
    {
        ClownFishOptions.JsonSerializer_CamelCase = true;

        string json1 = OneLog.ToJson();

        Assert.IsTrue(json1.Contains("\"actionType\":100"));
        Assert.IsTrue(json1.Contains("\"startTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));

        ClownFishOptions.JsonSerializer_CamelCase = false;
    }


    [TestMethod]
    public void Test_KeepType_0()
    {
        JsonStyleTestX1 x1 = new JsonStyleTestX1 {
            Data1 = OneLog
        };

        string json1 = x1.ToJson(JsonStyle.KeepType);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"$type\":\"ClownFish.Log.Logging.InvokeLog, ClownFish.net\""));
        Assert.IsFalse(json1.Contains("\n"));
    }

    [TestMethod]
    public void Test_KeepType_1()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = true;

        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() => {
            return new Newtonsoft.Json.JsonSerializerSettings() {
               TypeNameHandling = TypeNameHandling.Auto,
            };
        });


        JsonStyleTestX1 x1 = new JsonStyleTestX1 {
            Data1 = OneLog
        };

        string json1 = x1.ToJson();
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"$type\":\"ClownFish.Log.Logging.InvokeLog, ClownFish.net\""));
        Assert.IsFalse(json1.Contains("\n"));
    }


    public class JsonStyleTestX1
    {
        public object Data1 { get; set; }
    }


    [TestMethod]
    public void Test_Indented_0()
    {
        string json1 = OneLog.ToJson(JsonStyle.Indented);
        Console.WriteLine(json1);
        Assert.IsTrue(json1.Contains("\n"));
    }

    [TestMethod]
    public void Test_Indented_1()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = true;

        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() => {
            return new Newtonsoft.Json.JsonSerializerSettings() {
                Formatting = Formatting.Indented,
            };
        });

        string json1 = OneLog.ToJson();
        Console.WriteLine(json1);
        Assert.IsTrue(json1.Contains("\n"));
    }


    [TestMethod]
    public void Test_CamelCase_0()
    {
        string json1 = OneLog.ToJson(JsonStyle.CamelCase);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"actionType\":100"));
        Assert.IsTrue(json1.Contains("\"startTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));
    }

    [TestMethod]
    public void Test_CamelCase_1()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = true;

        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() => {
            return new Newtonsoft.Json.JsonSerializerSettings() {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        });


        string json1 = OneLog.ToJson();
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"actionType\":100"));
        Assert.IsTrue(json1.Contains("\"startTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));
    }

    [TestMethod]
    public void Test_CamelCase_2()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = true;

        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() => {
            return new Newtonsoft.Json.JsonSerializerSettings() {
                ContractResolver = new DefaultContractResolver(),
            };
        });


        string json1 = OneLog.ToJson();
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"ActionType\":100"));
        Assert.IsTrue(json1.Contains("\"StartTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));
    }



    [TestMethod]
    public void Test_TimeFormat19_0()
    {
        string json1 = OneLog.ToJson(JsonStyle.TimeFormat19);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"ActionType\":100"));
        Assert.IsTrue(json1.Contains("\"StartTime\":\"2024-01-02 11:22:33\""));
        Assert.IsFalse(json1.Contains("\n"));
    }

    [TestMethod]
    public void Test_TimeFormat19_1()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = true;

        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() => {
            return new Newtonsoft.Json.JsonSerializerSettings() {
                DateFormatString = DateTimeStyle.Time19
            };
        });


        string json1 = OneLog.ToJson(JsonStyle.TimeFormat19);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"ActionType\":100"));
        Assert.IsTrue(json1.Contains("\"StartTime\":\"2024-01-02 11:22:33\""));
        Assert.IsFalse(json1.Contains("\n"));
    }


    [TestMethod]
    public void Test_NameToLower()
    {
        string json1 = OneLog.ToJson(JsonStyle.NameToLower);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"actiontype\":100"));
        Assert.IsTrue(json1.Contains("\"starttime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));
    }


    [TestMethod]
    public void Test_KeepNull_0()
    {
        InvokeLog log = new InvokeLog {
            ActionType = 3,
            StartTime = new DateTime(2024, 1, 2, 11, 22, 33),
        };

        string json1 = log.ToJson(JsonStyle.KeepNull);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"AppName\":null"));
        Assert.IsTrue(json1.Contains("\"ProcessId\":null"));
        Assert.IsTrue(json1.Contains("\"Title\":null"));

        Assert.IsTrue(json1.Contains("\"ExecuteTime\":\"00:00:00\""));
        Assert.IsTrue(json1.Contains("\"IsSlow\":0"));
        Assert.IsTrue(json1.Contains("\"IsLongTask\":0"));
        Assert.IsTrue(json1.Contains("\"Status\":0"));
        Assert.IsTrue(json1.Contains("\"HasError\":0"));

        Assert.IsTrue(json1.Contains("\"StartTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));
    }


    [TestMethod]
    public void Test_KeepNull_1()
    {
        InvokeLog log = new InvokeLog {
            ActionType = 3,
            StartTime = new DateTime(2024, 1, 2, 11, 22, 33),
        };

        string json1 = log.ToJson();
        Console.WriteLine(json1);


        Assert.IsFalse(json1.Contains("\"AppName\":null"));
        Assert.IsFalse(json1.Contains("\"ProcessId\":null"));
        Assert.IsFalse(json1.Contains("\"Title\":null"));

        Assert.IsTrue(json1.Contains("\"ExecuteTime\":\"00:00:00\""));
        Assert.IsTrue(json1.Contains("\"IsSlow\":0"));
        Assert.IsTrue(json1.Contains("\"IsLongTask\":0"));
        Assert.IsTrue(json1.Contains("\"Status\":0"));
        Assert.IsTrue(json1.Contains("\"HasError\":0"));

        Assert.IsTrue(json1.Contains("\"StartTime\":\"2024-01-02T11:22:33+08:00\""));
        Assert.IsFalse(json1.Contains("\n"));
    }



    [TestMethod]
    public void Test_UtcTime_0()
    {
        string json1 = OneLog.ToJson(SimpleEsClient.EsJsonStyle);
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"actionType\":100"));
        Assert.IsTrue(json1.Contains("\"startTime\":\"2024-01-02T11:22:33Z\""));
        Assert.IsFalse(json1.Contains("\n"));
    }


    [TestMethod]
    public void Test_UtcTime_1()
    {
        ClownFishOptions.JsonSerializer_CreateDefault = true;

        Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() => {
            return new Newtonsoft.Json.JsonSerializerSettings() {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatString = "yyyy-MM-ddTHH:mm:ssZ",
                ContractResolver = new CamelCasePropertyNamesContractResolver(),                
            };
        });


        string json1 = OneLog.ToJson();
        Console.WriteLine(json1);

        Assert.IsTrue(json1.Contains("\"actionType\":100"));
        Assert.IsTrue(json1.Contains("\"startTime\":\"2024-01-02T11:22:33Z\""));
        Assert.IsFalse(json1.Contains("\n"));
    }







}
