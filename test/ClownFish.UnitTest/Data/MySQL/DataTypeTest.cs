using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.MySQL;

[TestClass]
public class DataTypeTest
{
    private static MySqlDataType GetTestData1()
    {
        MySqlDataType data = new MySqlDataType();
        data.StrValue = "MySQL各种数据库类型";
        data.Str2Value = "";
        data.TextValue = new string('a', 256);
        data.Text2Value = "";
        data.IntValue = int.MaxValue;
        data.Int2Value = int.MinValue;
        data.ShortValue = short.MaxValue;
        data.Short2Value = short.MinValue;
        data.LongValue = long.MaxValue;
        data.Long2Value = long.MinValue;
        data.CharValue = 'a';
        data.Char2Value = 'b';
        data.BoolValue = true;
        data.Bool2Value = false;
        data.TimeValue = new DateTime(2000, 1, 1);
        data.Time2Value = new DateTime(2020, 2, 2);
        data.DecimalValue = 2546.2445m;  // decimal.MaxValue;
        data.Decimal2Value = -275427.1572m;  //decimal.MinValue;
        data.FloatValue = 3.14f;  // float.MaxValue;
        data.Float2Value = -3.14f; // float.MinValue;
        data.DoubleValue = 3.1415926d;  // double.MaxValue;
        data.Double2Value = -3.1415926d;  //double.MinValue;
        data.GuidValue = new Guid();
        data.Guid2Value = Guid.Empty;
        data.ByteValue = byte.MaxValue;
        data.Byte2Value = byte.MinValue;
        data.SByteValue = sbyte.MaxValue;
        data.SByte2Value = sbyte.MinValue;
        data.TimeSpanValue = new TimeSpan(5, 23, 59);
        data.TimeSpan2Value = TimeSpan.Zero;
        data.WeekValue = DayOfWeek.Monday;
        data.Week2Value = DayOfWeek.Sunday;
        data.BinValue = "这个测试用例主要用于检验各种数据类型是否能正常运行".GetBytes();
        data.Bin2Value = new byte[0];
        data.UShortValue = ushort.MaxValue;
        data.UShort2Value = ushort.MinValue;
        data.UIntValue = uint.MaxValue;
        data.UInt2Value = uint.MinValue;
        data.ULongValue = ulong.MaxValue;
        data.ULong2Value = ulong.MinValue;
        return data;
    }

    private static MySqlDataType GetTestData2()
    {
        MySqlDataType data = new MySqlDataType();
        data.StrValue = "MySQL各种数据库类型";
        data.Str2Value = null;
        data.TextValue = new string('x', 256);
        data.Text2Value = null;
        data.IntValue = int.MaxValue - 2;
        data.Int2Value = null;
        data.ShortValue = short.MaxValue - 2;
        data.Short2Value = null;
        data.LongValue = long.MaxValue - 2;
        data.Long2Value = null;
        data.CharValue = 'c';
        data.Char2Value = null;
        data.BoolValue = true;
        data.Bool2Value = null;
        data.TimeValue = new DateTime(2200, 3, 3);
        data.Time2Value = null;
        data.DecimalValue = 25462.2445m;  // decimal.MaxValue;
        data.Decimal2Value = null;
        data.FloatValue = 32.14f;  // float.MaxValue;
        data.Float2Value = null;
        data.DoubleValue = 32.1415926d;  // double.MaxValue;
        data.Double2Value = null;
        data.GuidValue = new Guid();
        data.Guid2Value = null;
        data.ByteValue = byte.MaxValue - 2;
        data.Byte2Value = null;
        data.SByteValue = sbyte.MaxValue - 2;
        data.SByte2Value = null;
        data.TimeSpanValue = new TimeSpan(5, 23, 59);
        data.TimeSpan2Value = null;
        data.WeekValue = DayOfWeek.Friday;
        data.Week2Value = null;
        data.BinValue = "检验各种数据类型".GetBytes();
        data.Bin2Value = null;
        data.UShortValue = ushort.MaxValue - 2;
        data.UShort2Value = null;
        data.UIntValue = uint.MaxValue - 2;
        data.UInt2Value = null;
        data.ULongValue = ulong.MaxValue - 2;
        data.ULong2Value = null;
        return data;
    }


    [TestMethod]
    public async Task Test_MySQL_各种数据库类型_读取写入()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            dbContext.BeginTransaction();

