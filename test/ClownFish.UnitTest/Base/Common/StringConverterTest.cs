namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class StringConverterTest
{
    private class TestData
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public Guid Guid { get; set; }

        public DateTime Time { get; set; }

        public decimal Amount { get; set; }

        public DayOfWeek Week { get; set; }

        public TimeSpan Time2 { get; set; }

        public byte[] Bytes { get; set; }

        public X1 X1 { get; set; }
    }

    private class X1
    {
        public string Value { get; set; }

        public static implicit operator X1(string text)
        {
            return new X1 { Value = text };
        }
    }

    [TestMethod]
    public void Test_ToObject()
    {
        string guid = Guid.NewGuid().ToString();
        string time = DateTime.Now.ToTimeString();
        string time2 = TimeSpan.FromSeconds(3.14d).ToString();
        byte[] bytes = new byte[2];
        bytes[0] = 12;
        bytes[1] = 23;
        string base64 = bytes.ToBase64();

        string keyValues = $@"Id=2;


text=abc
guid={guid}

Time={time};
Amount=3.14;Week={DayOfWeek.Saturday};time2={time2};bytes={base64};x1=1qaz";
        TestData data = keyValues.ToObject<TestData>();
        Assert.AreEqual(2, data.Id);
        Assert.AreEqual("abc", data.Text);
        Assert.AreEqual(new Guid(guid), data.Guid);
        Assert.AreEqual(DateTime.Today, data.Time.Date);
        Assert.AreEqual(3.14m, data.Amount);
        Assert.AreEqual(DayOfWeek.Saturday, data.Week);
        Assert.AreEqual(TimeSpan.FromSeconds(3.14d), data.Time2);
        data.Bytes.IsEqual(bytes);
        Assert.AreEqual("1qaz", data.X1.Value);


        string json = data.ToJson();
        data = json.ToObject<TestData>();
        Assert.AreEqual(2, data.Id);
        Assert.AreEqual("abc", data.Text);
        Assert.AreEqual(new Guid(guid), data.Guid);
        Assert.AreEqual(DateTime.Today, data.Time.Date);
        Assert.AreEqual(3.14m, data.Amount);
        Assert.AreEqual(DayOfWeek.Saturday, data.Week);
        Assert.AreEqual(TimeSpan.FromSeconds(3.14d), data.Time2);
        data.Bytes.IsEqual(bytes);
        Assert.AreEqual("1qaz", data.X1.Value);
    }

    [ExpectedException(typeof(InvalidCastException))]
    [TestMethod]
    public void Test_ToObject2()
    {
        string keyValues = $"Id=2cc;text=abc";
        TestData data = keyValues.ToObject<TestData>();
    }

    [TestMethod]
    public void Test_ToObject3()
    {
        List<TestData> list = new List<TestData> {
            new TestData{ Id = 2},
            new TestData {Id = 3}
        };

        string json = list.ToJson();

        List<TestData> list2 = json.ToObject<List<TestData>>();

        Assert.AreEqual(2, list2.Count);
        Assert.AreEqual(2, list2[0].Id);
        Assert.AreEqual(3, list2[1].Id);

        Assert.IsNull(StringConverter.ToObject<TestData>(null));
        Assert.IsNull(StringConverter.ToObject<TestData>(string.Empty));

        MyAssert.IsError<ArgumentNullException>(()=> {
            StringConverter.SetObjectValues<TestData>(null, "xx");
        });

        TestData data = new TestData();
        StringConverter.SetObjectValues(data, null);
        Assert.AreEqual(0, data.Id);
        Assert.IsNull(data.Text);
    }


    [TestMethod]
    public void Test_ToObject4()
    {
        string text1 = "DbType=1;Server=10.5.10.171;Port=3306;Database=db1;UserName=sa;Password=xxxxx;Args=";
        DbConfig dbConfig1 = text1.ToObject<DbConfig>();

        Assert.AreEqual(DatabaseType.MySQL, dbConfig1.DbType);
        Assert.AreEqual("10.5.10.171", dbConfig1.Server);
        Assert.AreEqual(3306, dbConfig1.Port.Value);
        Assert.AreEqual("db1", dbConfig1.Database);
        Assert.AreEqual("sa", dbConfig1.UserName);
        Assert.AreEqual("xxxxx", dbConfig1.Password);
        Assert.AreEqual(string.Empty, dbConfig1.Args);



        string text2 = "Server=10.5.10.171;Database=db1;UserName=sa;Password=xxxxx;";
        DbConfig dbConfig2 = text2.ToObject<DbConfig>();

        Assert.AreEqual(DatabaseType.SQLSERVER, dbConfig2.DbType);
        Assert.AreEqual("10.5.10.171", dbConfig2.Server);
        Assert.IsFalse(dbConfig2.Port.HasValue);
        Assert.AreEqual("db1", dbConfig2.Database);
        Assert.AreEqual("sa", dbConfig2.UserName);
        Assert.AreEqual("xxxxx", dbConfig2.Password);
        Assert.IsNull(dbConfig2.Args);
    }

    [TestMethod]
    public void Test_ToTimeSpan()
    {
        string ticks = TimeSpan.FromSeconds(3.14).Ticks.ToString();
        TimeSpan time = StringConverter.ToTimeSpan(ticks);
        Assert.AreEqual(TimeSpan.FromSeconds(3.14d), time);

        TimeSpan time4 = StringConverter.ToTimeSpan(TimeSpan.FromSeconds(3.14).ToString());
        Assert.AreEqual(TimeSpan.FromSeconds(3.14d), time4);


        TimeSpan time2 = StringConverter.ToTimeSpan("123s");
        Assert.AreEqual(TimeSpan.FromSeconds(123), time2);

        TimeSpan time3 = StringConverter.ToTimeSpan("123f");
        Assert.AreEqual(TimeSpan.FromMilliseconds(123), time3);
    }


    [TestMethod]
    public void Test_ChangeType()
    {
        string guid = Guid.NewGuid().ToString();
        object value1 = StringConverter.ChangeType(guid, typeof(string));
        Assert.IsInstanceOfType(value1, typeof(string));
        Assert.AreEqual(guid, (string)value1);

        object value2 = StringConverter.ChangeType(null, typeof(string));
        Assert.IsNull(value2);

        object value3 = StringConverter.ChangeType(string.Empty, typeof(string));
        Assert.AreEqual(string.Empty, (string)value3);

        object value4 = StringConverter.ChangeType(string.Empty, typeof(int));
        Assert.IsNull(value4);

        object value5 = StringConverter.ChangeType("123", typeof(int));
        Assert.IsInstanceOfType(value5, typeof(int));
        Assert.AreEqual(123, (int)value5);

        object value6 = StringConverter.ChangeType("123", typeof(long));
        Assert.IsInstanceOfType(value6, typeof(long));
        Assert.AreEqual(123L, (long)value6);

        object value7 = StringConverter.ChangeType("123.45", typeof(decimal));
        Assert.IsInstanceOfType(value7, typeof(decimal));
        Assert.AreEqual(123.45m, (decimal)value7);

        object value8 = StringConverter.ChangeType("true", typeof(bool));
        Assert.IsInstanceOfType(value8, typeof(bool));
        Assert.AreEqual(true, (bool)value8);

        object value9 = StringConverter.ChangeType("false", typeof(bool));
        Assert.IsInstanceOfType(value9, typeof(bool));
        Assert.AreEqual(false, (bool)value9);

        object value10 = StringConverter.ChangeType(guid, typeof(Guid));
        Assert.IsInstanceOfType(value10, typeof(Guid));
        Assert.AreEqual(guid, value10.ToString());

        object value11 = StringConverter.ChangeType("Indented", typeof(JsonStyle));
        Assert.IsInstanceOfType(value11, typeof(JsonStyle));
        Assert.AreEqual(JsonStyle.Indented, value11);

        object value12 = StringConverter.ChangeType("5.17:37:57.3651594", typeof(TimeSpan));
        Assert.IsInstanceOfType(value12, typeof(TimeSpan));
        TimeSpan ts = (TimeSpan)value12;
        Assert.AreEqual(5, ts.Days);
        Assert.AreEqual(17, ts.Hours);
        Assert.AreEqual(37, ts.Minutes);
        Assert.AreEqual(57, ts.Seconds);
        Assert.AreEqual(365, ts.Milliseconds);  // 注意：这里有精度丢失

        byte[] bb1 = guid.GetBytes();
        string base64 = bb1.ToBase64();
        object value13 = StringConverter.ChangeType(base64, typeof(byte[]));
        Assert.IsInstanceOfType(value13, typeof(byte[]));
        Assert.IsTrue(bb1.IsEqual((byte[])value13));


        Assert.IsNull(StringConverter.ChangeType(null, typeof(DateTime)));
        Assert.IsNull(StringConverter.ChangeType("", typeof(DateTime)));

        MyAssert.IsError<NotSupportedException>(() => {
            _= StringConverter.ChangeType("xxx", typeof(Product2));
        });

    }


    [TestMethod]
    public void Test_ChangeType_Enum()
    {
        DatabaseType t1 = (DatabaseType)StringConverter.ChangeType("1", typeof(DatabaseType));
        Assert.AreEqual(DatabaseType.MySQL, t1);

        DatabaseType t2 = (DatabaseType)StringConverter.ChangeType("MySQL", typeof(DatabaseType));
        Assert.AreEqual(DatabaseType.MySQL, t2);

        DatabaseType t3 = (DatabaseType)StringConverter.ChangeType("mysql", typeof(DatabaseType));
        Assert.AreEqual(DatabaseType.MySQL, t3);
    }


    [TestMethod]
    public void Test_ToObject_DbConfig()
    {
        string value = "DbType=MYSQL;Server=MySqlHost;Database=db1;UserName=sa;Password=abcd";
        DbConfig config = value.ToObject<DbConfig>();

        Assert.AreEqual(DatabaseType.MySQL, config.DbType);
        Assert.AreEqual("MySqlHost", config.Server);
        Assert.AreEqual("db1", config.Database);
        Assert.AreEqual("sa", config.UserName);
        Assert.AreEqual("abcd", config.Password);
    }
}