            // 清除所有数据
            dbContext.CPQuery.Create("delete from TestType").ExecuteNonQuery();

            MySqlDataType row1 = GetTestData1();
            MySqlDataType row2 = GetTestData2();
            List<MySqlDataType> list0 = new List<MySqlDataType> { row1, row2 };

            // 写入二条数据
            long newId1 = row1.Insert(dbContext, true);
            long newId2 = await row2.InsertAsync(dbContext, true);
            Assert.IsTrue(newId1 + 1 == newId2);


            // 用二种方式加载数据，然后判断结果是不是一致，用于检验二套数据加载过程
            List<MySqlDataType> list1 = dbContext.CPQuery.Create("select * from TestType").ToList<MySqlDataType>();

            DataTable table = dbContext.CPQuery.Create("select * from TestType").ToDataTable();
            List<MySqlDataType> list2 = table.ToList<MySqlDataType>();

            // 提交事务
            dbContext.Commit();

            list1[0].Rid = 0;
            list1[1].Rid = 0;
            list2[0].Rid = 0;
            list2[1].Rid = 0;

            string json0 = list0.ToJson(JsonStyle.Indented);
            string json1 = list1.ToJson(JsonStyle.Indented);
            string json2 = list2.ToJson(JsonStyle.Indented);

            //File.WriteAllText(@"temp\MySqlDataType_0.json", json0, Encoding.UTF8);
            //File.WriteAllText(@"temp\MySqlDataType_1.json", json1, Encoding.UTF8);
            //File.WriteAllText(@"temp\MySqlDataType_2.json", json2, Encoding.UTF8);


            // 检查二种加载方式的结果是否一致
            Assert.AreEqual(json0, json1);
            Assert.AreEqual(json0, json2);
        }
    }



    [TestMethod]
    public void Test_DateTime_DataType()
    {
        string sql = "select now()";
        DateTime time1, time2;

        using( DbContext dbContext = DbContext.Create("mysql") ) {
            time1 = dbContext.CPQuery.Create(sql).ExecuteScalar<DateTime>();
        }


        // mysql2 的连接字符串不一样，因此，内部的ExecuteScalar返回值类型是 MySqlDateTime
        // 在 MySqlConnector <= 0.63.1 版本中，MySqlDateTime不能转换成 DateTime

        using( DbContext dbContext = DbContext.Create("mysql2") ) {
            time2 = dbContext.CPQuery.Create(sql).ExecuteScalar<DateTime>();
        }

        //Assert.AreEqual(time1, time2);

        Assert.IsTrue((time2 - time1).TotalSeconds < 5);
    }


    [TestMethod]
    public void Test_DateTime_DataType2()
    {
        string sql = "select now()";

        using( DbContext dbContext = DbContext.Create("mysql") ) {

            using( DbDataReader reader = dbContext.CPQuery.Create(sql).ExecuteReader() ) {
                reader.Read();
                DateTime time = reader.GetDateTime(0);

                Assert.AreEqual(DateTime.Today, time.Date);
            }
        }
    }


    [TestMethod]
    public void Test_DateTime_DataType3()
    {
        using( DbContext dbContext = DbContext.Create("mysql2") ) {

            string version = dbContext.CPQuery.Create("select version();").ExecuteScalar<string>();

            // mysql8 已经不允许零值日期值
            Version mysqlVersion = new Version(version);
            if( mysqlVersion.Major >= 8 )
                return;

            List<TestDateTimeTable> list = dbContext.CPQuery.Create("select * from TestDateTimeTable").ToList<TestDateTimeTable>();
            if( list.Count == 0 ) {
                string sql = "insert into TestDateTimeTable(Time1, Time2, Time3) values(null, '0000-00-00 00:00:00', '2020-04-14 12:58:03') ";
                dbContext.CPQuery.Create(sql).ExecuteNonQuery();
            }

            TestDateTimeTable row = dbContext.CPQuery.Create("select * from TestDateTimeTable limit 1").ToSingle<TestDateTimeTable>();

            Assert.IsFalse(row.Time1.HasValue);
            Assert.AreEqual(DateTime.MinValue, row.Time2);
            Assert.AreEqual(new DateTime(2020, 4, 14, 12, 58, 3), row.Time3);
        }
    }


}
